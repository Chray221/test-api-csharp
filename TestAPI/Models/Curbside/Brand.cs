using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAPI.Models.Curbside
{
    public class Brand : BaseModel
    {
        /*
         * {
              "id": 4,
              "name": "Dole",
              "image": "https://i.imgur.com/W7tL6s7.png",
              "product_count": 140
            },
         */
        public string Name { get; set; }
        [ForeignKey("ImageFile")]
        public int ImageId { get; set; }
        public ImageFile ImageFile { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Brand()
        {
        }

        public object BrandFormat()
        {
            return new
            {
                id = Id,
                name = Name,
                image = ImageFile?.ThumbUrl,
            };
        }

        public object BrandFormat(int product_count)
        {
            return new
            {
                id = Id,
                name = Name,
                image = ImageFile?.ThumbUrl,
                product_count
            };
        }
    }
}
