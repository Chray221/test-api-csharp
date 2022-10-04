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
            Url = Path.Combine($"images", $"{ImageName}");
            ThumbUrl = Path.Combine($"images", $"thumb_{ImageName}");
        }

        public void Update(Guid UsedId)
        {
            ImageHelper.CreateFolder($"images/{UsedId}");
            Url = Url.Replace("images",$"images/{UsedId}");
            ThumbUrl = ThumbUrl.Replace("images", $"images/{UsedId}");
        }
    }
}
