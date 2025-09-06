using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.Auth;

namespace RealEstateMillion.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<bool>> LogoutAsync(string refreshToken);
    }
}
