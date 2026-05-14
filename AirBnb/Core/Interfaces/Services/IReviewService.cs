using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Review;

namespace AirBnb.Core.Interfaces.Services
{
    public interface IReviewService
    {
        Task<ServiceResult<ReviewResponseDto>> CreateReviewAsync(CreateReviewDto dto, string guestId);
        Task<ServiceResult<IReadOnlyList<ReviewResponseDto>>> GetReviewsByPropertyAsync(int propertyId);
    }
}
