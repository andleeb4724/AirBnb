using AirBnb.Core.Interfaces;
using AirBnb.Core.Interfaces.Repositories;
using AirBnb.Infrastructure.Data;

namespace AirBnb.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IPropertyRepository Properties { get; }
        public IBookingRepository Bookings { get; }
        public IPaymentRepository Payments { get; }
        public IReviewRepository Reviews { get; }

        public UnitOfWork(AppDbContext context,
            IPropertyRepository properties,
            IBookingRepository bookings,
            IPaymentRepository payments,
            IReviewRepository reviews)
        {
            _context = context;
            Properties = properties;
            Bookings = bookings;
            Payments = payments;
            Reviews = reviews;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
