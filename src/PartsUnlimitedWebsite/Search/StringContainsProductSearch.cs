// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using PartsUnlimited.Repository;

namespace PartsUnlimited.Search
{
    public class StringContainsProductSearch : IProductSearch
    {
        private readonly IProductRepository _repository;

        public StringContainsProductSearch(IProductRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<dynamic>> Search(string query)
        {
            var searchCriteria = new ProductSearchCriteria { TitleSearch = query.ToLower() };
            return _repository.Search(searchCriteria);   
        }
    }

    public class ProductSearchCriteria
    {
        public string TitleSearch { get; set; }
    }
}