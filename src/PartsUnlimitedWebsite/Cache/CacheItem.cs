// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;

namespace PartsUnlimited.Cache
{
    internal class CacheItem<T>
    {
        public CacheItem(T value)
        {
            Value = value;
        }

        public TimeSpan? SlidingCacheTime { get; set; }

        public T Value { get; }
    }
}