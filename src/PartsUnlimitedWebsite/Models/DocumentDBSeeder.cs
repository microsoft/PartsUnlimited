using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using PartsUnlimited.Repository;
using PartsUnlimited.WebsiteConfiguration;

namespace PartsUnlimited.Models
{
    public class DocumentDBSeeder : IDataSeeder
    {
        private readonly IDocumentDBConfiguration _configuration;
        private readonly SQLDataSeeder _sqlDataSeeder;
        private readonly DocumentDBProductRepository _productRepository;

        public DocumentDBSeeder(IDocumentDBConfiguration configuration, SQLDataSeeder sqlDataSeeder, DocumentDBProductRepository productRepository)
        {
            _configuration = configuration;
            _sqlDataSeeder = sqlDataSeeder;
            _productRepository = productRepository;
        }

        public async Task Seed(SampleData data)
        {
            //See remaining items which exist within sql.
            await _sqlDataSeeder.Seed(data, categories => CreateDocumentDBProducts(data, categories));            
        }

        private async Task<IEnumerable<IProduct>> CreateDocumentDBProducts(SampleData data, IEnumerable<Category> categories)
        {
            try
            {
                var client = _configuration.BuildClient();
                await CreateDatabaseIfNotExists(client);
                await CreateCollectionIfNotExists(client);
                return await CreateProducts(client, data, categories);
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                throw;
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine(baseException.Message);
                throw;
            }
        }

        private async Task<IEnumerable<IProduct>> CreateProducts(DocumentClient client, SampleData data, IEnumerable<Category> categories)
        {
            var collectionId = _configuration.BuildProductCollectionLink();

            var existingProducts = await client.ReadEntireDocumentFeedAsync<Product>(collectionId);
            if (existingProducts.Any())
            {
                return existingProducts;
            }

            // No products in DocumentDB. Bootstrap from SampleData:
            var products = data.GetProducts(categories);

            foreach (var prod in products)
            {
                await _productRepository.Add(prod, CancellationToken.None);
            }

            return products;
        }

        private async Task CreateDatabaseIfNotExists(DocumentClient client)
        {
            var databaseResponse = await client.ReadDatabaseFeedAsync();
            var database = databaseResponse.Where(d => d.Id == _configuration.DatabaseId).AsEnumerable().SingleOrDefault();
            if (database == null)
            {
                var newDb = new Database { Id = _configuration.DatabaseId };
                await client.CreateDatabaseAsync(newDb);
            }
        }

        private async Task CreateCollectionIfNotExists(DocumentClient client)
        {
            var databaseLink = _configuration.BuildDatabaseLink();
            var collection = client.CreateDocumentCollectionQuery(databaseLink).Where(c => c.Id == _configuration.CollectionId).ToList().FirstOrDefault();
            if (collection == null)
            {
                var productCollection = new DocumentCollection { Id = _configuration.CollectionId };
                
                // Add indexing across all String and Number properties for ordering and searching
                productCollection.IndexingPolicy.IncludedPaths.Add(
                    new IncludedPath
                    {
                        Path = "/*",
                        Indexes =
                        {
                            new RangeIndex(DataType.String, precision: -1),
                            new RangeIndex(DataType.Number, precision: -1)
                        }
                    });

                await client.CreateDocumentCollectionAsync(databaseLink, productCollection);
            }
        }
    }
}
