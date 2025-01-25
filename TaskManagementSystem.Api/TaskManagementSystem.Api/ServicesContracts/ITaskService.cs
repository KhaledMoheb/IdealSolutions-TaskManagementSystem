using System.Security.Claims;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.ServicesContracts
{
    /// <summary>
    /// Interface defining the contract for task-related operations.
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Creates a new task asynchronously.
        /// </summary>
        /// <param name="task">The task request containing task details.</param>
        /// <returns>A result containing the created task or an error.</returns>
        Task<Result<DBTask, TaskError>> CreateTaskAsync(TaskRequest task);

        /// <summary>
        /// Retrieves a task by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <param name="user">The authenticated user making the request.</param>
        /// <returns>A result containing the task response or an error.</returns>
        Task<Result<TaskResponse, TaskError>> GetTaskByIdAsync(int id, ClaimsPrincipal user);

        /// <summary>
        /// Retrieves all tasks assigned to a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID to retrieve tasks for.</param>
        /// <returns>A result containing a list of task responses or an error.</returns>
        Task<Result<IEnumerable<TaskResponse>, TaskError>> GetTasksByUserIdAsync(int userId);

        /// <summary>
        /// Retrieves all tasks in the system asynchronously.
        /// </summary>
        /// <returns>A list of all task responses.</returns>
        Task<IEnumerable<TaskResponse>> GetAllTasksAsync();

        /// <summary>
        /// Updates a task asynchronously.
        /// </summary>
        /// <param name="id">The ID of the task to update.</param>
        /// <param name="updatedTask">The task request containing updated task details.</param>
        /// <param name="user">The authenticated user making the request.</param>
        /// <returns>A result containing the updated task response or an error.</returns>
        Task<Result<TaskResponse, TaskError>> UpdateTaskAsync(int id, TaskRequest updatedTask, ClaimsPrincipal user);

        /// <summary>
        /// Deletes a task asynchronously.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>A result indicating success or failure of the delete operation.</returns>
        Task<Result<bool, TaskError>> DeleteTaskAsync(int id);
    }
}
