// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using PartsUnlimited.Models;
using PartsUnlimited.WebsiteConfiguration;
using System.Threading.Tasks;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "Recommendations")]
    public class RecommendationsComponent : ViewComponent
    {
        private readonly IWebsiteOptions _options;

        public RecommendationsComponent(IWebsiteOptions options)
        {
            _options = options;
        }

        public Task<IViewComponentResult> InvokeAsync(Product product)
        {
            if (_options.ShowRecommendations)
            {
                return Task.FromResult<IViewComponentResult>(View(product));
            }
            else
            {
                return Task.FromResult<IViewComponentResult>(new EmptyViewComponentResult());
            }
        }

        private class EmptyViewComponentResult : IViewComponentResult
        {
            public void Execute(ViewComponentContext context) { }

            public Task ExecuteAsync(ViewComponentContext context)
            {
                return Task.FromResult(0);
            }
        }
    }
}