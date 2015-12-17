// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace PartsUnlimited.Models
{
    public class ShoppingCart
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IProductLoader _productLoader;
        private string ShoppingCartId { get; set; }

        public ShoppingCart(IPartsUnlimitedContext db, IProductLoader productLoader)
        {
            _db = db;
            _productLoader = productLoader;
        }

        public static ShoppingCart GetCart(IPartsUnlimitedContext db, HttpContext context, IProductLoader loader)
        {
            var cart = new ShoppingCart(db, loader);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public void AddToCart(Product product)
        {
            // Get the matching cart and product instances
            var cartItem = _db.CartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ProductId == product.ProductId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };

                _db.CartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                cartItem.Count++;
            }
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = _db.CartItems.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.CartItemId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _db.CartItems.Remove(cartItem);
                }
            }

            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = _db.CartItems.Where(cart => cart.CartId == ShoppingCartId);
            _db.CartItems.RemoveRange(cartItems.ToArray());
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var cartItems = _db.CartItems.Where(cart => cart.CartId == ShoppingCartId).ToList();
            //TODO: Auto population of the related product data not available until EF feature is lighted up.
            foreach (var cartItem in cartItems)
            {
                cartItem.Product = await _productLoader.Load(cartItem.ProductId);
            }

            return cartItems;
        }

        public int GetCount()
        {
            int sum = 0;
            //https://github.com/aspnet/EntityFramework/issues/557
            // Get the count of each item in the cart and sum them up
            var cartItemCounts = (from cartItems in _db.CartItems
                                  where cartItems.CartId == ShoppingCartId
                                  select (int?)cartItems.Count);

            cartItemCounts.ForEachAsync(carItemCount =>
            {
                if (carItemCount.HasValue)
                {
                    sum += carItemCount.Value;
                }
            });

            // Return 0 if all entries are null
            return sum;
        }

       public async Task<int> CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = await GetCartItems();

            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in cartItems)
            {
                var product = await _productLoader.Load(item.ProductId);
                    

                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderId = order.OrderId,
                    UnitPrice = product.Price,
                    Quantity = item.Count,
                };

                // Set the order total of the shopping cart
                orderTotal += (item.Count * product.Price);

                _db.OrderDetails.Add(orderDetail);
            }

            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Empty the shopping cart
            EmptyCart();

            // Return the OrderId as the confirmation number
            return order.OrderId;
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContext context)
        {
            var sessionCookie = context.Request.Cookies["Session"];
            string cartId = null;

            if (string.IsNullOrWhiteSpace(sessionCookie))
            {
                //A GUID to hold the cartId. 
                cartId = Guid.NewGuid().ToString();

                // Send cart Id as a cookie to the client.
                context.Response.Cookies.Append("Session", cartId);
            }
            else
            {
                cartId = sessionCookie;
            }

            return cartId;
        }
    }
}