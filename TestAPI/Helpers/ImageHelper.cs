using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace TestAPI.Helpers
{
    public static class ImageHelper
    {

        public static async Task<bool> SaveImage(IFormFile postedFile,string filePath)
        {
            ////Extract Image File Name.
            //string fileName = System.IO.Path.GetFileName(postedFile.FileName);

            ////Set the Image File Path.
            //string filePath = "~/Uploads/" + fileName;

            ////Save the Image File in Folder.
            //postedFile.SaveAs(Server.MapPath(filePath));

            ////Insert the Image File details in Table.
            //FilesEntities entities = new FilesEntities();
            //entities.Files.Add(new File
            //{
            //    Name = fileName,
            //    Path = filePath
            //});
            //entities.SaveChanges();

            //Redirect to Index Action.
            //return RedirectToAction("Index");

            //var uploads = Path.Combine(rootPath, "uploads");
            
            if (postedFile.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postedFile.CopyToAsync(fileStream);
                }
            }
            return true;
        }

        public static byte[] RetrieveImage(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }
    }
}
