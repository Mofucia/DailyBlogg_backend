using dailyblogg_backend.Models;
using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using System.Linq;
using dailyblogg_backend.Repositories;

namespace dailyblogg_backend.Services.HashtagServices
{
    public class HashtagService : IHashtagService
    {
        private readonly IHashtagRepository<Hashtag> _hashtagRepo;
        public HashtagService(IHashtagRepository<Hashtag> hashtagRepo)
        {
            _hashtagRepo = hashtagRepo;
        }

        public async Task<ApiResponse<List<HashtagResponseDTO>>> GetAllHashtag()
        {
            var hashtags = await _hashtagRepo.GetAllAsync();

            if (hashtags == null)
                return ApiResponse<List<HashtagResponseDTO>>.FailureResult("There are no hashtags yet");

            var resp = hashtags.Select(h => new HashtagResponseDTO
            {
                HashtagId = h.HashtagId,
                HashtagName = h.HashtagName
            }).ToList();

            return ApiResponse<List<HashtagResponseDTO>>.SuccessResult(resp);
        }

        public async Task<ApiResponse<List<string>>> GetTrendingHashtags()
        {
            var hashtags = await _hashtagRepo.GetTrendingAsync();

            if (hashtags == null)
                return ApiResponse< List<string>>.FailureResult("There are no trending hashtags yet");

            return ApiResponse< List<string>>.SuccessResult(hashtags);
        }
    }
}
