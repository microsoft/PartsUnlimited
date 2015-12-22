// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace PartsUnlimited.Security
{
    public class ConfigurationLoginProviderCredentials : ILoginProviderCredentials
    {
        public ConfigurationLoginProviderCredentials(IConfiguration config)
        {
            Key = config["Key"];
            Secret = config["Secret"];

            Use = !string.IsNullOrWhiteSpace(Key) && !string.IsNullOrWhiteSpace(Secret);
        }

        public string Key { get; }
        public string Secret { get; }
        public bool Use { get; protected set; }
    }
}