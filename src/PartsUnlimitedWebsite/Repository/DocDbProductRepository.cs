using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Data.Entity;
using PartsUnlimited.Areas.Admin.Controllers;
using PartsUnlimited.Models;
using PartsUnlimited.Search;
using PartsUnlimited.WebsiteConfiguration;

namespace PartsUnlimited.Repository
{
    public class DocDbProductRepository : IProductRepository
    {
        private readonly IDocDbConfiguration _configuration;
        private readonly SqlProductRepository _sqlProductRepository;
        private DocumentClient _client;

        public DocDbProductRepository(IDocDbConfiguration configuration, SqlProductRepository sqlProductRepository)
        {
            _configuration = configuration;
            _sqlProductRepository = sqlProductRepository;
            _client = _configuration.BuildClient();
        }

        public Task<IEnumerable<IProduct>> Search(ProductSearchCriteria searchCriteria)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IProduct>> LoadSaleProducts()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IProduct>> LoadAllProducts()
        {
            throw new System.NotImplementedException();
        }

        public Task Add(IProduct product, CancellationToken requestAborted)
        {
            throw new System.NotImplementedException();
        }

        public Task<IProduct> GetLatestProduct()
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(IProduct product, CancellationToken requestAborted)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IProduct>> LoadProductsForCategory(int categoryId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IProduct>> LoadProductsFromRecommendation(IEnumerable<string> recommendedProductIds)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<IProduct>> LoadTopSellingProducts(int count)
        {
            IEnumerable<int> productIds = await _sqlProductRepository.LoadTopSellingProduct(count);
            productIds = productIds.ToList();
            var collection = UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionId);
            IEnumerable<Product> products = _client.CreateDocumentQuery<Product>(collection)
                .Where(s => productIds.Contains(s.ProductId))
                .AsEnumerable().ToList();
            return await Task.FromResult(products);
        }

        public Task<IEnumerable<IProduct>> LoadNewProducts(int count)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IProduct>> LoadAllProducts(SortField sortField, SortDirection sortDirection)
        {
            throw new System.NotImplementedException();
        }

        public Task Save(IProduct product, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public Task<IProduct> Load(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}