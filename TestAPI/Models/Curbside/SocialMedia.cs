using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAPI.Models.Curbside
{
    public class SocialMedia : BaseModel
    {
        public string SocialId { get; set; }
        public int ProfileId { get; set; }

        public SocialMedia()
        {
        }
    }
}
