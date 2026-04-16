using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;

        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task<bool> CreateBlob(string name, IFormFile file, string containerName, BlobModel blobModel)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            IDictionary<string, string> metaData = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(blobModel.Title))
            {
                metaData.Add("title", blobModel.Title);
            }

            if (!string.IsNullOrWhiteSpace(blobModel.Comment))
            {
                metaData.Add("comment", blobModel.Comment);
            }

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metaData);
            return result != null;
        }

        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            var result = await blobClient.DeleteIfExistsAsync();
            return result.Value;
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            List<string> blobNames = new();

            await foreach (BlobItem blob in blobContainerClient.GetBlobsAsync())
            {
                blobNames.Add(blob.Name);
            }

            return blobNames;
        }

        public async Task<List<BlobModel>> GetAllBlobsWithUri(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            List<BlobModel> blobList = new();

            await foreach (BlobItem blob in blobContainerClient.GetBlobsAsync())
            {
                BlobClient blobClient = blobContainerClient.GetBlobClient(blob.Name);
                BlobProperties properties = (await blobClient.GetPropertiesAsync()).Value;

                BlobModel blobModel = new()
                {
                    Uri = blobClient.Uri.AbsoluteUri
                };

                if (properties.Metadata.ContainsKey("title"))
                {
                    blobModel.Title = properties.Metadata["title"];
                }

                if (properties.Metadata.ContainsKey("comment"))
                {
                    blobModel.Comment = properties.Metadata["comment"];
                }

                blobList.Add(blobModel);
            }

            return blobList;
        }

        public Task<string> GetBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            return Task.FromResult(blobClient.Uri.AbsoluteUri);
        }
    }

    public class DisabledBlobService : IBlobService
    {
        private const string Message = "BlobConnection is missing. Add it to appsettings.json or user secrets before using blob features.";

        public Task<List<string>> GetAllBlobs(string containerName)
        {
            return Task.FromResult(new List<string>());
        }

        public Task<List<BlobModel>> GetAllBlobsWithUri(string containerName)
        {
            return Task.FromResult(new List<BlobModel>());
        }

        public Task<string> GetBlob(string name, string containerName)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> CreateBlob(string name, IFormFile file, string containerName, BlobModel blobModel)
        {
            throw new InvalidOperationException(Message);
        }

        public Task<bool> DeleteBlob(string name, string containerName)
        {
            throw new InvalidOperationException(Message);
        }
    }
}
