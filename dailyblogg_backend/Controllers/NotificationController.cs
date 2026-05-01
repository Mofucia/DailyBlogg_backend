using dailyblogg_backend.Services.NotificationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dailyblogg_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _notificationService.GetAllNotifications(userId);

            if (!result.Success)
                return BadRequest(result);

            // return the notifications payload
            return Ok(result.Data);
        }
        [Authorize]
        [HttpDelete("delete/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var result = await _notificationService.DeleteNotification(notificationId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(new { message = "Notification deleted successfully" });
        }
    }
}
