using AirBnb.Core.Interfaces.Repositories;
using AirBnb.Core.Specifications;
using AirBnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AirBnb.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id) =>
            await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() =>
            await _context.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec) =>
            await ApplySpecification(spec).ToListAsync();

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<int> CountAsync(ISpecification<T> spec) =>
            await ApplySpecification(spec).CountAsync();

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            var query = _context.Set<T>().AsQueryable();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            query = spec.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }
    }
}
