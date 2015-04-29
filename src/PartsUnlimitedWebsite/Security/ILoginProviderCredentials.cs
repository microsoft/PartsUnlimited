// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.Security
{
    public interface ILoginProviderCredentials
    {
        string Key { get; }
        string Secret { get; }
        bool Use { get; }
    }
}