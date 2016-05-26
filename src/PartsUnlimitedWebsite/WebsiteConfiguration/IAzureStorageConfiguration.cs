// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using Microsoft.WindowsAzure.Storage;

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IAzureStorageConfiguration
    {
        CloudStorageAccount StorageAccount { get; }
        string ContainerName { get; }
        bool SupportImageUpload { get; }
    }
}