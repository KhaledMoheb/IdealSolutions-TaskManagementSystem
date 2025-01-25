using TaskManagementSystem.Api.RepositoryContracts;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Api.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DBTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task<DBTask?> GetByIdAsync(int id)
        {
            return await _context.Tasks.Include(t => t.AssignedUser).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<DBTask>> GetAllAsync()
        {
            return await _context.Tasks.Include(t => t.AssignedUser).ToListAsync();
        }

        public async Task<IEnumerable<DBTask>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks.Include(t => t.AssignedUser).Where(t => t.AssignedUserId == userId).ToListAsync();
        }

        public async Task UpdateAsync(DBTask task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DBTask task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
