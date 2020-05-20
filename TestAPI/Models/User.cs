using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TestAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("first_name")]
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}
