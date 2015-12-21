using System;

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IDocDbConfiguration
    {
        string URI { get; }
        string Key { get; }

        string DatabaseId { get; }
        string CollectionId { get; }
    }
}