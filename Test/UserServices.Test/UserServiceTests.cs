using Xunit;
using Moq;
using UserServices;
using UserServices.Repositories;
using Microsoft.Extensions.Options;
using UserServices.Models;
using UserServices.DTOs;
using Core.Interfaces;
using System;
using System.Threading.Tasks;
using static UserServices.Models.jwtSettings;
using Microsoft.AspNetCore.Mvc;
using UserServices.Controllers;
using Core.Exceptions;

namespace UserServices.Test
{
    public class UserServiceTests
    {
        private readonly Mock<IRepository<User>> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IRepository<User>>(); // Inicialización del mock
            var jwtSettings = Options.Create(new jwtSettings
            {
                Secret = "MKDsañdsidajidsaiodjsklajpovivcknqasdklñas"
            });
            _userService = new UserService(_userRepositoryMock.Object, jwtSettings);
        }

        public class UserRegisterTests : UserServiceTests
        {
            [Fact]
            public async Task CreateUserAsync_ShouldAddUser_WhenUserIsValid()
            {
                // Arrange
                var newUserDto = new RegisterUserDTO
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
                    CreatedAt = DateTime.UtcNow
                };

                _userRepositoryMock.Setup(repo => repo.AddAsync(It.Is<User>(u =>
                    u.Name == expectedUser.Name &&
                    u.Email == expectedUser.Email &&
                    u.Password == expectedUser.Password
                ))).Verifiable();

                // Act
                var result = await _userService.CreateUserAsync(newUserDto);

                // Assert
                _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
                Assert.NotNull(result);
                Assert.Equal(expectedUser.Name, result.Name);
                Assert.Equal(expectedUser.Email, result.Email);
            }

            
        }

        public class UserUpdateTests : UserServiceTests
        {
            [Fact]
            public async Task UpdateUserAsync_ShouldReturnUser_WhenCredentialsAreValid()
            {
                // Arrange
                var userId = 1;
                var existingUser = new User
                {
                    Id = userId,
                    Name = "Test",
                    Email = "test@example.com",
                    Password = "securepassword123"
                };

                var updateDto = new UpdateUserDTO
                {
                    Name = "Jane Doe",
                    Email = "janedoe@example.com"
                };

                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
                _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Callback<User>(updatedUser =>
                {
                    existingUser.Name = updatedUser.Name;
                    existingUser.Email = updatedUser.Email;
                });

                // Act
                var result = await _userService.UpdateUserAsync(userId, updateDto);

                // Assert
                Assert.Equal(updateDto.Name, result.Name);
                Assert.Equal(updateDto.Email, result.Email);
                _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
                _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
            }

            [Fact]
            public async Task UpdateUserAsync_ShouldThrowException_WhenUserNotFound()
            {
                // Arrange
                var userId = 1;
                var updateDTO = new UpdateUserDTO { Name = "New Name", Email = "newemail@example.com" };

                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(userId, updateDTO));
                Assert.Equal($"User with ID {userId} not found.", exception.Message);

                _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            }
        }

        public class DeleteUserTests : UserServiceTests
        {
            [Fact]
            public async Task DeleteUserAsync_UserDoesNotExist_ThrowsNotFoundException()
            {
                // Arrange
                var id = 1;

                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((User)null);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUserAsync(id));
                Assert.Equal($"User with ID {id} not found.", exception.Message);

                _userRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
                _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
            }

            [Fact]
            public async Task DeleteUserAsync_ShouldDelete_WhenIdExists()
            {
                // Arrange
                var id = 1;
                var existingUser = new User
                {
                    Id = id,
                    Name = "Test User",
                    Email = "testuser@example.com",
                    Password = "password123"
                };

                _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingUser);

                // Act
                await _userService.DeleteUserAsync(id);

                // Assert
                _userRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
                _userRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
            }
        }
    }
}
