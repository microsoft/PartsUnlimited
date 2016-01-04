using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using PartsUnlimited.Models;
using PartsUnlimited.Search;

namespace PartsUnlimited.Repository
{
    public interface IProductRepository  : IProductLoader
    {
        Task<IEnumerable<IProduct>> Search(ProductSearchCriteria searchCriteria);
        Task<IEnumerable<IProduct>> LoadSaleProducts();
        Task<IEnumerable<IProduct>> LoadAllProducts();
        Task Add(IProduct product, CancellationToken requestAborted);
        Task<IProduct> GetLatestProduct();
        Task Delete(IProduct product, CancellationToken requestAborted);
        Task<IEnumerable<IProduct>> LoadProductsForCategory(int categoryId);
        Task<IEnumerable<IProduct>> LoadProductsFromRecommendation(IEnumerable<string> recommendedProductIds);
        Task<IEnumerable<IProduct>> LoadRelatedProducts(int productId);
        Task<IEnumerable<IProduct>> LoadTopSellingProducts(int count);
        Task<IEnumerable<IProduct>> LoadNewProducts(int count);
        Task<IEnumerable<IProduct>> LoadAllProducts(SortField sortField, SortDirection sortDirection);
        Task Save(IProduct product, CancellationToken token);
    }
}
