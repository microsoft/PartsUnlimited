// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using PartsUnlimited.Cache;
using PartsUnlimited.Models;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "CategoryMenu")]
    public class CategoryMenuComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly ICacheCoordinator _cacheCoordinator;

        public CategoryMenuComponent(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator)
        {
            _db = context;
            _cacheCoordinator = cacheCoordinator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var options = new PartsUnlimitedCacheOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).WithSecondLevelCacheAsRatio(0.5m);
            List<Category> categoryList = await _cacheCoordinator.GetAsync(CacheConstants.Key.Category, 
                GetCategories(), new CacheCoordinatorOptions().WithCacheOptions(options));
            return View(categoryList);
        }

        private Func<Task<List<Category>>> GetCategories()
        {
            return async () => await _db.Categories.ToListAsync();
        }
    }
}
