// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using PartsUnlimited.Models;

namespace PartsUnlimited.Search
{
    public interface IProductSearch
    {
        Task<IEnumerable<IProduct>> Search(string query);
    }
}