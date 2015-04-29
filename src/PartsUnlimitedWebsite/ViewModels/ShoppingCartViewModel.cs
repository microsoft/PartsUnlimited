// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using PartsUnlimited.Models;

namespace PartsUnlimited.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public int CartCount { get; set; }
        public OrderCostSummary OrderCostSummary { get; set; }
    }
}
