using System.ComponentModel.DataAnnotations;

namespace RealEstateMillion.Application.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public UserInfo User { get; set; } = null!;
    }

    public class UserInfo
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
}
