using AirBnb.Core.Interfaces.Repositories;

namespace AirBnb.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPropertyRepository Properties { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }
        IReviewRepository Reviews { get; }
        Task<int> CompleteAsync();
    }
}
