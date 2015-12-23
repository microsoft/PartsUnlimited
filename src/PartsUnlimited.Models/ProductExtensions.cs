// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System.Linq;

namespace PartsUnlimited.Models
{
    public static class ProductExtensions
    {
        public static IQueryable<IProduct> Sort(this IQueryable<Product> products, SortField sortField, SortDirection sortDirection)
        {
            if (sortField == SortField.Name)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Category.Name);
                }

                return products.OrderByDescending(o => o.Category.Name);
            }

            if (sortField == SortField.Price)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Price);
                }

                return products.OrderByDescending(o => o.Price);
            }

            if (sortField == SortField.Title)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Title);
                }

                return products.OrderByDescending(o => o.Title);
            }

            // Should not reach here, but return products for compiler
            return products;
        }

    }
}