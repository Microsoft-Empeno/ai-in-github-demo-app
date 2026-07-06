using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/tasks")
            .WithTags("Tasks");

        group.MapGet("/", GetAllTasks);
        group.MapGet("/{id:guid}", GetTaskById);
        group.MapPost("/", CreateTask);
        group.MapPut("/{id:guid}", UpdateTask);
        group.MapDelete("/{id:guid}", DeleteTask);
    }

    private static IResult GetAllTasks(TaskRepository repo)
    {
        return Results.Ok(repo.GetAll());
    }

    private static IResult GetTaskById(Guid id, TaskRepository repo)
    {
        var task = repo.GetById(id);
        return task is not null ? Results.Ok(task) : Results.NotFound();
    }

    private static IResult CreateTask(TaskItem task, TaskRepository repo)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "title", new[] { "Title is required." } }
            });

        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;

        var created = repo.Add(task);
        return Results.Created($"/tasks/{created.Id}", created);
    }

    private static IResult UpdateTask(Guid id, TaskItem updated, TaskRepository repo)
    {
        if (string.IsNullOrWhiteSpace(updated.Title))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "title", new[] { "Title is required." } }
            });

        var result = repo.Update(id, updated);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static IResult DeleteTask(Guid id, TaskRepository repo)
    {
        var deleted = repo.Delete(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
