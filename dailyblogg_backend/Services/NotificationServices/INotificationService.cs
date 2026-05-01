using dailyblogg_backend.Models;
using dailyblogg_backend.Models.DTOs;
using System.Runtime.CompilerServices;

namespace dailyblogg_backend.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<ApiResponse<List<NotificationResponseDTO>>> GetAllNotifications(string userId);
        Task<ApiResponse<bool>> DeleteNotification(int notificationId);
    }
}
