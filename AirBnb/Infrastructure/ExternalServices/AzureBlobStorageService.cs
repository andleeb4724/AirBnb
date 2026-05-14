using AirBnb.Core.Interfaces.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace AirBnb.Infrastructure.ExternalServices
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"]!;
            _containerName = configuration["AzureStorage:ContainerName"]!;
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public string? GetBlobSasUri(string fileName, int expiryMinutes = 60)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            if (!blobClient.Exists())
                return null;

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;
            return blobClient.Uri + sasToken;
        }
    }
}
