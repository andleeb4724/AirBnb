using AirBnb.Core.Entities;

namespace AirBnb.Core.Interfaces.Repositories
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<bool> GuestHasBookedPropertyAsync(string guestId, int propertyId);
        Task<IReadOnlyList<Booking>> GetBookingsByGuestAsync(string guestId);
        Task<IReadOnlyList<Booking>> GetAllWithDetailsAsync();
    }
}
