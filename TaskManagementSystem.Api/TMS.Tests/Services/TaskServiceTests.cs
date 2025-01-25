using Moq;
using System.Security.Claims;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Services;
using TaskManagementSystem.Api.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Moq.Protected;

namespace TaskManagementSystem.Api.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<UserManager<DBUser>> _userManagerMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();

            _userManagerMock = new Mock<UserManager<DBUser>>(
                Mock.Of<IUserStore<DBUser>>(),
                null, null, null, null, null, null, null, null);

            _taskService = new TaskService(_taskRepositoryMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnSuccess_WhenUserIsFound()
        {
            // Arrange
            var taskRequest = new TaskRequest
            {
                AssignedUserId = 1,
                Title = "Test Task",
                Status = "pending",
                Description = "Task Description"
            };

            var dbUser = new DBUser { Id = 1, UserName = "user1" };

            // Mock FindByIdAsync to return a valid user
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(dbUser);

            _taskRepositoryMock.Setup(x => x.AddAsync(It.IsAny<DBTask>())).Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.CreateTaskAsync(taskRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(taskRequest.Title, result.Value.Title);
            _taskRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DBTask>()), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            var taskRequest = new TaskRequest
            {
                AssignedUserId = 9999, // User ID doesn't exist
                Title = "Test Task",
                Status = "pending",
                Description = "Task Description"
            };

            // Mock FindByIdAsync to return null (user not found)
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((DBUser)null);

            // Act
            var result = await _taskService.CreateTaskAsync(taskRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Assigned user with ID 9999 not found.", result.Err.Message);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTask_WhenUserHasPermission()
        {
            // Arrange
            var taskId = 1;
            var dbTask = new DBTask { Id = taskId, Title = "Test Task", Status = "pending", Description = "Task Description", AssignedUserId = 1 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }));

            _taskRepositoryMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(dbTask);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId, user);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dbTask.Title, result.Value.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnForbidden_WhenUserDoesNotHavePermission()
        {
            // Arrange
            var taskId = 1;
            var dbTask = new DBTask { Id = taskId, Title = "Test Task", Status = "pending", Description = "Task Description", AssignedUserId = 2 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }));

            _taskRepositoryMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(dbTask);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId, user);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("You do not have permission to access this task.", result.Err.Message);
        }

        [Fact]
        public async Task GetTasksByUserIdAsync_ShouldReturnTasks_WhenUserHasTasks()
        {
            // Arrange
            var userId = 1;
            var dbUser = new DBUser { Id = userId, UserName = "user1" };
            var dbTasks = new List<DBTask>
            {
                new DBTask { Id = 1, Title = "Task 1", AssignedUserId = userId },
                new DBTask { Id = 2, Title = "Task 2", AssignedUserId = userId }
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(dbUser);
            _taskRepositoryMock.Setup(x => x.GetTasksByUserIdAsync(userId)).ReturnsAsync(dbTasks);

            // Act
            var result = await _taskService.GetTasksByUserIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetTasksByUserIdAsync_ShouldReturnError_WhenUserHasNoTasks()
        {
            // Arrange
            var userId = 1;
            var dbUser = new DBUser { Id = userId, UserName = "user1" };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(dbUser);
            _taskRepositoryMock.Setup(x => x.GetTasksByUserIdAsync(userId)).ReturnsAsync(new List<DBTask>());

            // Act
            var result = await _taskService.GetTasksByUserIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No tasks found for this user", result.Err.Message);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnSuccess_WhenTaskIsFound()
        {
            // Arrange
            var taskId = 1;
            var dbTask = new DBTask { Id = taskId, Title = "Test Task" };
            _taskRepositoryMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(dbTask);
            _taskRepositoryMock.Setup(x => x.DeleteAsync(dbTask)).Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            Assert.True(result.IsSuccess);
            _taskRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<DBTask>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnError_WhenTaskNotFound()
        {
            // Arrange
            var taskId = 1;
            _taskRepositoryMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync((DBTask)null);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Task not found", result.Err.Message);
        }
    }
}
