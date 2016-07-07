// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using PartsUnlimited.WebsiteConfiguration;
using System.Threading.Tasks;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "Configuration")]
    public class ConfigurationComponent : ViewComponent
    {
        private readonly IApplicationInsightsSettings _appInsights;

        public ConfigurationComponent(IApplicationInsightsSettings appInsights)
        {
            _appInsights = appInsights;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View(_appInsights));
        }
    }
}