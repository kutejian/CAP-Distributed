using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

    
        [HttpGet(Name = "GetWeatherForecast")]
        [CapSubscribe("cap.test.queue")]
        public void HandleMessage(string message)
        {
            Console.Write(DateTime.Now.ToString() + "收到消息:" + message);
        }
    }
}