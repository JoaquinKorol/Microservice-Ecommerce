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
using UserServices.Exceptions;
using System.ComponentModel.DataAnnotations;

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
        public class UserRegisterTests : UserServiceTests
        {
            [Fact]
            public async Task CreateUserAsync_ShouldAddUser_WhenUserIsValid()
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
            public async Task CreateUserAsync_ShouldNotAddUser_WhenEmailIsUsed()
            {
                // Arrange
                var newUserDto = new RegisterDTO
                {
                    Name = "Jane Doe",
                    Email = "jane.doe@example.com",
                    Password = "ValidPassword123"  // Contraseña válida
                };

                _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

                // Act 
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(newUserDto));


                // Assert
                Assert.Equal("The email is already in use.", exception.Message);
                _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
            }
        }


        public class UserUpdateTest : UserServiceTests
        {
            [Fact]
            public async Task UpdateUserAsync_ShouldReturnUser_WhenCredentialsAreValid()
            {

                //Arrange
                var existingUser = new User
                {
                    Id = 1,
                    Name = "Test",
                    Email = "test@example.com",
                    Password = "securepassword123"
                };

                var updateDto = new UpdateDTO
                {
                    Name = "Jane Doe",
                    Email = "janedoe@example.com"
                };

                // Act
                _userRepositoryMock.Setup(repo => repo.UpdateAsync(1, updateDto))
                    .Callback(() =>
                    {
                        existingUser.Name = updateDto.Name;
                        existingUser.Email = updateDto.Email;
                    });



                await _userRepositoryMock.Object.UpdateAsync(1, updateDto);

                // Assert
                Assert.Equal(updateDto.Name, existingUser.Name);
                Assert.Equal(updateDto.Email, existingUser.Email);
            }

            [Fact]
            public async Task UpdateUserAsync_ShouldThrowException_WhenUserNotFound()
            {

                //Arrange
                var id = 1;
                var updateDTO = new UpdateDTO { Name = "New Name", Email = "valid@example.com" };

                _userRepositoryMock.Setup(s => s.UpdateAsync(id, updateDTO));

                // Act
                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((User)null);

                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(id, updateDTO));
            }

            [Fact]
            public async Task UpdateUserAsync_UserDoesNotExist_ThrowsNotFoundException()
            {
                // Arrange
                var id = 1;
                var updateDTO = new UpdateDTO { Name = "New Name", Email = "valid@example.com" };

                // Act 
                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((User)null);

                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(id, updateDTO));
            }
        }

        public class DeleteUserTest : UserServiceTests
        {
            [Fact]
            public async Task DeleteUserAsync_UserDoesNotExist_ThrowsNotFoundException()
            {
                // Arrange
                var id = 1;

                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((User)null);

                await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUserAsync(id));

                _userRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);

                _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<User>()), Times.Never);
            }

            [Fact]
            public async Task UserDeleteAsync_ShouldDelete_WhenIdExist()
            {
                //Arrange
                var id = 1;

                var existingUser = new User
                {
                    Id = id,
                    Name = "Test User",
                    Email = "testuser@example.com",
                    Password = "password123"
                };

                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingUser);

                await _userService.DeleteUserAsync(id);

                _userRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
                _userRepositoryMock.Verify(repo => repo.DeleteAsync(existingUser), Times.Once);

            }
        }
    }
}

