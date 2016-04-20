// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using PartsUnlimited.WebsiteConfiguration;

namespace PartsUnlimited.Repository
{
    public class DocumentDBImageRepository : IImageRepository
    {
        private readonly IAzureStorageConfiguration _storageConfiguration;
        private readonly IDocumentDBConfiguration _documentDbConfiguration;

        public DocumentDBImageRepository(IAzureStorageConfiguration storageConfiguration, IDocumentDBConfiguration documentDbConfiguration)
        {
            _storageConfiguration = storageConfiguration;
            _documentDbConfiguration = documentDbConfiguration;
        }

        public async Task<string> Upload(Stream image, string contentDisposition, string contentType)
        {
            var client = _storageConfiguration.StorageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(_storageConfiguration.ContainerName);

            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(contentDisposition);
            var fileName = WebUtility.UrlEncode(parsedContentDisposition.FileName.Replace("\"", ""));

            var newBlob = container.GetBlockBlobReference(fileName);
            newBlob.Properties.ContentType = contentType;

            using (var stream = new MemoryStream())
            {
                //await newBlob.UploadFromStreamAsync(fileStream);

                // opting for UploadFromByteArrayAsync() as a work around to an existing issue with certain images in Azure Storage SDK
                // see https://github.com/Azure/azure-storage-net/issues/202
                image.CopyTo(stream);
                var fileBytes = stream.ToArray();
                await newBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);

                return newBlob.Uri.ToString();
            }
        }

        public async Task<string> UploadAndAttachToProduct(int productId, IEnumerable<string> colors, IEnumerable<string> categories, byte[] fileBytes)
        {
            var client = _storageConfiguration.StorageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(_storageConfiguration.ContainerName);

            var fileName = $"{Guid.NewGuid()}.jpg";

            var newBlob = container.GetBlockBlobReference(fileName);
            await newBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);

            var imageUrl = newBlob.Uri.ToString();

            await AttachToDocumentDB(productId, imageUrl, categories.ToArray(), colors.ToArray());

            return imageUrl;
        }

        private async Task AttachToDocumentDB(int productId, string imageUrl, string[] productArtCategories, string[] productArtColors)
        {
            var productLink = _documentDbConfiguration.BuildProductLink(productId);
            var client = _documentDbConfiguration.BuildClient();

            await client.CreateAttachmentAsync(productLink, new { id = productId.ToString(), contentType = "image/jpeg", media = imageUrl, categories = productArtCategories, colors = productArtColors });
        }
    }
}