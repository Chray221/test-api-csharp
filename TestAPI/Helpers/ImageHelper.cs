﻿using System;
using System.IO;
using System.Threading.Tasks;
//using System.Web;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Hosting; //*get RootPath
//using System.Drawing;               //CoreCompat Image handling
//using System.Drawing.Drawing2D;     //CoreCompat Image handling
//using System.Drawing.Imaging;       //CoreCompat Image handling
//using Microsoft.AspNetCore.Authorization;   //Identity: Register,Login
//using Microsoft.AspNetCore.Authentication;  //Identity: Register,Login
//using Microsoft.AspNetCore.Identity;        //Identity: Register,Login
//using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using TestAPI.Controllers;

namespace TestAPI.Helpers
{
    public static class ImageHelper
    {
        public static string ImagePath = Path.Combine($"Uploads", "Images");

        public static async Task<bool> SaveImageAsync(IFormFile postedFile,string filePath)
        {
            ////Extract Image File Name.
            //string fileName = System.IO.Path.GetFileName(postedFile.FileName);

            ////Set the Image File Path.
            //string filePath = "~/Uploads/" + fileName;

            ////Save the Image File in Folder. USING ENTITY
            //postedFile.SaveAs(Server.MapPath(filePath));
            if (postedFile.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postedFile.CopyToAsync(fileStream);
                }
            }
            return true;
        }

        public static void CreateFolder(string folderPath)
        {
            var directory = Directory.CreateDirectory(folderPath);
            Logger.Log($"DIRECTORY ? {directory == null} {folderPath}");
            if (directory != null)
                Logger.Log($"DIRECTORY {directory.FullName}");
        }

        public static byte[] RetrieveImage(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }

        public static void SaveThumbImage(IFormFile postedFile, string thumbFilePath)
        {
            //return SaveResizeImage(postedFile, thumbFilePath, 200);
            SaveResizedImage(postedFile, thumbFilePath, 200);
        }

        public static void SaveResizedImage(IFormFile postedFile, string thumbFilePath, int newWidth)
        {

            // Image.Load(string path) is a shortcut for our default type. 
            // Other pixel formats use Image.Load<TPixel>(string path))
            var imageInfo = Image.Identify(postedFile.OpenReadStream());

            double dblWidth_origial = imageInfo.Width;
            double dblHeigth_origial = imageInfo.Height;
            double relation_heigth_width = dblHeigth_origial / dblWidth_origial;
            int newHeight = (int)(newWidth * relation_heigth_width);

            using (Image image = Image.Load(postedFile.OpenReadStream()))
            {
                image.Mutate(x => x
                     .Resize(newWidth, newHeight));
                image.Save(thumbFilePath); // Automatic encoder selected based on extension.
            }
        }

        public static Task<bool> SaveUserImageAsync(IFormFile postedFile, string imagePathName)
        {
            string imagePath = Path.Combine( ImagePath, imagePathName);
            return SaveImageAsync(postedFile, imagePath);
        }

        public static byte[] GetUserImageAsync(string imagePathName)
        {
            string imagePath = Path.Combine(ImagePath, imagePathName);
            return RetrieveImage(imagePath);
        }

        public static string GetUserImageString(this MyControllerBase controller,string imagePathName)
        {
            return Path.Combine(controller.RootPath, ImagePath, imagePathName);
        }

        public static bool IsImageExist(string imagePath)
        {
            return File.Exists(imagePath);
        }
    }
}
