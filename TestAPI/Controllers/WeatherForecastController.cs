using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TestAPI.Helpers;
using TestAPI.Models;

namespace TestAPI.Controllers
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

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            
            var rng = new Random();
            _logger.Log(LogLevel.Debug, $"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            Debug.WriteLine($"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            
            using (var reader = new StreamReader(this.Request.Body))
            {
                var json = await reader.ReadToEndAsync();                
                Debug.WriteLine($"BODY => {json}");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        
        [HttpGet("{id}/{name}")]
        public IEnumerable<WeatherForecast> Get([FromRoute]int id,[FromRoute]string name)
        {

            var rng = new Random();
            _logger.Log(LogLevel.Debug, $"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            Debug.WriteLine($"Summary = {Summaries[rng.Next(Summaries.Length)]} {id} {name}");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost("add")]
        public async Task< IEnumerable<WeatherForecast> > AddWeather()
        {
            var rng = new Random();
            _logger.Log(LogLevel.Debug, $"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            //Debug.WriteLine($"Summary = {Summaries[rng.Next(Summaries.Length)]} {json} {name}");
            var json = await this.GetRequestJObjectAsync();
            Debug.WriteLine($"BODY = {json["id"]}");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
