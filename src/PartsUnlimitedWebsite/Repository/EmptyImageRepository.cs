// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PartsUnlimited.Repository
{
    public class EmptyImageRepository : IImageRepository
    {
        public Task<string> Upload(Stream image, string contentDisposition, string contentType)
        {
            //Todo fill in when we allow uploading of images without storing in doc db.
            throw new NotImplementedException();
        }

        public Task<string> UploadAndAttachToProduct(int productId, IEnumerable<string> colors, IEnumerable<string> categories, byte[] fileBytes)
        {
            //Todo fill in when we allow uploading of images without storing in doc db.
            throw new NotImplementedException();
        }
    }
}