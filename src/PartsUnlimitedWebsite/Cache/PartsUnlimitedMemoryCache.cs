using Microsoft.Framework.Caching.Memory;

namespace PartsUnlimited.Cache
{
    public class PartsUnlimitedMemoryCache : IPartsUnlimitedCache
    {
        private readonly IMemoryCache _memoryCache;

        public PartsUnlimitedMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set<T>(string key, T value, PartsUnlimitedMemoryCacheEntryOptions options)
        {
            var memCacheOptions = BuildOptions(options);
            _memoryCache.Set(key, value, memCacheOptions);
        }

        public CacheResult<T> TryGetValue<T>(string key)
        {
            T value;
            if (_memoryCache.TryGetValue(key, out value))
            {
                return new CacheResult<T>(value);
            }

            return CacheResult<T>.Empty();
        }

        private static MemoryCacheEntryOptions BuildOptions(PartsUnlimitedMemoryCacheEntryOptions options)
        {
            CacheItemPriority pri;
            switch (options.Priority)
            {
                case PartsUnlimitedCacheItemPriority.High:
                    pri = CacheItemPriority.High;
                    break;
                default: 
                    pri = CacheItemPriority.Normal;
                    break;
            }

            var memCacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = options.SlidingExpiration,
                Priority =pri
            };
            return memCacheOptions;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}