using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.RepositoryContracts
{
    /// <summary>
    /// Interface defining the contract for task-related repository operations.
    /// </summary>
    public interface ITaskRepository
    {
        /// <summary>
        /// Adds a new task to the repository.
        /// </summary>
        /// <param name="task">The task object to be added.</param>
        Task AddAsync(DBTask task);

        /// <summary>
        /// Retrieves a task by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <returns>The task with the specified ID, or null if not found.</returns>
        Task<DBTask?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all tasks from the repository.
        /// </summary>
        /// <returns>A list of all tasks.</returns>
        Task<IEnumerable<DBTask>> GetAllAsync();

        /// <summary>
        /// Retrieves all tasks assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose tasks to retrieve.</param>
        /// <returns>A list of tasks assigned to the user.</returns>
        Task<IEnumerable<DBTask>> GetTasksByUserIdAsync(int userId);

        /// <summary>
        /// Updates an existing task in the repository.
        /// </summary>
        /// <param name="task">The task object with updated values.</param>
        Task UpdateAsync(DBTask task);

        /// <summary>
        /// Deletes a task from the repository.
        /// </summary>
        /// <param name="task">The task to be deleted.</param>
        Task DeleteAsync(DBTask task);
    }
}
