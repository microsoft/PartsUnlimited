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
        private readonly ICacheCoordinator _cacheCoordinator;

        public StoreController(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator)
        {
            _db = context;
            _cacheCoordinator = cacheCoordinator;
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
            string productKey = CacheConstants.Key.ProductKey(id);
            var options = new PartsUnlimitedCacheOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
            var productData = await _cacheCoordinator.GetAsync(productKey, LoadProductWithId(id), 
                new CacheCoordinatorOptions().WithCacheOptions(options));
            return View(productData);
        }

        private Func<Product> LoadProductWithId(int id)
        {
            return delegate
            {
                var productData = _db.Products.Single(a => a.ProductId == id);
                productData.Category = _db.Categories.Single(g => g.CategoryId == productData.CategoryId);
                return productData;
            };
        }
    }
}