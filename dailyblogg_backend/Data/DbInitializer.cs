using Bogus;
using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

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

            // (Child) Friendship
            var friendshipFaker = new Faker<Friendship>()
                .RuleFor(f => f.RequestorId, f => f.PickRandom(Mockusers).Id)
                .RuleFor(f => f.ReceiverId, (f, friendship) =>
                    f.PickRandom(Mockusers.Where(u => u.Id != friendship.RequestorId)).Id) //This is so that the user doens't pick themselves as a friend
                .RuleFor(f => f.Status, f => f.PickRandom(FriendshipStatus.Accepted, FriendshipStatus.Pending));

            var friendships = friendshipFaker.Generate(10);
            await context.Friendships.AddRangeAsync(friendships);

            // (Child) Notification
            var notificationFaker = new Faker<Notification>()
                .RuleFor(n => n.UserId, f => f.PickRandom(Mockusers).Id)
                .RuleFor(n => n.Message, f => f.Lorem.Sentence())
                .RuleFor(n => n.IsRead, f => f.Random.Bool())
                .RuleFor(n => n.CreatedAt, f => f.Date.Recent(7));

            var notifications = notificationFaker.Generate(30);
            await context.Notifications.AddRangeAsync(notifications);

            await context.SaveChangesAsync();
        }
        var users = await context.Users.ToListAsync();

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

            var posts = postFaker.Generate(30); // Create 30 posts
            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();

            var commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Text, f => f.Lorem.Sentence())
                .RuleFor(c => c.CreatedDate, f => f.Date.Recent(7))
                .RuleFor(c => c.PostId, f => f.PickRandom(posts).Id) // Random Post
                .RuleFor(c => c.UserId, f => f.PickRandom(users).Id); // Random Author

            var comments = commentFaker.Generate(100);
            await context.Comments.AddRangeAsync(comments);

            // (Child) Like
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
    }
}