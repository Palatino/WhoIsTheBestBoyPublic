using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhoIsTheBestBoyAPI.Services.IServices
{
    public interface IBlobService
    {
        public Task<BlobInfo> GetBlobAsync(string blobName, string containerName);
        public string GetBlobURL(string blobName, string containerName);
        public Task UploadFileBlobAsync(string filePath, string fileName, string containerName);
        public Task<string> UploadContentBlobAsync(byte[] bytes, string filename, string containerName);
        public Task DeleteBlobAsync(string blobName, string containerName);
    }
}
