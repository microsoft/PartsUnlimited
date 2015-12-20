// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Cache;
using PartsUnlimited.Repository;

namespace PartsUnlimited.Controllers
{
    public class StoreController : Controller
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly ICacheCoordinator _cacheCoordinator;
        private readonly IProductRepository _productRepository;

        public StoreController(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator, IProductRepository productRepository)
        {
            _db = context;
            _cacheCoordinator = cacheCoordinator;
            _productRepository = productRepository;
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
        public async Task<IActionResult> Browse(int categoryId)
        {
            // Retrieve Category category and its Associated associated Products products from database
            // TODO [EF] Swap to native support for loading related data when available
            var categoryModel = _db.Categories.Single(g => g.CategoryId == categoryId);
            var products = await _productRepository.LoadProductsForCategory(categoryModel.CategoryId);
            var browse = new Browse { Products  = products, Category = categoryModel};
            return View(browse);
        }

        public async Task<IActionResult> Details(int id)
        {
            string productKey = CacheConstants.Key.ProductKey(id);
            var options = new PartsUnlimitedCacheOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .WithSecondLevelCacheAsRatio(0.33m);
            var productData = await _cacheCoordinator.GetAsync(productKey, LoadProductWithId(id), 
                new CacheCoordinatorOptions().WithCacheOptions(options));
            return View(productData);
        }

        private Func<Task<dynamic>> LoadProductWithId(int id)
        {
            return async () =>
            {
                dynamic productData = await _productRepository.Load(id);
                int categoryId = productData.CategoryId;
                productData.Category = _db.Categories.Single(g => g.CategoryId == categoryId);
                return productData;
            };
        }
    }
}