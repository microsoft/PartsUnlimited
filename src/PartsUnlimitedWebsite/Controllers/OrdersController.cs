// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsUnlimited.Queries;
using PartsUnlimited.Telemetry;
using PartsUnlimited.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PartsUnlimited.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrdersQuery _ordersQuery;
        private readonly ITelemetryProvider _telemetry;

        public OrdersController(IOrdersQuery ordersQuery, ITelemetryProvider telemetryProvider)
        {
            _ordersQuery = ordersQuery;
            _telemetry = telemetryProvider;
        }

        public async Task<IActionResult> Index(DateTime? start, DateTime? end, string invalidOrderSearch)
        {
            var username = User.Identity.Name;

            return View(await _ordersQuery.IndexHelperAsync(username, start, end, 10, invalidOrderSearch, false));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _telemetry.TrackTrace("Order/Server/NullId");
                return RedirectToAction("Index", new { invalidOrderSearch = Request.Query["id"] });
            }
          
            var order = await _ordersQuery.FindOrderAsync(id.Value);
            var username = User.Identity.Name;

            // If the username isn't the same as the logged in user, return as if the order does not exist
            if (order == null || !String.Equals(order.Username, username, StringComparison.Ordinal))
            {
                _telemetry.TrackTrace("Order/Server/UsernameMismatch");
                return RedirectToAction("Index", new { invalidOrderSearch = id.ToString() });
            }

            // Capture order review event for analysis
            var eventProperties = new Dictionary<string, string>()
                {
                    {"Id", id.ToString() },
                    {"Username", username }
                };
            if (order.OrderDetails == null)
            {
                _telemetry.TrackEvent("Order/Server/NullDetails", eventProperties, null);
            }
            else
            {
                var eventMeasurements = new Dictionary<string, double>()
                {
                    {"LineItemCount", order.OrderDetails.Count }
                };
                _telemetry.TrackEvent("Order/Server/Details", eventProperties, eventMeasurements);
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
