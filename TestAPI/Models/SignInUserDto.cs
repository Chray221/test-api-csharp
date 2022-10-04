using System;
using System.ComponentModel.DataAnnotations;
using TestAPI.Helpers;

namespace TestAPI.Models
{
    public class SignInUserRequestDto
    {
        [Required]
        [MinLength(8, ErrorMessage = "Username is too short")]
        public string Username { get; set; }
        [Required]
        [MinLength(8,ErrorMessage = "Password is too short")]
        public string Password { get; set; }
        public SignInUserRequestDto()
        {
        }
    }
}
