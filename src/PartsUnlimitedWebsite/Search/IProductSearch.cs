// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsUnlimited.Search
{
    public interface IProductSearch
    {
        Task<IEnumerable<Product>> Search(string query);
    }
}