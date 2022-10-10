using System;
using System.ComponentModel.DataAnnotations;
using TestAPI.Data;

namespace TestAPI.Models
{
    public class UserDto : PublicProfile
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }

        public UserDto(){}
        public UserDto(User user, string rootUrl = "") : base(user,rootUrl)
        {
            Username = user.Username;
            Email = user.Email;
        }
    }
}
