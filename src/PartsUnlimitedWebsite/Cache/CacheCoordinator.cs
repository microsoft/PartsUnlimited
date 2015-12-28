// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;
using System.Threading.Tasks;
using PartsUnlimited.Telemetry;

namespace PartsUnlimited.Cache
{
    public class CacheCoordinator : ICacheCoordinator
    {
        private readonly IPartsUnlimitedCache _cache;
        private readonly ITelemetryProvider _telemetryProvider;

        public CacheCoordinator(IPartsUnlimitedCache cache, ITelemetryProvider telemetryProvider)
        {
            _cache = cache;
            _telemetryProvider = telemetryProvider;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> loadFromSource, CacheCoordinatorOptions options)
        {
            Lazy<Task<T>> sourceLoader = new Lazy<Task<T>>(loadFromSource.Invoke);

            try
            {
                var result = await _cache.GetValueAsync<T>(key);
                if (result.HasValue)
                {
                    return result.Value;
                }

                //initial population.
                var sourceValue = await sourceLoader.Value;
                await _cache.SetValueAsync(key, sourceValue, options.CacheOption);

                if (sourceValue == null && options.RemoveIfNull)
                {
                    await Remove(key);
                }

                return sourceValue;
            }
            catch (Exception ex)
            {
                _telemetryProvider.TrackException(ex);
            }

            //Cache has failed, fail back to source system.
            if (options.CallFailOverOnError || sourceLoader.IsValueCreated)
            {
                return await sourceLoader.Value;
            }

            throw new InvalidOperationException($"Item in cache with key '{key}' not found");
        }

        public Task<T> GetAsync<T>(string key, Func<T> fallback, CacheCoordinatorOptions options)
        {
            return GetAsync(key, () => Task.FromResult(fallback()), options);
        }

        public Task Remove(string key)
        {
            return _cache.RemoveAsync(key);
        }
    }
}