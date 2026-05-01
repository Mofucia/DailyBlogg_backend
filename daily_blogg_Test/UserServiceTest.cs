using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace daily_blogg_Test
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository<ApplicationUser>> _userRepositoryMock;
        public UserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository<ApplicationUser>>();
        }
        private UserService CreateService() => new UserService(_userRepositoryMock.Object);

        [Fact]
        public async Task GetAllProfile_ShouldReturnSuccess_WhenUsersExist()
        {
            // ARRANGE
            var fakeUsers = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", Name = "Alice", UserName = "alice123", Email = "alice@test.com" },
                new ApplicationUser { Id = "2", Name = "Bob", UserName = "bobname", Email = "bob@test.com" }
            };

            var aliceRoles = new List<string> { "Admin", "User" };
            var bobRoles = new List<string> { "User" };

            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync())
                     .ReturnsAsync(fakeUsers);

            _userRepositoryMock.Setup(repo => repo.GetRolesAsync(fakeUsers[0]))
                     .ReturnsAsync(aliceRoles);

            _userRepositoryMock.Setup(repo => repo.GetRolesAsync(fakeUsers[1]))
                     .ReturnsAsync(bobRoles);

            var svc = CreateService();
            // ACT
            var response = await svc.GetAllProfile();

            // ASSERT
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count);

            Assert.Equal("Alice", response.Data[0].Name);
            Assert.Contains("Admin", response.Data[0].Roles);

            _userRepositoryMock.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllProfile_ShouldReturnFailure_WhenNoUsersFound()
        {
            // ARRANGE
            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync())
                     .ReturnsAsync((List<ApplicationUser>)null);

            var svc = CreateService();
            // ACT
            var response = await svc.GetAllProfile();

            // ASSERT
            Assert.False(response.Success);
            Assert.Equal("No User found.", response.Error);
        }

        [Fact]
        public async Task GetAllUserByUsername_ShouldReturnMatchingUsers_WhenUsernameExists()
        {
            // 1. ARRANGE
            var searchToken = "dev";
            var fakeUsers = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", UserName = "dev_alice", Name = "Alice" },
                new ApplicationUser { Id = "2", UserName = "dev_bob", Name = "Bob" }
            };

            // Mock: make sure the repo receives the exact string "dev"
            _userRepositoryMock.Setup(r => r.GetUsersByNameAsync(searchToken))
                         .ReturnsAsync(fakeUsers);

            _userRepositoryMock.Setup(r => r.GetRolesAsync(It.IsAny<ApplicationUser>()))
                         .ReturnsAsync(new List<string> { "User" });

            var svc = CreateService();

            // 2. ACT
            var result = await svc.GetAllUserByUsername(searchToken);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count);

            _userRepositoryMock.Verify(r => r.GetUsersByNameAsync("dev"), Times.Once);
        }

        [Fact]
        public async Task GetAllUserByUsername_ShouldReturnFailure_WhenNoMatches()
        {
            // 1. ARRANGE
            var searchToken = "IamVoid";
            _userRepositoryMock.Setup(r => r.GetUsersByNameAsync(searchToken))
                         .ReturnsAsync((List<ApplicationUser>)null);

            var svc = CreateService();

            // 2. ACT
            var result = await svc.GetAllUserByUsername(searchToken);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("No User found.", result.Error);
        }

        [Fact]
        public async Task GetProfileByUserId_ShouldReturnUser_WhenIdIsValid()
        {
            // 1. ARRANGE
            var targetId = "user-abc-123";
            var fakeUser = new ApplicationUser
            {
                Id = targetId,
                Name = "Test Subject",
                UserName = "testsubject",
                Email = "test@dailyblogg.com",
                Bio = "This is a test bio."
            };
            var fakeRoles = new List<string> { "User" };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(targetId))
                         .ReturnsAsync(fakeUser);

            _userRepositoryMock.Setup(r => r.GetRolesAsync(fakeUser))
                         .ReturnsAsync(fakeRoles);

            var svc = CreateService();

            // 2. ACT
            var result = await svc.GetProfileByUserId(targetId);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(targetId, result.Data.Id);
            Assert.Equal("Test Subject", result.Data.Name);
            Assert.Equal("This is a test bio.", result.Data.Bio);
        }

        [Fact]
        public async Task GetProfileByUserId_ShouldReturnFailure_WhenIdDoesNotExist()
        {
            // 1. ARRANGE
            var invalidId = "noId";

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(invalidId))
                         .ReturnsAsync((ApplicationUser)null);

            var svc = CreateService();

            // 2. ACT
            var result = await svc.GetProfileByUserId(invalidId);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("No User found.", result.Error);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task DeleteUser_ShouldReturnSuccess_WhenUserIsDeleted()
        {
            // 1. ARRANGE
            var targetId = "user-to-delete";
            var fakeUser = new ApplicationUser { Id = targetId, UserName = "Bygone" };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(targetId))
                         .ReturnsAsync(fakeUser);

            var svc = CreateService();

            // 2. ACT
            var result = await svc.DeleteUser(targetId);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.True(result.Data);

            //Have to make sure the Remove and SaveChanges run properly
            _userRepositoryMock.Verify(r => r.Remove(fakeUser), Times.Once);
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFailure_WhenRepositoryThrows()
        {
            // 1. ARRANGE
            var targetId = "user-id";
            var fakeUser = new ApplicationUser { Id = targetId };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(targetId))
                         .ReturnsAsync(fakeUser);

            //Force the Remove method to throw an error
            _userRepositoryMock.Setup(r => r.Remove(It.IsAny<ApplicationUser>()))
                         .Throws(new Exception("Database connection lost"));

            var svc = CreateService();

            // 2. ACT
            var result = await svc.DeleteUser(targetId);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Failed to delete the user", result.Error);
        }
        [Fact]
        public async Task UpdateUserProfile_ShouldReturnFailure_WhenFileIsTooLarge()
        {
            // 1. ARRANGE
            var userId = "user-123";
            var fakeUser = new ApplicationUser { Id = userId, Name = "Old Name" };

            // Create a mock file that is larger than 2MB
            var fileMock = new Mock<IFormFile>();
            var content = "fake image content";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            fileMock.Setup(_ => _.Length).Returns(3 * 1024 * 1024); // 3MB
            fileMock.Setup(_ => _.ContentType).Returns("image/jpeg");

            var dto = new UpdateProfileDTO { Name = "New Name", ImageUrl = fileMock.Object };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(fakeUser);

            var svc = CreateService();
            // 2. ACT
            var result = await svc.UpdateUserProfile(userId, dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("File too large", result.Error);

            //Verify SaveChanges don't run
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldUpdateTextOnly_WhenNoImageProvided()
        {
            // 1. ARRANGE
            var userId = "user-123";
            var fakeUser = new ApplicationUser { Id = userId, Name = "Old Name", Bio = "Old Bio" };
            var dto = new UpdateProfileDTO { Name = "Updated Name", Bio = "Updated Bio", ImageUrl = null };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(fakeUser);
            _userRepositoryMock.Setup(r => r.GetRolesAsync(fakeUser)).ReturnsAsync(new List<string> { "User" });

            var svc = CreateService();
            // 2. ACT
            var result = await svc.UpdateUserProfile(userId, dto);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.Equal("Updated Name", result.Data.Name);
            Assert.Equal("Updated Bio", result.Data.Bio);

            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        //No test with returning the Picture because i update the picture by communicating directly
        //with the System.IO so it's hard to create a mock data for it
    }
}
