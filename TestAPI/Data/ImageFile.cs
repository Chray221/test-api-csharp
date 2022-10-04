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
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public ImageFile() : base()
        {

        }

        public ImageFile(string ImageName) : base()
        {
            Id = Guid.NewGuid();
            Url = Path.Combine(ImageHelper.ImagePath, $"{ImageName}");
            ThumbUrl = Path.Combine(ImageHelper.ImagePath, $"thumb_{ImageName}");
        }

        public ImageFile(Guid UserId) : base()
        {
            Id = Guid.NewGuid();
            string imagePath = Path.Combine(ImageHelper.ImagePath, $"{UserId}");
            ImageHelper.CreateFolder(imagePath);
            Url = Path.Combine(imagePath, $"{Id}");
            ThumbUrl = Path.Combine(imagePath, $"{Id}_thumb");
        }

        public void Update(Guid UserId)
        {
            this.UserId = UserId;
            string imagePath = Path.Combine(ImageHelper.ImagePath, $"{UserId}");
            ImageHelper.CreateFolder(imagePath);
            Url = Path.Combine(imagePath,$"{Id}");
            ThumbUrl = Path.Combine(imagePath, $"{Id}_thumb");
            //Url = Url.Replace("images",$"images/{UserId}");
            //ThumbUrl = ThumbUrl.Replace("images", $"images/{UserId}");
        }
    }
}
