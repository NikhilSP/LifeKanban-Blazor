using LifeKanbanApi.DTO;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace LifeKanbanApi.Controller
{
    [ApiController]
    [Route("")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectsController(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        // Project endpoints
        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken = default)
        {
            var projects = await _projectRepository.GetProjects(cancellationToken);
            var projectDtos = projects.Adapt<List<ProjectDto>>();
            var response = new { Projects = projectDtos };
            return Ok(response);
        }

        [HttpGet("project/{id}")]
        public async Task<IActionResult> GetProject(Guid id, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetProject(id, cancellationToken);
            
            if (project == null)
                return NotFound();
                
            var projectDto = project.Adapt<ProjectDto>();
            var response = new { Project = projectDto };
            return Ok(response);
        }

        [HttpPost("addProject")]
        public async Task<IActionResult> AddProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken = default)
        {
            var maxPosition = await _projectRepository.GetMaxProjectPosition(cancellationToken);
            
            // Map DTO to domain model
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Project.Name,
                Description = request.Project.Description,
                Position = maxPosition + 10,
                Tasks = new List<ProjectTask>(),
                Milestones = new List<Milestone>()
            };

            var result = await _projectRepository.AddProject(project, cancellationToken);

            if (result == null)
                return BadRequest("Failed to create project");

            return Ok(new { Id = result.Value });
        }

        [HttpPost("updateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectRequest request, CancellationToken cancellationToken = default)
        {
            // Map DTO to domain model
            var project = new Project
            {
                Id = request.Project.Id,
                Name = request.Project.Name,
                Description = request.Project.Description,
                Position = request.Project.Position
            };

            var result = await _projectRepository.UpdateProject(project, cancellationToken);

            if (result == null)
                return BadRequest("Failed to update project");

            return Ok(new { Id = result.Value });
        }

        [HttpDelete("deleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken = default)
        {
            var isSuccess = await _projectRepository.DeleteProject(id, cancellationToken);

            if (!isSuccess)
                return BadRequest($"Project with Id {id} was not deleted");

            return Ok(new { IsSuccess = isSuccess });
        }

        [HttpPost("updateProjectPosition")]
        public async Task<IActionResult> UpdateProjectPosition([FromBody] UpdateProjectPositionRequest request, CancellationToken cancellationToken = default)
        {
            var success = await _projectRepository.UpdateProjectPosition(request.ProjectId, request.NewPosition, cancellationToken);
            return Ok(new { Success = success });
        }

        // Task endpoints
        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask([FromBody] AddTaskRequest request, CancellationToken cancellationToken = default)
        {
            // Convert DTO to domain model
            var task = new ProjectTask
            {
                Id = Guid.NewGuid(),
                Title = request.Task.Title,
                Description = request.Task.Description,
                Status = request.Task.Status,
                ProjectId = request.ProjectGuid,
                ColumnPosition = request.Task.ColumnPosition,
                SubTasks = new List<SubTask>()
            };

            // Handle milestone if present
            if (request.Task.Milestone != null)
            {
                task.MilestoneId = request.Task.Milestone.Id;
            }

            // Add subtasks if there are any
            if (request.Task.SubTasks != null && request.Task.SubTasks.Any())
            {
                int position = 10;
                foreach (var subtaskDto in request.Task.SubTasks)
                {
                    task.SubTasks.Add(new SubTask
                    {
                        Id = Guid.NewGuid(),
                        Title = subtaskDto.Title,
                        IsCompleted = subtaskDto.IsCompleted,
                        ProjectTaskId = task.Id,
                        Position = subtaskDto.Position > 0 ? subtaskDto.Position : position
                    });
                    position += 10;
                }
            }

            var result = await _projectRepository.AddTask(task, request.ProjectGuid, cancellationToken);

            if (!result)
                return BadRequest("Task was not created");

            return Ok(new { Id = task.Id });
        }

        [HttpPost("updateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequest request, CancellationToken cancellationToken = default)
        {
            // Convert DTO to domain model
            var task = new ProjectTask
            {
                Id = request.Task.Id,
                Title = request.Task.Title,
                Description = request.Task.Description,
                Status = request.Task.Status,
                ProjectId = request.ProjectGuid,
                ColumnPosition = request.Task.ColumnPosition,
                SubTasks = new List<SubTask>()
            };

            // Handle milestone if present
            if (request.Task.Milestone != null)
            {
                task.MilestoneId = request.Task.Milestone.Id;
            }

            // Get the current task to compare subtasks
            var existingTask = await _projectRepository.GetTaskById(task.Id, cancellationToken);
            
            if (existingTask == null)
                return NotFound($"Task with ID {task.Id} not found");
            
            // Handle subtasks
            if (request.Task.SubTasks != null)
            {
                // Remove deleted subtasks
                foreach (var existingSubtask in existingTask.SubTasks.ToList())
                {
                    if (request.Task.SubTasks.All(s => s.Id != existingSubtask.Id))
                    {
                        await _projectRepository.DeleteSubTask(existingSubtask.Id, cancellationToken);
                    }
                }
                
                // Add or update subtasks
                foreach (var subtaskDto in request.Task.SubTasks)
                {
                    var existingSubtask = existingTask.SubTasks.FirstOrDefault(s => s.Id == subtaskDto.Id);
            
                    if (existingSubtask == null)
                    {
                        // New subtask
                        var subtask = new SubTask
                        {
                            Id = subtaskDto.Id != Guid.Empty ? subtaskDto.Id : Guid.NewGuid(),
                            Title = subtaskDto.Title,
                            IsCompleted = subtaskDto.IsCompleted,
                            ProjectTaskId = task.Id,
                            Position = subtaskDto.Position
                        };
                        
                        // Calculate position if not provided
                        if (subtask.Position <= 0)
                        {
                            var maxPosition = existingTask.SubTasks.Any() 
                                ? existingTask.SubTasks.Max(s => s.Position) 
                                : 0;
                            subtask.Position = maxPosition + 10;
                        }
                
                        await _projectRepository.AddSubTask(subtask, task.Id, cancellationToken);
                    }
                    else
                    {
                        // Update existing subtask
                        existingSubtask.Title = subtaskDto.Title;
                        existingSubtask.IsCompleted = subtaskDto.IsCompleted;
                        existingSubtask.Position = subtaskDto.Position;
                        await _projectRepository.UpdateSubTask(existingSubtask, cancellationToken);
                    }
                }
            }
            
            var result = await _projectRepository.UpdateTask(task, cancellationToken);

            if (!result)
                return BadRequest("Task was not updated");

            return Ok(new { Id = task.Id });
        }

        [HttpDelete("deleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken = default)
        {
            var isSuccess = await _projectRepository.DeleteTask(id, cancellationToken);

            if (!isSuccess)
                return BadRequest($"Task with Id {id} was not deleted");

            return Ok(new { IsSuccess = isSuccess });
        }

        // Milestone endpoints
        [HttpPost("addMilestone")]
        public async Task<IActionResult> AddMilestone([FromBody] AddMilestoneRequest request, CancellationToken cancellationToken = default)
        {
            // Convert DTO to domain model
            var milestone = new Milestone
            {
                Id = Guid.NewGuid(),
                Name = request.Milestone.Name,
                ProjectId = request.ProjectGuid,
                Tasks = new List<ProjectTask>()
            };

            var result = await _projectRepository.AddMilestone(milestone, request.ProjectGuid, cancellationToken);
            
            if (!result)
                return BadRequest("Milestone was not created");

            return Ok(new { Id = milestone.Id });
        }

        [HttpPost("updateMilestone")]
        public async Task<IActionResult> UpdateMilestone([FromBody] UpdateMilestoneRequest request, CancellationToken cancellationToken = default)
        {
            // Convert DTO to domain model
            var milestone = new Milestone
            {
                Id = request.Milestone.Id,
                Name = request.Milestone.Name,
                ProjectId = request.ProjectGuid
            };
            
            var result = await _projectRepository.UpdateMilestone(milestone, cancellationToken);

            if (!result)
                return BadRequest("Milestone was not updated");

            return Ok(new { Id = milestone.Id });
        }

        [HttpDelete("deleteMilestone/{id}")]
        public async Task<IActionResult> DeleteMilestone(Guid id, CancellationToken cancellationToken = default)
        {
            var isSuccess = await _projectRepository.DeleteMilestone(id, cancellationToken: cancellationToken);

            if (!isSuccess)
                return BadRequest($"Milestone with Id {id} was not deleted");

            return Ok(new { IsSuccess = isSuccess });
        }

        // QuickTodo endpoints
        [HttpGet("quickTodos")]
        public async Task<IActionResult> GetQuickTodos(CancellationToken cancellationToken = default)
        {
            var quickTodos = await _projectRepository.GetQuickTodos(cancellationToken);
            var quickTodoDtos = quickTodos.Adapt<List<QuickTodoDto>>();
            var response = new { QuickTodos = quickTodoDtos };
            return Ok(response);
        }

        [HttpPost("addQuickTodo")]
        public async Task<IActionResult> AddQuickTodo([FromBody] AddQuickTodoRequest request, CancellationToken cancellationToken = default)
        {
            // Convert DTO to domain model
            var todo = new QuickTodo
            {
                Id = Guid.NewGuid(),
                Title = request.QuickTodo.Title,
                IsCompleted = request.QuickTodo.IsCompleted,
                DateCreated = request.QuickTodo.DateCreated,
                DateCompleted = request.QuickTodo.DateCompleted
            };

            var result = await _projectRepository.AddQuickTodo(todo, cancellationToken);
            
            if (result == null)
                return BadRequest("Quick todo was not created");
            
            return Ok(new { Id = todo.Id });
        }

        [HttpPost("updateQuickTodo")]
        public async Task<IActionResult> UpdateQuickTodo([FromBody] UpdateQuickTodoRequest request, CancellationToken cancellationToken = default)
        {
            // Convert DTO to domain model
            var todo = new QuickTodo
            {
                Id = request.QuickTodo.Id,
                Title = request.QuickTodo.Title,
                IsCompleted = request.QuickTodo.IsCompleted,
                DateCreated = request.QuickTodo.DateCreated,
                DateCompleted = request.QuickTodo.DateCompleted
            };
            
            var result = await _projectRepository.UpdateQuickTodo(todo, cancellationToken);
            
            if (!result)
                return BadRequest($"Quick todo with Id {todo.Id} was not updated");
            
            return Ok(new { IsSuccess = true });
        }

        [HttpDelete("deleteQuickTodo/{id}")]
        public async Task<IActionResult> DeleteQuickTodo(Guid id, CancellationToken cancellationToken = default)
        {
            var isSuccess = await _projectRepository.DeleteQuickTodo(id, cancellationToken);
            
            if (!isSuccess)
                return BadRequest($"Quick todo with Id {id} was not deleted");
            
            return Ok(new { IsSuccess = isSuccess });
        }
    }

    // Request record classes using DTOs
    public record CreateProjectRequest(ProjectDto Project);
    public record UpdateProjectRequest(ProjectDto Project);
    public record AddTaskRequest(ProjectTaskDto Task, Guid ProjectGuid);
    public record UpdateTaskRequest(ProjectTaskDto Task, Guid ProjectGuid);
    public record AddMilestoneRequest(MilestoneDto Milestone, Guid ProjectGuid);
    public record UpdateMilestoneRequest(MilestoneDto Milestone, Guid ProjectGuid);
    public record AddQuickTodoRequest(QuickTodoDto QuickTodo);
    public record UpdateQuickTodoRequest(QuickTodoDto QuickTodo);
    public record UpdateProjectPositionRequest(Guid ProjectId, int NewPosition);
}