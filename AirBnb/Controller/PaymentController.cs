using AirBnb.Data;
using AirBnb.Dtos;
using AirBnb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirBnb.Controller
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Guest")] // Only Guests can make payments
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);
            if (booking == null)
                return NotFound("Booking not found.");

            // Ensure the logged-in user is the guest who made the booking
            if (booking.GuestId != User.Identity.Name)
                return Unauthorized("You are not authorized to pay for this booking.");

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                PaymentStatus = "Completed",
                Amount = booking.TotalAmount,
                PaymentDate = DateTime.UtcNow
            };

            booking.IsPaid = true;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return Ok(payment);
        }
    }
}
