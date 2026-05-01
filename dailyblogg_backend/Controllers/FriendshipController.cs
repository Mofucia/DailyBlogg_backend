using dailyblogg_backend.Services.FriendshipService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using System.Security.Claims;

namespace dailyblogg_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        public FriendshipController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [Authorize]
        [HttpGet("pending")]
        public async Task<IActionResult> GetAllPending()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _friendshipService.GetAllPendingRequest(userId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }
        [Authorize]
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _friendshipService.GetAllAcceptedFriend(userId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost("send/{receiverId}")]
        public async Task<IActionResult> SendFriendRequest(string receiverId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (senderId == null)
                return Unauthorized();

            var result = await _friendshipService.SendRequest(senderId, receiverId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [Authorize]
        [HttpPut("accept/{senderId}")]
        public async Task<IActionResult> AcceptFriendRequest(string senderId)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUser == null)
                return Unauthorized();

            var result = await _friendshipService.AcceptRequest(senderId, currentUser);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }
        [Authorize]
        [HttpDelete("decline/{senderId}")]
        public async Task<IActionResult> DeclineFriendRequest(string senderId)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUser == null)
                return Unauthorized();

            var result = await _friendshipService.DeclineRequest(senderId, currentUser);
            
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }
    }
}
