using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace TestAPI.Helpers
{
    public static class ImageHelper
    {

        public static async Task<bool> SaveImage(IFormFile postedFile,string filePath, string user_id = "0")
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

        public static async Task SaveResizeImage(IFormFile postedFile, string filePath, int width, int height)
        {
            if (postedFile.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postedFile.CopyToAsync(fileStream);
                }
            }
            //postedFile.CopyToAsync();
        }

        public static byte[] RetrieveImage(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }



    }
}
