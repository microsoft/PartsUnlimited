// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
namespace PartsUnlimited.Cache
{
    /// <summary>
    /// Specifies how items are prioritized for preservation during a memory pressure triggered cleanup.
    /// </summary>
    public enum PartsUnlimitedCacheItemPriority
    {
        Normal,
        High
    }
}