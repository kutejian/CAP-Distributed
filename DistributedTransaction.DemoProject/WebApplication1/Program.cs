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
        //rabbitmq服务器地址
        rb.HostName = "192.168.72.164";

        rb.UserName = "guest";
        rb.Password = "guest";

        //指定Topic exchange名称，不指定的话会用默认的
        rb.ExchangeName = "cap.text.exchange";
    });
    x.UseSqlServer("Server=LAPTOP-AVFER53P\\SQLEXPRESS;Database=UserService;User Id=sa;Password=123456;");//使用SQLServer的容器---数据库连接
    x.FailedRetryCount = 10;//重试次数
    x.FailedRetryInterval = 60;//重试的间隔频率
    x.FailedThresholdCallback = failed =>
    {
        var logger = failed.ServiceProvider.GetService<ILogger<Program>>();

        logger.LogError($@"MessageType {failed.MessageType} 失败了， 重试了 {x.FailedRetryCount} 次, 
                        消息名称: {failed.Message}");//do anything
    };//失败超出次数后的回调

    //最高10次---万一真的失败10次---回调发通知---人工恢复数据让业务能通过---然后修改数据库的retries

    x.TopicNamePrefix = "ZhaoxiDistributedTransaction";

    #region 注册Consul可视化
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
//启用cap中间件
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