using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace AirBnb.Entities
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public string? HostId { get; set; }
        public User Host { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
