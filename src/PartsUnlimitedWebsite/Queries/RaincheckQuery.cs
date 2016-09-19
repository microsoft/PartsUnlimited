// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace PartsUnlimited.Queries
{
    public class RaincheckQuery : IRaincheckQuery
    {
        private readonly IPartsUnlimitedContext _context;

        public RaincheckQuery(IPartsUnlimitedContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Raincheck>> GetAllAsync()
        {
            var rainchecks = await _context.RainChecks.AsAsyncEnumerable().ToList();

            foreach (var raincheck in rainchecks)
            {
                await FillRaincheckValuesAsync(raincheck);
            }

            return rainchecks;
        }

        public async Task<Raincheck> FindAsync(int id)
        {
            var raincheck = await _context.RainChecks.AsAsyncEnumerable().FirstOrDefault(r => r.RaincheckId == id);

            if (raincheck == null)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            await FillRaincheckValuesAsync(raincheck);

            return raincheck;
        }

        public async Task<int> AddAsync(Raincheck raincheck)
        {
            var addedRaincheck = _context.RainChecks.Add(raincheck);

            await _context.SaveChangesAsync(CancellationToken.None);

            return addedRaincheck.Entity.RaincheckId;
        }

        /// <summary>
        /// Lazy loading is not currently available with EF 7.0, so this loads the Store/Product/Category information
        /// </summary>
        private async Task FillRaincheckValuesAsync(Raincheck raincheck)
        {
            raincheck.IssuerStore = await _context.Stores.AsAsyncEnumerable().First(s => s.StoreId == raincheck.StoreId);
            raincheck.Product = await _context.Products.AsAsyncEnumerable().First(p => p.ProductId == raincheck.ProductId);
            raincheck.Product.Category = await _context.Categories.AsAsyncEnumerable().First(c => c.CategoryId == raincheck.Product.CategoryId);
        }
    }
}