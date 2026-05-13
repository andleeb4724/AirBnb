using AirBnb.Data;
using AirBnb.Dtos;
using AirBnb.Entities;
using AirBnb.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirBnb.Services
{
    public class PropertyService : IPropertyInterface
    {
        private readonly AppDbContext _context;
        private readonly AzureBlobStorageService _azureBlobStorageService;

        public PropertyService(AppDbContext context, AzureBlobStorageService azureBlobStorageService)
        {
            _context = context;
            _azureBlobStorageService = azureBlobStorageService;
        }

        public async Task<IActionResult> AddProperty([FromForm] CreatePropertyDto dto)
        {
            string imageUrl = null;
            if (dto.ImageFile != null)
            {
                using (var stream = dto.ImageFile.OpenReadStream())
                {
                    imageUrl = await _azureBlobStorageService.UploadImageAsync(stream, dto.ImageFile.FileName);
                }
            }

            var property = new Property
            {
                Title = dto.Title,
                Description = dto.Description,
                PricePerNight = dto.PricePerNight,
                ImageUrl = imageUrl,
                HostId = "15383F0B-069C-48E5-80B7-82FEEF01E50D"
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return new OkObjectResult(property);
        }

        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _context.Properties.ToListAsync();
            return new OkObjectResult(properties);
        }

        public async Task<IActionResult> GetPropertiesById(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return new NotFoundObjectResult("Property Not Found");
            }
            return new OkObjectResult(property);
        }

        public async Task<IActionResult> UpdateProperty(int id, [FromBody] UpdatePropertyDto dto)
        {
            string imageUrl = null;
            if (dto.ImageFile != null)
            {
                using (var stream = dto.ImageFile.OpenReadStream())
                {
                    imageUrl = await _azureBlobStorageService.UploadImageAsync(stream, dto.ImageFile.FileName);
                }
            }
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return new NotFoundObjectResult("Property not found");
            }

            property.Title = dto.Title;
            property.Description = dto.Description;
            property.PricePerNight = dto.PricePerNight;
            property.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();

            return new OkObjectResult(property);
        }

        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return new NotFoundObjectResult("Property not found");
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return new OkObjectResult("Property deleted successfully");
        }

        public async Task<IActionResult> UploadPropertyImage(int id, IFormFile file, AzureBlobStorageService blobStorageService)
        {
            if (file == null || file.Length == 0)
                return new BadRequestObjectResult("File is empty.");

            using var stream = file.OpenReadStream();
            string imageUrl = await blobStorageService.UploadImageAsync(stream, $"{id}/{file.FileName}");

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return new NotFoundObjectResult("Property not found");

            var propertyImage = new PropertyImage { PropertyId = id, ImageUrl = imageUrl };
            _context.PropertyImages.Add(propertyImage);
            await _context.SaveChangesAsync();

            return new OkObjectResult(imageUrl);
        }
    }
}
