using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models;
namespace dailyblogg_backend.Services.UserServices
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserResponseDTO>>> GetAllProfile();
        Task<ApiResponse<List<UserResponseDTO>>> GetAllUserByUsername(string username);
        Task<ApiResponse<UserResponseDTO?>> GetProfileByUserId(string userId);//switch to string
        Task<ApiResponse<bool>> DeleteUser(string userId);//switch to string
        Task<ApiResponse<UserResponseDTO?>> UpdateUserProfile(string userId, UpdateProfileDTO dto); //switch to string

    }
    
}
