using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class VisionApiConfiguration : IVisionApiConfiguration
    {
        public string Key { get; }
        public string Uri { get; }

        public VisionApiConfiguration(IConfiguration config)
        {
            Key = config["Key"];
            Uri = config["Uri"];
        }

        public async Task<AnalysisResult> AnalyseImage(string imageUrl)
        {
            var vision = new VisionServiceClient(Key);
            var visualFeatures = new[] { "Color", "Categories" };
            return await vision.AnalyzeImageAsync(imageUrl, visualFeatures);
        }

        public async Task<byte[]> GenerateThumbnail(string imageUrl)
        {
            var vision = new VisionServiceClient(Key);
            return await vision.GetThumbnailAsync(imageUrl, 295, 295, true);
        }
    }
}

