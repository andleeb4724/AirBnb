using System.Linq.Expressions;

namespace AirBnb.Interface
{
    public interface IAsyncRepository<T>
    {
        Task<T> GetByIdAsync(long id);
        Task<T> GetByIdAsync(long id, string[] paths = null);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, bool noTracking = false);
        Task<IReadOnlyList<T>> ListSelectedColumnsAsync(ISpecification<T> spec, string Fields = "", bool noTracking = false);
        Task<T> AddAsync(T entity, bool updateUserActivity = true);
        Task UpdateAsync(T entity, bool updateUserActivity = true);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(ISpecification<T> spec);
        void DetachEntity(T entity);
        Task<IReadOnlyList<T>> FindByPredicate(Expression<Func<T, bool>> predicate);
        Task<IReadOnlyList<T>> FindByPredicateSingleResultset(Expression<Func<T, bool>> predicate);
        Task AddRange(IEnumerable<T> entities);
        Task RemoveRangeAsync(IEnumerable<T> entities);
    }
}
