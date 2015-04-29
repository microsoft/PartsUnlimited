// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.WebJobs.UpdateProductInventory
{
    public class ProductItem
    {
        public string SkuNumber { get; set; }
        public int Inventory { get; set; }
        public int LeadTime { get; set; }
    }
}