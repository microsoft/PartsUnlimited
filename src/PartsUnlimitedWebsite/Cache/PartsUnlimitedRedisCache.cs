using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace PartsUnlimited.Cache
{
    public class PartsUnlimitedRedisCache : IPartsUnlimitedCache, IDisposable
    {
        private static IRedisCacheConfiguration _configuration;

        public PartsUnlimitedRedisCache(IRedisCacheConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static readonly Lazy<ConnectionMultiplexer> _lazyConnection = new Lazy<ConnectionMultiplexer>(
            () => ConnectionMultiplexer.Connect(_configuration.BuildOptions()));

        private static IDatabase Database => _lazyConnection.Value.GetDatabase();

        public void Set<T>(string key, T value, PartsUnlimitedMemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            TimeSpan span = TimeSpan.Zero;
            var commandFlags = BuildFlags(options);
            var cacheItem = new CacheItem<T>(value, commandFlags);

            if (options.SlidingExpiration.HasValue)
            {
                span = options.SlidingExpiration.Value;
                cacheItem.SlidingCacheTime = options.SlidingExpiration.Value;
            }

            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                span = options.AbsoluteExpirationRelativeToNow.Value;
            }

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string stringValue= JsonConvert.SerializeObject(cacheItem, Formatting.Indented, settings);
            Database.StringSet(key, stringValue, span, When.Always, commandFlags);
        }

        private static CommandFlags BuildFlags(PartsUnlimitedMemoryCacheEntryOptions options)
        {
            switch (options.Priority)
            {
                case PartsUnlimitedCacheItemPriority.High:
                    return CommandFlags.HighPriority;
                default:
                    return CommandFlags.None;
            }
        }

        public CacheResult<T> TryGetValue<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            RedisValue redisValue = Database.StringGet(key);
            if (redisValue.HasValue)
            {
                CacheItem<T> cacheValue = JsonConvert.DeserializeObject<CacheItem<T>>(redisValue);
                if (cacheValue.SlidingCacheTime.HasValue)
                {
                    var timeSpan = cacheValue.SlidingCacheTime.Value;
                    Database.KeyExpire(key, timeSpan, cacheValue.Flags);
                }
                return new CacheResult<T>(cacheValue.Value);
            }

            return CacheResult<T>.Empty();
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            Database.KeyDelete(key);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_lazyConnection.IsValueCreated)
                {
                    var connection = _lazyConnection.Value;
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
    }
}