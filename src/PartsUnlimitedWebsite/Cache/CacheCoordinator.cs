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

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> fallback, CacheCoordinatorOptions options)
        {
            Lazy<T> sourceLoader = new Lazy<T>(delegate
            {
                Task<T> task = fallback.Invoke();
                return task.Result;
            });

            try
            {
                var result = await _cache.GetValue<T>(key);
                if (result.HasValue)
                {
                    return result.Value;
                }

                //initial population.
                var fallbackResult = sourceLoader.Value;
                await _cache.SetValue(key, fallbackResult, options.CacheOption);

                if (fallbackResult == null && options.RemoveIfNull)
                {
                    await Remove(key);
                }

                return fallbackResult;
            }
            catch (Exception ex)
            {
                _telemetryProvider.TrackException(ex);
            }

            //Cache has failed, fail back to source system.
            if (options.CallFailOverOnError || sourceLoader.IsValueCreated)
            {
                return sourceLoader.Value;
            }

            throw new InvalidOperationException($"Item in cache with key '{key}' not found");
        }

        public Task<T> GetAsync<T>(string key, Func<T> fallback, CacheCoordinatorOptions options)
        {
            return GetAsync(key, () => Task.FromResult(fallback()), options);
        }

        public Task Remove(string key)
        {
            return _cache.Remove(key);
        }
    }
}