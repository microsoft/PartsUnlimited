// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Framework.Configuration;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class AzureMLFrequentlyBoughtTogetherConfig : IAzureMLFrequentlyBoughtTogetherConfig
    {
        public AzureMLFrequentlyBoughtTogetherConfig(IConfiguration config)
        {
            AccountKey = GetString(config, "AccountKey");
            ModelName = GetString(config, "ModelName");
        }

        private string GetString(IConfiguration config, string key)
        {
            string s;
            if (config.TryGet(key, out s))
            {
                return s;
            }

            return null;
        }

        public string AccountKey { get; }
        public string ModelName { get; }
    }
}