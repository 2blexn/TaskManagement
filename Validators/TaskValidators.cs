using FluentValidation;
using TaskManagement.DTOs;

namespace TaskManagement.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future")
                .When(x => x.DueDate.HasValue);
        }
    }

    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value")
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future")
                .When(x => x.DueDate.HasValue);
        }
    }

    public class TaskFilterDtoValidator : AbstractValidator<TaskFilterDto>
    {
        public TaskFilterDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value")
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

            RuleFor(x => x.DueDateFrom)
                .LessThan(x => x.DueDateTo).WithMessage("Due date from must be less than due date to")
                .When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue);
        }
    }
}
