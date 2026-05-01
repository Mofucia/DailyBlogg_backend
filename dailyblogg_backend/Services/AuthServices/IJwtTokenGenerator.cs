using dailyblogg_backend.Models.Entities;

namespace dailyblogg_backend.Services.AuthServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
