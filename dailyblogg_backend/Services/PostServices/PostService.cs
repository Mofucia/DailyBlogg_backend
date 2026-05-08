using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace dailyblogg_backend.Services.PostServices
{
    public class PostService : IPostService
    {
        private readonly IPostRepository<Post> _postRepo;
        private readonly ILikeRepository<Like> _likeRepo;
        private readonly ICommentRepository<Comment> _commentRepo;
        private readonly IUserRepository<ApplicationUser> _userRepo;
        private readonly IHashtagRepository<Hashtag> _hashtagRepo;
        private readonly IFriendshipRepository<Friendship> _friendshipRepo;
        private readonly INotificationRepository<Notification> _notificationRepo;
        private readonly IWebHostEnvironment _environment;

        public PostService(IPostRepository<Post> postRepo,
                           ILikeRepository<Like> likeRepo,
                           ICommentRepository<Comment> commentRepo,
                           IUserRepository<ApplicationUser> userRepo,
                           IHashtagRepository<Hashtag> hashtagRepo,
                           IFriendshipRepository<Friendship> friendshipRepo,
                           INotificationRepository<Notification> notificationRepo,
                           IWebHostEnvironment environment)
        {
            _postRepo = postRepo;
            _likeRepo = likeRepo;
            _commentRepo = commentRepo;
            _userRepo = userRepo;
            _hashtagRepo = hashtagRepo;
            _friendshipRepo = friendshipRepo;
            _notificationRepo = notificationRepo;
            _environment = environment;
        }

        private static List<string> ExtractHashtags(string? text)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(text))
                return result;

            var pattern = "#([a-zA-Z0-9_]+)";
            var matches = System.Text.RegularExpressions.Regex.Matches(text!, pattern);
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                if (m.Groups.Count > 1)
                {
                    var tag = m.Groups[1].Value.Trim().ToLower();
                    if (!string.IsNullOrWhiteSpace(tag) && !result.Contains(tag))
                        result.Add(tag);
                }
            }

            return result;
        }

        public async Task<ApiResponse<List<PostResponseDTO>>> GetPostsByHashtag(string userId, string hashtagName)
        {
            if (string.IsNullOrWhiteSpace(hashtagName))
                return ApiResponse<List<PostResponseDTO>>.FailureResult("Hashtag name is required");

            // search posts with matching hashtag name (case-insensitive)
            var posts = await _postRepo.GetPostsByHashtagAsync(hashtagName);
            var filtered = posts.ToList();

            if (!filtered.Any())
                return ApiResponse<List<PostResponseDTO>>.FailureResult("There are no posts with that hashtag yet");

            var result = new List<PostResponseDTO>();
            foreach (var p in filtered)
            {
                var likeCount = await _likeRepo.LikeCountForPost(p.Id);
                var hasLiked = await _likeRepo.HasLikedByCurrentUser(p.Id, userId);

                result.Add(new PostResponseDTO
                {
                    PostId = p.Id,
                    Title = p.Title,
                    ImageUrl = p.ImageUrl,
                    CreatedDate = p.CreatedDate,
                    UserId = p.UserId,
                    Name = p.User?.Name ?? string.Empty,
                    Comments = p.Comments.Select(c => new CommentResponseDTO
                    {
                        CommentId = c.Id,
                        Text = c.Text,
                        UserId = c.UserId,
                        Name = c.User?.Name ?? string.Empty
                    }).ToList(),
                    LikeCount = likeCount,
                    HasLikedByCurrentUser = hasLiked,
                    Hashtags = p.Hashtags?.Select(h => new HashtagResponseDTO
                    {
                        HashtagId = h.HashtagId,
                        HashtagName = h.HashtagName
                    }).ToList() ?? new List<HashtagResponseDTO>()
                });
            }

            return ApiResponse<List<PostResponseDTO>>.SuccessResult(result);
        }

        public async Task<ApiResponse<PostResponseDTO?>> GetPostById(string userId, int postId)
        {
            var post = await _postRepo.GetPostByIdAsync(postId);
            if (post == null)
                return ApiResponse<PostResponseDTO?>.FailureResult("Post not found");

            var dto = new PostResponseDTO
            {
                PostId = post.Id,
                Title = post.Title,
                ImageUrl = post.ImageUrl,
                CreatedDate = post.CreatedDate,
                UserId = post.UserId,
                Name = post.User?.Name ?? string.Empty,
                Comments = post.Comments.Select(c => new CommentResponseDTO
                {
                    CommentId = c.Id,
                    Text = c.Text,
                    UserId = c.UserId,
                    Name = c.User?.Name ?? string.Empty
                }).ToList(),
                LikeCount = await _likeRepo.LikeCountForPost(postId),
                HasLikedByCurrentUser = await _likeRepo.HasLikedByCurrentUser(postId, userId),
                Hashtags = post.Hashtags?.Select(h => new HashtagResponseDTO
                {
                    HashtagId = h.HashtagId,
                    HashtagName = h.HashtagName
                }).ToList() ?? new List<HashtagResponseDTO>()
            };

            return ApiResponse<PostResponseDTO?>.SuccessResult(dto);
        }

        public async Task<ApiResponse<List<PostResponseDTO>>> GetAllPostsByName(string userId, string name)
        {
            var posts = await _postRepo.GetAllPostsByTitleAsync(name);
            var result = new List<PostResponseDTO>();

            foreach (var p in posts)
            {
                var likeCount = await _likeRepo.LikeCountForPost(p.Id);
                var hasLiked = await _likeRepo.HasLikedByCurrentUser(p.Id, userId);

                result.Add(new PostResponseDTO
                {
                    PostId = p.Id,
                    Title = p.Title,
                    ImageUrl = p.ImageUrl,
                    CreatedDate = p.CreatedDate,
                    UserId = p.UserId,
                    Name = p.User?.Name ?? string.Empty,
                    Comments = p.Comments.Select(c => new CommentResponseDTO
                    {
                        CommentId = c.Id,
                        Text = c.Text,
                        UserId = c.UserId,
                        Name = c.User?.Name ?? string.Empty
                    }).ToList(),
                    LikeCount = likeCount,
                    HasLikedByCurrentUser = hasLiked,
                    Hashtags = p.Hashtags?.Select(h => new HashtagResponseDTO
                    {
                        HashtagId = h.HashtagId,
                        HashtagName = h.HashtagName
                    }).ToList() ?? new List<HashtagResponseDTO>()
                });
            }

            return ApiResponse<List<PostResponseDTO>>.SuccessResult(result);
        }

        public async Task<ApiResponse<List<PostResponseDTO>>> GetAllPost(string userId)
        {
            var posts = await _postRepo.GetAllPostsAsync();
            var result = new List<PostResponseDTO>();

            foreach (var p in posts)
            {
                var likeCount = await _likeRepo.LikeCountForPost(p.Id);
                var hasLiked = await _likeRepo.HasLikedByCurrentUser(p.Id, userId);

                result.Add(new PostResponseDTO
                {
                    PostId = p.Id,
                    Title = p.Title,
                    ImageUrl = p.ImageUrl,
                    CreatedDate = p.CreatedDate,
                    UserId = p.UserId,
                    Name = p.User?.Name ?? string.Empty,
                    Comments = p.Comments.Select(c => new CommentResponseDTO
                    {
                        CommentId = c.Id,
                        Text = c.Text,
                        UserId = c.UserId,
                        Name = c.User?.Name ?? string.Empty
                    }).ToList(),
                    LikeCount = likeCount,
                    HasLikedByCurrentUser = hasLiked,
                    Hashtags = p.Hashtags?.Select(h => new HashtagResponseDTO
                    {
                        HashtagId = h.HashtagId,
                        HashtagName = h.HashtagName
                    }).ToList() ?? new List<HashtagResponseDTO>()
                });
            }

            return ApiResponse<List<PostResponseDTO>>.SuccessResult(result);
        }

        public async Task<ApiResponse<List<PostResponseDTO>>> GetAllPostsByUserId(string currentUserId, string userId)
        {
            var posts = await _postRepo.GetPostsByUserIdAsync(userId);

            if (posts == null)
                return ApiResponse<List<PostResponseDTO>>.FailureResult("can't find posts");

            var result = new List<PostResponseDTO>();

            foreach (var p in posts)
            {
                var likeCount = await _likeRepo.LikeCountForPost(p.Id);
                var hasLiked = await _likeRepo.HasLikedByCurrentUser(p.Id, currentUserId);

                result.Add(new PostResponseDTO
                {
                    PostId = p.Id,
                    Title = p.Title,
                    ImageUrl = p.ImageUrl,
                    CreatedDate = p.CreatedDate,
                    UserId = p.UserId,
                    Name = p.User?.Name ?? string.Empty,
                    Comments = p.Comments.Select(c => new CommentResponseDTO
                    {
                        CommentId = c.Id,
                        Text = c.Text,
                        UserId = c.UserId,
                        Name = c.User?.Name ?? string.Empty
                    }).ToList(),
                    LikeCount = likeCount,
                    HasLikedByCurrentUser = hasLiked,
                    Hashtags = p.Hashtags?.Select(h => new HashtagResponseDTO
                    {
                        HashtagId = h.HashtagId,
                        HashtagName = h.HashtagName
                    }).ToList() ?? new List<HashtagResponseDTO>()
                });
            }

            return ApiResponse<List<PostResponseDTO>>.SuccessResult(result);
        }

        public async Task<ApiResponse<PostResponseDTO?>> CreatePost(CreatePostDTO post, string userId)
        {
            if (string.IsNullOrWhiteSpace(post.Title) && (post.ImageUrl == null || post.ImageUrl.Length == 0))
                return ApiResponse<PostResponseDTO?>.FailureResult("Title or image required");

            string imageUrl = string.Empty;
            if (post.ImageUrl != null && post.ImageUrl.Length > 0)
            {
                imageUrl = await SavePostImageAsync(userId, post.ImageUrl);
            }

            var newPost = new Post
            {
                Title = post.Title,
                ImageUrl = imageUrl,
                CreatedDate = DateTime.UtcNow,
                UserId = userId
            };

            // extract hashtags from title
            var hashtags = ExtractHashtags(post.Title);

            foreach (var tag in hashtags)
            {
                var existingTag = await _hashtagRepo.GetByNameAsync(tag);
                if (existingTag == null)
                {
                    var ht = new Hashtag { HashtagName = tag };
                    await _hashtagRepo.AddAsync(ht);
                    newPost.Hashtags.Add(ht);
                }
                else
                {
                    newPost.Hashtags.Add(existingTag);
                }
            }

            await _postRepo.AddAsync(newPost);
            await _postRepo.SaveChangesAsync();

            var user = await _userRepo.GetUserByIdAsync(userId);

            // create notification for followers/friends (simple approach: notify all friends)
            try
            {
                var friends = await _friendshipRepo.GetAllFriendship(userId);
                foreach (var f in friends)
                {
                    var friendUser = f.RequestorId == userId ? f.Receiver : f.Requestor;
                    var notification = new Notification
                    {
                        UserId = friendUser.Id,
                        Message = $"{user?.Name ?? "Someone"} has created a new post",
                        Type = "PostCreated",
                        RelatedId = newPost.Id,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _notificationRepo.AddAsync(notification);
                }
                await _notificationRepo.SaveChangesAsync();
            }
            catch
            {
                // ignore notification creation error
            }

            var dto = new PostResponseDTO
            {
                PostId = newPost.Id,
                Title = newPost.Title,
                ImageUrl = newPost.ImageUrl,
                CreatedDate = newPost.CreatedDate,
                UserId = newPost.UserId,
                Name = user?.Name ?? string.Empty,
                LikeCount = 0,
                HasLikedByCurrentUser = false,
                Comments = new List<CommentResponseDTO>(),
                Hashtags = hashtags.Select(tagName => new HashtagResponseDTO
                {
                    HashtagName = tagName
                }).ToList()
            };

            return ApiResponse<PostResponseDTO?>.SuccessResult(dto);
        }

        private async Task<string> SavePostImageAsync(string userId, IFormFile file)
        {
            var rootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var postFolder = Path.Combine(rootPath, "Contents", userId, "Posts");

            if (!Directory.Exists(postFolder))
                Directory.CreateDirectory(postFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(postFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/Contents/{userId}/Posts/{fileName}";
        }

        public async Task<ApiResponse<PostResponseDTO?>> UpdatePost(string userId, int postId, UpdatePostDTO dto)
        {
            var post = await _postRepo.GetPostByIdAsync(postId);
            if (post == null)
                return ApiResponse<PostResponseDTO?>.FailureResult("Post not found");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                post.Title = dto.Title;

            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var imageUrl = await SavePostImageAsync(userId, dto.ImageUrl);
                post.ImageUrl = imageUrl;
            }

            // update hashtags if title change
            if (!string.IsNullOrWhiteSpace(dto.Title) && !dto.Title.Equals(post.Title))
            {
                var newTags = ExtractHashtags(dto.Title);
                // clear current tags
                post.Hashtags.Clear();
                foreach (var tag in newTags)
                {
                    var existingTag = await _hashtagRepo.GetByNameAsync(tag);
                    if (existingTag == null)
                    {
                        var ht = new Hashtag { HashtagName = tag };
                        await _hashtagRepo.AddAsync(ht);
                        post.Hashtags.Add(ht);
                    }
                    else
                    {
                        post.Hashtags.Add(existingTag);
                    }
                }
            }

            await _postRepo.SaveChangesAsync();

            var response = new PostResponseDTO
            {
                PostId = post.Id,
                Title = post.Title,
                ImageUrl = post.ImageUrl,
                CreatedDate = post.CreatedDate,
                UserId = post.UserId,
                Name = post?.User?.Name ?? string.Empty,
                Comments = post?.Comments?.Select(c => new CommentResponseDTO
                {
                    CommentId = c.Id,
                    Text = c.Text,
                    UserId = c.UserId,
                    Name = c.User?.Name ?? string.Empty
                }).ToList() ?? new List<CommentResponseDTO>(),
                LikeCount = await _likeRepo.LikeCountForPost(postId),
                HasLikedByCurrentUser = await _likeRepo.HasLikedByCurrentUser(post.Id, userId),
                Hashtags = post?.Hashtags?.Select(h => new HashtagResponseDTO
                {
                    HashtagId = h.HashtagId,
                    HashtagName = h.HashtagName
                }).ToList() ?? new List<HashtagResponseDTO>()
            };

            return ApiResponse<PostResponseDTO?>.SuccessResult(response);
        }

        public async Task<ApiResponse<bool>> DeletePost(int postId)
        {
            var post = await _postRepo.GetPostByIdAsync(postId);
            if (post == null)
                return ApiResponse<bool>.FailureResult("Post not found");

            try
            {
                await _postRepo.Remove(post);
                await _postRepo.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true);
            }
            catch
            {
                return ApiResponse<bool>.FailureResult("Failed to delete post");
            }
        }

        public async Task<ApiResponse<(bool IsLiked, int Counts)>> ToggleLike(string userId, int postId)
        {
            var postExists = await _postRepo.GetPostByIdAsync(postId);
            if (postExists == null)
                return ApiResponse<(bool, int)>.FailureResult("Post not found");

            var existingLike = await _likeRepo.PostLike(userId, postId);
            bool IsLiked;
            if (existingLike != null)
            {
                await _likeRepo.Remove(existingLike);
                IsLiked = false;
            }
            else
            {
                await _likeRepo.AddAsync(new Like { UserId = userId, PostId = postId });
                IsLiked = true;
            }
            await _likeRepo.SaveChangesAsync();

            // create notification when a user likes someone else post
            try
            {
                if (IsLiked)
                {
                    var post = await _postRepo.GetPostByIdAsync(postId);
                    if (post != null && post.UserId != userId)
                    {
                        var liker = await _userRepo.GetUserByIdAsync(userId);
                        var notif = new Notification
                        {
                            UserId = post.UserId,
                            Message = $"{liker?.Name ?? "Someone"} liked your post",
                            Type = "PostLike",
                            RelatedId = postId,
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _notificationRepo.AddAsync(notif);
                        await _notificationRepo.SaveChangesAsync();
                    }
                }
            }
            catch
            {
                // ignore notification errors
            }

            int Counts = await _likeRepo.LikeCountForPost(postId);

            return ApiResponse<(bool, int)>.SuccessResult((IsLiked, Counts));
        }

        public async Task<ApiResponse<CommentResponseDTO>> AddComment(string userId, int postId, CreateCommentDTO dto)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            var newComment = new Comment
            {
                Text = dto.Text,
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
            };

            await _commentRepo.AddAsync(newComment);
            await _commentRepo.SaveChangesAsync();

            var response = new CommentResponseDTO
            {
                CommentId = newComment.Id,
                Text = newComment.Text,
                UserId = userId,
                Name = user?.Name ?? string.Empty,
            };

            // create notification for post owner
            try
            {
                var postOwner = await _postRepo.GetPostByIdAsync(postId);
                if (postOwner != null && postOwner.UserId != userId)
                {
                    var notif = new Notification
                    {
                        UserId = postOwner.UserId,
                        Message = $"{user?.Name ?? "Someone"} commented on your post",
                        Type = "PostComment",
                        RelatedId = postId,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _notificationRepo.AddAsync(notif);
                    await _notificationRepo.SaveChangesAsync();
                }
            }
            catch
            {
                // ignore notification errors
            }

            return ApiResponse<CommentResponseDTO>.SuccessResult(response);
        }

        public async Task<ApiResponse<bool>> DeleteComment(string userId, int commentId)
        {
            var comment = await _commentRepo.GetCommentByUserId(userId, commentId);

            if (comment == null)
                return ApiResponse<bool>.FailureResult("Comment not found");

            try
            {
                await _commentRepo.Remove(comment);
                await _commentRepo.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true);
            }
            catch
            {
                return ApiResponse<bool>.FailureResult("Failed to delete comment");
            }
        }
    }
}
