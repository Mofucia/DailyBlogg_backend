using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace dailyblogg_backend.Services.AuthServices.Commands
{
    public class RegisterUser : IRegisterUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterUser(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<(AuthResponseDTO? Response, string? Error)> ExecuteAsync(RegisterDTO dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);

            if (existingUser != null)
                return (null, "Username is already taken");

            var user = new ApplicationUser
            {
                Name = dto.Name,
                UserName = dto.UserName,
                Email = dto.Email
            };

            //No need for bcrypt because UserManager already have it's own way to Hash the Password
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return (null, string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");

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
