using AirBnb.Core.Entities;

namespace AirBnb.Core.Interfaces.Repositories
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<IReadOnlyList<Property>> GetPropertiesWithHostAsync();
        Task<Property?> GetPropertyWithHostAsync(int id);
    }
}
