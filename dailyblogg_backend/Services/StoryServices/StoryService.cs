using dailyblogg_backend.Models;
using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;

namespace dailyblogg_backend.Services.StoryServices
{
    public class StoryService : IStoryService
    {
        private readonly IStoryRepository<Story> _storyRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserRepository<ApplicationUser> _userRepo;
        public StoryService(IStoryRepository<Story> storyRepo, 
                            IWebHostEnvironment environment,
                            IUserRepository<ApplicationUser> userRepo)
        {
            _storyRepo = storyRepo;
            _environment = environment;
            _userRepo = userRepo;
        }
        public async Task<ApiResponse<StoryResponseDTO>> GetStoryById(int storyId)
        {
            var story = await _storyRepo.GetStoryById(storyId);
            
            if (story == null)
                return ApiResponse<StoryResponseDTO>.FailureResult("Can't find story");
            return ApiResponse < StoryResponseDTO > .SuccessResult(new StoryResponseDTO
            {
                Id = story.Id,
                Content = story.Content,
                StoryUrl = story.StoryUrl,
                CreatedDate = story.CreatedDate,
                UserId = story.UserId,
                Name = story.User.Name ?? "Unknown"
            });
        }
        public async Task<ApiResponse<List<StoryResponseDTO>>> GetAllActiveStory()
        {
            var storyExpireDate = DateTime.Now.AddHours(-24);
            var stories = await _storyRepo.AllActiveStory(storyExpireDate);
            if (stories == null)
                return ApiResponse<List<StoryResponseDTO>>.FailureResult("Can't find any active story");
            
            var data = stories.Select(s => new StoryResponseDTO
            {
                Id = s.Id,
                Content = s.Content,
                StoryUrl = s.StoryUrl,
                CreatedDate = s.CreatedDate,
                UserId = s.UserId,
                Name = s.User.Name ?? "Unknown"
            }).ToList();
            return ApiResponse<List<StoryResponseDTO>>.SuccessResult(data);
        }
        public async Task<ApiResponse<bool>> DeleteStory(int storyId)
        {
            var story = await _storyRepo.GetStoryById(storyId);
            if (story == null) return ApiResponse<bool>.FailureResult("Can't find the story");
            try
            {
                await _storyRepo.Remove(story);
                await _storyRepo.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true);
            }
            catch
            {
                return ApiResponse<bool>.FailureResult("Failed to delete the story");
            }
        }
        public async Task<ApiResponse<StoryResponseDTO?>> UpdateStory(string userId , UpdateStoryDTO dto)
        {
            var story = await _storyRepo.GetStoryById(dto.Id);
            if (story == null) return ApiResponse<StoryResponseDTO?>.FailureResult("Can't find the story");

            string imageUrl = string.Empty;
            if (dto.StoryUrl != null && dto.StoryUrl.Length > 0)
            {
                imageUrl = await SaveStoryImageAsync(userId, dto.StoryUrl);
                story.StoryUrl = imageUrl;
            }

            if(!string.IsNullOrWhiteSpace(dto.Content)) story.Content = dto.Content;

            await _storyRepo.SaveChangesAsync();

            return ApiResponse<StoryResponseDTO?>.SuccessResult(new StoryResponseDTO
            {
                Id = story.Id,
                Content = story.Content,
                StoryUrl = story.StoryUrl,
                CreatedDate = story.CreatedDate,
                UserId = story.UserId,
                Name = story.User.Name ?? "Unknown"
            });
        }
        public async Task<ApiResponse<StoryResponseDTO?>> CreateStory(string userId,CreateStoryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content)) return ApiResponse<StoryResponseDTO?>.FailureResult("The Content is empty");
            string imageUrl = string.Empty;
            if (dto.StoryUrl != null && dto.StoryUrl.Length > 0)
            {
                imageUrl = await SaveStoryImageAsync(userId, dto.StoryUrl);
            }
            var user = await _userRepo.GetUserByIdAsync(userId);
            var newStory = new Story
            {
                Content = dto.Content,
                StoryUrl= imageUrl,
                CreatedDate = DateTime.Now,
                UserId = userId,
                Name = user?.Name ?? "Unknown"
            };
            await _storyRepo.AddAsync(newStory);
            await _storyRepo.SaveChangesAsync();

            return ApiResponse<StoryResponseDTO?>.SuccessResult(new StoryResponseDTO{
                Id = newStory.Id,
                Content = newStory.Content,
                StoryUrl = newStory.StoryUrl,
                CreatedDate = newStory.CreatedDate,
                UserId = userId
            });
        }
        private async Task<string> SaveStoryImageAsync(string userId, IFormFile file)
        {
            var rootPath = _environment.WebRootPath;
            var storyFolder = Path.Combine(rootPath, "Contents", userId, "Stories");

            if (!Directory.Exists(storyFolder))
            {
                Directory.CreateDirectory(storyFolder);
            }

            // Generate a clean filename
            var fileName = $"story_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(storyFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/Contents/{userId}/Stories/{fileName}";
        }
    }
}
