using AirBnb.Data;
using AirBnb.Dtos;
using AirBnb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirBnb.Controller
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Guest)
                .ToListAsync();
            return Ok(bookings);
        }

        [HttpPost]
        [Authorize(Roles = "Guest")] // Only Guests can create bookings
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            var property = await _context.Properties.FindAsync(dto.PropertyId);
            if (property == null)
                return NotFound("Property not found.");

            var booking = new Booking
            {
                PropertyId = dto.PropertyId,
                GuestId = User.Identity.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalAmount = (decimal)(dto.EndDate - dto.StartDate).TotalDays * property.PricePerNight,
                IsPaid = false
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }
    }
}
