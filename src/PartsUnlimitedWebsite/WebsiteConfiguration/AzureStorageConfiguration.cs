// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class AzureStorageConfiguration : IAzureStorageConfiguration
    {
        public string ConnectionString { get; }
        private readonly Lazy<CloudStorageAccount> _account; 

        public AzureStorageConfiguration(IConfiguration config)
        {
            ConnectionString = config["ConnectionString"];
            _account = new Lazy<CloudStorageAccount>(() =>CloudStorageAccount.Parse(ConnectionString));
            ContainerName = "product";
        }

        public CloudStorageAccount StorageAccount => _account.Value;

        public string ContainerName { get; }
        public bool SupportImageUpload => true;
    }
}

