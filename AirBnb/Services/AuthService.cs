using AirBnb.Core.Common;
using AirBnb.Core.DTOs.Auth;
using AirBnb.Core.Entities;
using AirBnb.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace AirBnb.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult<AuthResponseDto>.Unauthorized("Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return ServiceResult<AuthResponseDto>.Unauthorized("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return ServiceResult<AuthResponseDto>.Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles
            });
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ServiceResult<string>.Fail(errors);
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
            return ServiceResult<string>.Ok("User registered successfully.");
        }
    }
}
