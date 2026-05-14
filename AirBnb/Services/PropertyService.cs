using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Property;
using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces;
using AirBnb.Core.Interfaces.Services;
using AirBnb.Infrastructure.Extensions;

namespace AirBnb.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _uow;
        private readonly IBlobStorageService _blobService;

        public PropertyService(IUnitOfWork uow, IBlobStorageService blobService)
        {
            _uow = uow;
            _blobService = blobService;
        }

        public async Task<ServiceResult<IReadOnlyList<PropertyResponseDto>>> GetAllAsync()
        {
            var properties = await _uow.Properties.GetPropertiesWithHostAsync();
            var result = properties.Select(p => p.ToResponseDto()).ToList();
            return ServiceResult<IReadOnlyList<PropertyResponseDto>>.Ok(result);
        }

        public async Task<ServiceResult<PropertyResponseDto>> GetByIdAsync(int id)
        {
            var property = await _uow.Properties.GetPropertyWithHostAsync(id);
            if (property is null)
                return ServiceResult<PropertyResponseDto>.NotFound("Property not found.");

            return ServiceResult<PropertyResponseDto>.Ok(property.ToResponseDto());
        }

        public async Task<ServiceResult<PropertyResponseDto>> CreateAsync(CreatePropertyDto dto, string hostId)
        {
            string? imageUrl = null;
            if (dto.ImageFile != null)
            {
                using var stream = dto.ImageFile.OpenReadStream();
                imageUrl = await _blobService.UploadImageAsync(stream, dto.ImageFile.FileName);
            }

            var property = new Property
            {
                Title = dto.Title,
                Description = dto.Description,
                PricePerNight = dto.PricePerNight,
                ImageUrl = imageUrl,
                HostId = hostId
            };

            await _uow.Properties.AddAsync(property);
            await _uow.CompleteAsync();

            var saved = await _uow.Properties.GetPropertyWithHostAsync(property.Id);
            return ServiceResult<PropertyResponseDto>.Ok(saved!.ToResponseDto());
        }

        public async Task<ServiceResult<PropertyResponseDto>> UpdateAsync(int id, UpdatePropertyDto dto, string hostId)
        {
            var property = await _uow.Properties.GetByIdAsync(id);
            if (property is null)
                return ServiceResult<PropertyResponseDto>.NotFound("Property not found.");

            if (property.HostId != hostId)
                return ServiceResult<PropertyResponseDto>.Forbidden("You are not the owner of this property.");

            if (dto.ImageFile != null)
            {
                using var stream = dto.ImageFile.OpenReadStream();
                property.ImageUrl = await _blobService.UploadImageAsync(stream, dto.ImageFile.FileName);
            }

            property.Title = dto.Title;
            property.Description = dto.Description;
            property.PricePerNight = dto.PricePerNight;

            await _uow.Properties.UpdateAsync(property);
            await _uow.CompleteAsync();

            var updated = await _uow.Properties.GetPropertyWithHostAsync(id);
            return ServiceResult<PropertyResponseDto>.Ok(updated!.ToResponseDto());
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id, string hostId)
        {
            var property = await _uow.Properties.GetByIdAsync(id);
            if (property is null)
                return ServiceResult<bool>.NotFound("Property not found.");

            if (property.HostId != hostId)
                return ServiceResult<bool>.Forbidden("You are not the owner of this property.");

            await _uow.Properties.DeleteAsync(property);
            await _uow.CompleteAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<string>> UploadImageAsync(int propertyId, Stream fileStream, string fileName)
        {
            var property = await _uow.Properties.GetByIdAsync(propertyId);
            if (property is null)
                return ServiceResult<string>.NotFound("Property not found.");

            var imageUrl = await _blobService.UploadImageAsync(fileStream, $"{propertyId}/{fileName}");
            property.ImageUrl = imageUrl;

            await _uow.Properties.UpdateAsync(property);
            await _uow.CompleteAsync();

            return ServiceResult<string>.Ok(imageUrl);
        }
    }
}
