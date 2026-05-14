using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Review;
using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces;
using AirBnb.Core.Interfaces.Services;
using AirBnb.Infrastructure.Extensions;

namespace AirBnb.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _uow;

        public ReviewService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<ReviewResponseDto>> CreateReviewAsync(CreateReviewDto dto, string guestId)
        {
            var hasBooked = await _uow.Bookings.GuestHasBookedPropertyAsync(guestId, dto.PropertyId);
            if (!hasBooked)
                return ServiceResult<ReviewResponseDto>.Fail("You have not booked this property.");

            var review = new Review
            {
                PropertyId = dto.PropertyId,
                GuestId = guestId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _uow.Reviews.AddAsync(review);
            await _uow.CompleteAsync();

            return ServiceResult<ReviewResponseDto>.Ok(review.ToResponseDto());
        }

        public async Task<ServiceResult<IReadOnlyList<ReviewResponseDto>>> GetReviewsByPropertyAsync(int propertyId)
        {
            var reviews = await _uow.Reviews.GetReviewsByPropertyAsync(propertyId);
            var result = reviews.Select(r => r.ToResponseDto()).ToList();
            return ServiceResult<IReadOnlyList<ReviewResponseDto>>.Ok(result);
        }
    }
}
