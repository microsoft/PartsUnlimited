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
    public class DocumentDBProductRepository : IProductRepository
    {
        private readonly IDocumentDBConfiguration _configuration;
        private readonly SqlProductRepository _sqlProductRepository;
        private readonly DocumentClient _client;
        private readonly RelatedProductsQueryBuilder _queryBuilder;

        public DocumentDBProductRepository(IDocumentDBConfiguration configuration, SqlProductRepository sqlProductRepository, RelatedProductsQueryBuilder queryBuilder)
        {
            _configuration = configuration;
            _sqlProductRepository = sqlProductRepository;
            _queryBuilder = queryBuilder;
            _client = _configuration.BuildClient();
        }

        /// <summary>
        /// DocumentDB query throttling helper function. Automatically retries queries which fail
        /// due to rate throttling.
        /// </summary>
        public static async Task<V> ExecuteTaskWithThrottlingSafety<V>(Func<Task<V>> func)
        {
            while (true)
            {
                try
                {
                    return await func().ConfigureAwait(continueOnCapturedContext: true);
                }
                catch (AggregateException ae) when (ae.InnerException is DocumentClientException)
                {
                    var de = (DocumentClientException)ae.InnerException;
                    if (de.StatusCode != null && (int)de.StatusCode == 429)
                    {
                        await Task.Delay(de.RetryAfter);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public static Task ExecuteTaskWithThrottlingSafety(Func<Task> func)
        {
            return ExecuteTaskWithThrottlingSafety(() => func().ContinueWith<object>(_ => null));
        }

        public async Task<IEnumerable<IProduct>> Search(ProductSearchCriteria searchCriteria)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var lowercaseQuery = searchCriteria.TitleSearch.ToLower();
            return await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                      .Where(p => p.TitleLowerCase.Contains(lowercaseQuery))
                                                                      .ToAsyncEnumerable().ToList());
        }

        public async Task<IEnumerable<IProduct>> LoadSaleProducts()
        {
            var collection = _configuration.BuildProductCollectionLink();
            return await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                      .Where(p => p.SalePrice != p.Price)
                                                                      .ToAsyncEnumerable().ToList());
        }

        public async Task<IEnumerable<IProduct>> LoadAllProducts()
        {
            var collection = _configuration.BuildProductCollectionLink();
            return await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                      .ToAsyncEnumerable().ToList());
        }

        public async Task Add(IProduct product, CancellationToken cancellationToken)
        {
            if (product.Category == null)
            {
                throw new InvalidOperationException("Category not set");
            }

            //TODO - add CancellationToken support
            var collection = _configuration.BuildProductCollectionLink();

            await ExecuteTaskWithThrottlingSafety(async () => {

                product.ProductId = await GetNextProductId();
                await _client.CreateDocumentAsync(collection, product);

            });
        }

        public async Task<int> GetNextProductId()
        {
            var collection = _configuration.BuildProductCollectionLink();
            var nextIds = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<int>(collection, "SELECT TOP 1 VALUE p.ProductId FROM p ORDER BY p.ProductId DESC")
                                                                             .ToAsyncEnumerable().ToList());

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
            var latestProduct = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                                   .OrderByDescending(p => p.Created)
                                                                                   .Take(1)
                                                                                   .ToAsyncEnumerable().ToList());

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

            await ExecuteTaskWithThrottlingSafety(() => _client.DeleteDocumentAsync(productLink));
        }

        public async Task<IEnumerable<IProduct>> LoadProductsForCategory(int categoryId)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var productsInCategory = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                                        .Where(p => p.CategoryId == categoryId)
                                                                                        .ToAsyncEnumerable().ToList());

            productsInCategory.ForEach(async p => await LoadProductImageUrl(p));

            return productsInCategory;
        }

        public async Task<IEnumerable<IProduct>> LoadProductsFromRecommendation(IEnumerable<string> recommendedProductIds)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                              .Where(p => recommendedProductIds.Contains(p.RecommendationId.ToString()))
                                                                              .ToAsyncEnumerable().ToList());

            return products;
        }

        public async Task<IEnumerable<IProduct>> LoadRelatedProducts(int productId)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var product = (Product)await Load(productId);
            var query = _queryBuilder.BuildQuery(product);

            var relatedProducts = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection, query)
                                                                                     .ToAsyncEnumerable()
                                                                                     .ToList());

            return relatedProducts;
        }

        public async Task<IEnumerable<IProduct>> LoadTopSellingProducts(int count)
        {
            var productIds = await _sqlProductRepository.LoadTopSellingProduct(count);

            var collection = _configuration.BuildProductCollectionLink();

            var products = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                              .Where(s => productIds.Contains(s.ProductId))
                                                                              .ToAsyncEnumerable().ToList());
            return products;
        }

        public async Task<IEnumerable<IProduct>> LoadNewProducts(int count)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                              .OrderByDescending(p => p.Created)
                                                                              .Take(count)
                                                                              .ToAsyncEnumerable().ToList());

            return products;
        }

        public async Task<IEnumerable<IProduct>> LoadAllProducts(SortField sortField, SortDirection sortDirection)
        {
            var collection = _configuration.BuildProductCollectionLink();
            return await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                      .Sort(sortField, sortDirection)
                                                                      .ToAsyncEnumerable().ToList());
        }

        public Task Save(IProduct product, CancellationToken token)
        {
            //TODO - wrap CancellationToken around request
            var collection = _configuration.BuildProductCollectionLink();

            return ExecuteTaskWithThrottlingSafety(() => _client.UpsertDocumentAsync(collection, product));
        }

        public async Task<IProduct> Load(int id)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await ExecuteTaskWithThrottlingSafety(() => _client.CreateDocumentQuery<Product>(collection)
                                                                              .Where(p => p.ProductId == id)
                                                                              .ToAsyncEnumerable().ToList());

            if (!products.Any())
            {
                return null;
            }

            await LoadProductImageUrl(products.First());

            return products.First();
        }

        private async Task LoadProductImageUrl(IProduct product)
        {
            var attachmentLink = _configuration.BuildAttachmentLink(product.ProductId);
            try
            {
                Attachment attachment = await ExecuteTaskWithThrottlingSafety(() => _client.ReadAttachmentAsync(attachmentLink));
                product.ProductArtUrl = attachment.MediaLink;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode != null && (int) e.StatusCode != 404)
                {
                    throw;
                }
            }
        }
    }
}