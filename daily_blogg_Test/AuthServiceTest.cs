using System.Timers;
using dailyblogg_backend.Services.AuthServices;
using dailyblogg_backend.Repositories;
using dailyblogg_backend.Models.Entities;
using dailyblogg_backend.Models.DTOs;
using Moq;
namespace daily_blogg_Test
{
    public class AuthServiceTest
    {
        private readonly Mock<IUserRepository<ApplicationUser>> _userRepo;
        private readonly Mock<IAuthRepository<ApplicationUser>> _authRepo;
        private readonly Mock<IJwtTokenGenerator> _jwtGen;
        public AuthServiceTest()
        {
            _userRepo = new Mock<IUserRepository<ApplicationUser>>();
            _authRepo = new Mock<IAuthRepository<ApplicationUser>>();
            _jwtGen = new Mock<IJwtTokenGenerator>();
        }
        private AuthService CreateService() => new AuthService(_authRepo.Object, _userRepo.Object, _jwtGen.Object);

        [Fact]
        public async Task LoginUser_WithValidCredentials_ReturnsTokenAndUser()
        {
            // Arrange
            var dto = new LoginDTO { Email = "test@example.com", Password = "iampassword" };
            var user = new ApplicationUser { Id = "1", Email = dto.Email, Name = "Test", UserName = "test" };

                //Telling the Repo what it should return
            _userRepo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _authRepo.Setup(r => r.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
                
            var roles = new List<string> { "User" };

                //Telling the Repo what it should return
            _userRepo.Setup(r => r.GetRolesAsync(user)).ReturnsAsync(roles);
            _jwtGen.Setup(j => j.GenerateToken(user, roles)).Returns("fake-token");

            var svc = CreateService();

            // Act
            var result = await svc.LoginUser(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("fake-token", result.Data.Token);
            Assert.Equal(user.Email, result.Data.User.Email);

            // Verify the repo was actually called exactly once for Login
            _userRepo.Verify(r => r.FindByEmailAsync(dto.Email), Times.Once);
            _authRepo.Verify(r => r.CheckPasswordAsync(user, dto.Password), Times.Once);
        }

        [Fact]
        public async Task LoginUser_WithUnknownEmail_ReturnsFailure()
        {
            // Arrange
            var dto = new LoginDTO { Email = "missing@example.com", Password = "iampassword" };
            _userRepo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

            var svc = CreateService();

            // Act
            var result = await svc.LoginUser(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Invalid email or password", result.Error);

            // Verify the repo was actually called exactly once for Login
            _userRepo.Verify(r => r.FindByEmailAsync(dto.Email), Times.Once);
            _authRepo.Verify(r => r.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginUser_WithWrongPassword_ReturnsFailure()
        {
            // Arrange
            var dto = new LoginDTO { Email = "test@example.com", Password = "iampassword" };
            var user = new ApplicationUser { Id = "1", Email = dto.Email };
            _userRepo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _authRepo.Setup(r => r.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            var svc = CreateService();

            // Act
            var result = await svc.LoginUser(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Invalid email or password", result.Error);
            _authRepo.Verify(r => r.CheckPasswordAsync(user, dto.Password), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_WithFullCredentials_ReturnSuccessAndData()
        {
            //Arrange
            var dto = new RegisterDTO { Name = "john", UserName = "NotJohn", Email = "test@example.com", Password = "iampassword" };

            _userRepo.Setup(r => r.FindByNameAsync(dto.UserName)).ReturnsAsync((ApplicationUser?)null);

            var roles = new List<string> { "User" };

            _userRepo.Setup(r => r.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
            _jwtGen.Setup(j => j.GenerateToken(It.IsAny<ApplicationUser>(), roles)).Returns("fake-token");

            var svc = CreateService();
            //Act

            var result = await svc.RegisterUser(dto);
            //Assert

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("fake-token", result.Data.Token);
            _authRepo.Verify( r => r.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_WhenUserNameExists_ReturnFailure()
        {
            //Arrange
            var dto = new RegisterDTO { Name = "john", UserName = "NotJohn", Email = "test@example.com", Password = "iampassword" };

            _userRepo.Setup(r => r.FindByNameAsync(dto.UserName))
                     .ReturnsAsync( new ApplicationUser { UserName ="NotJohn"});

            var svc = CreateService();
            //Act

            var result = await svc.RegisterUser(dto);
            //Assert

            Assert.False(result.Success);
            Assert.Null(result.Data);
            _authRepo.Verify(a => a.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }
    }
}
