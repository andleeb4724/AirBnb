using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Payment;

namespace AirBnb.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentResponseDto>> ProcessPaymentAsync(CreatePaymentDto dto, string guestId);
    }
}
