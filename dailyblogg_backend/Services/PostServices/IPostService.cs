using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models;
namespace dailyblogg_backend.Services.PostServices
{
    public interface IPostService
    {
        Task<ApiResponse<PostResponseDTO?>> GetPostById(string userId, int postId);
        Task<ApiResponse<List<PostResponseDTO>>> GetAllPostsByName(string userId, string name);
        Task<ApiResponse<List<PostResponseDTO>>> GetAllPost(string userId);
        Task<ApiResponse<List<PostResponseDTO>>> GetAllPostsByUserId(string userId);
        Task<ApiResponse<List<PostResponseDTO>>> GetPostsByHashtag(string userId, string hashtagName);
        Task<ApiResponse<PostResponseDTO?>> CreatePost(CreatePostDTO post, string userId);
        Task<ApiResponse<PostResponseDTO?>> UpdatePost(string userId, int postId, UpdatePostDTO dto);
        Task<ApiResponse<bool>> DeletePost(int postId);
        Task<ApiResponse<(bool IsLiked, int Counts)>> ToggleLike(string userId, int postId);
        Task<ApiResponse<CommentResponseDTO>> AddComment(string userId, int postId, CreateCommentDTO dto);
        Task<ApiResponse<bool>> DeleteComment(string userId, int commentId);

        //Them toggle like,add/delete comment

    }
}
