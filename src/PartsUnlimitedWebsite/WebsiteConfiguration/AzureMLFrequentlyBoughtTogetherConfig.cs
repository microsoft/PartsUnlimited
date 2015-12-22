// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class AzureMLFrequentlyBoughtTogetherConfig : IAzureMLFrequentlyBoughtTogetherConfig
    {
        public AzureMLFrequentlyBoughtTogetherConfig(IConfiguration config)
        {
            AccountKey = config["AccountKey"];
            ModelName = config["ModelName"];
        }

        public string AccountKey { get; }
        public string ModelName { get; }
    }
}