using dailyblogg_backend.Models;
using dailyblogg_backend.Models.DTOs;

namespace dailyblogg_backend.Services.HashtagServices
{
    public interface IHashtagService
    {
        Task<ApiResponse<List<HashtagResponseDTO>>> GetAllHashtag();
        Task<ApiResponse<List<string>>> GetTrendingHashtags();
    }
}
