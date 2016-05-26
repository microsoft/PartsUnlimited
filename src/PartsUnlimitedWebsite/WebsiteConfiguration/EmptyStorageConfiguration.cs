// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using Microsoft.WindowsAzure.Storage;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class EmptyStorageConfiguration : IAzureStorageConfiguration
    {
        public CloudStorageAccount StorageAccount => null;
        public string ContainerName => null;
        public bool SupportImageUpload => false;
    }
}