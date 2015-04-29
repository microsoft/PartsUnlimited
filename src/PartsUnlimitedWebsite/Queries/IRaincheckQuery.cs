// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsUnlimited.Queries
{
    public interface IRaincheckQuery
    {
        Task<int> AddAsync(Raincheck raincheck);
        Task<Raincheck> FindAsync(int id);
        Task<IEnumerable<Raincheck>> GetAllAsync();
    }
}