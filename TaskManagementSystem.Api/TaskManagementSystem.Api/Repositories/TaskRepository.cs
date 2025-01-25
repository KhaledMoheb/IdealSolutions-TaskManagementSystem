using TaskManagementSystem.Api.RepositoryContracts;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Api.Repositories
{
    /// <summary>
    /// Repository class for handling task-related database operations.
    /// Implements the ITaskRepository interface to perform CRUD operations on tasks.
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext into the repository
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new task to the database.
        /// </summary>
        /// <param name="task">The task object to be added.</param>
        public async Task AddAsync(DBTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a task by its ID.
        /// Includes the assigned user details.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <returns>The task with the specified ID, or null if not found.</returns>
        public async Task<DBTask?> GetByIdAsync(int id)
        {
            return await _context.Tasks.Include(t => t.AssignedUser).FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Gets all tasks in the database.
        /// Includes the assigned user details.
        /// </summary>
        /// <returns>A list of all tasks.</returns>
        public async Task<IEnumerable<DBTask>> GetAllAsync()
        {
            return await _context.Tasks.Include(t => t.AssignedUser).ToListAsync();
        }

        /// <summary>
        /// Gets all tasks assigned to a specific user by their user ID.
        /// Includes the assigned user details.
        /// </summary>
        /// <param name="userId">The ID of the user whose tasks to retrieve.</param>
        /// <returns>A list of tasks assigned to the user.</returns>
        public async Task<IEnumerable<DBTask>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks.Include(t => t.AssignedUser).Where(t => t.AssignedUserId == userId).ToListAsync();
        }

        /// <summary>
        /// Updates an existing task in the database.
        /// </summary>
        /// <param name="task">The task object with updated values.</param>
        public async Task UpdateAsync(DBTask task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a task from the database.
        /// </summary>
        /// <param name="task">The task to be deleted.</param>
        public async Task DeleteAsync(DBTask task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
