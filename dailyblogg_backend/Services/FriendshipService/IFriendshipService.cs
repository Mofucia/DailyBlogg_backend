using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models;
namespace dailyblogg_backend.Services.FriendshipService
{
    public interface IFriendshipService
    {
        Task<ApiResponse<FriendshipResponseDTO>> SendRequest(string sender, string receiver);
        Task<ApiResponse<FriendshipResponseDTO>> AcceptRequest(string sender, string receiver);
        Task<ApiResponse<FriendshipResponseDTO>> DeclineRequest(string sender, string receiver);
        Task<ApiResponse<List<FriendResponseDTO>>> GetAllAcceptedFriend(string userId);
        Task<ApiResponse<List<FriendResponseDTO>>> GetAllPendingRequest(string userId);
    }
}
