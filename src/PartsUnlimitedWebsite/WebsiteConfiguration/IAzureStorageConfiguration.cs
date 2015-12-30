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
        Task<string> Upload(IFormFile file);
        Task<string> UploadAndAttachToProduct(int productId, byte[] fileBytes, AnalysisResult imageAnalysis);
    }
}