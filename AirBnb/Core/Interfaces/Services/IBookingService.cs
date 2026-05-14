using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Booking;

namespace AirBnb.Core.Interfaces.Services
{
    public interface IBookingService
    {
        Task<ServiceResult<IReadOnlyList<BookingResponseDto>>> GetAllAsync();
        Task<ServiceResult<BookingResponseDto>> CreateBookingAsync(CreateBookingDto dto, string guestId);
    }
}
