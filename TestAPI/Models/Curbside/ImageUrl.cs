using System;
using System.IO;
using TestAPI.Helpers;

namespace TestAPI.Models.Curbside
{
    public class ImageUrl : BaseModel
    {
        public string Url { get; set; }
        public string ThumbUrl { get; set; }
        public ImageUrl() : base()
        {
            
        }
        public ImageUrl(string ImageName) : base()
        {
            Url = Path.Combine($"images", $"{ImageName}");
            ThumbUrl = Path.Combine($"images", $"thumb_{ImageName}");
        }

        public void Update(int UsedId)
        {
            ImageHelper.CreateFolder($"images/{UsedId}");
            Url = Url.Replace("images", $"images/{UsedId}");
            ThumbUrl = ThumbUrl.Replace("images", $"images/{UsedId}");
        }

        public void ProductUpdate(int UsedId)
        {
            ImageHelper.CreateFolder($"images/products/{UsedId}");
            Url = Url.Replace("images", $"images/products/{UsedId}");
            ThumbUrl = ThumbUrl.Replace("images", $"images/products/{UsedId}");
        }
    }
}
