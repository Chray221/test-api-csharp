using System;
using System.IO;
//using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestAPI.Helpers;
using TestAPI.ModelContexts;
using TestAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestAPI.Controllers
{
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        TestContext testDB;
        private readonly IWebHostEnvironment _environment;
        string RootPath { get { return _environment.WebRootPath ?? _environment.ContentRootPath; } }

        public ImagesController(TestContext context, IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            testDB = context;
            testDB.Database.EnsureCreated();
        }

        // GET images/image.jpg
        [HttpGet("{imageName}")]
        public object GetImage(string imageName)
        {
            string filePath = $"{RootPath}/images/{imageName}";
            Logger.Log(filePath);            
            Logger.Log($"CONNECTIONS: {HttpContext.Connection.LocalIpAddress}:{HttpContext.Connection.LocalPort} | {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort} ");
            
            //download the image
            //return File(ImageHelper.RetrieveImage(filePath), $"image/{Path.GetExtension(imageName)}");
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName).Replace(".","")}",true);
        }

        // GET images
        [HttpGet]
        public object Get()
        {
            string imageName = "Logo.png";
            string filePath = $"{RootPath}/images/{imageName}";
            Logger.Log(filePath);
            Logger.Log($"CONNECTIONS: {HttpContext.Connection.LocalIpAddress}:{HttpContext.Connection.LocalPort} | {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort} ");

            //download the image
            //return File(ImageHelper.RetrieveImage(filePath), $"image/{Path.GetExtension(imageName)}");
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName).Replace(".", "")}", true);
        }
    }
}
