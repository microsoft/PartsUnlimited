// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Recommendations
{
    public class EmptyRecommendationsEngine : IRecommendationEngine
    {
        public Task<IEnumerable<string>> GetRecommendationsAsync(string recommendationId)
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }
    }
}