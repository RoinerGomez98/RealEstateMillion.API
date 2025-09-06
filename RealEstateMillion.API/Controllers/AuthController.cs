using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.Auth;
using RealEstateMillion.Application.Services.Interfaces;

namespace RealEstateMillion.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            logger.LogInformation("Login request for email: {Email}", request.Email);

            var result = await authService.LoginAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>New JWT token</returns>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="401">Invalid refresh token</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] string refreshToken)
        {
            var result = await authService.RefreshTokenAsync(refreshToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// User logout
        /// </summary>
        /// <param name="refreshToken">Refresh token to invalidate</param>
        /// <returns>Logout confirmation</returns>
        /// <response code="200">Logout successful</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] string refreshToken)
        {
            var result = await authService.LogoutAsync(refreshToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        /// <returns>Current user details</returns>
        /// <response code="200">User information retrieved</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserInfo>), StatusCodes.Status401Unauthorized)]
        public ActionResult<ApiResponse<UserInfo>> GetCurrentUser()
        {
            var user = new UserInfo
            {
                Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "",
                Name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "",
                Roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            var response = ApiResponse<UserInfo>.SuccessResponse(user);
            return Ok(response);
        }

        /// <summary>
        /// Demo endpoint - Available demo users
        /// </summary>
        /// <returns>List of demo users for testing</returns>
        [HttpGet("demo-users")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public ActionResult<ApiResponse<object>> GetDemoUsers()
        {
            var demoUsers = new
            {
                users = new[]
                {
                new { email = "admin@realestate.com", password = "admin123", roles = new[] { "Admin", "Agent" } },
                new { email = "agent@realestate.com", password = "agent123", roles = new[] { "Agent" } },
                new { email = "demo@realestate.com", password = "demo123", roles = new[] { "User" } }
            },
                note = "These are demo credentials for testing purposes only"
            };

            var response = ApiResponse<object>.SuccessResponse(demoUsers, "Demo users for testing");
            return Ok(response);
        }
    }
}
