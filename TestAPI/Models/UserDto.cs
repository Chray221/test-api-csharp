using System;
using System.ComponentModel.DataAnnotations;
using TestAPI.Data;

namespace TestAPI.Models
{
    public class UserDto : PublicProfile
    {
        public string Username { get; set; }

        public UserDto(){}
        public UserDto(User user) :base(user)
        {
            Id = user.Id;
        }
    }
}
