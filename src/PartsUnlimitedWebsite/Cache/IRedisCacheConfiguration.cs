using StackExchange.Redis;

namespace PartsUnlimited.Cache
{
    public interface IRedisCacheConfiguration
    {
        ConfigurationOptions BuildOptions();
    }
}