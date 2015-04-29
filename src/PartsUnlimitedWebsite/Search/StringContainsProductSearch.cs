// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Search
{
    public class StringContainsProductSearch : IProductSearch
    {
        private readonly IPartsUnlimitedContext _context;

        public StringContainsProductSearch(IPartsUnlimitedContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> Search(string query)
        {
            var lowercase_query = query.ToLower();

            var q = _context.Products
                .Where(p => p.Title.ToLower().Contains(lowercase_query));

            return await q.ToAsyncEnumerable().ToList();
        }
    }
}