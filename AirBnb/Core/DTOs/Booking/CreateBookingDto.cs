using System.ComponentModel.DataAnnotations;

namespace AirBnb.Core.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
