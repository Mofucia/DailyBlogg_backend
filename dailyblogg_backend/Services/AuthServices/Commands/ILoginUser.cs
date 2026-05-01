using dailyblogg_backend.Models.DTOs;

namespace dailyblogg_backend.Services.AuthServices.Commands
{
    public interface ILoginUser
    {
        Task<(AuthResponseDTO? Response, string? Error)> ExecuteAsync(LoginDTO dto);
    }
}
