using Microsoft.Azure.Documents.Client;

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IDocDbConfiguration
    {
        string DatabaseId { get; }
        string CollectionId { get; }

        DocumentClient BuildClient();
    }
}