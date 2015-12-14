// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Cache;

namespace PartsUnlimited.Controllers
{
    public class StoreController : Controller
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IPartsUnlimitedCache _cache;

        public StoreController(IPartsUnlimitedContext context, IPartsUnlimitedCache cache)
        {
            _db = context;
            _cache = cache;
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

        public async Task<IActionResult> Details(int id)
        {
            Product productData;
            string productKey = $"product_{id}";
            var productResult = await _cache.TryGetValue<Product>(productKey);
            if (!productResult.HasValue)
            {
                productData = _db.Products.Single(a => a.ProductId == id);
                productData.Category = _db.Categories.Single(g => g.CategoryId == productData.CategoryId);

                if (productData != null)
                {
                    await _cache.Set(
                        productKey, productData, new PartsUnlimitedMemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                }                
            }
            else
            {
                productData = productResult.Value;
            }

            return View(productData);
        }
    }
}