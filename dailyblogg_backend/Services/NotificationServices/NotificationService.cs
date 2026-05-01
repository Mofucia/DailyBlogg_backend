using dailyblogg_backend.Models;
using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;

namespace dailyblogg_backend.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository<Notification> _notificationRepo;
        public NotificationService(INotificationRepository<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<ApiResponse<List<NotificationResponseDTO>>> GetAllNotifications(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ApiResponse<List<NotificationResponseDTO>>.FailureResult("UserId is required");

            var notifications = await _notificationRepo.GetAllNotificationsByUserId(userId);
            if (notifications == null)
                return ApiResponse<List<NotificationResponseDTO>>.FailureResult("Currently don't have any notification");

            var resp = notifications.Select(n => new NotificationResponseDTO
            {
                Id = n.Id,
                Message = n.Message,
                Type = n.Type ?? string.Empty,
                RelatedId = n.RelatedId?.ToString() ?? string.Empty,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();

            return ApiResponse<List<NotificationResponseDTO>>.SuccessResult(resp);
        }
        public async Task<ApiResponse<bool>> DeleteNotification(int notificationId)
        {
            var notification = await _notificationRepo.FindNotification(notificationId);
            if(notification == null)
                return ApiResponse<bool>.FailureResult("Can't find the notification");

            try 
            {
                await _notificationRepo.Remove(notification);
                await _notificationRepo.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true);
            }
            catch
            {
                return ApiResponse<bool>.FailureResult("Failed to Delete the notification");
            }
        }
    }
}
