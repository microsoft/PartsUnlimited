// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

namespace PartsUnlimited.Models
{
    public interface IProduct
    {
        string SkuNumber { get; }
        int ProductId { get; set; }
        int RecommendationId { get; }
        int CategoryId { get; }
        string Title { get; }
        decimal Price { get; }
        decimal SalePrice { get; }
        string ProductArtUrl { get; set; }
        Category Category { get; set; }
        string Description { get; }
        string ProductDetails { get; }
        string id { get; set; }
    }
}