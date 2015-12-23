// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
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

        public Task SetValue<T>(string key, T value, PartsUnlimitedCacheOptions options)
        {
            var memCacheOptions = BuildOptions(options);
            _memoryCache.Set(key, value, memCacheOptions);
            return Task.FromResult(0);
        }

        public Task<CacheResult<T>> GetValue<T>(string key)
        {
            T value;
            if (_memoryCache.TryGetValue(key, out value))
            {
                return Task.FromResult(new CacheResult<T>(value));
            }

            return Task.FromResult(CacheResult<T>.Empty());
        }

        private static MemoryCacheEntryOptions BuildOptions(PartsUnlimitedCacheOptions options)
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
            _memoryCache.Remove(key);
            return Task.FromResult(0);
        }
    }
}