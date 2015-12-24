// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.ProjectOxford.Vision.Contract;

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IAzureStorageConfiguration
    {
        Task Download(string fileName, string containerName, Stream outStream);

        Task<byte[]> Download(string fileName, string containerName);

        Task Delete(string fileName, string containerName);

        Task<bool> Exists(string fileName, string containerName);

        Task<string> Upload(string containerName, IFormFile file);
        Task<string> UploadAndAttachToProduct(int productId, string containerName, byte[] fileBytes, AnalysisResult imageAnalysis);
    }
}