// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace PartsUnlimited.WebJobs.ProcessOrder
{
    public class LegacyOrder
    {
        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Discount { get; set; }

        public List<LegacyOrderItem> Items { get; set; }
    }
}