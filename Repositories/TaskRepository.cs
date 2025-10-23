using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Repositories
{
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(TaskManagementContext context) : base(context)
        {
        }

        public override async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetFilteredAsync(TaskFilterDto filter, int userId)
        {
            var query = _dbSet.Include(t => t.User).Where(t => t.UserId == userId);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.DueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);

            if (filter.DueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(t => t.Title.Contains(filter.SearchTerm) ||
                                       (t.Description != null && t.Description.Contains(filter.SearchTerm)));
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetFilteredCountAsync(TaskFilterDto filter, int userId)
        {
            var query = _dbSet.Where(t => t.UserId == userId);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.DueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);

            if (filter.DueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(t => t.Title.Contains(filter.SearchTerm) ||
                                       (t.Description != null && t.Description.Contains(filter.SearchTerm)));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(t => t.User)
                .Where(t => t.UserId == userId && 
                           t.DueDate.HasValue && 
                           t.DueDate < now && 
                           t.Status != TaskStatus.Completed)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status, int userId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Where(t => t.UserId == userId && t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
