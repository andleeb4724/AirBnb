using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces.Repositories;
using AirBnb.Infrastructure.Data;

namespace AirBnb.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }
    }
}
