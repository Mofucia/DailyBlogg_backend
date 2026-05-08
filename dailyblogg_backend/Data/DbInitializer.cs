using Bogus;
using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public static class DbInitializer
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        //User
        if (!await context.Users.AnyAsync())
        {
            var userFaker = new Faker<ApplicationUser>()
                //Core Identity Fields
                .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.Name))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(u => u.PasswordHash, f => "AQAAAAEAACcQAAAA")

                //Custom Fields
                .RuleFor(u => u.ImageUrl, f => f.Internet.Avatar())
                .RuleFor(u => u.Bio, f => f.Lorem.Sentence())

                //Navigation
                .RuleFor(u => u.SentFriendRequests, f => new List<Friendship>())
                .RuleFor(u => u.ReceivedFriendRequests, f => new List<Friendship>())
                .RuleFor(u => u.Notifications, f => new List<Notification>());

            var Mockusers = userFaker.Generate(15);
            await context.Users.AddRangeAsync(Mockusers);
            await context.SaveChangesAsync();
        }
        
        var users = await context.Users.ToListAsync();

        if (!await context.Friendships.AnyAsync() && users.Any())
        {
            var friendshipFaker = new Faker<Friendship>()
                .RuleFor(f => f.RequestorId, f => f.PickRandom(users).Id)
                .RuleFor(f => f.ReceiverId, (f, friendship) =>
                    f.PickRandom(users.Where(u => u.Id != friendship.RequestorId)).Id) //This is so that the user doens't pick themselves as a friend
                .RuleFor(f => f.Status, f => f.PickRandom(FriendshipStatus.Accepted, FriendshipStatus.Pending));
            var friendships = friendshipFaker.Generate(10);
            await context.Friendships.AddRangeAsync(friendships);
        }

        if (!await context.Notifications.AnyAsync() && users.Any())
        {
            var notificationFaker = new Faker<Notification>()
                .RuleFor(n => n.UserId, f => f.PickRandom(users).Id)
                .RuleFor(n => n.Message, f => f.Lorem.Sentence())
                .RuleFor(n => n.IsRead, f => f.Random.Bool())
                .RuleFor(n => n.CreatedAt, f => f.Date.Recent(7));

            var notifications = notificationFaker.Generate(30);
            await context.Notifications.AddRangeAsync(notifications);
            await context.SaveChangesAsync();
        }

        //Post
        if (!await context.Posts.AnyAsync() && users.Any())
        {
            var postFaker = new Faker<Post>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(5))
            .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl()) // Random images from Picsum
            .RuleFor(p => p.CreatedDate, f => f.Date.Past(1)) // Any time in the last year

            .RuleFor(p => p.UserId, f => f.PickRandom(users).Id) // Random User

            //Navigation
            .RuleFor(p => p.Comments, f => new HashSet<Comment>())
            .RuleFor(p => p.Likes, f => new HashSet<Like>())
            .RuleFor(p => p.Hashtags, f => new HashSet<Hashtag>());

            var Mockposts = postFaker.Generate(30); // Create 30 posts
            await context.Posts.AddRangeAsync(Mockposts);
            await context.SaveChangesAsync();
        }

        var posts = await context.Posts.ToListAsync();

        if(!await context.Comments.AnyAsync() && users.Any())
        {
            var commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Text, f => f.Lorem.Sentence())
                .RuleFor(c => c.CreatedDate, f => f.Date.Recent(7))
                .RuleFor(c => c.PostId, f => f.PickRandom(posts).Id) // Random Post
                .RuleFor(c => c.UserId, f => f.PickRandom(users).Id); // Random Author

            var comments = commentFaker.Generate(100);
            await context.Comments.AddRangeAsync(comments);
        }
        if (!await context.Likes.AnyAsync() && users.Any())
        {
            // Using Loops because we don't want a user to like a post twice or more
            var likes = new List<Like>();
            for (int i = 0; i < 50; i++)
            {
                var randomPost = posts[new Random().Next(posts.Count)];
                var randomUser = users[new Random().Next(users.Count)];

                // check if any user haven't liked any post yet
                if (!likes.Any(l => l.PostId == randomPost.Id && l.UserId == randomUser.Id))
                {
                    likes.Add(new Like { PostId = randomPost.Id, UserId = randomUser.Id });
                }
            }
            await context.Likes.AddRangeAsync(likes);
            await context.SaveChangesAsync();
        }

            //Story
            if (!await context.Stories.AnyAsync() && users.Any())
        {
            var storyFaker = new Faker<Story>()
            .RuleFor(s => s.Name, f => f.Person.FirstName) // Random Display name
            .RuleFor(s => s.Content, f => f.Lorem.Sentence())
            .RuleFor(s => s.StoryUrl, f => f.Image.PicsumUrl(1080, 1920))
            .RuleFor(s => s.CreatedDate, f => f.Date.Recent(1)) // 1 day expire date
            .RuleFor(s => s.UserId, f => f.PickRandom(users).Id);

            var stories = storyFaker.Generate(15);
            await context.Stories.AddRangeAsync(stories);
            await context.SaveChangesAsync();
        }

        //hashtag
        if (!await context.Hashtags.AnyAsync())
        {
            var hashtagNames = new[] { "dotnet", "csharp", "react", "programming", "dailyblogg", "backend", "fullstack" };

            var hashtags = hashtagNames.Select(name => new Hashtag
            {
                HashtagName = name
            }).ToList();

            await context.Hashtags.AddRangeAsync(hashtags);
            await context.SaveChangesAsync();

            // Liên kết Hashtag vào Posts (Mối quan hệ Many-to-Many)
            var allPosts = await context.Posts.Include(p => p.Hashtags).ToListAsync();
            var allHashtags = await context.Hashtags.ToListAsync();

            if (allPosts.Any() && allHashtags.Any())
            {
                var random = new Random();
                foreach (var post in allPosts)
                {
                    // Chọn ngẫu nhiên 1-3 hashtag từ danh sách
                    var tagsToAdd = allHashtags
                        .OrderBy(x => Guid.NewGuid())
                        .Take(random.Next(1, 4))
                        .ToList();

                    foreach (var tag in tagsToAdd)
                    {
                        // Thêm trực tiếp vào Collection của Post
                        post.Hashtags.Add(tag);
                    }
                }

                // Lưu lại sự thay đổi của các thực thể Post
                await context.SaveChangesAsync();
            }
        }
    }
}