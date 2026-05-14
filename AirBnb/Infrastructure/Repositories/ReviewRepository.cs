using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces.Repositories;
using AirBnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AirBnb.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Review>> GetReviewsByPropertyAsync(int propertyId) =>
            await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();
    }
}
