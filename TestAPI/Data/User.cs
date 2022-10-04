using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Newtonsoft.Json;

namespace TestAPI.Data
{
    public class User : BaseModel
    {
        [Required]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        [AllowNull]
        public Guid? ImageFileId { get; set; }
        [ForeignKey("ImageFileId")]
        public virtual ImageFile ImageFile { get; set; }
        //public string Image { get; set; }
        [NotMapped]
        public string Image { get { return ImageFile?.ThumbUrl; } }
        //public bool IsVerified { get; set; }

        public User() : base()
        {

        }

        public User(string username, string firstName, string lastName, string password) : base()
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }

        public object UserFormat(string rootURL="")
        {
            return new
            {
                Id,
                Username,
                Image = rootURL + "/"+ Image,
                FirstName,
                LastName
            };
        }
    }
}
