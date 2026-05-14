using AirBnb.Core.DTOs.Auth;
using AirBnb.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirBnb.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result.IsSuccess
                ? Ok(new { message = result.Data })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }
    }
}
