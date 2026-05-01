using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Services.NotificationServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace daily_blogg_Test
{
    public class NotificationTest
    {
        private readonly Mock<INotificationRepository<Notification>> _notificationRepoMock;
        public NotificationTest()
        {
            _notificationRepoMock = new Mock<INotificationRepository<Notification>>();
        }

        private NotificationService CreateService() => new NotificationService(_notificationRepoMock.Object);
        [Fact]
        public async Task GetAllNotifications_ShouldReturnMappedList_WhenNotificationsExist()
        {
            // 1. ARRANGE
            var userId = "user-123";
            var fakeNotifications = new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    Message = "Someone liked your post",
                    Type = "Like",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Notification
                {
                    Id = 2,
                    Message = "New comment on your blog",
                    Type = "Comment",
                    IsRead = true,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                }
            };

            _notificationRepoMock.Setup(r => r.GetAllNotificationsByUserId(userId))
                                 .ReturnsAsync(fakeNotifications);

            var svc = CreateService();
            // 2. ACT
            var result = await svc.GetAllNotifications(userId);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count);

            Assert.Equal("Like", result.Data[0].Type);
            Assert.False(result.Data[0].IsRead);
            Assert.Equal("Comment", result.Data[1].Type);
            Assert.True(result.Data[1].IsRead);
        }

        [Fact]
        public async Task GetAllNotifications_ShouldReturnFailure_WhenUserIdIsEmpty()
        {
            // 1. ARRANGE
            string emptyUserId = "";
            var svc = CreateService();
            // 2. ACT
            var result = await svc.GetAllNotifications(emptyUserId);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("UserId is required", result.Error);

            _notificationRepoMock.Verify(r => r.GetAllNotificationsByUserId(It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async Task DeleteNotification_ShouldReturnSuccess_WhenNotificationExists()
        {
            // 1. ARRANGE
            int targetId = 99;
            var fakeNotification = new Notification { Id = targetId, Message = "Test Notification" };

            _notificationRepoMock.Setup(r => r.FindNotification(targetId))
                                 .ReturnsAsync(fakeNotification);
            var svc = CreateService();
            // 2. ACT
            var result = await svc.DeleteNotification(targetId);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.True(result.Data);

            // Verify remove and SaveChanges are called once
            _notificationRepoMock.Verify(r => r.Remove(fakeNotification), Times.Once);
            _notificationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteNotification_ShouldReturnFailure_WhenNotificationNotFound()
        {
            // 1. ARRANGE
            int missingId = 404;

            // Mock: Returns null (not found)
            _notificationRepoMock.Setup(r => r.FindNotification(missingId))
                                 .ReturnsAsync((Notification)null);

            var svc = CreateService();
            // 2. ACT
            var result = await svc.DeleteNotification(missingId);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Can't find the notification", result.Error);

            // Verify that remove is never called
            _notificationRepoMock.Verify(r => r.Remove(It.IsAny<Notification>()), Times.Never);
        }
    }
}
