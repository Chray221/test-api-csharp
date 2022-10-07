using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Newtonsoft.Json;

namespace TestAPI.Data
{
    [Index(nameof(Username),IsUnique =true)]
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
        public string Email { get; set; }
        [AllowNull]
        public Guid? ImageFileId { get; set; }
        [ForeignKey("ImageFileId")]
        public virtual ImageFile ImageFile { get; set; }


        [NotMapped]
        public string Image { get { return ImageFile?.ThumbUrl; } }

        public User() : base()
        {

        }

        public User(string username, string firstName, string lastName, string password , string email ) : base()
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            Email = email;
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
