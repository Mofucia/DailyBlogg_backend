using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Services.HashtagServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dailyblogg_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HashtagController : ControllerBase
    {
        private readonly IHashtagService _hashtagService;
        public HashtagController(IHashtagService hashtagService)
        {
            _hashtagService = hashtagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHashtag()
        {
            var result = await _hashtagService.GetAllHashtag();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingHashtags()
        {
            var result = await _hashtagService.GetTrendingHashtags();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
