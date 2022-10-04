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
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : MyControllerBase
    {

        public ImagesController(IWebHostEnvironment environment):base(environment)
        {            
        }

        // GET images/image.jpg
        [HttpGet("{imageName}")]
        public object GetImage(string imageName)
        {
            string filePath = this.GetUserImageString($"{imageName}");
            if (!ImageHelper.IsImageExist(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName).Replace(".","")}",true);
        }

        // GET images/0A674720-E802-42C1-B28C-53403E0F8800/0A674720-E802-42C1-B28C-53403E0F8800
        [HttpGet("{user_id}/{imageName}")]
        public object GetImage(string user_id, string imageName)
        {
            string filePath = this.GetUserImageString($"{user_id}/{imageName}");
            if (!ImageHelper.IsImageExist(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName).Replace(".", "")}", true);
        }

        // GET images
        [HttpGet]
        public object GetLogo()
        {
            string imageName = "Logo.png";
            string filePath = this.GetUserImageString($"{imageName}");
            if (!ImageHelper.IsImageExist(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, "image", Path.GetExtension(imageName).Replace(".", ""), true);
        }

        // GET images/image.jpg/download
        [HttpGet("{user_id}/{imageName}/download")]
        public object DownloaImage(string user_id, string imageName)
        {
            string filePath = this.GetUserImageString($"{user_id}/{imageName}");
            if (!ImageHelper.IsImageExist(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName)}", true);
        }

        public override NotFoundResult NotFound()
        {
            Logger.Log("NOT FOUND RESULT");
            return base.NotFound();
        }
    }
}
