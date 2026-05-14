using System.ComponentModel.DataAnnotations;

namespace AirBnb.Core.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Host|Guest)$", ErrorMessage = "Role must be 'Host' or 'Guest'")]
        public string Role { get; set; } = string.Empty;
    }
}
