using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobClient;

        public ContainerService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerNames = new();

            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerNames.Add(blobContainerItem.Name);
            }

            return containerNames;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerAndBlobNames = new()
            {
                "-----Account Name : " + _blobClient.AccountName + "-----",
                "---------------------------------------------------------------"
            };

            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add("-----" + blobContainerItem.Name);
                BlobContainerClient blobContainer = _blobClient.GetBlobContainerClient(blobContainerItem.Name);

                await foreach (BlobItem blobItem in blobContainer.GetBlobsAsync())
                {
                    BlobClient blobClient = blobContainer.GetBlobClient(blobItem.Name);
                    BlobProperties blobProperties = (await blobClient.GetPropertiesAsync()).Value;
                    string tempBlobToAdd = blobItem.Name;

                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        tempBlobToAdd += "(" + blobProperties.Metadata["title"] + ")";
                    }

                    containerAndBlobNames.Add(">>" + tempBlobToAdd);
                }

                containerAndBlobNames.Add("---------------------------------------------------------------");
            }

            return containerAndBlobNames;
        }
    }

    public class DisabledContainerService : IContainerService
    {
        private const string Message = "BlobConnection is missing. Add it to appsettings.json or user secrets before using blob features.";

        public Task CreateContainer(string containerName)
        {
            throw new InvalidOperationException(Message);
        }

        public Task DeleteContainer(string containerName)
        {
            throw new InvalidOperationException(Message);
        }

        public Task<List<string>> GetAllContainer()
        {
            return Task.FromResult(new List<string>());
        }

        public Task<List<string>> GetAllContainerAndBlobs()
        {
            return Task.FromResult(new List<string> { Message });
        }
    }
}
