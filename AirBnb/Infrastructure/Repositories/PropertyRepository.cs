using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces.Repositories;
using AirBnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AirBnb.Infrastructure.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Property>> GetPropertiesWithHostAsync() =>
            await _context.Properties.Include(p => p.Host).ToListAsync();

        public async Task<Property?> GetPropertyWithHostAsync(int id) =>
            await _context.Properties.Include(p => p.Host).FirstOrDefaultAsync(p => p.Id == id);
    }
}
