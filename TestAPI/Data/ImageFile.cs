using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using TestAPI.Helpers;

namespace TestAPI.Data
{
    public class ImageFile : BaseModel
    {
        public string Url { get; set; }
        public string ThumbUrl { get; set; }

        public ImageFile() : base()
        {

        }
    }
}
