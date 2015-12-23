// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using Microsoft.AspNet.Mvc.ModelBinding;

namespace PartsUnlimited.Models
{
    public class ProductBuilder : IProductBuilder
    {
        public IProduct Build(ModelBindingContext context)
        {
            var product = new Product
            {
                SkuNumber = context.GetValue<string>("SkuNumber"),
                ProductId = context.GetValue<int>("ProductId"),
                RecommendationId = context.GetValue<int>("RecommendationId"),
                CategoryId = context.GetValue<int>("CategoryId"),
                Title = context.GetValue<string>("Title"),
                Price = context.GetValue<decimal>("Price"),
                SalePrice = context.GetValue<decimal>("SalePrice"),
                ProductArtUrl = context.GetValue<string>("ProductArtUrl"),
                Description = context.GetValue<string>("Description"),
                ProductDetails = context.GetValue<string>("ProductDetails"),
                Category = context.GetValue<Category>("Category")
            };
            return product;
        }
    }
}