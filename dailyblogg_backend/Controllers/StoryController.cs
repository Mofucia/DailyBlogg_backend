using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Services.StoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dailyblogg_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        public readonly IStoryService _storyService;
        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }
        [HttpGet("{storyId}")]
        public async Task<IActionResult> GetStoryByUserId(int storyId)
        {
            var story = await _storyService.GetStoryById(storyId);
            if (story == null) return NotFound();
            return Ok(story);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllActiveStory()
        {
            var stories = await _storyService.GetAllActiveStory();
            if (stories == null) return NotFound();
            return Ok(stories);
        }
        [Authorize]
        [HttpDelete("delete/{storyId}")]
        public async Task<IActionResult> DeleteStory(int storyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Unauthorized();
            var result = await _storyService.DeleteStory(storyId);
            if (!result.Success)
                return NotFound(new { message = result.Error });
            return Ok(new { message = "Story deleted successfully" });

        }
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateStory([FromForm]UpdateStoryDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var result = await _storyService.UpdateStory(userId, dto);
            if(result == null) return NotFound();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateStory([FromForm]CreateStoryDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var result = await _storyService.CreateStory(userId,dto);
            if (result == null) return BadRequest("Invalid file type");
            return Ok(result);
        }
        //I haven't migrate Story yet
    }
}
