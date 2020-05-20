using System;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace TestAPI.Helpers
{
    public static class MessageExtension
    {
        public static object ShowCustomMessage(string title, string content, string button = "Okay", string icon = null, int icon_type = 0,int statusCode = 200)
        {
            return new { custom_message = new { title, icon, icon_type, content, button },status = statusCode };
        }

        public static object ShowRequiredMessage(string propertyName)
        {
            return new { error = $"{propertyName} is required!." ,status = 422 };
        }
    }
}
