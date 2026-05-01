using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models;
namespace dailyblogg_backend.Services.AuthServices
{
    public interface IAuthService
    {
        Task<ApiResponse<UserResponseDTO?>> ValidateToken(string userId);
        Task<ApiResponse<AuthResponseDTO?>> LoginUser(LoginDTO dto);
        Task<ApiResponse<AuthResponseDTO?>> RegisterUser(RegisterDTO dto);
    }
}
