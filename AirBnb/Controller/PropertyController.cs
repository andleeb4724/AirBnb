using AirBnb.Data;
using AirBnb.Dtos;
using AirBnb.Entities;
using AirBnb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AirBnb.Interface;

namespace AirBnb.Controller
{
    [Route("api/properties")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyInterface _propertyService;

        public PropertyController(IPropertyInterface propertyService)
        {
            _propertyService = propertyService;
        }

        // Get All Properties
        [HttpGet]
        [Route("GetAllProperties")]
        public async Task<IActionResult> GetAllProperties()
        {
            return await _propertyService.GetAllProperties();
        }

        // Get Property by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertiesById(int id)
        {
            return await _propertyService.GetPropertiesById(id);
        }

        // Create a Property
        [HttpPost("addProperty")]
        public async Task<IActionResult> AddProperty([FromForm] CreatePropertyDto dto)
        {
            return await _propertyService.AddProperty(dto);
        }

        // Update Property
        [HttpPut("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> UpdateProperty(int id, [FromBody] UpdatePropertyDto dto)
        {
            return await _propertyService.UpdateProperty(id, dto);
        }

        // Delete Property
        [HttpDelete("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            return await _propertyService.DeleteProperty(id);
        }

        // Upload Property Image
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadPropertyImage(int id, IFormFile file, [FromServices] AzureBlobStorageService blobStorageService)
        {
            return await _propertyService.UploadPropertyImage(id, file, blobStorageService);
        }
    }

}
