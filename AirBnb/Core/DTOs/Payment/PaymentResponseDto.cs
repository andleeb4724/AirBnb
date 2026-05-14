namespace AirBnb.Core.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }
}
