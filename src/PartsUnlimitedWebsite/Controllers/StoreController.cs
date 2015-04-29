// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using PartsUnlimited.Models;
using System;
using System.Linq;

namespace PartsUnlimited.Controllers
{
    public class StoreController : Controller
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IMemoryCache _cache;

        public StoreController(IPartsUnlimitedContext context, IMemoryCache memoryCache)
        {
            _db = context;
            _cache = memoryCache;
        }

        //
        // GET: /Store/

        public IActionResult Index()
        {
            var category = _db.Categories.ToList();

            return View(category);
        }

        //
        // GET: /Store/Browse?category=Brakes

        public IActionResult Browse(int categoryId)
        {
            // Retrieve Category category and its Associated associated Products products from database

            // TODO [EF] Swap to native support for loading related data when available
            var categoryModel = _db.Categories.Single(g => g.CategoryId == categoryId);
            categoryModel.Products = _db.Products.Where(a => a.CategoryId == categoryModel.CategoryId).ToList();

            return View(categoryModel);
        }

        public IActionResult Details(int id)
        {
            var product = _cache.GetOrSet(string.Format("product_{0}", id), context =>
            {
                //Remove it from cache if not retrieved in last 10 minutes
                context.SetSlidingExpiration(TimeSpan.FromMinutes(10));

                var productData = _db.Products.Single(a => a.ProductId == id);

                // TODO [EF] We don't query related data as yet. We have to populate this until we do automatically.
                productData.Category = _db.Categories.Single(g => g.CategoryId == productData.CategoryId);
                return productData;
            });

            return View(product);
        }
    }
}