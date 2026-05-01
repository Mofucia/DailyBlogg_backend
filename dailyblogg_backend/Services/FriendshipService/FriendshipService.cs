using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Models;
namespace dailyblogg_backend.Services.FriendshipService
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository<Friendship> _friendshipRepo;
        private readonly INotificationRepository<Notification> _notificationRepo;
        public FriendshipService(IFriendshipRepository<Friendship> friendshipRepo,
                                 INotificationRepository<Notification> notificationRepo)
        {
            _friendshipRepo = friendshipRepo;
            _notificationRepo = notificationRepo;
        }

        public async Task<ApiResponse<FriendshipResponseDTO>> AcceptRequest(string sender, string receiver)
        {
            var existing = await _friendshipRepo.FindRequest(sender, receiver);
            if (existing == null)
                return ApiResponse<FriendshipResponseDTO>.FailureResult("Friend request not found");

            existing.Status = FriendshipStatus.Accepted;
            await _friendshipRepo.SaveChangesAsync();
            // create notification to inform the requestor that their request was accepted
            try
            {
                var receiverUser = existing.Receiver; 
                var notif = new Notification
                {
                    UserId = existing.RequestorId,
                    Message = $"{receiverUser?.Name ?? "Someone"} accepted your friend request",
                    Type = "FriendAccepted",
                    RelatedId = null,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _notificationRepo.AddAsync(notif);
                await _notificationRepo.SaveChangesAsync();
            }
            catch
            {
                // ignore notification errors
            }

            return ApiResponse<FriendshipResponseDTO>.SuccessResult(new FriendshipResponseDTO { RequestorId = existing.RequestorId, ReceiverId = existing.ReceiverId, Status = existing.Status.ToString() });
        }

        public async Task<ApiResponse<FriendshipResponseDTO>> DeclineRequest(string sender, string receiver)
        {
            var existing = await _friendshipRepo.FindRequest(sender, receiver);
            if (existing == null)
                return ApiResponse<FriendshipResponseDTO>.FailureResult("Friend request not found");

            // remove the pending request
            await _friendshipRepo.Remove(existing);
            await _friendshipRepo.SaveChangesAsync();

            return ApiResponse<FriendshipResponseDTO>.SuccessResult(new FriendshipResponseDTO { RequestorId = existing.RequestorId, ReceiverId = existing.ReceiverId, Status = "Declined" });
        }

        public async Task<ApiResponse<List<FriendResponseDTO>>> GetAllAcceptedFriend(string userId)
        {
            var list = await _friendshipRepo.GetAllFriendship(userId);
            var result = new List<FriendResponseDTO>();
            foreach (var f in list)
            {
                var friendUser = f.RequestorId == userId ? f.Receiver : f.Requestor;
                result.Add(new FriendResponseDTO { Id = friendUser.Id, Name = friendUser.Name, ProfileImage = friendUser.ImageUrl ?? string.Empty });
            }
            return ApiResponse<List<FriendResponseDTO>>.SuccessResult(result);
        }

        public async Task<ApiResponse<List<FriendResponseDTO>>> GetAllPendingRequest(string userId)
        {
            var list = await _friendshipRepo.GetAllPendingRequest(userId);
            var result = new List<FriendResponseDTO>();
            foreach (var f in list)
            {
                var requester = f.Requestor;
                result.Add(new FriendResponseDTO { Id = requester.Id, Name = requester.Name, ProfileImage = requester.ImageUrl ?? string.Empty });
            }
            return ApiResponse<List<FriendResponseDTO>>.SuccessResult(result);
        }

        public async Task<ApiResponse<FriendshipResponseDTO>> SendRequest(string sender, string receiver)
        {
            // check existing request both directions
            var existing = await _friendshipRepo.FindRequest(sender, receiver);
            if (existing != null)
                return ApiResponse<FriendshipResponseDTO>.FailureResult("Friend request already exists");

            var reverse = await _friendshipRepo.FindRequest(receiver, sender);
            if (reverse != null)
                return ApiResponse<FriendshipResponseDTO>.FailureResult("Friend request already exists in reverse direction");

            var friendship = new Friendship
            {
                RequestorId = sender,
                ReceiverId = receiver,
                Status = FriendshipStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            await _friendshipRepo.AddAsync(friendship);
            await _friendshipRepo.SaveChangesAsync();

            var dto = new FriendshipResponseDTO
            {
                RequestorId = friendship.RequestorId,
                ReceiverId = friendship.ReceiverId,
                Status = friendship.Status.ToString()
            };

            // create a notification for the receiver
            try
            {
                // reload friendship to include navigation properties
                var full = await _friendshipRepo.FindRequest(sender, receiver);
                var senderUser = full?.Requestor;
                var notif = new Notification
                {
                    UserId = receiver,
                    Message = $"{senderUser?.Name ?? "Someone"} sent you a friend request",
                    Type = "FriendRequest",
                    RelatedId = null,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _notificationRepo.AddAsync(notif);
                await _notificationRepo.SaveChangesAsync();
            }
            catch
            {
                // ignore notification errors
            }

            return ApiResponse<FriendshipResponseDTO>.SuccessResult(dto);
        }

    }
}
