// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Framework.Configuration;

namespace PartsUnlimited.Security
{
    public class ConfigurationLoginProviderCredentials : ILoginProviderCredentials
    {
        public ConfigurationLoginProviderCredentials(IConfiguration config)
        {
            Key = GetString(config, "Key");
            Secret = GetString(config, "Secret");

            Use = !string.IsNullOrWhiteSpace(Key) && !string.IsNullOrWhiteSpace(Secret);
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

        public string Key { get; }
        public string Secret { get; }
        public bool Use { get; protected set; }
    }
}