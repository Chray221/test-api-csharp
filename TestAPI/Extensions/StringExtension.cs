using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestAPI.Extensions
{
    public static class StringExtension
    {
        public static bool ContainsKey(this JObject json, string key, string value)
        {
            if (json.ContainsKey(key) && json[key].ToString() == value)
                return true;
            return false;
        }


        public static bool ContainsKey(this JToken jToken, string key)
        {

            if (jToken.HasValues && ((JObject)jToken).ContainsKey(key))
                return true;
            return false;
        }

        /// <summary>
        /// 1000 -> 1,000
        /// 1521.52 -> 1,521.52
        /// </summary>
        /// <returns>The comma.</returns>
        /// <param name="number">number.</param>
        public static string ToComma(this int number)
        {
            return number.ToString(number >= 1000 || number <= -1000 ? "0,0.##" : "0.##");
        }

        /// <summary>
        /// 1000 -> 1,000
        /// 1521.52 -> 1,521.52
        /// </summary>
        /// <returns>The comma.</returns>
        /// <param name="number">number.</param>
        public static string ToComma(this double number)
        {
            return number.ToString(number >= 1000 || number <= -1000 ? "0,0.##" : "0.##");
        }

        /// <summary>
        /// Clone the specified source. Para dili ma usab ang source.
        /// </summary>
        /// <returns>The clone.</returns>
        /// <param name="source">Source.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        public static string SplitByCapitalLetter(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString()));
        }

        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static List<string> ToList<T>(this T @enum, bool isSplitByCapitalLetter = true) where T : System.Enum
        {
            List<string> businessType = new List<string>();
            foreach (T enumString in System.Enum.GetValues(typeof(T)))
            {
                if (isSplitByCapitalLetter)
                    businessType.Add(enumString.ToString().SplitByCapitalLetter().Replace("And", "&"));
                else
                    businessType.Add(enumString.ToString());
            }
            return businessType;
        }

        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> obj)
        {
            return JsonConvert.DeserializeObject<ObservableCollection<T>>(JsonConvert.SerializeObject(obj));
        }
    }
}

