const API = "/api/tasks";

const PRIORITY = {
  1: { label: "Low", cls: "badge--low" },
  2: { label: "Medium", cls: "badge--medium" },
  3: { label: "High", cls: "badge--high" },
};

const listEl = document.getElementById("task-list");
const formEl = document.getElementById("task-form");
const toastEl = document.getElementById("toast");

let toastTimer;
function toast(message) {
  toastEl.textContent = message;
  toastEl.classList.add("is-visible");
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => toastEl.classList.remove("is-visible"), 2500);
}

function escapeHtml(value) {
  const div = document.createElement("div");
  div.textContent = value ?? "";
  return div.innerHTML;
}

function formatDue(dueDate) {
  if (!dueDate) return "";
  const date = new Date(dueDate);
  return `Due ${date.toLocaleDateString(undefined, {
    month: "short",
    day: "numeric",
  })}`;
}

async function loadTasks() {
  try {
    const res = await fetch(API);
    if (!res.ok) throw new Error(`Request failed (${res.status})`);
    const tasks = await res.json();
    renderTasks(tasks);
  } catch (err) {
    listEl.innerHTML = `<div class="empty">Could not load tasks: ${escapeHtml(err.message)}</div>`;
  }
}

function renderTasks(tasks) {
  if (!tasks.length) {
    listEl.innerHTML = `<div class="empty">No tasks yet — add your first one above.</div>`;
    return;
  }

  listEl.innerHTML = tasks
    .map((task) => {
      const priority = PRIORITY[task.priority] ?? PRIORITY[2];
      const due = formatDue(task.dueDate);
      return `
        <div class="task ${task.isCompleted ? "is-complete" : ""}" data-id="${task.id}">
          <input class="task__check" type="checkbox" ${task.isCompleted ? "checked" : ""}
            aria-label="Toggle complete" />
          <div class="task__main">
            <div class="task__title">${escapeHtml(task.title)}</div>
            ${task.description ? `<div class="task__desc">${escapeHtml(task.description)}</div>` : ""}
          </div>
          <div class="task__meta">
            ${due ? `<span class="task__due">${due}</span>` : ""}
            <span class="badge ${priority.cls}">${priority.label}</span>
            <button class="btn btn--ghost btn--danger" data-action="delete">Delete</button>
          </div>
        </div>`;
    })
    .join("");
}

async function createTask(event) {
  event.preventDefault();
  const data = new FormData(formEl);
  const dueValue = data.get("dueDate");

  const payload = {
    title: data.get("title").trim(),
    description: data.get("description").trim() || null,
    priority: Number(data.get("priority")),
    isCompleted: false,
    dueDate: dueValue ? new Date(dueValue).toISOString() : null,
  };

  try {
    const res = await fetch(API, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
    if (!res.ok) throw new Error(`Create failed (${res.status})`);
    formEl.reset();
    document.getElementById("priority").value = "2";
    toast("Task created");
    await loadTasks();
  } catch (err) {
    toast(err.message);
  }
}

async function deleteTask(id) {
  try {
    const res = await fetch(`${API}/${id}`, { method: "DELETE" });
    if (!res.ok && res.status !== 204) throw new Error(`Delete failed (${res.status})`);
    toast("Task deleted");
    await loadTasks();
  } catch (err) {
    toast(err.message);
  }
}

async function toggleComplete(id, isCompleted) {
  try {
    const res = await fetch(`${API}/${id}`);
    if (!res.ok) throw new Error(`Load failed (${res.status})`);
    const task = await res.json();

    const payload = {
      title: task.title,
      description: task.description,
      priority: task.priority,
      isCompleted,
      dueDate: task.dueDate,
    };

    const update = await fetch(`${API}/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
    if (!update.ok) throw new Error(`Update failed (${update.status})`);
    toast(isCompleted ? "Task completed" : "Task reopened");
    await loadTasks();
  } catch (err) {
    toast(err.message);
    await loadTasks();
  }
}

listEl.addEventListener("click", (event) => {
  const taskEl = event.target.closest(".task");
  if (!taskEl) return;
  const id = taskEl.dataset.id;

  if (event.target.dataset.action === "delete") {
    deleteTask(id);
  }
});

listEl.addEventListener("change", (event) => {
  if (!event.target.classList.contains("task__check")) return;
  const taskEl = event.target.closest(".task");
  toggleComplete(taskEl.dataset.id, event.target.checked);
});

formEl.addEventListener("submit", createTask);

loadTasks();
