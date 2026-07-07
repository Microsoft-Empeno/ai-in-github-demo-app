# Task Manager API

A simple .NET 10 REST API for managing tasks — built to demonstrate **AI-powered development** with GitHub Copilot and GitHub's AI features.

The API runs on **Azure Functions** (isolated worker model) and the front end is a static
site designed for **Azure Static Web Apps**, with **Application Insights** wired up for
observability.

## Features

- Full CRUD for tasks (create, read, update, delete)
- Priority levels (Low, Medium, High)
- Due date tracking
- In-memory storage (no database setup required)
- Static front end (Tasks dashboard + GitHub AI features page)

## Architecture

```
Browser ──> Azure Static Web App (frontend/)  ──/api/*──>  Azure Functions (src/TaskManager.Api)
                                                                  │
                                                          Application Insights
```

- **Front end** (`frontend/`): static HTML/CSS/JS, hosted on Azure Static Web Apps.
- **API** (`src/TaskManager.Api`): Azure Functions, isolated worker, HTTP triggers.
  Uses the ASP.NET Core integration so handlers return `IActionResult`.
- Requests to `/api/*` are routed to the Functions backend (the `api` route prefix is
  configured in `host.json`).

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks` | List all tasks |
| GET | `/api/tasks/{id}` | Get a task by ID |
| POST | `/api/tasks` | Create a new task |
| PUT | `/api/tasks/{id}` | Update an existing task |
| DELETE | `/api/tasks/{id}` | Delete a task |

### Example: Create a task

```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Review pull request",
  "description": "Review the feature/add-filtering branch",
  "priority": 3,
  "dueDate": "2026-07-10T00:00:00Z"
}
```

### Example: Update a task

```http
PUT /api/tasks/{id}
Content-Type: application/json

{
  "title": "Review pull request",
  "isCompleted": true,
  "priority": 3
}
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- [Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local) (to run the API locally)
- [Azure Static Web Apps CLI](https://azure.github.io/static-web-apps-cli/) (optional, to run front end + API together)

### Run the API

```bash
# Start the Functions host (serves /api/tasks)
func start --script-root src/TaskManager.Api

# Run tests
dotnet test
```

### Run the front end + API together

```bash
swa start frontend --api-location src/TaskManager.Api
```

The SWA CLI serves the static front end and proxies `/api/*` to the Functions host.

## Project Structure

```
frontend/                     # Static front end (Azure Static Web Apps)
  index.html                  # Tasks dashboard
  ai-features.html            # GitHub AI features overview
  css/ , js/                  # Styles and client logic
  staticwebapp.config.json    # SWA routing / API runtime config
src/
  TaskManager.Api/
    Models/                   # TaskItem model and Priority enum
    Data/                     # In-memory TaskRepository
    Functions/                # HTTP-triggered Azure Functions
    Program.cs                # Isolated worker host + DI + App Insights
    host.json                 # Functions host config (route prefix, App Insights)
tests/
  TaskManager.Api.Tests/
    TaskRepositoryTests.cs
.github/
  copilot-instructions.md     # Repo-wide Copilot custom instructions
  workflows/ci.yml            # GitHub Actions CI (build + test)
  workflows/azure-static-web-apps.yml  # Deploy front end + API to Azure SWA
  dependabot.yml              # Automated dependency updates
  ISSUE_TEMPLATE/             # Bug report and feature request templates
```

## Deployment (Azure Static Web Apps)

The front end and the Azure Functions API deploy together to **Azure Static Web Apps**
via GitHub Actions ([.github/workflows/azure-static-web-apps.yml](.github/workflows/azure-static-web-apps.yml)).

1. Create a **Static Web App** resource in Azure (Free tier is fine for the demo),
   choosing this GitHub repository as the source.
2. In the resource, set **App location** to `frontend` and **Api location** to
   `src/TaskManager.Api`.
3. Azure adds an `AZURE_STATIC_WEB_APPS_API_TOKEN` secret to the repository; the
   workflow uses it to publish on every push to `main` and to create preview
   environments for pull requests.

Routing is controlled by [frontend/staticwebapp.config.json](frontend/staticwebapp.config.json):
calls to `/api/*` are served by the Functions backend, and unmatched routes fall back
to `index.html`.

## GitHub AI Features Demonstrated

This repository is used to demonstrate the following GitHub AI capabilities:

- **GitHub Copilot** — Inline code completions as you type
- **Copilot Chat** — Ask questions about the codebase, get explanations, request refactors
- **Copilot Edits / Agent mode** — Make changes across multiple files at once
- **GitHub Actions** — CI pipeline generated and improved with Copilot assistance
- **Copilot Code Review** — Automated PR review with AI-suggested improvements
- **Dependabot** — Automated dependency update pull requests
- **Secret scanning & code scanning** — Security features built into GitHub

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Commit your changes and open a pull request
4. GitHub Copilot will automatically review your PR
