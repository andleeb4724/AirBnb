using AirBnb.Core.Entities;

namespace AirBnb.Core.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IReadOnlyList<Review>> GetReviewsByPropertyAsync(int propertyId);
    }
}
