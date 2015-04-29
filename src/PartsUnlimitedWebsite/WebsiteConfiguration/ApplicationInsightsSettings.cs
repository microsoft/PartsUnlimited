// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Framework.ConfigurationModel;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class ConfigurationApplicationInsightsSettings : IApplicationInsightsSettings
    {
        public ConfigurationApplicationInsightsSettings(IConfiguration config)
        {
            InstrumentationKey = config.Get(nameof(InstrumentationKey));
        }

        public string InstrumentationKey { get; }
    }
}