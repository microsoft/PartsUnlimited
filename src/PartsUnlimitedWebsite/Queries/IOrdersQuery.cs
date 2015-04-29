// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using PartsUnlimited.ViewModels;
using System;
using System.Threading.Tasks;

namespace PartsUnlimited.Queries
{
    public interface IOrdersQuery
    {
        Task<Order> FindOrderAsync(int id);
        Task<OrdersModel> IndexHelperAsync(string username, DateTime? start, DateTime? end, int count, string invalidOrderSearch, bool isAdminSearch);
    }
}