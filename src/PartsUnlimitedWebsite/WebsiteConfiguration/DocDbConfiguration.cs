using Microsoft.Framework.Configuration;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class DocDbConfiguration : IDocDbConfiguration
    {
        public DocDbConfiguration(IConfiguration config)
        {
            URI = config["URI"];
            Key = config["Key"];
        }

        public string URI { get; }
        public string Key { get; }
    }
}