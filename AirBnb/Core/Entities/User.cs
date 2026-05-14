using Microsoft.AspNetCore.Identity;

namespace AirBnb.Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
