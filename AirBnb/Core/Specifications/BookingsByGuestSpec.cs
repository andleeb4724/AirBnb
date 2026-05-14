using AirBnb.Core.Entities;

namespace AirBnb.Core.Specifications
{
    public class BookingsByGuestSpec : BaseSpecification<Booking>
    {
        public BookingsByGuestSpec(string guestId)
        {
            AddCriteria(b => b.GuestId == guestId);
            AddInclude(b => b.Property);
        }
    }
}
