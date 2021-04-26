using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TestAPI.Enums.Curbside;
using TestAPI.Extensions;

namespace TestAPI.Models.Curbside
{
    public class Product : BaseModel
    {    
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public bool IsDiscounted { get; set; }
        public bool IsDiscountPercentage { get; set; }
        public int Quantity { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsTodaysDeal { get; set; }


        [ForeignKey("Image")]
        public int ImageId { get; set; }
        [ForeignKey("Brand")]
        public int BrandId { get; set; }
        [ForeignKey("Item")]
        public int ProductId { get; set; }

        public virtual ImageUrl Image { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<ProductItemCategory> Categories { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Product Item { get; set; }
        public virtual ICollection<Product> Items { get; set; }

        public Product()
        {
        }

        public string StockStatus()
        {
            return Quantity > 10 ? "In Stock" : Quantity > 0 ? $"{Quantity} Left" : "No Stock";
        }

        public object ProductFormat()
        {
            /*
                {
                  "id": 9,
                  "name": "Tomatoes",
                  "image": "https://i.imgur.com/29X6YgE.png",
                  "category": "Cooking & Baking",
                  "original_price": 84.00,
                  "discounted_price": 0.00,
                  "is_discounted": false,
                  "stock_status": "No Stocks"
                },
             */

            //var category = Categories?.Select((categoryData, index) => categoryData.Category.ToString().Replace("And", " & "));
            //string categoryString = category == null ? null : category.Any() ? string.Join(",", category) : null;
            string categoryString = string.Join(",", Categories?.Select((categoryData, index) => categoryData.Category.ToString().Replace("And", " & ")));
            return new
            {
                id = Id,
                name = Name,
                image = Image?.ThumbUrl,
                category = categoryString,
                original_price = Price,
                discounted_price = IsDiscounted ? IsDiscountPercentage ? Price - (Price * (Discount / 100)) : Discount : Price,
                is_discounted = IsDiscounted,
                stock_status = StockStatus()
            };
        }

        public object ProductDetailsFormat()
        {
            //var category = Categories?.Select((categoryData, index) => categoryData.Category.ToString().Replace("And", " & "));
            //string categoryString = category == null ? null : category.Any() ? string.Join(",", category) : null;
            string categoryString = string.Join(",", Categories?.Select((categoryData, index) => categoryData.Category.ToString().Replace("And", " & ")));
            return new
            {
                id = Id,
                name = Name,
                description = Description,
                image = Image?.ThumbUrl,
                images = Images?.Select((imageData, index) => imageData.Image.ThumbUrl),
                category = categoryString,
                original_price = Price,
                discounted_price = IsDiscounted ? IsDiscountPercentage ? Price - (Price * (Discount / 100)) : Discount : Price,
                is_discounted = IsDiscounted,
                stock_status = StockStatus(),
                items = new[] { ProductFormat() }
            };
        }
    }
}
