using Microsoft.AspNetCore.Identity;

namespace AirBnb.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
