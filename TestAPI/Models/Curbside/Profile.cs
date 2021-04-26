using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace TestAPI.Models.Curbside
{
    public class Profile : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false,ErrorMessage = "Email address is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [AllowNull, ForeignKey("Image")]
        public int ImageId { get; set; }

        public virtual ImageUrl ImageFile { get; set; }
        public virtual string Image { get { return ImageFile?.ThumbUrl; } }
        //public bool IsVerified { get; set; }
        public int SocialMediaId { get; set; }

        public virtual ICollection<SocialMedia> SocialMedias {get;set;}

        public Profile() : base()
        {

        }

        public Profile(string firstName, string lastName, string password) : base()
        {
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }

        public object UserFormat(string rootURL = "")
        {
            return new
            {
                Id,
                Image = rootURL + "/" + Image,
                FirstName,
                LastName,
                Email
            };
        }
    }
}
