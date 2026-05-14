using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Property;

namespace AirBnb.Core.Interfaces.Services
{
    public interface IPropertyReadService
    {
        Task<ServiceResult<IReadOnlyList<PropertyResponseDto>>> GetAllAsync();
        Task<ServiceResult<PropertyResponseDto>> GetByIdAsync(int id);
    }

    public interface IPropertyWriteService
    {
        Task<ServiceResult<PropertyResponseDto>> CreateAsync(CreatePropertyDto dto, string hostId);
        Task<ServiceResult<PropertyResponseDto>> UpdateAsync(int id, UpdatePropertyDto dto, string hostId);
        Task<ServiceResult<bool>> DeleteAsync(int id, string hostId);
        Task<ServiceResult<string>> UploadImageAsync(int propertyId, Stream fileStream, string fileName);
    }

    public interface IPropertyService : IPropertyReadService, IPropertyWriteService { }
}
