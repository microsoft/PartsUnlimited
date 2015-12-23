// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PartsUnlimited.Cache;
using PartsUnlimited.Models;
using PartsUnlimited.Repository;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "Announcement")]
    public class AnnouncementComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly ICacheCoordinator _cacheCoordinator;
        private readonly IProductRepository _productRepository;

        public AnnouncementComponent(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator, IProductRepository productRepository)
        {
            _db = context;
            _cacheCoordinator = cacheCoordinator;
            _productRepository = productRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var key = CacheConstants.Key.AnnouncementProduct;
            var options = new PartsUnlimitedCacheOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            dynamic announcementProduct = await _cacheCoordinator.GetAsync(key, GetLatestProduct, new CacheCoordinatorOptions().WithCacheOptions(options));
            return View(announcementProduct);
        }

        private async Task<dynamic> GetLatestProduct()
        {
            return await _productRepository.GetLatestProduct();
            
        }
    }
}
