using System;
using System.IO;
using TestAPI.Data;

namespace TestAPI.Models
{
    public class PublicProfile : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }

        public PublicProfile() { }
        public PublicProfile(User user, string rootUrl = "")
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Image = user.Image;

            if (!string.IsNullOrEmpty(Image) &&
                !Image.Contains(rootUrl))
            {
                Image = Path.Combine(rootUrl, Image);
            }
        }

        public T UpdateImagePath<T>(string rootUrl) where T : PublicProfile
        {
            return (T)this;
        }
    }
}
