using System.Security.Claims;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.ServicesContracts
{
    public interface ITaskService
    {
        Task<Result<DBTask, TaskError>> CreateTaskAsync(TaskRequest task);
        Task<Result<TaskResponse, TaskError>> GetTaskByIdAsync(int id, ClaimsPrincipal user);
        Task<Result<IEnumerable<TaskResponse>, TaskError>> GetTasksByUserIdAsync(int userId);
        Task<IEnumerable<TaskResponse>> GetAllTasksAsync();
        Task<Result<TaskResponse, TaskError>> UpdateTaskAsync(int id, TaskRequest updatedTask, ClaimsPrincipal user);
        Task<Result<bool, TaskError>> DeleteTaskAsync(int id);
    }
}
