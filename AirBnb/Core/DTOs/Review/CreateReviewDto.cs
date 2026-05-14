using System.ComponentModel.DataAnnotations;

namespace AirBnb.Core.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}
