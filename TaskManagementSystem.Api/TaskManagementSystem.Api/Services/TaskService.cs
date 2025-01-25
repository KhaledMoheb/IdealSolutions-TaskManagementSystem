using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.RepositoryContracts;
using TaskManagementSystem.Api.ServicesContracts;

namespace TaskManagementSystem.Api.Services
{
    /// <summary>
    /// Service responsible for handling task-related operations.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<DBUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskService"/> class.
        /// </summary>
        /// <param name="taskRepository">The repository for task-related operations.</param>
        /// <param name="userManager">The manager for user-related operations.</param>
        public TaskService(ITaskRepository taskRepository, UserManager<DBUser> userManager)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new task and assigns it to a user.
        /// </summary>
        /// <param name="task">The task details to create.</param>
        /// <returns>A result indicating success or failure.</returns>
        public async Task<Result<DBTask, TaskError>> CreateTaskAsync(TaskRequest task)
        {
            // Find the user by ID
            var assignedUser = await _userManager.FindByIdAsync(task.AssignedUserId.ToString());
            if (assignedUser == null)
            {
                // Return error if user is not found
                return Result<DBTask, TaskError>.Error(TaskError.UserNotFound($"Assigned user with ID {task.AssignedUserId} not found."));
            }

            var dbTask = new DBTask
            {
                Title = task.Title,
                Status = string.IsNullOrWhiteSpace(task.Status) ? "pending" : task.Status.ToLower(),
                Description = task.Description,
                AssignedUserId = task.AssignedUserId
            };

            // Add the task to the repository
            await _taskRepository.AddAsync(dbTask);
            return Result<DBTask, TaskError>.Success(dbTask);
        }

        /// <summary>
        /// Retrieves a task by ID, checking if the user has permission to access it.
        /// </summary>
        /// <param name="id">The task ID to retrieve.</param>
        /// <param name="user">The current user making the request.</param>
        /// <returns>A result containing the task response or an error.</returns>
        public async Task<Result<TaskResponse, TaskError>> GetTaskByIdAsync(int id, ClaimsPrincipal user)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return Result<TaskResponse, TaskError>.Error(TaskError.NotFound("Task not found"));
            }

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

            // Ensure user has access to the task based on role
            if (userRole != "Admin" && task.AssignedUserId != userId)
            {
                return Result<TaskResponse, TaskError>.Error(TaskError.ForbiddenAccess());
            }

            return Result<TaskResponse, TaskError>.Success(task.ToTaskResponse());
        }

        /// <summary>
        /// Retrieves all tasks assigned to a specific user.
        /// </summary>
        /// <param name="userId">The user ID to filter tasks by.</param>
        /// <returns>A result containing task responses or an error.</returns>
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

        /// <summary>
        /// Retrieves all tasks in the system.
        /// </summary>
        /// <returns>A list of all tasks.</returns>
        public async Task<IEnumerable<TaskResponse>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(t => t.ToTaskResponse());
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="id">The task ID to update.</param>
        /// <param name="updatedTask">The updated task details.</param>
        /// <param name="user">The current user making the request.</param>
        /// <returns>A result indicating success or failure.</returns>
        public async Task<Result<TaskResponse, TaskError>> UpdateTaskAsync(int id, TaskRequest updatedTask, ClaimsPrincipal user)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return Result<TaskResponse, TaskError>.Error(TaskError.NotFound("Task not found"));
            }

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

            // Admin can update all fields, user can only update the status of their own tasks
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

        /// <summary>
        /// Deletes a task from the system.
        /// </summary>
        /// <param name="id">The task ID to delete.</param>
        /// <returns>A result indicating success or failure.</returns>
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
