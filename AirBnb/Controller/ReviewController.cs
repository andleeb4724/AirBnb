using AirBnb.Data;
using AirBnb.Dtos;
using AirBnb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirBnb.Controller
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Guest")] // Only Guests can leave reviews
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.PropertyId == dto.PropertyId && b.GuestId == User.Identity.Name);
            if (booking == null)
                return BadRequest("You have not booked this property.");

            var review = new Review
            {
                PropertyId = dto.PropertyId,
                GuestId = User.Identity.Name,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return Ok(review);
        }

    }
}
