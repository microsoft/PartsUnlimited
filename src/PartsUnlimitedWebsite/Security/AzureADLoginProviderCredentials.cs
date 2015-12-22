// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace PartsUnlimited.Security
{
    public class AzureADLoginProviderCredentials : IAzureADLoginProviderCredentials
    {
        public AzureADLoginProviderCredentials(IConfiguration config)
        {
            ClientId = config["ClientId"];
            Authority = config["Authority"];
            RedirectUri = config["RedirectUri"];
            Caption = config["Caption"];

            Use = !string.IsNullOrWhiteSpace(ClientId)
                && !string.IsNullOrWhiteSpace(Authority)
                && !string.IsNullOrWhiteSpace(RedirectUri);
        }

        public string Authority { get; }
        public string Caption { get; }
        public string ClientId { get; }
        public string RedirectUri { get; }

        public bool Use { get; }
    }
}