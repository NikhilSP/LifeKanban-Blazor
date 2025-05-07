using LifeKanbanApi.Data;
using LifeKanbanApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LifeKanbanApi.Repository;

public class ProjectRepository(ProjectDbContext projectDbContext) : IProjectRepository
{
    public async Task<List<Project>> GetProjects(CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Projects.ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetProject(Guid id, CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Projects
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Milestone)
            .Include(p => p.Tasks)
            .ThenInclude(t => t.SubTasks)
            .Include(p => p.Milestones)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Guid?> AddProject(Project project, CancellationToken cancellationToken = default)
    {
        try
        {
            projectDbContext.Add(project);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return project.Id;
        }
        catch
        {
            return null;
        }
    }
    
    public async Task InitializeProjectPositions(CancellationToken cancellationToken = default)
    {
        var projects = await projectDbContext.Projects.ToListAsync(cancellationToken);
    
        // Check if we need to initialize positions
        bool needsUpdate = projects.Any(p => p.Position == 0);
    
        if (needsUpdate)
        {
            // Sort projects by Id or another property to maintain some order
            var orderedProjects = projects.OrderBy(p => p.Id).ToList();
        
            // Assign incremental positions
            for (int i = 0; i < orderedProjects.Count; i++)
            {
                orderedProjects[i].Position = (i + 1) * 10; // Use spacing for easier reordering later
            }
        
            await projectDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetMaxProjectPosition(CancellationToken cancellationToken = default)
    {
        if (!await projectDbContext.Projects.AnyAsync(cancellationToken))
            return 0;
        
        return await projectDbContext.Projects
            .MaxAsync(p => p.Position, cancellationToken);
    }
    
    public async Task<bool> AddTask(ProjectTask projectTask, Guid projectGuid, CancellationToken cancellationToken = default)
    {
        var project = await GetProject(projectGuid, cancellationToken);

        if (project is not null)
        {
            project.Tasks.Add(projectTask);
            
            projectTask.Milestone = null;
            
            projectDbContext.Tasks.Add(projectTask);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
    
    public async Task<bool> UpdateProjectPosition(Guid projectId, int newPosition, CancellationToken cancellationToken = default)
    {
        var project = await GetProject(projectId, cancellationToken);
    
        if (project != null)
        {
            project.Position = newPosition;
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    
        return false;
    }

    public async Task<bool> AddMilestone(Milestone milestone, Guid projectGuid, CancellationToken cancellationToken = default)
    {
        var project = await GetProject(projectGuid, cancellationToken);

        if (project is not null)
        {
            project.Milestones.Add(milestone);
            projectDbContext.Milestones.Add(milestone);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<Guid?> UpdateProject(Project project, CancellationToken cancellationToken = default)
    {
        try
        {
            // Only update the project properties, not its relationships
            var entry = projectDbContext.Entry(project);
            entry.State = EntityState.Modified;

            // Don't modify relationships through this method
            entry.Collection(p => p.Tasks).IsModified = false;
            entry.Collection(p => p.Milestones).IsModified = false;

            await projectDbContext.SaveChangesAsync(cancellationToken);
            return project.Id;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteProject(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await GetProject(id, cancellationToken);

        if (project is not null)
        {
            projectDbContext.Remove(project);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
    
    public async Task<ProjectTask?> GetTaskById(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Tasks
            .Include(t => t.Milestone)
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    public async Task<bool> UpdateTask(ProjectTask updatedTask, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the existing task that's already being tracked
            var existingTask = await projectDbContext.Tasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == updatedTask.Id, cancellationToken);
            
            if (existingTask == null)
            {
                return false;
            }
        
            // Update the properties of the tracked entity
            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.Status = updatedTask.Status;
            existingTask.ColumnPosition = updatedTask.ColumnPosition;
            existingTask.MilestoneId = updatedTask.MilestoneId;
        
            // Don't use Update() on the entity - just save changes to the tracked entity
            projectDbContext.Tasks.Update(existingTask);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteTask(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await GetTaskById(taskId, cancellationToken);
        if (task != null)
        {
            projectDbContext.Tasks.Remove(task);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    // Get a specific milestone by ID
    private async Task<Milestone?> GetMilestoneById(Guid milestoneId, CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Milestones
            .Include(m => m.Tasks)
            .FirstOrDefaultAsync(m => m.Id == milestoneId, cancellationToken);
    }

// Update milestone
    public async Task<bool> UpdateMilestone(Milestone milestone, CancellationToken cancellationToken = default)
    {
        try
        {
            projectDbContext.Milestones.Update(milestone);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

// Delete milestone (with option to reassign or delete tasks)
    public async Task<bool> DeleteMilestone(Guid milestoneId, bool deleteTasks = false,
        CancellationToken cancellationToken = default)
    {
        var milestone = await GetMilestoneById(milestoneId, cancellationToken);

        if (milestone != null)
        {
            if (deleteTasks)
            {
                // Remove associated tasks
                projectDbContext.Tasks.RemoveRange(milestone.Tasks);
            }
            else
            {
                // Just detach tasks from this milestone
                foreach (var task in milestone.Tasks)
                {
                    task.MilestoneId = null;
                    task.Milestone = null;
                }
            }

            projectDbContext.Milestones.Remove(milestone);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }


    
    
    public async Task<bool> AddSubTask(SubTask subtask, Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await GetTaskById(taskId, cancellationToken);
    
        if (task is not null)
        {
            subtask.ProjectTaskId = taskId;
        
            // Set position if not already set
            if (subtask.Position <= 0)
            {
                var maxPosition = task.SubTasks.Any() 
                    ? task.SubTasks.Max(s => s.Position) 
                    : 0;
                subtask.Position = maxPosition + 10;
            }
        
            task.SubTasks.Add(subtask);
            projectDbContext.SubTasks.Add(subtask);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    
        return false;
    }

// Update a subtask
    public async Task<bool> UpdateSubTask(SubTask subtask, CancellationToken cancellationToken = default)
    {
        try
        {
            projectDbContext.SubTasks.Update(subtask);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

// Delete a subtask
    public async Task<bool> DeleteSubTask(Guid subtaskId, CancellationToken cancellationToken = default)
    {
        var subtask = await projectDbContext.SubTasks.FindAsync([subtaskId], cancellationToken);
    
        if (subtask is not null)
        {
            projectDbContext.SubTasks.Remove(subtask);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    
        return false;
    }
    
    // In LifeKanbanApi/Repository/ProjectRepository.cs
    public async Task<List<QuickTodo>> GetQuickTodos(CancellationToken cancellationToken = default)
    {
        return await projectDbContext.QuickTodos
            .OrderBy(t => t.DateCreated)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid?> AddQuickTodo(QuickTodo todo, CancellationToken cancellationToken = default)
    {
        try
        {
            projectDbContext.QuickTodos.Add(todo);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return todo.Id;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateQuickTodo(QuickTodo todo, CancellationToken cancellationToken = default)
    {
        try
        {
            projectDbContext.QuickTodos.Update(todo);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteQuickTodo(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = await projectDbContext.QuickTodos.FindAsync([id], cancellationToken);
        if (todo != null)
        {
            projectDbContext.QuickTodos.Remove(todo);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        return false;
    }
}