using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Services.AuthServices;
using dailyblogg_backend.Services.FriendshipService;
using dailyblogg_backend.Services.HashtagServices;
using dailyblogg_backend.Services.NotificationServices;
using dailyblogg_backend.Services.PostServices;
using dailyblogg_backend.Services.StoryServices;
using dailyblogg_backend.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Sql server connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//Identity Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

//Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories for DI
builder.Services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
builder.Services.AddScoped(typeof(IPostRepository<>), typeof(PostRepository<>));
builder.Services.AddScoped(typeof(ILikeRepository<>), typeof(LikeRepository<>));
builder.Services.AddScoped(typeof(ICommentRepository<>), typeof(CommentRepository<>));
builder.Services.AddScoped(typeof(IAuthRepository<>), typeof(AuthRepository<>));
builder.Services.AddScoped(typeof(IStoryRepository<>), typeof(StoryRepository<>));
builder.Services.AddScoped(typeof(IHashtagRepository<>), typeof(HashtagRepository<>));
builder.Services.AddScoped(typeof(IFriendshipRepository<>), typeof(FriendshipRepository<>));
builder.Services.AddScoped(typeof(INotificationRepository<>), typeof(NotificationRepository<>));
//Main logic storage
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IHashtagService, HashtagService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
// Auth and other small services that depend on repositories / identity
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Creating Mock data using Bogus
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    await DbInitializer.SeedData(context);
}


// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
