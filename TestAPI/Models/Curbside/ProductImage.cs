using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAPI.Models.Curbside
{
    public class ProductImage 
    {
        [Key]
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ImageId")]
        public ImageUrl Image { get; set; }
        public ProductImage()
        {
        }
    }
}
