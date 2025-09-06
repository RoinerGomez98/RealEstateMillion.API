using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.Auth;
using RealEstateMillion.Application.Services.Interfaces;
using RealEstateMillion.Domain.Interfaces;

namespace RealEstateMillion.Application.Services.Implementations
{
    public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<AuthService> logger) : IAuthService
    {
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                logger.LogInformation("Login attempt for email: {Email}", request.Email);

                if (!IsValidUser(request.Email, request.Password))
                {
                    logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
                    return ApiResponse<LoginResponse>.ErrorResponse("Invalid email or password", 401);
                }

                var user = GetUserInfo(request.Email);
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                var response = new LoginResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = user
                };

                logger.LogInformation("Successful login for email: {Email}", request.Email);
                return ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return ApiResponse<LoginResponse>.ErrorResponse("An error occurred during login", 500);
            }
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var user = new UserInfo
                {
                    Email = "demo@realestate.com",
                    Name = "Demo User",
                    Roles = ["Admin"]
                };

                var newToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                var response = new LoginResponse
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = user
                };

                return ApiResponse<LoginResponse>.SuccessResponse(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error refreshing token");
                return ApiResponse<LoginResponse>.ErrorResponse("Invalid refresh token", 401);
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string refreshToken)
        {
            try
            {
                logger.LogInformation("User logged out");
                return ApiResponse<bool>.SuccessResponse(true, "Logout successful");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during logout");
                return ApiResponse<bool>.ErrorResponse("An error occurred during logout", 500);
            }
        }

        private static bool IsValidUser(string email, string password)
        {
            var validUsers = new Dictionary<string, string>
        {
            { "admin@realestate.com", "admin123" },
            { "agent@realestate.com", "agent123" },
            { "demo@realestate.com", "demo123" }
        };

            return validUsers.ContainsKey(email.ToLower()) && validUsers[email.ToLower()] == password;
        }

        private static UserInfo GetUserInfo(string email)
        {
            return email.ToLower() switch
            {
                "admin@realestate.com" => new UserInfo
                {
                    Email = email,
                    Name = "Administrator",
                    Roles = ["Admin", "Agent"]
                },
                "agent@realestate.com" => new UserInfo
                {
                    Email = email,
                    Name = "Real Estate Agent",
                    Roles = ["Agent"]
                },
                _ => new UserInfo
                {
                    Email = email,
                    Name = "Demo User",
                    Roles = ["User"]
                }
            };
        }

        private string GenerateJwtToken(UserInfo user)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "FhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPC1";
            var issuer = jwtSettings["Issuer"] ?? "RealEstateAPI";
            var audience = jwtSettings["Audience"] ?? "RealEstateClients";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new (ClaimTypes.Name, user.Name),
            new (JwtRegisteredClaimNames.Sub, user.Email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

}
