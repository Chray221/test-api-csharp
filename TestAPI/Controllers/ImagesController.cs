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
    public class ImagesController : Controller
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
        public object Get(string imageName)
        {
            string filePath = $"{RootPath}/images/{imageName}";
            Logger.Log(filePath);

            //using (FileStream fs = new FileStream($"{RootPath}/images/{imageName}", FileMode.Open, FileAccess.Read))
            //{
            //    HttpResponseMessage response = new HttpResponseMessage();
            //    response.Content = new StreamContent(fs);
            //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            //    return response;
            //}

            //download the image
            //return File(ImageHelper.RetrieveImage(filePath), $"image/{Path.GetExtension(imageName)}");
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName).Replace(".","")}",true);
            //return new Uri($"data:image/{Path.GetExtension(imageName)};base64,{(System.Convert.ToBase64String(ImageHelper.RetrieveImage($"{RootPath}/images/{imageName}")))}");
        }
    }
}
