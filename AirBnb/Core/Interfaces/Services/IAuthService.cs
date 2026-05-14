using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Auth;

namespace AirBnb.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ServiceResult<string>> RegisterAsync(RegisterDto dto);
    }
}
