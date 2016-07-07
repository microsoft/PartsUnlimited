// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
            List<Category> categoryList;
            if (!_cache.TryGetValue("category", out categoryList))
            {
                categoryList = await GetCategories(); 

                if (categoryList != null)
                {
                    _cache.Set("categoryList", categoryList, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
                }
            }
            return View(categoryList);
        }

        private async Task<List<Category>> GetCategories()
        {
            var category = await _db.Categories.ToListAsync();
            return category;
        }
    }
}
