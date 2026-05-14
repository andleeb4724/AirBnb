using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Payment;
using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces;
using AirBnb.Core.Interfaces.Services;
using AirBnb.Infrastructure.Extensions;

namespace AirBnb.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;

        public PaymentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<PaymentResponseDto>> ProcessPaymentAsync(CreatePaymentDto dto, string guestId)
        {
            var booking = await _uow.Bookings.GetByIdAsync(dto.BookingId);
            if (booking is null)
                return ServiceResult<PaymentResponseDto>.NotFound("Booking not found.");

            if (booking.GuestId != guestId)
                return ServiceResult<PaymentResponseDto>.Forbidden("You are not authorized to pay for this booking.");

            if (booking.IsPaid)
                return ServiceResult<PaymentResponseDto>.Fail("This booking has already been paid.", 409);

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = booking.TotalAmount,
                PaymentStatus = "Completed",
                PaymentDate = DateTime.UtcNow
            };

            booking.IsPaid = true;

            await _uow.Payments.AddAsync(payment);
            await _uow.Bookings.UpdateAsync(booking);
            await _uow.CompleteAsync();

            return ServiceResult<PaymentResponseDto>.Ok(payment.ToResponseDto());
        }
    }
}
