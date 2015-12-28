using System.Collections.Generic;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
namespace PartsUnlimited.Models
{
    public class Browse
    {
        public IEnumerable<IProduct> Products { get; set; }
        public Category Category { get; set; } 
    }
}