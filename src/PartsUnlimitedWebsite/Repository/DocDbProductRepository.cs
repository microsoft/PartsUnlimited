using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public Task Add(IProduct product, CancellationToken requestAborted)
        {
            throw new NotImplementedException();
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

        public Task Delete(IProduct product, CancellationToken requestAborted)
        {
            throw new NotImplementedException();
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
            var sortedQuery = products.Sort(sortField, sortDirection);
            var sortedProducts = await sortedQuery.ToAsyncEnumerable().ToList();
            return sortedProducts;
        }

        public Task Save(IProduct product, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<IProduct> Load(int id)
        {
            var collection = _configuration.BuildProductCollectionLink();
            var products = await _client.CreateDocumentQuery<Product>(collection)
                .Where(p => p.ProductId == id)
                .ToAsyncEnumerable().ToList();

            return products.Any() ? products.First() : null;
        }
    }
}