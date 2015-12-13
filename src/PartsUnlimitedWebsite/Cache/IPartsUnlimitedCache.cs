using System;

namespace PartsUnlimited.Cache
{
    public interface IPartsUnlimitedCache
    {
        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param><param name="value">The value to be cached.</param><param name="options">The <see cref="T:Microsoft.Framework.Caching.Memory.MemoryCacheEntryOptions"/>.</param>
        /// <returns>
        /// The object that was cached.
        /// </returns>
        void Set<T>(string key, T value, PartsUnlimitedMemoryCacheEntryOptions options);

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <returns>
        /// True if the key was found.
        /// </returns>
        CacheResult<T> TryGetValue<T>(string key);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(string key);
    }
}