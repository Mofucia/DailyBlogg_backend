using System.ComponentModel.DataAnnotations.Schema;

namespace dailyblogg_backend.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Ex. "FriendRequest", "PostLike"
        public int? RelatedId { get; set; } // The ID of the Post or User involv ed
        public bool IsRead { get; set; } = false; // Use to gray out the UI
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //User who receive the notification
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = default!;
    }
}
