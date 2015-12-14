using System;
using System.Threading.Tasks;

namespace PartsUnlimited.Cache
{
    public interface ICacheCoordinator
    {
        Task<T> GetAsync<T>(string key, Func<T> fallback, InvokerOptions options);

        Task<T> GetAsync<T>(string key, Func<Task<T>> fallback, InvokerOptions options);

        Task Remove(string key);
    }
}