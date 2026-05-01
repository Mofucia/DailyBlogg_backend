using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace dailyblogg_backend.Services.AuthServices.Commands
{
    public class LoginUser : ILoginUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUser(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<(AuthResponseDTO? Response, string? Error)> ExecuteAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return (null, "Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
                return (null, "Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return (new AuthResponseDTO
            {
                Token = token,
                User = new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    ImageUrl = user.ImageUrl,
                    Bio = user.Bio,
                    Roles = roles
                }
            }, null);
        }
    }
}
