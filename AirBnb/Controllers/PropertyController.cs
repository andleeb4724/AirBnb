using AirBnb.Core.DTOs.Property;
using AirBnb.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AirBnb.Controllers
{
    [Route("api/properties")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProperties()
        {
            var result = await _propertyService.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            var result = await _propertyService.GetByIdAsync(id);
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpPost("addProperty")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> AddProperty([FromForm] CreatePropertyDto dto)
        {
            var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _propertyService.CreateAsync(dto, hostId!);

            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> UpdateProperty(int id, [FromForm] UpdatePropertyDto dto)
        {
            var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _propertyService.UpdateAsync(id, dto, hostId!);

            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _propertyService.DeleteAsync(id, hostId!);

            return result.IsSuccess
                ? Ok(new { message = "Property deleted successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpPost("{id}/upload")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> UploadPropertyImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "File is empty." });

            using var stream = file.OpenReadStream();
            var result = await _propertyService.UploadImageAsync(id, stream, file.FileName);

            return result.IsSuccess
                ? Ok(new { imageUrl = result.Data })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }
    }
}
