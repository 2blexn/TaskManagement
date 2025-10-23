using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.Validators;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly CreateTaskDtoValidator _createTaskValidator;
        private readonly UpdateTaskDtoValidator _updateTaskValidator;
        private readonly TaskFilterDtoValidator _filterValidator;

        public TasksController(
            ITaskService taskService,
            CreateTaskDtoValidator createTaskValidator,
            UpdateTaskDtoValidator updateTaskValidator,
            TaskFilterDtoValidator filterValidator)
        {
            _taskService = taskService;
            _createTaskValidator = createTaskValidator;
            _updateTaskValidator = updateTaskValidator;
            _filterValidator = filterValidator;
        }

        /// <summary>
        /// Get all tasks for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks()
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetByUserIdAsync(userId);
            return Ok(tasks);
        }

        /// <summary>
        /// Get filtered and paginated tasks
        /// </summary>
        [HttpGet("filtered")]
        public async Task<ActionResult<PagedResultDto<TaskItemDto>>> GetFilteredTasks([FromQuery] TaskFilterDto filter)
        {
            var validationResult = await _filterValidator.ValidateAsync(filter);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = GetCurrentUserId();
            var result = await _taskService.GetFilteredAsync(filter, userId);
            return Ok(result);
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.GetByIdAsync(id, userId);
            if (task == null)
            {
                return NotFound("Task not found");
            }
            return Ok(task);
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var validationResult = await _createTaskValidator.ValidateAsync(createTaskDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = GetCurrentUserId();
            var task = await _taskService.CreateAsync(createTaskDto, userId);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskItemDto>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var validationResult = await _updateTaskValidator.ValidateAsync(updateTaskDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = GetCurrentUserId();
            var task = await _taskService.UpdateAsync(id, updateTaskDto, userId);
            if (task == null)
            {
                return NotFound("Task not found");
            }
            return Ok(task);
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _taskService.DeleteAsync(id, userId);
            if (!success)
            {
                return NotFound("Task not found");
            }
            return NoContent();
        }

        /// <summary>
        /// Complete a task
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<ActionResult<TaskItemDto>> CompleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.CompleteTaskAsync(id, userId);
            if (task == null)
            {
                return NotFound("Task not found");
            }
            return Ok(task);
        }

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetOverdueTasks()
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetOverdueTasksAsync(userId);
            return Ok(tasks);
        }

        /// <summary>
        /// Get tasks by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksByStatus(TaskStatus status)
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetTasksByStatusAsync(status, userId);
            return Ok(tasks);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}
