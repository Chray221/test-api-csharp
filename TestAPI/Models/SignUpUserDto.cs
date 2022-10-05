using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TestAPI.Helpers;

namespace TestAPI.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class SignUpUserRequestDto : SignInUserRequestDto
    {
        [Required]
        [FromFormSnakeCase]
        public string FirstName { get; set; }
        [Required]
        [FromFormSnakeCase]
        public string LastName { get; set; }
        public IFormFile Image { get; set; }

        public SignUpUserRequestDto()
        {
        }
    }
}
