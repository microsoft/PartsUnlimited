// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System.Linq;
using Microsoft.Azure.Documents;
using PartsUnlimited.Models;
namespace PartsUnlimited.Repository
{
    public class DefaultRelatedProductQueryStrategy : IRelatedProductsQueryStrategy
    {
        public bool AppliesTo(Product product)
        {
            return true;
        }
        public SqlQuerySpec BuildQuery(Product product)
        {
            //Query for other products with the same first value in the product detail list.
            var query = new SqlQuerySpec("SELECT * " +
                                         "FROM p " +
                                         "WHERE p.ProductDetailList[@key] = @value")
            {
                Parameters =
                    new SqlParameterCollection
                    {
                        new SqlParameter("@key", product.ProductDetailList.FirstOrDefault().Key),
                        new SqlParameter("@value", product.ProductDetailList.FirstOrDefault().Value)
                    }
            };
            return query;
        }
    }
}