using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Tests;

public class TaskRepositoryTests
{
    private TaskRepository CreateRepository() => new TaskRepository();

    [Fact]
    public void GetAll_ReturnsSeededTasks()
    {
        var repo = CreateRepository();
        var tasks = repo.GetAll();
        Assert.NotEmpty(tasks);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsTask()
    {
        var repo = CreateRepository();
        var existing = repo.GetAll().First();
        var result = repo.GetById(existing.Id);
        Assert.NotNull(result);
        Assert.Equal(existing.Id, result.Id);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        var repo = CreateRepository();
        var result = repo.GetById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void Add_NewTask_IncreasesCount()
    {
        var repo = CreateRepository();
        var before = repo.GetAll().Count();

        repo.Add(new TaskItem { Title = "Test task" });

        Assert.Equal(before + 1, repo.GetAll().Count());
    }

    [Fact]
    public void Add_NewTask_ReturnsAddedTask()
    {
        var repo = CreateRepository();
        var task = new TaskItem { Title = "Demo task", Priority = Priority.High };

        var result = repo.Add(task);

        Assert.Equal("Demo task", result.Title);
        Assert.Equal(Priority.High, result.Priority);
    }

    [Fact]
    public void Update_ExistingTask_ReturnsUpdatedTask()
    {
        var repo = CreateRepository();
        var existing = repo.GetAll().First();
        var updated = new TaskItem
        {
            Title = "Updated title",
            IsCompleted = true,
            Priority = Priority.Low
        };

        var result = repo.Update(existing.Id, updated);

        Assert.NotNull(result);
        Assert.Equal("Updated title", result.Title);
        Assert.True(result.IsCompleted);
        Assert.Equal(Priority.Low, result.Priority);
    }

    [Fact]
    public void Update_UnknownId_ReturnsNull()
    {
        var repo = CreateRepository();
        var result = repo.Update(Guid.NewGuid(), new TaskItem { Title = "X" });
        Assert.Null(result);
    }

    [Fact]
    public void Delete_ExistingTask_ReturnsTrueAndRemovesTask()
    {
        var repo = CreateRepository();
        var existing = repo.GetAll().First();
        var before = repo.GetAll().Count();

        var deleted = repo.Delete(existing.Id);

        Assert.True(deleted);
        Assert.Equal(before - 1, repo.GetAll().Count());
        Assert.Null(repo.GetById(existing.Id));
    }

    [Fact]
    public void Delete_UnknownId_ReturnsFalse()
    {
        var repo = CreateRepository();
        var result = repo.Delete(Guid.NewGuid());
        Assert.False(result);
    }
}
