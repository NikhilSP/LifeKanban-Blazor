// Services/KanbanService.cs
using LifeKanban.Client;
using LifeKanban.Model;
using LifeKanban.Model.ViewModels;

namespace LifeKanban.Services;

public class KanbanService
{
    private readonly ProjectsClient _client;
    
    public KanbanService(ProjectsClient client)
    {
        _client = client;
    }

    public async Task<(ProjectItem Project, List<KanbanTask> Tasks, List<KanbanMilestone> Milestones)> GetBoardData(Guid projectId)
    {
        var project = await _client.GetProjectById(projectId) ?? new ProjectItem();
        var tasks = new List<KanbanTask>();
        var milestones = new List<KanbanMilestone>();
        
        // Transform project tasks to kanban tasks
        foreach (var task in project.tasks.OrderBy(t => t.columnPosition))
        {
            // Convert string status to enum
            var taskStatus = BoardConfiguration.ToTaskStatus(task.status);
            
            // Get corresponding column
            var columnType = BoardConfiguration.GetColumnForStatus(taskStatus);
            
            tasks.Add(new KanbanTask
            {
                ColumnType = columnType,
                Task = task
            });
        }
        
        // Transform project milestones
        foreach (var milestone in project.milestones)
        {
            milestones.Add(new KanbanMilestone
            {
                Milestone = milestone
            });
        }
        
        return (project, tasks, milestones);
    }
    
    public async Task<bool> UpdateTaskStatus(ProjectTaskItem task, ColumnType newColumnType, Guid projectId, double position)
    {
        // Map column type to task status
        var mappedStatus = BoardConfiguration.GetMappedStatus(newColumnType);
        if (mappedStatus.HasValue)
        {
            task.status = BoardConfiguration.FromTaskStatus(mappedStatus.Value);
            task.columnPosition = position;
            
            var result = await _client.UpdateTask(task, projectId);
            return result.HasValue;
        }
        
        return false;
    }
    
    public async Task<bool> UpdateTaskPosition(ProjectTaskItem task, double newPosition, Guid projectId)
    {
        task.columnPosition = newPosition;
        var result = await _client.UpdateTask(task, projectId);
        return result.HasValue;
    }
    
    // Add more methods as needed for working with the board
}