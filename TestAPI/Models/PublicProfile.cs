using System;
using TestAPI.Data;

namespace TestAPI.Models
{
    public class PublicProfile : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }

        public PublicProfile() { }
        public PublicProfile(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Image = user.Image;
        }
    }
}
