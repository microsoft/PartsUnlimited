// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.WebsiteConfiguration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PartsUnlimited.Recommendations
{
    public class AzureMLAuthenticatedHttpClient : HttpClient, IAzureMLAuthenticatedHttpClient
    {
        public AzureMLAuthenticatedHttpClient(IAzureMLFrequentlyBoughtTogetherConfig config)
        {
            var accountKey = Encoding.ASCII.GetBytes(config.AccountKey);
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(accountKey));
            DefaultRequestHeaders.Authorization = header;
        }
    }
}