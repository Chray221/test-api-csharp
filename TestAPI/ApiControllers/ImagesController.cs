﻿using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestAPI.ApiControllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : MyControllerBase
    {

        public ImageController(IWebHostEnvironment environment):base(environment)
        {            
        }

        //image
        [HttpGet]
        public ActionResult GetLogo()
        {
            string imageName = "Logo.png";
            string filePath = this.GetImageString($"{imageName}");
            if (!ImageHelper.IsImageExist(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName).Replace(".", "")}", true);
        }

        // GET images/image.jpg
        [HttpGet("{imageName}")]
        public ActionResult GetImage(string imageName)
        {
            string filePath = this.GetImageString($"{imageName}");
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

        // GET images/image.jpg/download
        [HttpGet("{user_id}/{imageName}/download")]
        public ActionResult DownloaImage(string user_id, string imageName)
        {
            string filePath = this.GetUserImageString($"{user_id}/{imageName}");
            if (!ImageHelper.IsImageExist(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, $"image/{Path.GetExtension(imageName)}", imageName, true);
        }

        public override NotFoundResult NotFound()
        {
            Logger.Log("NOT FOUND RESULT");
            return base.NotFound();
        }

        // save user path /Images/{UserId}/{ImageId}
    }
}
