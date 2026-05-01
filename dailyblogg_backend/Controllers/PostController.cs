using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Services.PostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace dailyblogg_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllPostsByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var posts = await _postService.GetAllPostsByUserId(userId);
            if (!posts.Success)
                return BadRequest(posts);
            return Ok(posts.Data);
        }
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPostById(string userId,int postId)
        {
            var post = await _postService.GetPostById(userId, postId);
            if (!post.Success || post.Data == null)
                return BadRequest(post);
            return Ok(post.Data);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO dto)
        {
            //user verification
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var post = await _postService.CreatePost(dto, userId);
            if (!post.Success)
                return BadRequest(post);
            return Ok(post.Data);
        }
        [Authorize]
        [HttpPut("{postId}/update")]
        public async Task<IActionResult> UpdatePost(int postId,[FromBody] UpdatePostDTO dto)
        {
            //user verification
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var post = await _postService.UpdatePost(userId, postId, dto);
            if (!post.Success)
                return BadRequest(post);
            return Ok(post.Data);
        }
        [Authorize]
        [HttpDelete("delete/{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();
            
            var result = await _postService.DeletePost(postId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(new { message = "Post deleted successfully" });
        }
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var posts = await _postService.GetAllPost(userId);
            if (!posts.Success)
                return BadRequest(posts);
            return Ok(posts.Data);
        }

        [Authorize]
        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> AddComment(int postId,[FromBody] CreateCommentDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var posts = await _postService.AddComment(userId, postId, dto);
            if (!posts.Success)
                return BadRequest(posts);
            return Ok(posts.Data);
        }
        [Authorize]
        [HttpDelete("comment-delete")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _postService.DeleteComment(userId,commentId);
            if (!result.Success)
                return NotFound(new { message = result.Error });
            return Ok(new { message = "Comment deleted successfully" });
        }

        [Authorize]
        [HttpPost("{postId}/like")]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _postService.ToggleLike(userId, postId);
            if (!result.Success)
                return BadRequest(result);
            var data = result.Data;
            return Ok(new
            {
                isLiked = data.IsLiked,
                count = data.Counts,
                message = data.IsLiked ? "Liked" : "Unliked"
            });
        }
        [Authorize]
        [HttpGet("name/{text}")]
        public async Task<IActionResult> GetAllPostsByName(string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var result = await _postService.GetAllPostsByName(userId, text);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }
    }
}
