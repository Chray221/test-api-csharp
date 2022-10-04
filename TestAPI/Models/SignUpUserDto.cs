using System;
using TestAPI.Helpers;

namespace TestAPI.Models
{
    public class SignUpUserDto : UserDto
    {
        private new Guid Id {get;set;}
        public string Username { get; set; }
        public string Password { get; set; }
        public SignUpUserDto()
        {
        }

        public object VerifyRequired()
        {
            if (string.IsNullOrEmpty(Username))
            {
                return MessageExtension.ShowRequiredMessage("Username");
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                return MessageExtension.ShowRequiredMessage("Firstname");
            }

            if (string.IsNullOrEmpty(LastName))
            {
                return MessageExtension.ShowRequiredMessage("Lastname");
            }

            if (string.IsNullOrEmpty(Password))
            {
                return MessageExtension.ShowRequiredMessage("Password");
            }

            return null;
        }
    }
}
