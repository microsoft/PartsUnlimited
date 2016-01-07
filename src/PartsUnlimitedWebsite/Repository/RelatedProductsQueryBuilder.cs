// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using PartsUnlimited.Models;
namespace PartsUnlimited.Repository
{
    public class RelatedProductsQueryBuilder
    {
        private readonly IEnumerable<IRelatedProductsQueryStrategy> _strategies;
        public RelatedProductsQueryBuilder(IEnumerable<IRelatedProductsQueryStrategy> strategies)
        {
            _strategies = strategies;
        }
        public SqlQuerySpec BuildQuery(Product product)
        {
            var strategy = _strategies.FirstOrDefault(s => s.AppliesTo(product));
            if (strategy != null)
            {
                return strategy.BuildQuery(product);
            }
            //There should always be a default one.
            throw new InvalidOperationException("Unable to find query strategy.");
        }
    }
}