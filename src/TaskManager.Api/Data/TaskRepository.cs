using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

public class TaskRepository
{
    private readonly List<TaskItem> _tasks = new()
    {
        new TaskItem
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
            Title = "Set up CI/CD pipeline",
            Description = "Configure GitHub Actions workflows for build, test, and deploy.",
            Priority = Priority.High,
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            DueDate = DateTime.UtcNow.AddDays(-5)
        },
        new TaskItem
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000002"),
            Title = "Write unit tests for task endpoints",
            Description = "Cover all CRUD operations with positive and negative test cases.",
            Priority = Priority.High,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            DueDate = DateTime.UtcNow.AddDays(1)
        },
        new TaskItem
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000003"),
            Title = "Add pagination to task list endpoint",
            Description = "Support page and pageSize query parameters.",
            Priority = Priority.Medium,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            DueDate = DateTime.UtcNow.AddDays(7)
        },
        new TaskItem
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000004"),
            Title = "Update README with API documentation",
            Description = null,
            Priority = Priority.Low,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = null
        }
    };

    public IEnumerable<TaskItem> GetAll() => _tasks.AsReadOnly();

    public TaskItem? GetById(Guid id) =>
        _tasks.FirstOrDefault(t => t.Id == id);

    public TaskItem Add(TaskItem task)
    {
        _tasks.Add(task);
        return task;
    }

    public TaskItem? Update(Guid id, TaskItem updated)
    {
        var existing = GetById(id);
        if (existing is null) return null;

        existing.Title = updated.Title;
        existing.Description = updated.Description;
        existing.IsCompleted = updated.IsCompleted;
        existing.Priority = updated.Priority;
        existing.DueDate = updated.DueDate;

        return existing;
    }

    public bool Delete(Guid id)
    {
        var task = GetById(id);
        if (task is null) return false;

        _tasks.Remove(task);
        return true;
    }
}
