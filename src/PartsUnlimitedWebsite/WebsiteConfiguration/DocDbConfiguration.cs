using System;
using Microsoft.Framework.Configuration;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class DocDbConfiguration : IDocDbConfiguration
    {
        public DocDbConfiguration(IConfiguration config)
        {
            URI = config["URI"];
            Key = config["Key"];
            DatabaseId = "PartsUnlimited";
            CollectionId = "ProductCollection";
        }

        public string URI { get; }
        public string Key { get; }
        public string DatabaseId { get; }
        public string CollectionId { get; }
    }
}