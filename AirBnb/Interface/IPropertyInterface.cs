using AirBnb.Dtos;
using AirBnb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirBnb.Interface
{
    public interface IPropertyInterface
    {
        Task<IActionResult> AddProperty([FromBody] CreatePropertyDto dto);
        Task<IActionResult> GetAllProperties();
        Task<IActionResult> GetPropertiesById(int id);
        Task<IActionResult> UpdateProperty(int id, [FromBody] UpdatePropertyDto dto);
        Task<IActionResult> DeleteProperty(int id);
        Task<IActionResult> UploadPropertyImage(int id, IFormFile file, AzureBlobStorageService blobStorageService);
    }
}
