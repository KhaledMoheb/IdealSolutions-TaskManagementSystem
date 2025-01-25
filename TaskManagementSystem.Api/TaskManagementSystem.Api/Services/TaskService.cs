using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.RepositoryContracts;
using TaskManagementSystem.Api.ServicesContracts;


namespace TaskManagementSystem.Api.Services
{

    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<DBUser> _userManager;

        public TaskService(ITaskRepository taskRepository, UserManager<DBUser> userManager)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        public async Task<Result<DBTask, TaskError>> CreateTaskAsync(TaskRequest task)
        {
            var assignedUser = await _userManager.FindByIdAsync(task.AssignedUserId.ToString());
            if (assignedUser == null)
            {
                return Result<DBTask, TaskError>.Error(TaskError.UserNotFound($"Assigned user with ID {task.AssignedUserId} not found."));
            }

            var dbTask = new DBTask
            {
                Title = task.Title,
                Status = string.IsNullOrWhiteSpace(task.Status) ? "pending" : task.Status.ToLower(),
                Description = task.Description,
                AssignedUserId = task.AssignedUserId
            };

            await _taskRepository.AddAsync(dbTask);
            return Result<DBTask, TaskError>.Success(dbTask);
        }

        public async Task<Result<TaskResponse, TaskError>> GetTaskByIdAsync(int id, ClaimsPrincipal user)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return Result<TaskResponse, TaskError>.Error(TaskError.NotFound("Task not found"));
            }

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && task.AssignedUserId != userId)
            {
                return Result<TaskResponse, TaskError>.Error(TaskError.ForbiddenAccess());
            }

            return Result<TaskResponse, TaskError>.Success(task.ToTaskResponse());
        }

        public async Task<Result<IEnumerable<TaskResponse>, TaskError>> GetTasksByUserIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Result<IEnumerable<TaskResponse>, TaskError>.Error(TaskError.NotFound("User not found"));
            }

            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            if (!tasks.Any())
            {
                return Result<IEnumerable<TaskResponse>, TaskError>.Error(TaskError.NotFound("No tasks found for this user"));
            }

            return Result<IEnumerable<TaskResponse>, TaskError>.Success(tasks.Select(t => t.ToTaskResponse()));
        }

        public async Task<IEnumerable<TaskResponse>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(t => t.ToTaskResponse());
        }

        public async Task<Result<TaskResponse, TaskError>> UpdateTaskAsync(int id, TaskRequest updatedTask, ClaimsPrincipal user)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return Result<TaskResponse, TaskError>.Error(TaskError.NotFound("Task not found"));
            }

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Admin")
            {
                task.Title = updatedTask.Title;
                task.Description = updatedTask.Description;
                task.Status = updatedTask.Status;
                task.AssignedUserId = updatedTask.AssignedUserId;
            }
            else if (userRole == "user" && task.AssignedUserId == userId)
            {
                task.Status = updatedTask.Status;
            }
            else
            {
                return Result<TaskResponse, TaskError>.Error(TaskError.ForbiddenAccess());
            }

            await _taskRepository.UpdateAsync(task);
            return Result<TaskResponse, TaskError>.Success(task.ToTaskResponse());
        }

        public async Task<Result<bool, TaskError>> DeleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return Result<bool, TaskError>.Error(TaskError.NotFound("Task not found"));
            }

            await _taskRepository.DeleteAsync(task);
            return Result<bool, TaskError>.Success(true);
        }
    }
}
