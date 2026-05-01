namespace dailyblogg_backend.Models.DTOs
{
    public class AddNotificationDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string RelatedId { get; set; } = string.Empty;
    }
    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string RelatedId { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
