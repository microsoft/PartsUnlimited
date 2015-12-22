// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using PartsUnlimited.Telemetry;
using System;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class ConfigurationWebsiteOptions : IWebsiteOptions
    {
        public ConfigurationWebsiteOptions(IConfiguration config, ITelemetryProvider log)
        {
            try
            {
                ShowRecommendations = Boolean.Parse(config["ShowRecommendations"]);
            }
            catch (InvalidCastException e)
            {
                log.TrackException(e);
            }
        }

        public bool ShowRecommendations { get; }
    }
}