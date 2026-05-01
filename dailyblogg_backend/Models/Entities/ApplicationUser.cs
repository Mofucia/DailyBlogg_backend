using Microsoft.AspNetCore.Identity;

namespace dailyblogg_backend.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Bio { get; set; }

        public virtual ICollection<Friendship> SentFriendRequests { get; set; } = new List<Friendship>();
        public virtual ICollection<Friendship> ReceivedFriendRequests { get; set; } = new List<Friendship>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
