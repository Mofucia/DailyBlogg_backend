using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Services.AuthServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Claims;

namespace dailyblogg_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            //POST / api / Auth / register  ->  RegisterUser.ExecuteAsync()
            //1.Creates ApplicationUser via UserManager
            //2.Assigns "User" role
            //3.Generates JWT token with role claims
            //4.Returns { token, user }
            var result = await _authService.RegisterUser(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginUser(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }


        [Authorize] // <-- this check the JWT token format, signature, expiry date for you so no need extra logic
        [HttpGet("validate")]
        public async Task<IActionResult> Validate()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "Invalid token" });

            var result = await _authService.ValidateToken(userId);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result.Data);
        }

        //No need for a Logout function since the Frontend Just delete everything
        //including the token and user data(client)
    }
}
