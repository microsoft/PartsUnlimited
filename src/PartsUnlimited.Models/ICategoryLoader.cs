// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsUnlimited.Models
{
    public interface ICategoryLoader
    {
        Task<Category> Load(int categoryId);
        Task<IEnumerable<Category>> LoadAll();
    }
}