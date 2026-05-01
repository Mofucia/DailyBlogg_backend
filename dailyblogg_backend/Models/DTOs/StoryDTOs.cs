using dailyblogg_backend.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace dailyblogg_backend.Models.DTOs
{
    public class CreateStoryDTO
    {
        public string Content { get; set; } = string.Empty;
        public IFormFile? StoryUrl { get; set; }
    }
    public class UpdateStoryDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public IFormFile? StoryUrl { get; set; }

    }
    public class StoryResponseDTO
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string StoryUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = default!;
        public string Name {  get; set; } = string.Empty;
    }
}
