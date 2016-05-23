// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace PartsUnlimited.Models
{
    public interface IProductLoader
    {
        Task<IProduct> Load(int productId);
    }
}