namespace dailyblogg_backend.Models.DTOs
{
    public class RegisterDTO
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Bio { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class UpdateProfileDTO
    {
        public string? Name { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? Bio { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UserResponseDTO User { get; set; } = null!;
    }

    public class FriendshipResponseDTO
    {
        public string RequestorId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
    public class FriendResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name {  get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
    }
}
