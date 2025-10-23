using TaskManagement.Models;
using TaskManagement.DTOs;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }

    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId);
        Task<IEnumerable<TaskItem>> GetFilteredAsync(TaskFilterDto filter, int userId);
        Task<int> GetFilteredCountAsync(TaskFilterDto filter, int userId);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(int userId);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status, int userId);
    }
}
