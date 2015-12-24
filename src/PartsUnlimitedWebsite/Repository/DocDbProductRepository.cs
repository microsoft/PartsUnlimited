// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using PartsUnlimited.Models;
using PartsUnlimited.Search;
using PartsUnlimited.WebsiteConfiguration;
using PartsUnlimited.Areas.Admin.Controllers;

namespace PartsUnlimited.Repository
{
    public class DocDbProductRepository : IProductRepository
    {
        private readonly IDocDbConfiguration _configuration;
        private readonly SqlProductRepository _sqlProductRepository;
        private readonly DocumentClient _client;

        public DocDbProductRepository(IDocDbConfiguration configuration, SqlProductRepository sqlProductRepository)
        {
            _configuration = configuration;
            _sqlProductRepository = sqlProductRepository;
            _client = _configuration.BuildClient();
        }

        public async Task<IEnumerable<IProduct>> Search(ProductSearchCriteria searchCriteria)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var lowercaseQuery = searchCriteria.TitleSearch.ToLower();
            return await _client.CreateDocumentQuery<Product>(collection)
                .Where(p => p.Title.ToLower().Contains(lowercaseQuery))
                .ToAsyncEnumerable().ToList();
        }

        public async Task<IEnumerable<IProduct>> LoadSaleProducts()
        {
            var collection = _configuration.BuildProductCollectionLink();
            return await _client.CreateDocumentQuery<Product>(collection)
                .Where(p => p.SalePrice != p.Price)
                .ToAsyncEnumerable().ToList();
        }

        public async Task<IEnumerable<IProduct>> LoadAllProducts()
        {
            var collection = _configuration.BuildProductCollectionLink();
            return await _client.CreateDocumentQuery<Product>(collection)
                .ToAsyncEnumerable().ToList();
        }

        public async Task<int> Add(IProduct product, CancellationToken requestAborted)
        {
            //TODO Use built in Id's for looking up products.
            var collection = _configuration.BuildProductCollectionLink();
            var nextIds = await _client.CreateDocumentQuery<int>(collection,
                "SELECT TOP 1 VALUE p.ProductId " +
                "FROM p " +
                "ORDER BY p.ProductId DESC")
                .ToAsyncEnumerable().ToList(requestAborted);

            var newProductId = 1;
            if (nextIds.Any())
            {
                int nextId = nextIds.First() + 1;
                newProductId = nextId;
            }
            product.ProductId = newProductId;
            await _client.CreateDocumentAsync(collection, product);

            return product.ProductId;
        }

        public async Task<IProduct> GetLatestProduct()
        {
            var collection = _configuration.BuildProductCollectionLink();
            var latestProduct = await _client.CreateDocumentQuery<Product>(collection)
                .OrderByDescending(p => p.Created)
                .Take(1)
                .ToAsyncEnumerable().ToList();

            if (latestProduct.Any())
            {
                var firstProduct = latestProduct.First();
                if ((firstProduct != null) && ((firstProduct.Created - DateTime.UtcNow).TotalDays <= 2))
                {
                    return firstProduct;
                }
            }

            return null;
        }

        public async Task Delete(IProduct product, CancellationToken requestAborted)
        {
            //TODO - wrap CancellationToken around request
            var productLink = _configuration.BuildProductLink(product.ProductId);
            await _client.DeleteDocumentAsync(productLink);            
        }

        public async Task<IEnumerable<IProduct>> LoadProductsForCategory(int categoryId)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var productsInCategory = await _client.CreateDocumentQuery<Product>(collection)
                .Where(p => p.CategoryId == categoryId)
                .ToAsyncEnumerable().ToList();

            return productsInCategory;
        }

        public async Task<IEnumerable<IProduct>> LoadProductsFromRecommendation(IEnumerable<string> recommendedProductIds)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await _client.CreateDocumentQuery<Product>(collection)
                .Where(p => recommendedProductIds.Contains(p.RecommendationId.ToString()))
                .ToAsyncEnumerable().ToList();

            return products;
        }

        public async Task<IEnumerable<IProduct>> LoadTopSellingProducts(int count)
        {
            IEnumerable<int> productIds = await _sqlProductRepository.LoadTopSellingProduct(count);
            productIds = productIds.ToList();
            var collection = _configuration.BuildProductCollectionLink();
            IEnumerable<Product> products = _client.CreateDocumentQuery<Product>(collection)
                .Where(s => productIds.Contains(s.ProductId))
                .AsEnumerable().ToList();
            return await Task.FromResult(products);
        }

        public async Task<IEnumerable<IProduct>> LoadNewProducts(int count)
        {
            var collection = _configuration.BuildProductCollectionLink();
            IEnumerable<Product> products = await _client.CreateDocumentQuery<Product>(collection)
                .OrderByDescending(p => p.Created)
                .Take(count)
                .ToAsyncEnumerable().ToList();

            return products;
        }

        public async Task<IEnumerable<IProduct>> LoadAllProducts(SortField sortField, SortDirection sortDirection)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = _client.CreateDocumentQuery<Product>(collection);
            var sortedQuery = Sort(products, sortField, sortDirection);
            var sortedProducts = await sortedQuery.ToAsyncEnumerable().ToList();
            return sortedProducts;
        }

        public Task Save(IProduct product, CancellationToken token)
        {
            //Not required for DocDb implementation.
            return null;
        }

        public async Task<IProduct> Load(int id)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await _client.CreateDocumentQuery<Product>(collection)
                .Where(p => p.ProductId == id)
                .ToAsyncEnumerable().ToList();

            return products.Any() ? products.First() : null;
        }

        private IQueryable<IProduct> Sort(IQueryable<Product> products, SortField sortField, SortDirection sortDirection)
        {
            if (sortField == SortField.Name)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Category.Name);
                }

                return products.OrderByDescending(o => o.Category.Name);
            }

            if (sortField == SortField.Price)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Price);
                }

                return products.OrderByDescending(o => o.Price);
            }

            if (sortField == SortField.Title)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Title);
                }

                return products.OrderByDescending(o => o.Title);
            }

            // Should not reach here, but return products for compiler
            return products;
        }
    }
}