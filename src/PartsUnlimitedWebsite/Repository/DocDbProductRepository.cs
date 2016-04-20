// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using PartsUnlimited.Models;
using PartsUnlimited.Search;
using PartsUnlimited.WebsiteConfiguration;

namespace PartsUnlimited.Repository
{
    public class DocDbProductRepository : IProductRepository
    {
        private readonly IDocDbConfiguration _configuration;
        private readonly SqlProductRepository _sqlProductRepository;
        private readonly DocumentClient _client;
        private readonly RelatedProductsQueryBuilder _queryBuilder;

        public DocDbProductRepository(IDocDbConfiguration configuration, SqlProductRepository sqlProductRepository, RelatedProductsQueryBuilder queryBuilder)
        {
            _configuration = configuration;
            _sqlProductRepository = sqlProductRepository;
            _queryBuilder = queryBuilder;
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

        public async Task Add(IProduct product, CancellationToken cancellationToken)
        {
            //TODO - add CancellationToken support
            var collection = _configuration.BuildProductCollectionLink();
            product.ProductId = await GetNextProductId();
            await _client.CreateDocumentAsync(collection, product);
        }

        public async Task<int> GetNextProductId()
        {
            var collection = _configuration.BuildProductCollectionLink();
            var nextIds = await _client.CreateDocumentQuery<int>(collection, "SELECT TOP 1 VALUE p.ProductId FROM p ORDER BY p.ProductId DESC")
                                       .ToAsyncEnumerable().ToList();

            var newProductId = 1;
            if (nextIds.Any())
            {
                int nextId = nextIds.First() + 1;
                newProductId = nextId;
            }

            return newProductId;
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

            productsInCategory.ForEach(async p => await LoadProductImageUrl(p));

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

        public async Task<IEnumerable<IProduct>> LoadRelatedProducts(int productId)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var product = (Product)await Load(productId);
            var query = _queryBuilder.BuildQuery(product);

            var relatedProducts = await _client.CreateDocumentQuery<Product>(collection, query)
                                               .ToAsyncEnumerable()
                                               .ToList();

            return relatedProducts;
        }

        public async Task<IEnumerable<IProduct>> LoadTopSellingProducts(int count)
        {
            IEnumerable<int> productIds = await _sqlProductRepository.LoadTopSellingProduct(count);
            productIds = productIds.ToList();
            var collection = _configuration.BuildProductCollectionLink();
            var products = _client.CreateDocumentQuery<Product>(collection)
                                  .Where(s => productIds.Contains(s.ProductId))
                                  .ToAsyncEnumerable().ToList();
            return await products;
        }

        public async Task<IEnumerable<IProduct>> LoadNewProducts(int count)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await _client.CreateDocumentQuery<Product>(collection)
                                        .OrderByDescending(p => p.Created)
                                        .Take(count)
                                        .ToAsyncEnumerable().ToList();

            return products;
        }

        public async Task<IEnumerable<IProduct>> LoadAllProducts(SortField sortField, SortDirection sortDirection)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = _client.CreateDocumentQuery<Product>(collection);
            var sortedQuery = products.Sort(sortField, sortDirection);
            var sortedProducts = await sortedQuery.ToAsyncEnumerable().ToList();
            return sortedProducts;
        }

        public Task Save(IProduct product, CancellationToken token)
        {
            //TODO - wrap CancellationToken around request
            var collection = _configuration.BuildProductCollectionLink();
            return _client.UpsertDocumentAsync(collection, product);
        }

        public async Task<IProduct> Load(int id)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await _client.CreateDocumentQuery<Product>(collection)
                                        .Where(p => p.ProductId == id)
                                        .ToAsyncEnumerable().ToList();

            if (!products.Any())
                return null;

            await LoadProductImageUrl(products.First());

            return products.First();
        }

        private async Task LoadProductImageUrl(IProduct product)
        {
            var attachmentLink = _configuration.BuildAttachmentLink(product.ProductId);
            try
            {
                Attachment attachment = await _client.ReadAttachmentAsync(attachmentLink);
                product.ProductArtUrl = attachment.MediaLink;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode != null && (int)e.StatusCode != 404)
                    throw;
            }
        }
    }
}