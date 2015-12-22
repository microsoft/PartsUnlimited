// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class ConfigurationApplicationInsightsSettings : IApplicationInsightsSettings
    {
        public ConfigurationApplicationInsightsSettings(IConfiguration config)
        {
            InstrumentationKey = config[nameof(InstrumentationKey)];
        }

        public string InstrumentationKey { get; }
    }
}