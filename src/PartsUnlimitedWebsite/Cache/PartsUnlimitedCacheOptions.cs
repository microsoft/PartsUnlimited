// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;

namespace PartsUnlimited.Cache
{
    public class PartsUnlimitedCacheOptions
    {
        private TimeSpan? _slidingExpiration;
        private TimeSpan? _absoluteExpirationRelativeToNow;

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get
            {
                return _absoluteExpirationRelativeToNow;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(AbsoluteExpirationRelativeToNow),
                        value,
                        "The relative expiration value must be positive.");
                }

                _absoluteExpirationRelativeToNow = value;
            }
        }

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        public TimeSpan? SlidingExpiration
        {
            get
            {
                return _slidingExpiration;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(SlidingExpiration),
                        value,
                        "The sliding expiration value must be positive.");
                }
                _slidingExpiration = value;
            }
        }

        /// <summary>
        /// Gets or sets the priority for keeping the cache entry in the cache during a
        /// memory pressure triggered cleanup. The default is <see cref="PartsUnlimitedCacheItemPriority.Normal"/>.
        /// </summary>
        public PartsUnlimitedCacheItemPriority Priority { get; set; } = PartsUnlimitedCacheItemPriority.Normal;

        public PartsUnlimitedCacheOptions SecondLevelCacheOptions { get; private set; }

        public bool ShouldApplyToMultiLevelCache => SecondLevelCacheOptions != null;

        /// <summary>
        /// Applies a configuration for a second level cache by applying the ratio to the current options.
        /// If the ratio equal 0 then it's the same as not applying a second level cache.
        /// </summary>
        public PartsUnlimitedCacheOptions WithSecondLevelCacheAsRatio(decimal ratio)
        {
            if (ratio == 0)
            {
                // 0 ratio = no second level caching
                return this;
            }

            SecondLevelCacheOptions = new PartsUnlimitedCacheOptions
            {
                Priority = Priority,
                SlidingExpiration = SlidingExpiration.HasValue ? new TimeSpan((long)(SlidingExpiration.Value.Ticks * ratio)) : (TimeSpan?)null,
                AbsoluteExpirationRelativeToNow = AbsoluteExpirationRelativeToNow.HasValue ? new TimeSpan((long)(AbsoluteExpirationRelativeToNow.Value.Ticks * ratio)) : (TimeSpan?)null
            } ;
            return this;
        }
    }
}