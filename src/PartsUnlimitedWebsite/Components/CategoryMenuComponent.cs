// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Framework.Caching.Memory;
using PartsUnlimited.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Cache;
using PartsUnlimited.Controllers;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "CategoryMenu")]
    public class CategoryMenuComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IPartsUnlimitedCache _cache;

        public CategoryMenuComponent(IPartsUnlimitedContext context, IPartsUnlimitedCache cache)
        {
            _db = context;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Category> categoryList = null;
            var categoryResult = _cache.TryGetValue<List<Category>>("category");
            if (!categoryResult.HasValue)
            {
                categoryList = await GetCategories();

                if (categoryList != null)
                {
                    _cache.Set(
                        "categoryList", categoryList,
                        new PartsUnlimitedMemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
                }
            }
            else
            {
                categoryList = categoryResult.Value;
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
