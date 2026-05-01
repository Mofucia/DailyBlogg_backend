using dailyblogg_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace dailyblogg_backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //Nhập từng table ở đây
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; } 
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Phân Role
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            //Like many-to-many relationship
            builder.Entity<Like>()
                    .HasIndex(l => new { l.UserId, l.PostId })
                    .IsUnique();

            builder.Entity<Like>()
                    .HasOne(l => l.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.PostId);

            //Hashtag many-to-many relationship
            builder.Entity<Post>()
                .HasMany(p => p.Hashtags)
                .WithMany(h => h.PostHashtags)
                .UsingEntity(j => j.ToTable("PostHashtags"));

            //Self-referencing Friendship many-to-many relationship
            builder.Entity<Friendship>()
                .HasKey(f => new { f.RequestorId, f.ReceiverId });

            builder.Entity<Friendship>()
                .HasOne(f => f.Requestor)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(f => f.RequestorId)
                .OnDelete(DeleteBehavior.Restrict); //tránh rủi ro khi xoá user(nếu có)

            builder.Entity<Friendship>()
                .HasOne(f => f.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
