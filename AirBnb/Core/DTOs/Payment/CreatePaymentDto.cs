using System.ComponentModel.DataAnnotations;

namespace AirBnb.Core.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int BookingId { get; set; }
    }
}
