namespace dailyblogg_backend.Models.DTOs
{
    public class CreatePostDTO
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile? ImageUrl { get; set; }
    }

    public class UpdatePostDTO
    {
        public string? Title { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }

    public class CreateCommentDTO
    {
        public string Text { get; set; } = string.Empty;
    }

    public class CommentResponseDTO
    {
        public int CommentId { get; set; }
        public string Text { get; set; } = string.Empty;
        //public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    //public class LikeResponseDTO
    //{
    //    public int LikeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public string UserId { get; set; } = string.Empty;
    //    public string UserName { get; set; } = string.Empty;
    //    public int? LikeCount { get; set; }
    //}
    public class HashtagResponseDTO
    {
        public int HashtagId { get; set; }
        public string HashtagName { get; set; } = string.Empty;
    }
    public class PostResponseDTO
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        //Like Counter
        public int LikeCount { get; set; }
        public bool HasLikedByCurrentUser { get; set; }

        // Associated hashtags
        public List<HashtagResponseDTO> Hashtags { get; set; } = new();

        //for comments
        public List<CommentResponseDTO> Comments { get; set; } = new();
    }
}
