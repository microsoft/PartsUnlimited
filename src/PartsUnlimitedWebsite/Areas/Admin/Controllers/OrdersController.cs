// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using PartsUnlimited.Queries;
using PartsUnlimited.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Areas.Admin.Controllers
{
    public class OrdersController : AdminController
    {
        private readonly IOrdersQuery _ordersQuery;

        public OrdersController(IOrdersQuery ordersQuery)
        {
            _ordersQuery = ordersQuery;
        }

        public async Task<IActionResult> Index(string username, DateTime? start, DateTime? end, string invalidOrderSearch)
        {
            return View(await _ordersQuery.IndexHelperAsync(username, start, end, 10, invalidOrderSearch, true));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", new { invalidOrderSearch = Request.Query["id"] });
            }

            var order = await _ordersQuery.FindOrderAsync(id.Value);

            if (order == null)
            {
                return RedirectToAction("Index", new { invalidOrderSearch = id.ToString() });
            }
            var itemsCount = order.OrderDetails.Sum(x => x.Quantity);
            var subTotal = order.OrderDetails.Sum(x => x.Quantity * x.Product.Price);
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

            var viewModel = new OrderDetailsViewModel
            {
                OrderCostSummary = costSummary,
                Order = order
            };

            return View(viewModel);
        }
    }
}
