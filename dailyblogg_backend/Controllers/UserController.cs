using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dailyblogg_backend.Controllers
{
    //localhost:xxxx/api
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("id/{userId}")]
        public async Task<IActionResult> GetProfileByUserId(string userId)
        {
            var result = await _userService.GetProfileByUserId(userId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var result = await _userService.GetAllProfile();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [HttpGet("search/{username}")]
        public async Task<IActionResult> GetAllUserByUsername(string username)
        {
            var result = await _userService.GetAllUserByUsername(username);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateProfileDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();
            var result = await _userService.UpdateUserProfile(userId, dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        //Needed some Changes: this endpoint make the admin delete a user not themselves
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUser(userId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
