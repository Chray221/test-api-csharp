﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestAPI.Helpers
{
    public static class JsonExtensions
    {
        public static async Task<JObject> GetRequestJObjectAsync( this ControllerBase controllerBase)
        {
            using (var reader = new StreamReader(controllerBase.Request.Body))
            {
                return JObject.Parse(await reader.ReadToEndAsync());
            }
        }


        public static async Task<T> GetRequestRObject<T>(this ControllerBase controllerBase)
        {
            using (var reader = new StreamReader(controllerBase.Request.Body))
            {
                string json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>( json);
            }
        }

        public static object Default(this string stringValue)
        {
            return stringValue ?? "";
        }

        public static string ToStringOrEmpty(this JToken jObject)
        {
            return jObject.ToString() ?? "";
        }

        public static bool ContainsKeyOrNull(this JObject jObject, string propertyName)
        {
            return jObject.ContainsKey(propertyName) || string.IsNullOrEmpty(jObject[propertyName].ToString());
        }

        public static bool IsValidString(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

    }
}
