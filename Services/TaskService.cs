using AutoMapper;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories;
using TaskManagement.Services;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<TaskItemDto?> GetByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId) return null;

            var taskDto = _mapper.Map<TaskItemDto>(task);
            taskDto.UserName = task.User.Username;
            return taskDto;
        }

        public async Task<PagedResultDto<TaskItemDto>> GetFilteredAsync(TaskFilterDto filter, int userId)
        {
            var tasks = await _taskRepository.GetFilteredAsync(filter, userId);
            var totalCount = await _taskRepository.GetFilteredCountAsync(filter, userId);

            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                CompletedAt = t.CompletedAt,
                UserId = t.UserId,
                UserName = t.User.Username
            }).ToList();

            return new PagedResultDto<TaskItemDto>
            {
                Items = taskDtos,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<IEnumerable<TaskItemDto>> GetByUserIdAsync(int userId)
        {
            var tasks = await _taskRepository.GetByUserIdAsync(userId);
            return tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                CompletedAt = t.CompletedAt,
                UserId = t.UserId,
                UserName = t.User.Username
            });
        }

        public async Task<TaskItemDto> CreateAsync(CreateTaskDto createTaskDto, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found");

            var task = _mapper.Map<TaskItem>(createTaskDto);
            task.UserId = userId;
            task.CreatedAt = DateTime.UtcNow;

            var createdTask = await _taskRepository.AddAsync(task);
            var taskDto = _mapper.Map<TaskItemDto>(createdTask);
            taskDto.UserName = user.Username;
            return taskDto;
        }

        public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskDto updateTaskDto, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId) return null;

            _mapper.Map(updateTaskDto, task);
            task.UpdatedAt = DateTime.UtcNow;

            // If status is being changed to Completed, set CompletedAt
            if (updateTaskDto.Status == TaskStatus.Completed && task.Status != TaskStatus.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else if (updateTaskDto.Status != TaskStatus.Completed && task.Status == TaskStatus.Completed)
            {
                task.CompletedAt = null;
            }

            await _taskRepository.UpdateAsync(task);
            var taskDto = _mapper.Map<TaskItemDto>(task);
            taskDto.UserName = task.User.Username;
            return taskDto;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId) return false;

            await _taskRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<TaskItemDto>> GetOverdueTasksAsync(int userId)
        {
            var tasks = await _taskRepository.GetOverdueTasksAsync(userId);
            return tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                CompletedAt = t.CompletedAt,
                UserId = t.UserId,
                UserName = t.User.Username
            });
        }

        public async Task<IEnumerable<TaskItemDto>> GetTasksByStatusAsync(TaskStatus status, int userId)
        {
            var tasks = await _taskRepository.GetTasksByStatusAsync(status, userId);
            return tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                CompletedAt = t.CompletedAt,
                UserId = t.UserId,
                UserName = t.User.Username
            });
        }

        public async Task<TaskItemDto?> CompleteTaskAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId) return null;

            task.Status = TaskStatus.Completed;
            task.CompletedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
            var taskDto = _mapper.Map<TaskItemDto>(task);
            taskDto.UserName = task.User.Username;
            return taskDto;
        }
    }
}
