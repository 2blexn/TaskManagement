using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum TaskStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    public class TaskItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        // Foreign key
        public int UserId { get; set; }
        
        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
