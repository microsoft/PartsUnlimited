// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Repository;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "CartSummary")]
    public class CartSummaryComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IProductLoader _loader;

        public CartSummaryComponent(IPartsUnlimitedContext context, IProductLoader loader)
        {
            _db = context;
            _loader = loader;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartItems = await GetCartItems();

            ViewBag.CartCount = cartItems.Select(x => x.Count).Sum();
            ViewBag.CartSummary = string.Join("\n", cartItems.Distinct());

            return View();
        }

        private async Task<IOrderedEnumerable<CartSummeryComponentModel>> GetCartItems()
        {
            var cart = ShoppingCart.GetCart(_db, Context, _loader);
            List<CartItem> cartItems = await cart.GetCartItems();
            return cartItems
                .Select(a => new CartSummeryComponentModel { Title = a.Product.Title, Count = a.Count })
                .OrderBy(x => x.Title);
        }
    }

    public class CartSummeryComponentModel
    {
        public int Count { get; internal set; }
        public string Title { get; internal set; }
    }
}