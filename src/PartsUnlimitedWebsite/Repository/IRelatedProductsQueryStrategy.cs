// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using Microsoft.Azure.Documents;
using PartsUnlimited.Models;
namespace PartsUnlimited.Repository
{
    public interface IRelatedProductsQueryStrategy
    {
        bool AppliesTo(Product product);
        SqlQuerySpec BuildQuery(Product product);
    }
}