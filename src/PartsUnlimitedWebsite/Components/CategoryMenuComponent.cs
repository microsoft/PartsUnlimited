// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using PartsUnlimited.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "CategoryMenu")]
    public class CategoryMenuComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IMemoryCache _cache;

        public CategoryMenuComponent(IPartsUnlimitedContext context, IMemoryCache memoryCache)
        {
            _db = context;
            _cache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var category = await _cache.GetOrSet("category", async context =>
            {
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return await GetCategories();
            });

            return View(category);
        }

        private async Task<List<Category>> GetCategories()
        {
            var category = await _db.Categories.ToListAsync();
            return category;
        }
    }
}
