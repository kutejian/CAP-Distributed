using DotNetCore.CAP.Dashboard.NodeDiscovery;
using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhaoxi.DistributedTransaction.StorageServiceMySQL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region CAP配置
            string mysqlConn = this.Configuration["MysqlConn:url"];
            string rabbitMQHost = this.Configuration["RabbitMQOptions:HostName"];

            services.AddOptions<DotNetCore.CAP.MySqlOptions>().Configure(o =>
            {
                o.ConnectionString = mysqlConn;
            });

            services.AddCap(x =>
            {
                x.UseMySql(mysqlConn);
                x.UseRabbitMQ(rabbitMQHost);
                x.FailedRetryCount = 10;
                x.FailedRetryInterval = 60;
                x.FailedThresholdCallback = failed =>
                {
                    var logger = failed.ServiceProvider.GetService<ILogger<Startup>>();
                    logger.LogError($@"MessageType {failed.MessageType} 失败了， 重试了 {x.FailedRetryCount} 次, 
                        消息名称: {failed.Message.GetName()}");//do anything
                };

                #region 注册Consul可视化
                x.UseDashboard();
                DiscoveryOptions discoveryOptions = new DiscoveryOptions();
                this.Configuration.Bind(discoveryOptions);
                x.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = discoveryOptions.DiscoveryServerHostName;
                    d.DiscoveryServerPort = discoveryOptions.DiscoveryServerPort;
                    d.CurrentNodeHostName = discoveryOptions.CurrentNodeHostName;
                    d.CurrentNodePort = discoveryOptions.CurrentNodePort;
                    d.NodeId = discoveryOptions.NodeId;
                    d.NodeName = discoveryOptions.NodeName;
                    d.MatchPath = discoveryOptions.MatchPath;
                });
                #endregion
            });
            #endregion

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Zhaoxi.DistributedTransaction.StorageServiceMySQL", Version = "v1" });
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zhaoxi.DistributedTransaction.StorageServiceMySQL v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
