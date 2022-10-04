using System;
using System.ComponentModel.DataAnnotations;
using TestAPI.Helpers;

namespace TestAPI.Models
{
    public class SignUpUserRequestDto : SignInUserRequestDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public SignUpUserRequestDto()
        {
        }
    }
}
