// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace PartsUnlimited.Security
{
    public class ConfigurationLoginProviders : ILoginProviders
    {
        public ConfigurationLoginProviders(IConfiguration config)
        {
            Facebook = GetProvider(config, nameof(Facebook));
            Google = GetProvider(config, nameof(Google));
            Microsoft = GetProvider(config, nameof(Microsoft));
            Twitter = GetProvider(config, nameof(Twitter));
            Azure = new AzureADLoginProviderCredentials(config.GetSection(nameof(Azure)));
        }

        private ILoginProviderCredentials GetProvider(IConfiguration config, string providerName)
        {
            return new ConfigurationLoginProviderCredentials(config.GetSection(providerName));
        }

        public ILoginProviderCredentials Facebook { get; }
        public ILoginProviderCredentials Google { get; }
        public ILoginProviderCredentials Microsoft { get; }
        public ILoginProviderCredentials Twitter { get; }
        public IAzureADLoginProviderCredentials Azure { get; }
    }
}