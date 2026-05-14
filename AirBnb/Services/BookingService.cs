using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Booking;
using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces;
using AirBnb.Core.Interfaces.Services;
using AirBnb.Infrastructure.Extensions;

namespace AirBnb.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _uow;

        public BookingService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<IReadOnlyList<BookingResponseDto>>> GetAllAsync()
        {
            var bookings = await _uow.Bookings.GetAllWithDetailsAsync();
            var result = bookings.Select(b => b.ToResponseDto()).ToList();
            return ServiceResult<IReadOnlyList<BookingResponseDto>>.Ok(result);
        }

        public async Task<ServiceResult<BookingResponseDto>> CreateBookingAsync(CreateBookingDto dto, string guestId)
        {
            var property = await _uow.Properties.GetByIdAsync(dto.PropertyId);
            if (property is null)
                return ServiceResult<BookingResponseDto>.NotFound("Property not found.");

            var days = (dto.EndDate - dto.StartDate).Days;
            if (days <= 0)
                return ServiceResult<BookingResponseDto>.Fail("End date must be after start date.");

            var booking = new Booking
            {
                PropertyId = dto.PropertyId,
                GuestId = guestId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalAmount = days * property.PricePerNight,
                IsPaid = false
            };

            await _uow.Bookings.AddAsync(booking);
            await _uow.CompleteAsync();

            booking.Property = property;
            return ServiceResult<BookingResponseDto>.Ok(booking.ToResponseDto());
        }
    }
}
