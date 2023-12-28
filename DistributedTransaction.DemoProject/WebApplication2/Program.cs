using Microsoft.EntityFrameworkCore;
using Zhaoxi.DistributedTransaction.EFModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string conne = "Server=LAPTOP-AVFER53P\\SQLEXPRESS;Database=OrderService;User Id=sa;Password=123456;";
#region EFCore
builder.Services.AddDbContext<CommonServiceDbContext>(options =>
{
    options.UseSqlServer(conne);
});
#endregion
builder.Services.AddCap(x =>
{
    x.UseRabbitMQ(rb =>
    {
        //rabbitmq��������ַ
        rb.HostName = "192.168.72.164";

        rb.UserName = "guest";
        rb.Password = "guest";

        //ָ��Topic exchange���ƣ���ָ���Ļ�����Ĭ�ϵ�
        rb.ExchangeName = "cap.text.exchange";
    });
    x.UseSqlServer(conne);//ʹ��SQLServer������---���ݿ�����
    x.FailedRetryCount = 10;//���Դ���
    x.FailedRetryInterval = 60;//���Եļ��Ƶ��
    x.FailedThresholdCallback = failed =>
    {
        var logger = failed.ServiceProvider.GetService<ILogger<Program>>();

        logger.LogError($@"MessageType {failed.MessageType} ʧ���ˣ� ������ {x.FailedRetryCount} ��, 
                        ��Ϣ����: {failed.Message}");//do anything
    };//ʧ�ܳ���������Ļص�

    //���10��---��һ���ʧ��10��---�ص���֪ͨ---�˹��ָ�������ҵ����ͨ��---Ȼ���޸����ݿ��retries

    x.TopicNamePrefix = "ZhaoxiDistributedTransaction";

    #region ע��Consul���ӻ�
    x.UseDashboard();
    //DiscoveryOptions discoveryOptions = new DiscoveryOptions();
    //this.Configuration.Bind(discoveryOptions);
    //x.UseDiscovery(d =>
    //{
    //    d.DiscoveryServerHostName = discoveryOptions.DiscoveryServerHostName;
    //    d.DiscoveryServerPort = discoveryOptions.DiscoveryServerPort;
    //    d.CurrentNodeHostName = discoveryOptions.CurrentNodeHostName;
    //    d.CurrentNodePort = discoveryOptions.CurrentNodePort;
    //    d.NodeId = discoveryOptions.NodeId;
    //    d.NodeName = discoveryOptions.NodeName;
    //    d.MatchPath = discoveryOptions.MatchPath;
    //});
    #endregion
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
