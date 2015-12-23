// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
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