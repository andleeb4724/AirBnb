using AirBnb.Core.Entities;

namespace AirBnb.Core.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user, IList<string> roles);
    }
}
