using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Common.Extensions;
using WhoIsTheBestBoyAPI.Services.IServices;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WhoIsTheBestBoyAPI.Services
{
    public class BlobService : IBlobService
    {

        private readonly BlobServiceClient _blolbServiceClient;


        public BlobService(BlobServiceClient blolbServiceClient)
        {
            _blolbServiceClient = blolbServiceClient;
        }
        public async Task<BlobInfo> GetBlobAsync(string blobName, string containerName)
        {
            var containerClient = _blolbServiceClient.GetBlobContainerClient(containerName);
            var bobClient = containerClient.GetBlobClient(blobName);
            var blobDownloadInfo = await bobClient.DownloadAsync();


            return new BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
        }
        public string GetBlobURL(string blobName, string containerName)
        {



            var containerClient = _blolbServiceClient.GetBlobContainerClient(containerName);
            var bobClient = containerClient.GetBlobClient(blobName);

            //To avoid hot linking we can create a token with expiration date and change the blob to private
            string uri = bobClient.Uri.AbsoluteUri;
            string sasToken = GenerateSASToken(blobName, containerName);
            return $"{uri}?{sasToken}";


        }

        private static string GenerateSASToken(string blobName, string containerName)
        {

            var sharedKeyCredential = new StorageSharedKeyCredential(
                Environment.GetEnvironmentVariable("BLOB_ACCOUNT_NAME"),
                Environment.GetEnvironmentVariable("BLOB_ACCOUNT_KEY")
                );

            var blobSasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                StartsOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddMinutes(60),
                Protocol = SasProtocol.HttpsAndHttp,
            };

            blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            // Use the key to get the SAS token.
            string sasToken = blobSasBuilder.ToSasQueryParameters(sharedKeyCredential).ToString();
            return sasToken;
        }

        public async Task DeleteBlobAsync(string blobName, string containerName)
        {
            var containerClient = _blolbServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> UploadContentBlobAsync(byte[] bytes, string fileName, string containerName)
        {
            try
            {
                var containerClient = _blolbServiceClient.GetBlobContainerClient(containerName);
                //Check upload file is an image and get extension
                string format = bytes.GetImageExtension();
                if (string.IsNullOrEmpty(format))
                {
                    return "";
                }

                //Create final name including format
                fileName = fileName + "." + format;
                var blobClient = containerClient.GetBlobClient(fileName);
                var contentType = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = fileName.GetContentType() };

                //Upload file
                await using MemoryStream stream = new MemoryStream(bytes);
                var response = await blobClient.UploadAsync(stream, contentType);
                
                return fileName;
            }
            catch
            {
                return "";
            }


        }

        public async Task UploadFileBlobAsync(string filePath, string fileName, string containerName)
        {
            var containerClient = _blolbServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var contentType = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = filePath.GetContentType() };

            var res = await blobClient.UploadAsync(filePath, contentType);
        }


    }
}
