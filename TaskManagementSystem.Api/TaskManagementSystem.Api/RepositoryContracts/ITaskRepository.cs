using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.RepositoryContracts
{

    public interface ITaskRepository
    {
        Task AddAsync(DBTask task);
        Task<DBTask?> GetByIdAsync(int id);
        Task<IEnumerable<DBTask>> GetAllAsync();
        Task<IEnumerable<DBTask>> GetTasksByUserIdAsync(int userId);
        Task UpdateAsync(DBTask task);
        Task DeleteAsync(DBTask task);
    }
}
