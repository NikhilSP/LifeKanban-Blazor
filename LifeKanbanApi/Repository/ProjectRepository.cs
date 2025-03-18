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

    public async Task<List<ProjectTask>> GetTasksByProject(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Tasks
            .Where(t => t.Project.Id == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectTask?> GetTaskById(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Tasks
            .Include(t => t.Milestone)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    public async Task<bool> UpdateTask(ProjectTask task, CancellationToken cancellationToken = default)
    {
        try
        {
            task.Milestone = null;
            projectDbContext.Tasks.Update(task);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
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

    public async Task<List<Milestone>> GetMilestonesByProject(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Milestones
            .Where(m => m.Project.Id == projectId)
            .ToListAsync(cancellationToken);
    }

    // Get tasks grouped by status (for Kanban board columns)
    public async Task<Dictionary<string, List<ProjectTask>>> GetTasksByStatus(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var tasks = await projectDbContext.Tasks
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Milestone)
            .ToListAsync(cancellationToken);

        return tasks.GroupBy(t => t.Status)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

// Update task status (moving between Kanban columns)
    public async Task<bool> UpdateTaskStatus(Guid taskId, string newStatus,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskById(taskId, cancellationToken);
        if (task != null)
        {
            task.Status = newStatus;
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

// Assign task to milestone
    public async Task<bool> AssignTaskToMilestone(Guid taskId, Guid milestoneId,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskById(taskId, cancellationToken);
        var milestone = await GetMilestoneById(milestoneId, cancellationToken);

        if (task != null && milestone != null)
        {
            task.MilestoneId = milestoneId;
            task.Milestone = milestone;
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

// Remove task from milestone
    public async Task<bool> RemoveTaskFromMilestone(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await GetTaskById(taskId, cancellationToken);

        if (task != null)
        {
            task.MilestoneId = null;
            task.Milestone = null;
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    // Get a specific milestone by ID
    public async Task<Milestone?> GetMilestoneById(Guid milestoneId, CancellationToken cancellationToken = default)
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

// Get tasks by milestone
    public async Task<List<ProjectTask>> GetTasksByMilestone(Guid milestoneId,
        CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Tasks
            .Where(t => t.MilestoneId == milestoneId)
            .ToListAsync(cancellationToken);
    }

    // Expand your current GetProject method to include all related data
    public async Task<Project?> GetProjectWithAllDetails(Guid id, CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Projects
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Milestone)
            .Include(p => p.Milestones)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }


// Get project statistics
    public async Task<ProjectStatistics> GetProjectStatistics(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await GetProjectWithAllDetails(projectId, cancellationToken);

        if (project == null)
            return new ProjectStatistics();

        return new ProjectStatistics
        {
            TotalTasks = project.Tasks.Count,
            CompletedTasks = project.Tasks.Count(t => t.Status == "Done" || t.Status == "Completed"),
            TasksByStatus = project.Tasks.GroupBy(t => t.Status)
                .ToDictionary(g => g.Key, g => g.Count()),
            MilestoneCount = project.Milestones.Count,
            TasksWithNoMilestone = project.Tasks.Count(t => t.MilestoneId == null)
        };
    }

    // Get tasks that are not assigned to any milestone
    public async Task<List<ProjectTask>> GetTasksWithoutMilestone(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Tasks
            .Where(t => t.ProjectId == projectId && t.MilestoneId == null)
            .ToListAsync(cancellationToken);
    }

// Copy a task (create a duplicate)
    public async Task<Guid?> DuplicateTask(Guid taskId, CancellationToken cancellationToken = default)
    {
        var original = await GetTaskById(taskId, cancellationToken);

        if (original != null)
        {
            var duplicate = new ProjectTask
            {
                Id = Guid.NewGuid(),
                Title = $"{original.Title} (Copy)",
                Description = original.Description,
                Status = original.Status,
                ProjectId = original.ProjectId,
                MilestoneId = original.MilestoneId
            };

            projectDbContext.Tasks.Add(duplicate);
            await projectDbContext.SaveChangesAsync(cancellationToken);
            return duplicate.Id;
        }

        return null;
    }

// Search for tasks by title or description
    public async Task<List<ProjectTask>> SearchTasks(Guid projectId, string searchTerm,
        CancellationToken cancellationToken = default)
    {
        return await projectDbContext.Tasks
            .Where(t => t.ProjectId == projectId &&
                        (t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm)))
            .Include(t => t.Milestone)
            .ToListAsync(cancellationToken);
    }
}