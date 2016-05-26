// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;
using Microsoft.Azure.Documents;
using PartsUnlimited.Models;
namespace PartsUnlimited.Repository
{
    public class LightingRelatedProductQueryStrategy : IRelatedProductsQueryStrategy
    {
        public bool AppliesTo(Product product)
        {
            return product.Category.Name.Equals("Lighting", StringComparison.CurrentCultureIgnoreCase);
        }
        public SqlQuerySpec BuildQuery(Product product)
        {
            return new SqlQuerySpec("SELECT * " +
                                    "FROM products " +
                                    "ORDER BY products.ProductDetailList.Brightness.Lumens DESC");
        }
    }
}