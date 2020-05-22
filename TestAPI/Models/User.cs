using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Newtonsoft.Json;

namespace TestAPI.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Image { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpatedAt { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now;
            UpatedAt = DateTime.Now;
        }

        public User(string username, string firstName, string lastName, string password)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            CreatedAt = DateTime.Now;
            UpatedAt = DateTime.Now;
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
