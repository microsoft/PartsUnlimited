// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using PartsUnlimited.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Cache;
using PartsUnlimited.Controllers;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "Announcement")]
    public class AnnouncementComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IPartsUnlimitedCache _cache;

        public AnnouncementComponent(IPartsUnlimitedContext context, IPartsUnlimitedCache cache)
        {
            _db = context;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Product announcementProduct = null;
            var cacheResult = _cache.TryGetValue<Product>("announcementProduct");
            if (!cacheResult.HasValue)
            {
                announcementProduct = await GetLatestProduct();

                if (announcementProduct != null)
                {
                    _cache.Set("announcementProduct", announcementProduct, new PartsUnlimitedMemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
                }
            }
            else
            {
                announcementProduct = cacheResult.Value;
            }

            return View(announcementProduct);
        }

        private Task<Product> GetLatestProduct()
        {
            var latestProduct = _db.Products.OrderByDescending(a => a.Created).FirstOrDefault();
            if ((latestProduct != null) && ((latestProduct.Created - DateTime.UtcNow).TotalDays <= 2))
            {
                return Task.FromResult(latestProduct);
            }
            else
            {
                return Task.FromResult<Product>(null);
            }
        }
    }
}
