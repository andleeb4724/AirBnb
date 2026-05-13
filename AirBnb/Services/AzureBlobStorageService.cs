using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace AirBnb.Services
{
    public class AzureBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"];
            _containerName = configuration["AzureStorage:ContainerName"];
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None); // Keep it private

            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);

            return blobClient.Uri.ToString(); // Returns the blob URL (without SAS token)
        }

        public string GetBlobSasUri(string fileName, int expiryMinutes = 60)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            if (!blobClient.Exists())
                return null;

            // Create SAS token
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _containerName,
                BlobName = fileName,
                Resource = "b",  // "b" for blob access
                ExpiresOn = DateTime.UtcNow.AddMinutes(expiryMinutes) // Set expiry time for token
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read); // Read-only access

            var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;
            return blobClient.Uri + sasToken; // Returns SAS URL
        }
    }
}
