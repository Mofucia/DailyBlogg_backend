using System.ComponentModel.DataAnnotations.Schema;

namespace dailyblogg_backend.Models.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        // Foreign keys
        public int PostId { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Navigation
        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
    }
}
