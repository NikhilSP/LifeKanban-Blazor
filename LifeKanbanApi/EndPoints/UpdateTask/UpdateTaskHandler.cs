using LifeKanbanApi.CQRS;
using LifeKanbanApi.EndPoints.AddTask;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.UpdateTask;

public record UpdateTaskCommand(ProjectTask Task, Guid ProjectGuid)
    : ICommand<UpdateTaskResult>;

public record UpdateTaskResult(Guid Id);

public class UpdateTaskHandler(ProjectRepository projectRepository)
    : ICommandHandler<UpdateTaskCommand, UpdateTaskResult>
{
    public async Task<UpdateTaskResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        request.Task.ProjectId = request.ProjectGuid;
        request.Task.MilestoneId = request.Task.Milestone?.Id;
        
        // Get the current task from the database to compare subtasks
        var existingTask = await projectRepository.GetTaskById(request.Task.Id, cancellationToken);
        
        if (existingTask == null)
        {
            throw new Exception($"Task with ID {request.Task.Id} not found");
        }
        
        // Manage subtasks
        // Remove deleted subtasks
        foreach (var existingSubtask in existingTask.SubTasks.ToList())
        {
            if (request.Task.SubTasks.All(s => s.Id != existingSubtask.Id))
            {
                await projectRepository.DeleteSubTask(existingSubtask.Id, cancellationToken);
            }
        }
        
        // Add or update subtasks
        foreach (var subtask in request.Task.SubTasks)
        {
            var existingSubtask = existingTask.SubTasks.FirstOrDefault(s => s.Id == subtask.Id);
            
            if (existingSubtask == null)
            {
                // This is a new subtask
                await projectRepository.AddSubTask(subtask, request.Task.Id, cancellationToken);
            }
            else
            {
                // Update existing subtask
                existingSubtask.Title = subtask.Title;
                existingSubtask.IsCompleted = subtask.IsCompleted;
                await projectRepository.UpdateSubTask(existingSubtask, cancellationToken);
            }
        }
        
        var result = await projectRepository.UpdateTask(request.Task, cancellationToken);

        if (!result)
        {
            throw new Exception($"Task was not updated");
        }

        return new UpdateTaskResult(request.Task.Id);
    }
}