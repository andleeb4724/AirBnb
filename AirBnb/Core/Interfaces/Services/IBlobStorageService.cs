namespace AirBnb.Core.Interfaces.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName);
        string? GetBlobSasUri(string fileName, int expiryMinutes = 60);
    }
}
