using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Services
{
    public interface ITaskService
    {
        Task<TaskItemDto?> GetByIdAsync(int id, int userId);
        Task<PagedResultDto<TaskItemDto>> GetFilteredAsync(TaskFilterDto filter, int userId);
        Task<IEnumerable<TaskItemDto>> GetByUserIdAsync(int userId);
        Task<TaskItemDto> CreateAsync(CreateTaskDto createTaskDto, int userId);
        Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskDto updateTaskDto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<IEnumerable<TaskItemDto>> GetOverdueTasksAsync(int userId);
        Task<IEnumerable<TaskItemDto>> GetTasksByStatusAsync(TaskStatus status, int userId);
        Task<TaskItemDto?> CompleteTaskAsync(int id, int userId);
    }
}
