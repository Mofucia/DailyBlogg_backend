using System.ComponentModel.DataAnnotations.Schema;

namespace dailyblogg_backend.Models.Entities
{
    public class Friendship
    {
        public string RequestorId { get; set; } = string.Empty;

        [ForeignKey("RequestorId")]
        public ApplicationUser Requestor { get; set; } = null!;

        public string ReceiverId { get; set; } = string.Empty;

        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; } = null!;

        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Blocked
    }
}
