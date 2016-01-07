// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PartsUnlimited.Repository
{
    public interface IImageRepository
    {
        Task<string> Upload(Stream image, string contentDisposition, string contentType);
        Task<string> UploadAndAttachToProduct(int productId, IEnumerable<string> colors, IEnumerable<string> categories, byte[] fileBytes);
    }
}