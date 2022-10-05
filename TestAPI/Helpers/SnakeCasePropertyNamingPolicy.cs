using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace TestAPI.Helpers
{
    public class SnakeCasePropertyNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return string.Concat(name.Select((character, index) =>
                    index > 0 && char.IsUpper(character)
                        ? "_" + character
                        : character.ToString()))
                .ToLower();
        }
    }

    public class FromFormSnakeCase : FromFormAttribute
    {
        public FromFormSnakeCase([CallerMemberName] string propertyName = "" )
        {
            Name = ConvertName(propertyName);
        }

        public static string ConvertName(string name)
        {
            return string.Concat(name.Select((character, index) =>
                    index > 0 && char.IsUpper(character)
                        ? "_" + character
                        : character.ToString()))
                .ToLower();
        }
    }
}
