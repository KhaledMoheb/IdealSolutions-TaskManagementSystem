using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.ServicesContracts;
using Swashbuckle.AspNetCore.Annotations;

namespace TaskManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Create a new task.
        /// </summary>
        /// <remarks>
        /// Only Admins are authorized to create tasks.
        /// </remarks>
        /// <param name="task">The task to create.</param>
        /// <returns>Created task with 201 status code if successful, or 400/404 if failed.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create a new task", Description = "Create a task that can be assigned to a user.")]
        [SwaggerResponse(201, "Task created successfully.", typeof(TaskResponse))]
        [SwaggerResponse(400, "Bad request. Invalid data.")]
        [SwaggerResponse(404, "User not found.")]
        public async Task<IActionResult> CreateTask([FromBody] TaskRequest task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.CreateTaskAsync(task);
            return result.Match<IActionResult>(
                success => CreatedAtAction(nameof(GetTaskById), new { id = success.Id }, success),
                error => error.ErrorCode is TaskError.TaskErrorCode.UserNotFound ? NotFound(error.Message) : BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Get task details by ID.
        /// </summary>
        /// <param name="id">The task ID.</param>
        /// <returns>Task details with 200 status code, or 404/403 if task not found or access forbidden.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get task by ID", Description = "Retrieve a task's details by its ID.")]
        [SwaggerResponse(200, "Task details found.", typeof(TaskResponse))]
        [SwaggerResponse(404, "Task not found.")]
        [SwaggerResponse(403, "Forbidden access.")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var result = await _taskService.GetTaskByIdAsync(id, User);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => error.ErrorCode is TaskError.TaskErrorCode.NotFound ? NotFound(error.Message) : Forbid()
            );
        }

        /// <summary>
        /// Get all tasks for a specific user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>List of tasks assigned to the user, or 404 if user not found.</returns>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get tasks for a user", Description = "Retrieve all tasks assigned to a specific user.")]
        [SwaggerResponse(200, "List of tasks for the user.", typeof(IEnumerable<TaskResponse>))]
        [SwaggerResponse(404, "User not found.")]
        public async Task<IActionResult> GetUserTasks(int userId)
        {
            var result = await _taskService.GetTasksByUserIdAsync(userId);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => NotFound(error.Message)
            );
        }

        /// <summary>
        /// Get all tasks in the system.
        /// </summary>
        /// <returns>List of all tasks in the system.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get all tasks", Description = "Retrieve all tasks in the system.")]
        [SwaggerResponse(200, "List of all tasks.", typeof(IEnumerable<TaskResponse>))]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Update an existing task.
        /// </summary>
        /// <param name="id">The task ID.</param>
        /// <param name="updatedTask">The task update details.</param>
        /// <returns>Updated task details with 200 status code, or 404/403 if task not found or access forbidden.</returns>
        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Update an existing task", Description = "Update task details such as title, status, description, and assignee.")]
        [SwaggerResponse(200, "Task updated successfully.", typeof(TaskResponse))]
        [SwaggerResponse(404, "Task not found.")]
        [SwaggerResponse(403, "Forbidden access.")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskRequest updatedTask)
        {
            var result = await _taskService.UpdateTaskAsync(id, updatedTask, User);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => error.ErrorCode is TaskError.TaskErrorCode.NotFound ? NotFound(error.Message) : Forbid()
            );
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The task ID.</param>
        /// <returns>No content with 204 status code, or 404 if task not found.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a task", Description = "Delete a task by its ID.")]
        [SwaggerResponse(204, "Task deleted successfully.")]
        [SwaggerResponse(404, "Task not found.")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            return result.Match<IActionResult>(
                success => NoContent(),
                error => NotFound(error.Message)
            );
        }
    }
}
