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

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> fallback, InvokerOptions options)
        {
            try
            {
                var result = await _cache.GetValue<T>(key);
                if (result.HasValue)
                {
                    return result.Value;
                }
            }
            catch (Exception ex)
            {
                _telemetryProvider.TrackException(ex);
            }

            //bubble exceptions from the load up to caller if they occur
            var fallBackResult = await fallback.Invoke();

            try
            {
                if (options.ShouldPopulateCache && fallBackResult != null)
                {
                    await _cache.SetValue(key, fallBackResult, options.CacheOption);
                }

                if (fallBackResult == null && options.RemoveIfNull)
                {
                    await _cache.Remove(key);
                }
            }
            catch (Exception ex)
            {
                _telemetryProvider.TrackException(ex);
            }

            return fallBackResult;
        }

        public Task<T> GetAsync<T>(string key, Func<T> fallback, InvokerOptions options)
        {
            return GetAsync(key, () => Task.FromResult(fallback()), options);
        }

        public Task Remove(string key)
        {
            return _cache.Remove(key);
        }
    }
}