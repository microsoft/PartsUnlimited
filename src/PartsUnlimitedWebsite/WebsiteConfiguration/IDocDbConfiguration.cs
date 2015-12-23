using System;
using Microsoft.Azure.Documents.Client;

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IDocDbConfiguration
    {
        DocumentClient BuildClient();
        Uri BuildProductCollectionLink();
        Uri BuildDatabaseLink();
        Uri BuildProductLink(int productId);
        string DatabaseId { get; }
        string CollectionId { get; }
    }
}