// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using System.Collections.Generic;

namespace PartsUnlimited.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> NewProducts { get; set; }
        public List<Product> TopSellingProducts { get; set; }
        public List<CommunityPost> CommunityPosts { get; set; }
    }
}