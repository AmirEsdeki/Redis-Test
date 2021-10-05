using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Redis_Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis_Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDistributedCache _cache;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<DataContainer<IEnumerable<WeatherForecast>>> Get()
        {
            string whereItsLoaded;
            WeatherForecast[] weatherForecasts;

            var key = $"Get_WeatherForcast_{DateTime.Now.ToString("yyyyMMdd_hhmm")}";

            weatherForecasts = await _cache.GetRecordAsync<WeatherForecast[]>(key);

            if (weatherForecasts is not null)
            {
                whereItsLoaded = $"It's loaded from Redis on {DateTime.Now}";
            }
            else
            {
                await Task.Delay(2000);
                whereItsLoaded = $"It's loaded from API on {DateTime.Now}";
                var rng = new Random();
                weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = _summaries[rng.Next(_summaries.Length)]
                })
                .ToArray();
                await _cache.SetRecordAsync<WeatherForecast[]>(key, weatherForecasts);
            }

            return new DataContainer<IEnumerable<WeatherForecast>>(data: weatherForecasts, message: whereItsLoaded);
        }
    }
}
