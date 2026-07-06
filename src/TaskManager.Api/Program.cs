using TaskManager.Api.Data;
using TaskManager.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapTaskEndpoints();

app.Run();
