// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.Security
{
    public interface ILoginProviders
    {
        ILoginProviderCredentials Facebook { get; }
        ILoginProviderCredentials Google { get; }
        ILoginProviderCredentials Microsoft { get; }
        ILoginProviderCredentials Twitter { get; }
        IAzureADLoginProviderCredentials Azure { get; }
    }
}