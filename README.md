# Task Manager API

A simple .NET 8 REST API for managing tasks — built to demonstrate **AI-powered development** with GitHub Copilot and GitHub's AI features.

## Features

- Full CRUD for tasks (create, read, update, delete)
- Priority levels (Low, Medium, High)
- Due date tracking
- In-memory storage (no database setup required)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/tasks` | List all tasks |
| GET | `/tasks/{id}` | Get a task by ID |
| POST | `/tasks` | Create a new task |
| PUT | `/tasks/{id}` | Update an existing task |
| DELETE | `/tasks/{id}` | Delete a task |

### Example: Create a task

```http
POST /tasks
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
PUT /tasks/{id}
Content-Type: application/json

{
  "title": "Review pull request",
  "isCompleted": true,
  "priority": 3
}
```

## Getting Started

```bash
# Restore and run
dotnet run --project src/TaskManager.Api

# Run tests
dotnet test
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

## Project Structure

```
src/
  TaskManager.Api/
    Models/         # TaskItem model and Priority enum
    Data/           # In-memory TaskRepository
    Endpoints/      # Minimal API endpoint definitions
    Program.cs      # App bootstrap
tests/
  TaskManager.Api.Tests/
    TaskRepositoryTests.cs
.github/
  workflows/ci.yml          # GitHub Actions CI (build + test)
  dependabot.yml            # Automated dependency updates
  ISSUE_TEMPLATE/           # Bug report and feature request templates
```

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
