// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision.Contract;

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IVisionApiConfiguration
    {
        Task<AnalysisResult> AnalyseImage(string imageUrl);
        Task<byte[]> GenerateThumbnail(string imageUrl);
    }
}
