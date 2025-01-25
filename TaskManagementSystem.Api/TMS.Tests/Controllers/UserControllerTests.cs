using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using Xunit;
using TaskManagementSystem.Tests.Helpers;

namespace TaskManagementSystem.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<UserManager<DBUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole<int>>> _mockRoleManager;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserManager = MockHelpers.MockUserManager<DBUser>();
            _mockRoleManager = MockHelpers.MockRoleManager<IdentityRole<int>>();
            _controller = new UserController(_mockUserManager.Object, _mockRoleManager.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WithUsersInRole()
        {
            // Arrange
            var users = new List<DBUser> { new DBUser { Id = 1, UserName = "User1" } };
            _mockUserManager.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>()))
                            .ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<DBUser>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetUserById_ReturnsOkResult_WithUser()
        {
            // Arrange
            var user = new DBUser { Id = 1, UserName = "User1" };
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<DBUser>(okResult.Value);
            Assert.Equal("User1", returnValue.UserName);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                            .ReturnsAsync((DBUser)null);

            // Act
            var result = await _controller.GetUserById("1");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAtActionResult_WhenSuccessful()
        {
            // Arrange
            var userRequest = new UserRequest { UserName = "User1", Email = "user1@example.com" };
            var dbUser = userRequest.ToDBUser();

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<DBUser>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.CreateUser(userRequest, "Password123!");

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<UserRequest>(createdResult.Value);
            Assert.Equal("User1", returnValue.UserName);
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model");


            var userRequest = new UserRequest
            {
                UserName = "TestUser",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
            };

            // Act
            var result = await _controller.CreateUser(userRequest, "Password123!");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var existingUser = new DBUser { Id = 1, UserName = "User1", Email = "user1@example.com" };
            var updatedUser = new DBUser { Id = 1, UserName = "UpdatedUser", Email = "updated@example.com" };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<DBUser>()))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.UpdateUser(1, updatedUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<DBUser>(okResult.Value);
            Assert.Equal("UpdatedUser", returnValue.UserName);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var existingUser = new DBUser { Id = 1, UserName = "User1" };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<DBUser>()))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUser("1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                            .ReturnsAsync((DBUser)null);

            // Act
            var result = await _controller.DeleteUser("1");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
