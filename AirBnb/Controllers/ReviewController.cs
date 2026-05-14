using AirBnb.Core.DTOs.Review;
using AirBnb.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AirBnb.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var guestId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.CreateReviewAsync(dto, guestId!);

            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetReviewsByProperty(int propertyId)
        {
            var result = await _reviewService.GetReviewsByPropertyAsync(propertyId);
            return Ok(result.Data);
        }
    }
}
