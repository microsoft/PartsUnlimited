// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsUnlimited.Recommendations
{
    public interface IRecommendationEngine
    {
        Task<IEnumerable<string>> GetRecommendationsAsync(string recommendationId);
    }
}