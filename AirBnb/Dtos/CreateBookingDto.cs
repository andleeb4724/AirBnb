namespace AirBnb.Dtos
{
    public class CreateBookingDto
    {
        public int PropertyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
