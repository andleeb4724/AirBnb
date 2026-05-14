using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces.Repositories;
using AirBnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AirBnb.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<bool> GuestHasBookedPropertyAsync(string guestId, int propertyId) =>
            await _context.Bookings.AnyAsync(b => b.GuestId == guestId && b.PropertyId == propertyId);

        public async Task<IReadOnlyList<Booking>> GetBookingsByGuestAsync(string guestId) =>
            await _context.Bookings
                .Include(b => b.Property)
                .Where(b => b.GuestId == guestId)
                .ToListAsync();

        public async Task<IReadOnlyList<Booking>> GetAllWithDetailsAsync() =>
            await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Guest)
                .ToListAsync();
    }
}
