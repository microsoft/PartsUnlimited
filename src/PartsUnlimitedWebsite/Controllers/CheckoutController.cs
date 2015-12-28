// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using PartsUnlimited.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PartsUnlimited.Cache;
using PartsUnlimited.Repository;

namespace PartsUnlimited.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly ICacheCoordinator _cacheCoordinator;
        private readonly IProductRepository _productRepository;

        public CheckoutController(IPartsUnlimitedContext context, ICacheCoordinator cacheCoordinator, IProductRepository productRepository)
        {
            _db = context;
            _cacheCoordinator = cacheCoordinator;
            _productRepository = productRepository;
        }

        //
        // GET: /Checkout/

        public async Task<IActionResult> AddressAndPayment()
        {
            var id = User.GetUserId();
            var user = await _db.Users.FirstOrDefaultAsync(o => o.Id == id);

            var order = new Order
            {
                Name = user.Name,
                Email = user.Email,
                Username = user.UserName
            };

            return View(order);
        }

        //
        // POST: /Checkout/AddressAndPayment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment(Order order)
        {
            var formCollection = await HttpContext.Request.ReadFormAsync();

            try
            {
                var cacheOptions = new PartsUnlimitedCacheOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60));
                var promos = await _cacheCoordinator.GetAsync(CacheConstants.Key.Promos, () => _db.Promo.ToListAsync(), 
                    new CacheCoordinatorOptions().WithCacheOptions(cacheOptions));
                var promoCode = formCollection["PromoCode"].FirstOrDefault();
                var promo = promos.FirstOrDefault(p => p.Name.Equals(promoCode, StringComparison.InvariantCultureIgnoreCase));
                if (promo == null)
                {
                    return View(order);
                }

                order.Username = HttpContext.User.GetUserName();
                order.OrderDate = DateTime.Now;
                order.PromoId = promo.PromoId;

                //Add the Order
                _db.Orders.Add(order);

                //Process the order
                var cart = ShoppingCart.GetCart(_db, HttpContext, _productRepository);
                await cart.CreateOrder(order);

                // Save all changes
                await _db.SaveChangesAsync(HttpContext.RequestAborted);

                return RedirectToAction("Complete", new { id = order.OrderId });
            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete

        public IActionResult Complete(int id)
        {
            // Validate customer owns this order
            Order order = _db.Orders.FirstOrDefault(
                o => o.OrderId == id &&
                o.Username == HttpContext.User.GetUserName());

            if (order != null)
            {
                return View(order);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
