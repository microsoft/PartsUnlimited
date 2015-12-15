using System;
using System.Threading.Tasks;

namespace PartsUnlimited.Cache
{
    public interface ICacheCoordinator
    {
        Task<T> GetAsync<T>(string key, Func<T> fallback, CacheCoordinatorOptions options);

        Task<T> GetAsync<T>(string key, Func<Task<T>> loadFromSource, CacheCoordinatorOptions options);

        Task Remove(string key);
    }
}