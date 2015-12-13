using Microsoft.Framework.Configuration;
using PartsUnlimited.Cache;
using StackExchange.Redis;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class RedisCacheConfig : IRedisCacheConfiguration
    {
        public RedisCacheConfig(IConfiguration config)
        {
            HostName = config["HostName"];
            AccessKey = config["AccessKey"];
            KeepAliveTimeSeconds = config.Get("KeepAliveTimeSeconds", 15);
            SslEnabled = config.Get("SSLEnabled", true);
            Port = config.Get("Port", 6380);

            Options = new ConfigurationOptions
            {
                KeepAlive = KeepAliveTimeSeconds,
                Ssl = SslEnabled,
                Password = AccessKey
            };
            Options.EndPoints.Add(HostName, Port);
        }

        public string HostName { get; }
        public string AccessKey { get; }
        private int KeepAliveTimeSeconds { get; }
        private bool SslEnabled { get; }
        private int Port { get; }

        public ConfigurationOptions Options { get; }
    }
}