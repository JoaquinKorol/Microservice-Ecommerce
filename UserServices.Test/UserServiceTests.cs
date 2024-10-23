using Xunit;
using Moq;
using UserServices;
using UserServices.Services;
using UserServices.Repositories;
using Microsoft.Extensions.Options;
using static UserServices.Models.jwtSettings;
using UserServices.Models;
using UserServices.DTOs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UserServices.Test
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            var jwtSettings = Options.Create(new JwtSettings
            {
                // Inicializa las propiedades necesarias de JwtSettings aquí
                Secret = "MKDsañdsidajidsaiodjsklajpovivcknqasdklñas"
            });
            _userService = new UserService(_userRepositoryMock.Object, jwtSettings);
        }
        public class UserCreationTests : UserServiceTests
        {
            [Fact]
            public async Task AddUser_ShouldAddUser_WhenUserIsValid()
            {
                // Arrange: Init Variables
                var newUserDto = new RegisterDTO
                {
                    Name = "Jane Doe",
                    Email = "jane.doe@example.com",
                    Password = "Securepassword123"
                };

                var expectedUser = new User
                {
                    Name = newUserDto.Name,
                    Email = newUserDto.Email,
                    Password = newUserDto.Password,
                    CreatedAt = DateTime.UtcNow,
                };

                // Ajusta el setup para que verifique el objeto correcto
                _userRepositoryMock.Setup(repo => repo.AddAsync(It.Is<User>(u =>
                    u.Name == expectedUser.Name &&
                    u.Email == expectedUser.Email &&
                    u.Password == expectedUser.Password &&
                    u.CreatedAt.Date == expectedUser.CreatedAt.Date
                ))).Verifiable();

                // Act: Execute method to test
                var result = await _userService.CreateUserAsync(newUserDto);

                // Assert: comprobation values
                _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
                Assert.NotNull(result);
                Assert.Equal(expectedUser.Name, result.Name);
                Assert.Equal(expectedUser.Email, result.Email);
                Assert.Equal(expectedUser.Password, result.Password);
            }

            [Fact]
            public async Task AddUser_ShouldNotAddUser_WhenEmailIsUsed()
            {
                // Arrange
                var newUserDto = new RegisterDTO
                {
                    Name = "Jane Doe",
                    Email = "jane.doe@example.com",
                    Password = "ValidPassword123"  // Contraseña válida
                };

                _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());  // Simula que el email ya está en uso

                // Act & Assert
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(newUserDto));

                Assert.Equal("The email is already in use.", exception.Message);
                _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
            }
        }
        public class UserLoginTest : UserServiceTests
        {

            [Fact]
            public async Task LoginUser_ShouldThrowException_WhenInvalidCredentials()
            {
                var loginDto = new LoginDTO
                {
                    Email = "jane.doe@example.com",
                    Password = "wrongpassword"
                };

                _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(loginDto.Email))
                    .ReturnsAsync(new User { Password = "correctpassword" }); // Simula que el usuario existe pero con la contraseña incorrecta

                var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginUserAsync(loginDto));
                Assert.Equal("Invalid credentials", exception.Message);
            }

            [Fact]
            public async Task LoginUser_ShouldReturnUser_WhenCredentialsAreValid()
            {
                // Arrange
                var loginDto = new LoginDTO
                {
                    Email = "jane.doe@example.com",
                    Password = "securepassword123"
                };

                var expectedUser = new User
                {
                    Name = "Jane Doe",
                    Email = loginDto.Email,
                    Password = loginDto.Password
                };

                // Simula que el usuario existe y tiene la contraseña correcta
                _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(loginDto.Email))
                    .ReturnsAsync(expectedUser);

                // Act
                var result = await _userService.LoginUserAsync(loginDto);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expectedUser.Name, result.Name);
                Assert.Equal(expectedUser.Email, result.Email);
                Assert.Equal(expectedUser.Password, result.Password);
            }

        }

    }
}