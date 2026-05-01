using dailyblogg_backend.Models;
using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dailyblogg_backend.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository<ApplicationUser> _userRepo;
        public UserService(IUserRepository<ApplicationUser> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApiResponse<List<UserResponseDTO>>> GetAllProfile()
        {
            var users = await _userRepo.GetAllUsersAsync();
            if (users == null)
                return ApiResponse<List<UserResponseDTO>>.FailureResult("No User found.");

            var result = new List<UserResponseDTO>();

            foreach (var user in users)
            {
                var roles = await _userRepo.GetRolesAsync(user);

                result.Add(new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    ImageUrl = user.ImageUrl,
                    Bio = user.Bio,
                    Roles = roles
                });
            }

            return ApiResponse<List<UserResponseDTO>>.SuccessResult(result);
        }

        public async Task<ApiResponse<List<UserResponseDTO>>> GetAllUserByUsername(string username)
        {
            var users = await _userRepo.GetUsersByNameAsync(username);
            if (users == null)
                return ApiResponse<List<UserResponseDTO>>.FailureResult("No User found.");
            var result = new List<UserResponseDTO>();

            foreach (var user in users)
            {
                var roles = await _userRepo.GetRolesAsync(user);

                result.Add(new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    ImageUrl = user.ImageUrl,
                    Bio = user.Bio,
                    Roles = roles
                });
            }

            return ApiResponse<List<UserResponseDTO>>.SuccessResult(result);
        }
        public async Task<ApiResponse<UserResponseDTO?>> GetProfileByUserId(string userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null)
                return ApiResponse<UserResponseDTO?>.FailureResult("No User found.");

            var roles = await _userRepo.GetRolesAsync(user);

            return ApiResponse<UserResponseDTO?>.SuccessResult(new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Bio = user.Bio,
                Roles = roles
            });
        }
        public async Task<ApiResponse<bool>> DeleteUser(string userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null)
                return ApiResponse<bool>.FailureResult("Can't find the User");

            try
            {
                await _userRepo.Remove(user);
                await _userRepo.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true);
            }
            catch
            {
                // repository failed (could log the exception here)
                return ApiResponse<bool>.FailureResult("Failed to delete the user");
            }
        }

        public async Task<ApiResponse<UserResponseDTO?>> UpdateUserProfile(string userId, UpdateProfileDTO dto)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null)
                return ApiResponse<UserResponseDTO?>.FailureResult("Can't find the user");

            if (dto.ImageUrl != null)
            {
                // validate size (2 MB)
                if (dto.ImageUrl.Length > 2 * 1024 * 1024)
                    return ApiResponse<UserResponseDTO?>.FailureResult("File too large");

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowedTypes.Contains(dto.ImageUrl.ContentType))
                    return ApiResponse<UserResponseDTO?>.FailureResult("Wrong file format");

                // Root: wwwroot/Contents
                var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Contents");
                var userFolder = Path.Combine(rootFolder, userId);
                var avatarFolder = Path.Combine(userFolder, "Avatars");

                if (!Directory.Exists(avatarFolder))
                    Directory.CreateDirectory(avatarFolder);

                var fileExtension = Path.GetExtension(dto.ImageUrl.FileName);
                var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(avatarFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);
                }

                // Save public URL
                user.ImageUrl = $"/Contents/{userId}/Avatars/{fileName}";
            }

            if (dto.Name != null)
                user.Name = dto.Name;
            if (dto.Bio != null)
                user.Bio = dto.Bio;

            await _userRepo.SaveChangesAsync();

            var roles = await _userRepo.GetRolesAsync(user);

            return ApiResponse<UserResponseDTO?>.SuccessResult(new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Bio = user.Bio,
                Roles = roles
            });
        }
    }
}
