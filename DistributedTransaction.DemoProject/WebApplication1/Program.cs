using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Zhaoxi.DistributedTransaction.EFModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    x.UseSqlServer("Server=LAPTOP-AVFER53P\\SQLEXPRESS;Database=UserService;User Id=sa;Password=123456;");//ʹ��SQLServer������---���ݿ�����
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
#region EFCore
builder.Services.AddDbContext<CommonServiceDbContext>(options =>
{
    options.UseSqlServer("Server=LAPTOP-AVFER53P\\SQLEXPRESS;Database=UserService;User Id=sa;Password=123456;");
});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//����cap�м��
//app.UseCap();
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (ICapPublisher _publisher) =>
{
    _publisher.Publish("cap.test.queue", "111111111111111111okokok");
    return "ok";
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}