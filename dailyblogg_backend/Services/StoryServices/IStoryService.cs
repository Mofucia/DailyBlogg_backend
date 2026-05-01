using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models;
namespace dailyblogg_backend.Services.StoryServices
{
    public interface IStoryService
    {
        Task<ApiResponse<StoryResponseDTO>> GetStoryById(int storyId);
        Task<ApiResponse<List<StoryResponseDTO>>> GetAllActiveStory();
        Task<ApiResponse<bool>> DeleteStory(int storyId);
        Task<ApiResponse<StoryResponseDTO?>> UpdateStory(string userId, UpdateStoryDTO dto);
        Task<ApiResponse<StoryResponseDTO?>> CreateStory(string userId, CreateStoryDTO dto);
    }
}
