using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using PartsUnlimited.WebsiteConfiguration;

namespace PartsUnlimited.Models
{
    public class DocDbSeeder : IDataSeeder
    {
        private readonly IDocDbConfiguration _configuration;
        private readonly SQLDataSeeder _sqlDataSeeder;

        public DocDbSeeder(IDocDbConfiguration configuration, SQLDataSeeder sqlDataSeeder)
        {
            _configuration = configuration;
            _sqlDataSeeder = sqlDataSeeder;
        }

        public async Task Seed(SampleData data)
        {
            IList<IProduct> products = new List<IProduct>();

            var serviceEndpoint = new Uri(_configuration.URI);
            var client = new DocumentClient(serviceEndpoint, _configuration.Key, ConnectionPolicy.Default);

            try
            {
                await SeedintoDocDb(client, data);
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine(baseException.Message);
            }

            //See remaining items which exist within sql.
            await _sqlDataSeeder.Seed(data, products);
        }

        private async Task SeedintoDocDb(DocumentClient client, SampleData data)
        {
            await CreateDatabaseIfNotExists(client);
            await CreateCollectionIfNotExists(client);
            await CreateProducts(client, data);
        }

        private Task CreateProducts(DocumentClient client, SampleData data)
        {
            return Task.FromResult(1);
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
            var databaseLink = UriFactory.CreateDatabaseUri(_configuration.DatabaseId);
            var collection = client.CreateDocumentCollectionQuery(databaseLink).Where(c => c.Id == _configuration.CollectionId).ToArray().FirstOrDefault();
            if (collection == null)
            {
                var productCollection = new DocumentCollection {Id = _configuration.CollectionId };
                await client.CreateDocumentCollectionAsync(databaseLink, productCollection);
            }
        }
    }
}