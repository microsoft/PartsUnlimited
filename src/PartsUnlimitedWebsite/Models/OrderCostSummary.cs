// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.ViewModels
{
    public class OrderCostSummary
    {
        public string CartSubTotal { get; set; }
        public string CartShipping { get; set; }
        public string CartTax { get; set; }
        public string CartTotal { get; set; }
    }
}