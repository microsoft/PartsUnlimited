// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsUnlimited.Search
{
    public interface IProductSearch
    {
        Task<IEnumerable<dynamic>> Search(string query);
    }
}