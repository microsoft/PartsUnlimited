// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PartsUnlimited.Cache;
using PartsUnlimited.Models;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "Announcement")]
    public class AnnouncementComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly ICacheCoordinator _cacheCoordinator;

        public AnnouncementComponent(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator)
        {
            _db = context;
            _cacheCoordinator = cacheCoordinator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var key = CacheConstants.Key.AnnouncementProduct;
            var options = new PartsUnlimitedCacheOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            Product announcementProduct = await _cacheCoordinator.GetAsync(key, GetLatestProduct, new InvokerOptions().WithCacheOptions(options));
            return View(announcementProduct);
        }

        private Product GetLatestProduct()
        {
            var latestProduct = _db.Products.OrderByDescending(a => a.Created).FirstOrDefault();
            if ((latestProduct != null) && ((latestProduct.Created - DateTime.UtcNow).TotalDays <= 2))
            {
                return latestProduct;
            }

            return null;
        }
    }
}
