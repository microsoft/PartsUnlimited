// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Framework.Configuration;
using Microsoft.Net.Http.Headers;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class AzureStorageConfiguration : IAzureStorageConfiguration
    {
        public string ConnectionString { get; }
        public CloudStorageAccount CloudStorageAccount { get; }

        private readonly IDocDbConfiguration _docDbConfiguration;
        private readonly DocumentClient _documentDbClient;

        public AzureStorageConfiguration(IConfiguration config, IDocDbConfiguration configuration)
        {
            ConnectionString = config["ConnectionString"];
            CloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);
            _docDbConfiguration = configuration;
            _documentDbClient = configuration.BuildClient();
        }

        public async Task Download(string fileName, string containerName, Stream outStream)
        {
            await PerformOperation(fileName, containerName, blob => blob.DownloadRangeToStreamAsync(outStream, null, null));
        }

        public async Task<byte[]> Download(string fileName, string containerName)
        {
            byte[] bytes = null;
            await PerformOperation(
                fileName, containerName, async blob =>
                {
                    bytes = new byte[blob.Properties.Length];
                    await blob.DownloadRangeToByteArrayAsync(bytes, 0, null, null);
                });
            return bytes;
        }

        public async Task Delete(string fileName, string containerName)
        {
            await PerformOperation(fileName, containerName, blob => blob.DeleteAsync());
        }

        private async Task PerformOperation(string fileName, string containerName, Func<CloudBlockBlob, Task> operation)
        {
            var account = CloudStorageAccount;
            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);
            if (!(await container.ExistsAsync()))
                throw new ArgumentException("The specified container name does not exist", containerName);

            var blob = container.GetBlockBlobReference(fileName);
            if (!(await blob.ExistsAsync()))
                throw new FileNotFoundException("The specified file does not exist in the container");

            await operation(blob);
        }

        public async Task<bool> Exists(string fileName, string containerName)
        {
            var account = CloudStorageAccount;
            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);
            if (!(await container.ExistsAsync()))
                return false;

            var blob = container.GetBlockBlobReference(fileName);
            return await blob.ExistsAsync();
        }

        public async Task<string> Upload(string containerName, IFormFile file)
        {
            var client = CloudStorageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = WebUtility.UrlEncode(parsedContentDisposition.FileName.Replace("\"", ""));

            var newBlob = container.GetBlockBlobReference(fileName);
            newBlob.Properties.ContentType = file.ContentType;

            using (var stream = new MemoryStream())
            {
                //await newBlob.UploadFromStreamAsync(fileStream);

                // opting for UploadFromByteArrayAsync() as a work around to an existing issue with certain images in Azure Storage SDK
                // see https://github.com/Azure/azure-storage-net/issues/202
                file.OpenReadStream().CopyTo(stream);
                var fileBytes = stream.ToArray();
                await newBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);
                
                return newBlob.Uri.ToString();
            }
        }

        public async Task<string> UploadAndAttachToProduct(int productId, string containerName, byte[] fileBytes, AnalysisResult imageAnalysis)
        {
            var client = CloudStorageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            var fileName = $"{new Guid()}.jpg";

            var newBlob = container.GetBlockBlobReference(fileName);
            await newBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);

            var imageUrl = newBlob.Uri.ToString();

            await AttachToDocumentDb(productId, fileName, imageUrl, imageAnalysis);

            return imageUrl;

        }

        private async Task AttachToDocumentDb(int productId, string imageId, string imageUrl, AnalysisResult imageAnalysis)
        {
            var productLink = _docDbConfiguration.BuildProductLink(productId);

            var productArtCategories = imageAnalysis.Categories.Select(c => c.Name).ToArray();
            var productArtColors = imageAnalysis.Color.DominantColors;

            await _documentDbClient.CreateAttachmentAsync(productLink, new { id = imageId, contentType = "image/jpeg", media = imageUrl, categories = productArtCategories, colors = productArtColors });
        }
    }
}

