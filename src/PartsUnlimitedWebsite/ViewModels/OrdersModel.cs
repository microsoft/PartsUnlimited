// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using System;
using System.Collections.Generic;

namespace PartsUnlimited.ViewModels
{
    public class OrdersModel
    {
        public bool IsAdminSearch { get; }
        public string InvalidOrderSearch { get; }
        public IEnumerable<Order> Orders { get; }
        public string Username { get; }
        public DateTimeOffset StartDate { get; }
        public DateTimeOffset EndDate { get; }

        public OrdersModel(IEnumerable<Order> orders, string username, DateTimeOffset startDate, DateTimeOffset endDate, string invalidOrderSearch, bool isAdminSearch)
        {
            Orders = orders;
            Username = username;
            StartDate = startDate;
            EndDate = endDate;
            InvalidOrderSearch = invalidOrderSearch;
            IsAdminSearch = isAdminSearch;
        }
    }
}