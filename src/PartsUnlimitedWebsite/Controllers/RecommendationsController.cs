// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsUnlimited.Models;
using PartsUnlimited.Recommendations;
using PartsUnlimited.WebsiteConfiguration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Controllers
{
    public class RecommendationsController : Controller
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IRecommendationEngine _recommendation;
        private readonly IWebsiteOptions _option;

        public RecommendationsController(IPartsUnlimitedContext context, IRecommendationEngine recommendationEngine, IWebsiteOptions websiteOptions)
        {
            _db = context;
            _recommendation = recommendationEngine;
            _option = websiteOptions;
        }

        public async Task<IActionResult> GetRecommendations(string recommendationId)
        {
            if (!_option.ShowRecommendations)
            {
                return new EmptyResult();
            }

            var recommendedProductIds = await _recommendation.GetRecommendationsAsync(recommendationId);

            var productTasks = recommendedProductIds
                .Select(item => _db.Products.SingleOrDefaultAsync(c => c.RecommendationId == Convert.ToInt32(item)))
                .ToList();

            await Task.WhenAll(productTasks);

            var recommendedProducts = productTasks
                .Select(p => p.Result)
                .Where(p => p != null && p.RecommendationId != Convert.ToInt32(recommendationId))
                .ToList();

            return PartialView("_Recommendations", recommendedProducts);
        }
    }
}