// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.Security
{
    public interface IAzureADLoginProviderCredentials
    {
        string ClientId { get; }
        string Caption { get; }
        string Authority { get; }
        string RedirectUri { get; }
        bool Use { get; }
    }
}