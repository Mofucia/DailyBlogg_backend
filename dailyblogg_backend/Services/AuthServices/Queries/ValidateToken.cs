using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace dailyblogg_backend.Services.AuthServices.Queries
{
    public class ValidateToken : IValidateToken
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ValidateToken(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(UserResponseDTO? Response, string? Error)> ExecuteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return (null, "User no longer exists");

            var roles = await _userManager.GetRolesAsync(user);

            return (new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Bio = user.Bio,
                Roles = roles
            }, null);
        }
    }
}
