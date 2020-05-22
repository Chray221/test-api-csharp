using System;
using System.IO;

namespace TestAPI.Models
{
    public class ImageFile : BaseModel
    {
        public string Url { get; set; }
        public string ThumbUrl { get; set; }
        public ImageFile() : base()
        {

        }
        public ImageFile(string ImageName) : base()
        {
            Url = Path.Combine($"images", $"{ImageName}");
            ThumbUrl = Path.Combine($"images", $"thumb_{ImageName}");
        }
    }
}
