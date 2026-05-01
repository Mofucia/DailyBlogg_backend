using dailyblogg_backend.Models.DTOs;

namespace dailyblogg_backend.Services.AuthServices.Queries
{
    public interface IValidateToken
    {
        Task<(UserResponseDTO? Response, string? Error)> ExecuteAsync(string userId);
    }
}
