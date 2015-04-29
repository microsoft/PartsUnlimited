// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using PartsUnlimited.Telemetry;
using PartsUnlimited.WebsiteConfiguration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PartsUnlimited.Recommendations
{
    /// <summary>
    /// This class implements Azure ML Frequently Bought Together recommendation engine
    /// Details can be found at https://datamarket.azure.com/dataset/amla/mba
    /// </summary>
    public class AzureMLFrequentlyBoughtTogetherRecommendationEngine : IRecommendationEngine
    {
        private readonly IAzureMLFrequentlyBoughtTogetherConfig _config;
        private readonly IAzureMLAuthenticatedHttpClient _client;
        private readonly ITelemetryProvider _telemetry;

        private class AzureMLFrequentlyBoughtTogetherServiceResponse
        {
            public List<string> ItemSet { get; set; }
            public int Value { get; set; }
        }

        public AzureMLFrequentlyBoughtTogetherRecommendationEngine(IAzureMLFrequentlyBoughtTogetherConfig configFile, IAzureMLAuthenticatedHttpClient httpClient, ITelemetryProvider telemetryProvider)
        {
            _config = configFile;
            _client = httpClient;
            _telemetry = telemetryProvider;
        }

        public async Task<IEnumerable<string>> GetRecommendationsAsync(string recommendationId)
        {
            //The Azure ML service takes in a recommendation model name (trained ahead of time) and a product id
            string uri = $"https://api.datamarket.azure.com/data.ashx/amla/mba/v1/Score?Id=%27{_config.ModelName}%27&Item=%27{recommendationId}%27";

            try
            {
                //The Azure ML service returns a set of numbers, which indicate the recommended product id
                var response = await _client.GetStringAsync(uri);
                AzureMLFrequentlyBoughtTogetherServiceResponse deserializedResponse = JsonConvert.DeserializeObject<AzureMLFrequentlyBoughtTogetherServiceResponse>(response);
                //When there is no recommendation, The Azure ML service returns a JSON object that does not contain ItemSet
                var recommendation = deserializedResponse.ItemSet;
                if (recommendation == null)
                {
                    return Enumerable.Empty<string>();
                }
                else
                {
                    return recommendation;
                }
            }
            catch (HttpRequestException e)
            {
                _telemetry.TrackException(e);

                return Enumerable.Empty<string>();
            }
        }
    }
}