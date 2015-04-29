// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace PartsUnlimited.WebJobs.UpdateProductInventory
{
    public class ProductMessage
    {
        public List<ProductItem> ProductList { get; set; }
    }
}