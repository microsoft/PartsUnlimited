using System;
using System.Threading.Tasks;
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

        public Task Set<T>(string key, T value, PartsUnlimitedMemoryCacheEntryOptions options)
        {
            var memCacheOptions = BuildOptions(options);
            return Task.Run(() => _memoryCache.Set(key, value, memCacheOptions));
        }

        public Task<CacheResult<T>> TryGetValue<T>(string key)
        {
            T value;
            if (_memoryCache.TryGetValue(key, out value))
            {
                return Task.FromResult(new CacheResult<T>(value));
            }

            return Task.FromResult(CacheResult<T>.Empty());
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

        public Task Remove(string key)
        {
            return Task.Run(() => _memoryCache.Remove(key));
        }
    }
}