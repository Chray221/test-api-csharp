using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAPI.Models.Curbside
{
    public class ProductItemCategory : BaseModel
    {
        public int ProductId { get; set; }
        public ProductCategory Category { get; set; }

        public ProductItemCategory()
        {
        }
    }
}
