using System.ComponentModel.DataAnnotations.Schema;

namespace dailyblogg_backend.Models.Entities
{
    public class Story
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string StoryUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //Foreign keys
        public string UserId { get; set; } = default!;

        //Navigation
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = default!;
    }
}
