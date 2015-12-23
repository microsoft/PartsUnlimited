// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System;
using System.Threading.Tasks;
using PartsUnlimited.Telemetry;

namespace PartsUnlimited.Cache
{
    /// <summary>
    /// Multi level cache implementation.
    /// </summary>
    public class PartUnlimitedMultilevelCache : IPartsUnlimitedCache
    {
        private readonly PartsUnlimitedMemoryCache _memoryCache;
        private readonly PartsUnlimitedRedisCache _redisCache;
        private readonly ITelemetryProvider _telemetryProvider;

        public PartUnlimitedMultilevelCache(PartsUnlimitedMemoryCache memoryCache
            , PartsUnlimitedRedisCache redisCache, ITelemetryProvider telemetryProvider)
        {
            _memoryCache = memoryCache;
            _redisCache = redisCache;
            _telemetryProvider = telemetryProvider;
        }

        public async Task SetValue<T>(string key, T value, PartsUnlimitedCacheOptions options)
        {
            if (options.ShouldApplyToMultiLevelCache)
            {
                await _memoryCache.SetValue(key, value, options.SecondLevelCacheOptions);
            }

            await _redisCache.SetValue(key, value, options);
        }

        public async Task<CacheResult<T>> GetValue<T>(string key)
        {
            var memoryResult = await _memoryCache.GetValue<T>(key);
            if (memoryResult.HasValue)
            {
                return memoryResult;
            }

            return await _redisCache.GetValue<T>(key);
        }

        public Task Remove(string key)
        {
            Task[] tasks = { _memoryCache.Remove(key) , _redisCache.Remove(key) };
            try
            {
                return Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                foreach (var t in tasks)
                {
                    if (t.Exception != null)
                    {
                        _telemetryProvider.TrackException(t.Exception);
                    }
                }
            }
            return Task.FromResult(0);
        }
    }
}