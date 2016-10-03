// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using PartsUnlimited.Models;
using PartsUnlimited.Telemetry;
using PartsUnlimited.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PartsUnlimited.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly ITelemetryProvider _telemetry;
        private readonly IAntiforgery _antiforgery;

        public ShoppingCartController(IPartsUnlimitedContext context, ITelemetryProvider telemetryProvider, IAntiforgery antiforgery)
        {
            _db = context;
            _telemetry = telemetryProvider;
            _antiforgery = antiforgery;
        }

        //
        // GET: /ShoppingCart/

        public IActionResult Index()
        {
            var cart = ShoppingCart.GetCart(_db, HttpContext);

            var items = cart.GetCartItems();
            var itemsCount = items.Sum(x => x.Count);
            var subTotal = items.Sum(x => x.Count * x.Product.Price);
            var shipping = itemsCount * (decimal)5.00;
            var tax = (subTotal + shipping) * (decimal)0.05;
            var total = subTotal + shipping + tax;

            var costSummary = new OrderCostSummary
            {
                CartSubTotal = subTotal.ToString("C"),
                CartShipping = shipping.ToString("C"),
                CartTax = tax.ToString("C"),
                CartTotal = total.ToString("C")
            };


            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = items,
                CartCount = itemsCount,
                OrderCostSummary = costSummary
            };

            // Track cart review event with measurements
            _telemetry.TrackTrace("Cart/Server/Index");

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /ShoppingCart/AddToCart/5

        public async Task<IActionResult> AddToCart(int id)
        {
            // Retrieve the product from the database
            var addedProduct = _db.Products
                .Single(product => product.ProductId == id);

            // Start timer for save process telemetry
            var startTime = System.DateTime.Now;

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(_db, HttpContext);

            cart.AddToCart(addedProduct);

            await _db.SaveChangesAsync(HttpContext.RequestAborted);

            // Trace add process
            var measurements = new Dictionary<string, double>()
            {
                {"ElapsedMilliseconds", System.DateTime.Now.Subtract(startTime).TotalMilliseconds }
            };
            _telemetry.TrackEvent("Cart/Server/Add", null, measurements);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(Request request)
        {
            // Retrieve the current user's shopping cart
            var cart = ShoppingCart.GetCart(_db, HttpContext);

            // Get the name of the album to display confirmation
            var cartItem = await _db.CartItems
                .Where(item => item.CartItemId == request.Id)
                .Include(c => c.Product)
                .SingleOrDefaultAsync();

            string message;
            int itemCount;
            if (cartItem != null)
            {
                // Remove from cart
                itemCount = cart.RemoveFromCart(request.Id);

                await _db.SaveChangesAsync(request.CancellationToken);

                string removed = (itemCount > 0) ? " 1 copy of " : string.Empty;
                message = removed + cartItem.Product.Title + " has been removed from your shopping cart.";
            }
            else
            {
                itemCount = 0;
                message = "Could not find this item, nothing has been removed from your shopping cart.";
            }

            // Display the confirmation message

            var results = new ShoppingCartRemoveViewModel
            {
                Message = message,
                CartTotal = cart.GetTotal().ToString(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = request.Id
            };

            return Json(results);
        }
    }
}