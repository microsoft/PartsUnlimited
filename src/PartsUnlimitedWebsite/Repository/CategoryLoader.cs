// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using PartsUnlimited.Cache;
using PartsUnlimited.Models;

namespace PartsUnlimited.Repository
{
    public class CategoryLoader : ICategoryLoader
    {
        private readonly IPartsUnlimitedContext _context;
        private readonly ICacheCoordinator _cacheCoordinator;

        public CategoryLoader(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator)
        {
            _context = context;
            _cacheCoordinator = cacheCoordinator;
        }

        public async Task<Category> Load(int categoryId)
        {
            IEnumerable<Category> categories = await LoadAll();
            return categories.FirstOrDefault(c => c.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Category>> LoadAll()
        {
            var options = new PartsUnlimitedCacheOptions()
                 .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).WithSecondLevelCacheAsRatio(0.5m);
            return await _cacheCoordinator.GetAsync(CacheConstants.Key.Category,
                GetCategories(), new CacheCoordinatorOptions().WithCacheOptions(options));
        }

        private Func<Task<List<Category>>> GetCategories()
        {
            return async () => await _context.Categories.ToListAsync();
        }
    }
}