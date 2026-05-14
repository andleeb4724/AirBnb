using System.ComponentModel.DataAnnotations;

namespace AirBnb.Core.DTOs.Property
{
    public class UpdatePropertyDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000, ErrorMessage = "Price per night must be between 1 and 100000")]
        public decimal PricePerNight { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
