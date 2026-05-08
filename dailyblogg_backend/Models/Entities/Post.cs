using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace dailyblogg_backend.Models.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign key
        public string UserId { get; set; } = default!;

        // Navigation property
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = default!;

        // Related comments and likes
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<Like> Likes { get; set; } = new HashSet<Like>();

        public ICollection<Hashtag> Hashtags { get; set; } = new HashSet<Hashtag>();
    }
}
