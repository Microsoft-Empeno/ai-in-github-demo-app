# Copilot Instructions — Task Manager API

These instructions apply to the whole repository. GitHub Copilot (Chat, Edits, and
the Coding Agent) automatically follows them when generating or reviewing code here.

## Project overview

- **What it is:** A small REST API for managing tasks, used to demonstrate GitHub's
  AI features (Copilot, Copilot Code Review, Coding Agent, Advanced Security).
- **Stack:** .NET 10, C#, **Azure Functions (isolated worker model)** with HTTP triggers.
  Uses the ASP.NET Core integration so handlers return `IActionResult`.
- **Hosting:** Designed for **Azure Static Web Apps** (front end) + **Azure Functions**
  (API backend). Application Insights is wired up for observability.
- **Storage:** In-memory only (`TaskRepository` registered as a singleton). No database.
- **Layout:**
  - `src/TaskManager.Api/Models/` — domain models (`TaskItem`, `Priority` enum)
  - `src/TaskManager.Api/Data/` — `TaskRepository` (in-memory store, seeded data)
  - `src/TaskManager.Api/Functions/` — HTTP-triggered Azure Functions
  - `src/TaskManager.Api/Program.cs` — isolated worker host bootstrap / DI registration
  - `src/TaskManager.Api/host.json` — Functions host config (route prefix `api`, App Insights)
  - `frontend/` — static front end (HTML/CSS/JS) served by Azure Static Web Apps
  - `tests/TaskManager.Api.Tests/` — xUnit tests

## Coding conventions

- Target **.NET 10** and modern C#. Enable and respect nullable reference types.
- Use **file-scoped namespaces** (e.g. `namespace TaskManager.Api.Models;`).
- Use the **Azure Functions isolated worker** model. Define each endpoint as a method
  decorated with `[Function("Name")]` and an `[HttpTrigger(...)]` binding
  (see `TaskFunctions`). Do not use ASP.NET Core Minimal APIs or MVC controllers.
- Inject dependencies (e.g. `TaskRepository`) via the function class constructor.
- Handlers return `IActionResult` (`OkObjectResult`, `CreatedResult`, `NotFoundResult`,
  `NoContentResult`, `BadRequestObjectResult`).
- Define routes on the trigger with `Route = "tasks"` / `Route = "tasks/{id:guid}"`.
  The `api` prefix is added by the host (`host.json` `routePrefix`), so the public path
  is `/api/tasks`.
- Read request bodies with `await req.ReadFromJsonAsync<T>()`.
- Models use auto-properties. Use `required` for mandatory members (e.g. `Title`)
  and nullable types (`string?`, `DateTime?`) for optional ones.
- Use `DateTime.UtcNow` for timestamps — never local time.
- Use `Guid` for identifiers; generate new IDs server-side, not from client input.
- Keep dependency injection registrations in `Program.cs`.

## Validation & behavior

- Validate required fields inside the function. For a missing/blank `Title`, return
  `BadRequestObjectResult` with a `ValidationProblemDetails` containing a `title` error
  entry (see existing `CreateTask` / `UpdateTask`).
- On create: assign `Id = Guid.NewGuid()` and `CreatedAt = DateTime.UtcNow` on the
  server; return `CreatedResult` with a `/api/tasks/{id}` location.
- On update: return the updated entity or `NotFoundResult` if the id is unknown. Do not
  allow `CreatedAt` to be overwritten by the client.
- On delete: return `NoContentResult` on success, `NotFoundResult` if not found.

## Testing conventions

- Use **xUnit** (`[Fact]`). Follow the existing naming pattern:
  `Method_Scenario_ExpectedResult` (e.g. `GetById_UnknownId_ReturnsNull`).
- Cover both positive and negative cases (found vs. not found, valid vs. invalid).
- Construct a fresh `TaskRepository` per test; do not share state between tests.
- Use `Assert` (not FluentAssertions unless it's added as a dependency).
- When you add or change a function or repository method, add matching tests.

## Source control workflow

- The `main` branch is **protected**: never commit or push directly to `main`.
- Make all changes on a **feature branch** (e.g. `feature/short-description`) created
  off the latest `main`.
- Finish work by opening a **pull request** targeting `main`; do not merge locally.
- Keep pull requests focused and let Copilot Code Review run; resolve review
  conversations before merging.
- Prefer **squash and merge**; the feature branch is deleted after merge.

## What to avoid

- Don't add a database, ORM, or external persistence unless explicitly asked.
- Don't switch back to Minimal APIs or MVC controllers — this project uses Azure Functions.
- Don't introduce new NuGet packages without calling it out.
- Don't log or expose secrets. Keep real values out of `local.settings.json`
  (it is not published) and never commit connection strings.

## Handy commands

```bash
# Run the Functions API locally (requires Azure Functions Core Tools)
func start --script-root src/TaskManager.Api

# Build and test
dotnet build --configuration Release
dotnet test

# Serve the front end + API together locally (requires the SWA CLI)
swa start frontend --api-location src/TaskManager.Api
```
