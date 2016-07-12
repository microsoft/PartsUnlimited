// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PartsUnlimited.Models
{
    public class Product
    {
        [Required]
        [Display(Name = "Sku Number")]
        public string SkuNumber { get; set; }

        [ScaffoldColumn(false)]
        public int ProductId { get; set; }

        public int RecommendationId { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [Range(0.01, 500.00)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(0.01, 500.00)]
        [DataType(DataType.Currency)]
        [Display(Name = "Sale Price")]
        public decimal SalePrice { get; set; }

        [Required]
        [Display(Name = "Product Art URL")]
        [StringLength(1024)]
        public string ProductArtUrl { get; set; }

        public virtual Category Category { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }

        [ScaffoldColumn(false)]
        [BindNever]
        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Product Details")]
        public string ProductDetails { get; set; }

        public int Inventory { get; set; }

        public int LeadTime { get; set; }

        public Dictionary<string, string> ProductDetailList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ProductDetails))
                {
                    return new Dictionary<string, string>();
                }
                try
                {
                    var obj = JToken.Parse(ProductDetails);
                }
                catch (Exception)
                {
                    throw new FormatException("Product Details only accepts json format.");
                }
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(ProductDetails);
            }
        }

        /// <summary>
        /// TODO: Temporary hack to populate the orderdetails until EF does this automatically. 
        /// </summary>
        public Product()
        {
            OrderDetails = new List<OrderDetail>();
            Created = DateTime.UtcNow;
        }
    }
}
