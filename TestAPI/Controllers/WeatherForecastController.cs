using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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

        public string RootPath { get { return Host.HostEnvironment.WebRootPath ?? Host.HostEnvironment.ContentRootPath; } }
        
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            Host.HostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            Logger.Log($"PATH {RootPath}");
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            
            var rng = new Random();
            _logger.Log(LogLevel.Information, $"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            Logger.Log($"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            
            using (var reader = new StreamReader(this.Request.Body))
            {
                var json = await reader.ReadToEndAsync();                
                Logger.Log($"BODY => {json}");
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
            _logger.Log(LogLevel.Information, $"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            Logger.Log($"Summary = {Summaries[rng.Next(Summaries.Length)]}");
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
            _logger.Log(LogLevel.Information, $"Summary = {Summaries[rng.Next(Summaries.Length)]}");
            //Logger.Log($"Summary = {Summaries[rng.Next(Summaries.Length)]} {json} {name}");
            var json = await this.GetRequestJObjectAsync();
            Logger.Log($"BODY = {json["id"]}");
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
