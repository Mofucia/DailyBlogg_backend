using dailyblogg_backend.Models.DTOs;

namespace dailyblogg_backend.Services.AuthServices.Commands
{
    public interface IRegisterUser
    {
        Task<(AuthResponseDTO? Response, string? Error)> ExecuteAsync(RegisterDTO dto);
    }
}
