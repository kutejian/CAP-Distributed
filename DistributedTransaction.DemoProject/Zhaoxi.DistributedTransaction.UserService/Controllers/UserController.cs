using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zhaoxi.DistributedTransaction.EFModel;
using Zhaoxi.DistributedTransaction.Utility;

namespace Zhaoxi.DistributedTransaction.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string PublishName = "RabbitMQ.SQLServer.UserService";

        private readonly IConfiguration _iConfiguration;
        /// <summary>
        /// 构造函数注入---默认IOC容器完成---注册是在AddCAP
        /// </summary>
        private readonly ICapPublisher _iCapPublisher;
        private readonly CommonServiceDbContext _UserServiceDbContext;
        private readonly ILogger<UserController> _Logger;

        public UserController(ICapPublisher capPublisher, IConfiguration configuration, CommonServiceDbContext userServiceDbContext, ILogger<UserController> logger)
        {
            this._iCapPublisher = capPublisher;
            this._iConfiguration = configuration;
            this._UserServiceDbContext = userServiceDbContext;
            this._Logger = logger;
        }
        //下面3个都是订阅者
/*        [NonAction]//必须的
        [CapSubscribe(PublishName, Group = "Group.Queue1")]//必须的--注册-监听某个队列
        public void Subscriber(User u)//默认找body数据反序列化
        {
            Console.WriteLine($@"{DateTime.Now} Subscriber11111111111 invoked, Info: {u}");
        }
        [NonAction]//必须的
        [CapSubscribe(PublishName, Group = "Group.Queue2")]//必须的--注册-监听某个队列
        public void Subscriber1(User u)//默认找body数据反序列化
        {
            Console.WriteLine($@"{DateTime.Now} Subscriber2222222222222 invoked, Info: {u}");
        }
        [NonAction]//必须的
        [CapSubscribe(PublishName, Group = "Group.Queue3")]//必须的--注册-监听某个队列
        public void Subscriber3(User u)//默认找body数据反序列化
        {
            Console.WriteLine($@"{DateTime.Now} Subscriber3333333333 invoked, Info: {u}");
        }*/

        //没有使用事务
        [Route("/without/transaction")]//根目录
        public async Task<IActionResult> WithoutTransaction()
        {
            var user = this._UserServiceDbContext.User.Find(1);
            this._Logger.LogWarning($"This is WithoutTransaction Invoke");
            await _iCapPublisher.PublishAsync(PublishName, user);//应该把数据写到publish表
            return Ok();
        }
        //自定义事务
        [Route("/adotransaction/sync")]//根目录
        public IActionResult AdoTransaction()
        {
            var user = this._UserServiceDbContext.User.First();
            IDictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("Teacher", "Eleven");
            dicHeader.Add("Student", "Seven");
            dicHeader.Add("Version", "1.2");

            using (var connection = new SqlConnection(this._iConfiguration.GetConnectionString("UserServiceConnection")))
            {
                using (var transaction = connection.BeginTransaction(this._iCapPublisher, true))
                {
                    //user.Name += "2021";
                    //this._UserServiceDbContext.SaveChanges();
                    _iCapPublisher.Publish(PublishName, user, dicHeader);//带header
                }
            }
            this._Logger.LogWarning($"This is AdoTransaction Invoke");
            return Ok();
        }
        //事务
        [Route("/efcoretransaction/async")]//根目录
        public IActionResult EFCoreTransaction()
        {
            var user = this._UserServiceDbContext.User.First();//读个数据
            var userNew = new User()
            {
                Name = "Eleven" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                CompanyId = 1,
                CompanyName = "Steven Company",
                CreateTime = DateTime.Now,
                CreatorId = 1,
                LastLoginTime = DateTime.Now,
                LastModifierId = 1,
                LastModifyTime = DateTime.Now,
                Password = "123456",
                State = 1,
                Account = "Administrator",
                Email = "57265177@qq.com",
                Mobile = "18664876677",
                UserType = 1
            };//new个对象

            IDictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("Teacher", "Eleven");
            dicHeader.Add("Student", "Steven");
            dicHeader.Add("Version", "1.2");
            dicHeader.Add("Group.Queue1", "Group.Queue1");
            //完成 业务+publish的本地事务
            using (var trans = this._UserServiceDbContext.Database.BeginTransaction(this._iCapPublisher, autoCommit: false))
            {
                this._UserServiceDbContext.User.Add(userNew);//数据库插入对象
                this._UserServiceDbContext.SaveChanges();//提交---Context事务的

                _iCapPublisher.Publish(PublishName, user, dicHeader);//带header
                //publish做的就只是把数据写入到publish表

                //throw new Exception();就都写不进去了

                Console.WriteLine("数据库业务数据已经插入");
                trans.Commit();
            }
            this._Logger.LogWarning($"This is EFCoreTransaction Invoke");
            return Ok("Done");
        }

        #region 多节点贯穿协作
 
        [Route("/Distributed/Demo/{id}")]//根目录
        public IActionResult Distributed(int? id)
        {
            int index = id ?? 11;
            string publishName = ConstPublish.UserPublishName;

            var user = this._UserServiceDbContext.User.Find(1);
            var userNew = new User()
            {
                Name = "Steven" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                CompanyId = 1,
                CompanyName = "Steven" + index,
                CreateTime = DateTime.Now,
                CreatorId = 1,
                LastLoginTime = DateTime.Now,
                LastModifierId = 1,
                LastModifyTime = DateTime.Now,
                Password = "123456" + index,
                State = 1,
                Account = "Administrator" + index,
                Email = "Steven@qq.com",
                Mobile = "Steven",
                UserType = 1
            };

            IDictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("Teacher", "Steven");
            dicHeader.Add("Student", "Seven");
            dicHeader.Add("Version", "1.2");
            dicHeader.Add("Index", index.ToString());

            using (var trans = this._UserServiceDbContext.Database.BeginTransaction(this._iCapPublisher, autoCommit: false))
            {
                this._UserServiceDbContext.User.Add(userNew);
                this._iCapPublisher.Publish(publishName, user, dicHeader);//带header
                this._UserServiceDbContext.SaveChanges();
                Console.WriteLine("数据库业务数据已经插入");
                trans.Commit();
            }
            this._Logger.LogWarning($"This is EFCoreTransaction Invoke");
            return Ok("Done");
        }

        #endregion

    }
}