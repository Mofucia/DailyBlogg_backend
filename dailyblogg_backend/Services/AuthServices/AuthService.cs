using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Models;
using Microsoft.AspNetCore.Identity;

namespace dailyblogg_backend.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository<ApplicationUser> _authRepo;
        private readonly IUserRepository<ApplicationUser> _userRepo;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(IAuthRepository<ApplicationUser> authRepo,
                           IUserRepository<ApplicationUser> userRepo,
                           IJwtTokenGenerator jwtTokenGenerator)
        {
            _authRepo = authRepo;
            _userRepo = userRepo;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<ApiResponse<UserResponseDTO?>> ValidateToken(string userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null)
                return ApiResponse<UserResponseDTO?>.FailureResult("User no longer exists");

            var roles = await _userRepo.GetRolesAsync(user);

            var dto = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Bio = user.Bio,
                Roles = roles
            };

            return ApiResponse<UserResponseDTO?>.SuccessResult(dto);
        }
        public async Task<ApiResponse<AuthResponseDTO?>> LoginUser(LoginDTO dto)
        {
            var user = await _userRepo.FindByEmailAsync(dto.Email);

            if (user == null)
                return ApiResponse<AuthResponseDTO?>.FailureResult("Invalid email or password");

            var isPasswordValid = await _authRepo.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
                return ApiResponse<AuthResponseDTO?>.FailureResult("Invalid email or password");

            var roles = await _userRepo.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            var resp = new AuthResponseDTO
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
            };

            return ApiResponse<AuthResponseDTO?>.SuccessResult(resp);
        }
        public async Task<ApiResponse<AuthResponseDTO?>> RegisterUser(RegisterDTO dto)
        {
            var existingUser = await _userRepo.FindByNameAsync(dto.UserName);

            if (existingUser != null)
                return ApiResponse<AuthResponseDTO?>.FailureResult("Username is already taken");

            var user = new ApplicationUser
            {
                Name = dto.Name,
                UserName = dto.UserName,
                Email = dto.Email
            };

            try
            {
                await _authRepo.CreateAsync(user, dto.Password);
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDTO?>.FailureResult("Failed to create user: " + ex.Message);
            }

            await _authRepo.AddToRoleAsync(user, "User");

            var roles = await _userRepo.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            var resp = new AuthResponseDTO
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
            };

            return ApiResponse<AuthResponseDTO?>.SuccessResult(resp);
        }
    }
}


