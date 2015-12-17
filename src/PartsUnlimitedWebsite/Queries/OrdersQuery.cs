// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using PartsUnlimited.ViewModels;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Cache;

namespace PartsUnlimited.Queries
{
    public class OrdersQuery : IOrdersQuery
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly ICacheCoordinator _cacheCoordinator;
        private readonly IProductLoader _productLoader;

        public OrdersQuery(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator, IProductLoader productLoader)
        {
            _db = context;
            _cacheCoordinator = cacheCoordinator;
            _productLoader = productLoader;
        }

        public async Task<OrdersModel> IndexHelperAsync(string username, DateTime? start, DateTime? end, int count, string invalidOrderSearch, bool isAdminSearch)
        {
            // The datetime submitted is only expected to have a resolution of a day, so we remove
            // the time of day from start and end.  We add a day for queryEnd to ensure the date
            // includes the whole day requested
            var queryStart = (start ?? DateTime.Now).Date;
            var queryEnd = (end ?? DateTime.Now).Date.AddDays(1).AddSeconds(-1);

            var results = await GetOrderQuery(username, queryStart, queryEnd, count).ToListAsync();

            await FillOrderDetails(results);

            return new OrdersModel(results, username, queryStart, queryEnd, invalidOrderSearch, isAdminSearch);
        }

        private IQueryable<Order> GetOrderQuery(string username, DateTime start, DateTime end, int count)
        {
            if (string.IsNullOrEmpty(username))
            {
                return _db
                    .Orders
                    .Where(o => o.OrderDate < end && o.OrderDate >= start)
                    .OrderBy(o => o.OrderDate)
                    .Take(count);
            }

            return _db
                .Orders
                .Where(o => o.OrderDate < end && o.OrderDate >= start && o.Username == username)
                .OrderBy(o => o.OrderDate)
                .Take(count);
        }

        public async Task<Order> FindOrderAsync(int id)
        {
            var orders = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

            if (orders == null)
            {
                return null;
            }
            else
            {
                await FillOrderDetails(new[] { orders });
                return orders;
            }
        }

        private async Task FillOrderDetails(IEnumerable<Order> orders)
        {
            var promo = await LoadPromoCodes();
            foreach (var order in orders)
            {
                if (order.PromoId.HasValue)
                {
                    order.Promo = promo.First(s => s.PromoId == order.PromoId);
                }
                order.OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == order.OrderId).ToListAsync();

                foreach (var details in order.OrderDetails)
                {
                    var key = CacheConstants.Key.ProductKey(details.ProductId);
                    var cacheOptions = new PartsUnlimitedCacheOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
                    var invokerOptions = new CacheCoordinatorOptions().WithCacheOptions(cacheOptions).WhichFailsOver();
                    var product = await _cacheCoordinator.GetAsync( key, () => _productLoader.Load(details.ProductId), invokerOptions);
                    details.Product = product;
                }
            }
        }

        private async Task<List<Promo>> LoadPromoCodes()
        {
            var key = CacheConstants.Key.Promos;
            var cacheOptions = new PartsUnlimitedCacheOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60));
            var invokerOptions = new CacheCoordinatorOptions().WithCacheOptions(cacheOptions).WhichFailsOver();
            List<Promo> product = await _cacheCoordinator.GetAsync(key, () => _db.Promo.ToListAsync(), invokerOptions);
            return product;
        }
    }
}
