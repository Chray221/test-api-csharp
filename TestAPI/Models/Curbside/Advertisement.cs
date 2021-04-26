using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAPI.Models.Curbside
{
    public class Advertisement : BaseModel
    { /*
        {
          "id": 1,
          "image": "https://lh3.googleusercontent.com/4EDgkPTDJTcXchVjAndkJB-d0XHG5GKg6x0yvNVrvqb0wovAPYHjKswGkM6dqqgTu_JUySA=s170",
          "url": "https://lh3.googleusercontent.com/4EDgkPTDJTcXchVjAndkJB-d0XHG5GKg6x0yvNVrvqb0wovAPYHjKswGkM6dqqgTu_JUySA=s170"
        },
        */
        [ForeignKey("Image")]
        public int ImageId { get; set; }
        [NotMapped]
        public virtual ImageUrl Image { get; set; }
        public string Url { get; set; }
        public Advertisement()
        {
        }
    }
}
