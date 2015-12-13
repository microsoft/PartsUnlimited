using System;
using StackExchange.Redis;

namespace PartsUnlimited.Cache
{
    internal class CacheItem<T>
    {
        public CacheItem(T value, CommandFlags flags)
        {
            Value = value;
            Flags = flags;
        }

        public TimeSpan? SlidingCacheTime { get; set; }
        public CommandFlags Flags { get; }

        public T Value { get; }
    }
}