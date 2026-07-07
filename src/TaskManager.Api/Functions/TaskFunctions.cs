using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Functions;

public class TaskFunctions
{
    private readonly TaskRepository _repo;

    public TaskFunctions(TaskRepository repo)
    {
        _repo = repo;
    }

    [Function("GetAllTasks")]
    public IActionResult GetAllTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks")] HttpRequest req)
    {
        return new OkObjectResult(_repo.GetAll());
    }

    [Function("GetTaskById")]
    public IActionResult GetTaskById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks/{id:guid}")] HttpRequest req,
        Guid id)
    {
        var task = _repo.GetById(id);
        return task is not null ? new OkObjectResult(task) : new NotFoundResult();
    }

    [Function("CreateTask")]
    public async Task<IActionResult> CreateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")] HttpRequest req)
    {
        var task = await req.ReadFromJsonAsync<TaskItem>();

        if (task is null || string.IsNullOrWhiteSpace(task.Title))
            return new BadRequestObjectResult(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    { "title", new[] { "Title is required." } }
                }));

        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;

        var created = _repo.Add(task);
        return new CreatedResult($"/api/tasks/{created.Id}", created);
    }

    [Function("UpdateTask")]
    public async Task<IActionResult> UpdateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "tasks/{id:guid}")] HttpRequest req,
        Guid id)
    {
        var updated = await req.ReadFromJsonAsync<TaskItem>();

        if (updated is null || string.IsNullOrWhiteSpace(updated.Title))
            return new BadRequestObjectResult(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    { "title", new[] { "Title is required." } }
                }));

        var result = _repo.Update(id, updated);
        return result is not null ? new OkObjectResult(result) : new NotFoundResult();
    }

    [Function("DeleteTask")]
    public IActionResult DeleteTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "tasks/{id:guid}")] HttpRequest req,
        Guid id)
    {
        var deleted = _repo.Delete(id);
        return deleted ? new NoContentResult() : new NotFoundResult();
    }
}
