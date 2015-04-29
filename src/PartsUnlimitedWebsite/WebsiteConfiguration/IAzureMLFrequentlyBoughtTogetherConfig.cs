// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IAzureMLFrequentlyBoughtTogetherConfig
    {
        string AccountKey { get; }
        string ModelName { get; }
    }
}