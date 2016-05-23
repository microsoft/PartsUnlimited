// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;
using Microsoft.Azure.Documents;
using PartsUnlimited.Models;
namespace PartsUnlimited.Repository
{
    public class WheelsAndTiresTiresRelatedProductQueryStrategy : IRelatedProductsQueryStrategy
    {
        public bool AppliesTo(Product product)
        {
            return product.Category.Name.Equals("Wheels & Tires", StringComparison.CurrentCultureIgnoreCase)
                   && product.ProductDetailList.ContainsKey("Spokes") && product.ProductDetailList.ContainsKey("Finish");
        }
        public SqlQuerySpec BuildQuery(Product product)
        {
            var query = new SqlQuerySpec("SELECT * " +
                                         "FROM products " +
                                         "WHERE products.ProductDetailList.Spokes = @spokeCount " +
                                         "OR products.ProductDetailList.Finish = @finish");
            string spokeCount = product.ProductDetailList["Spokes"];
            string finishValue = product.ProductDetailList["Finish"];
            query.Parameters.Add(new SqlParameter("@spokeCount", spokeCount));
            query.Parameters.Add(new SqlParameter("@finish", finishValue));
            return query;
        }
    }
}