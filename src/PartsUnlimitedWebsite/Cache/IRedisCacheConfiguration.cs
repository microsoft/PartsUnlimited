using StackExchange.Redis;

namespace PartsUnlimited.Cache
{
    public interface IRedisCacheConfiguration
    {
        ConfigurationOptions Options { get; }
    }
}