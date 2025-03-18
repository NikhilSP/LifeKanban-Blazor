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
        
        var result = await projectRepository.UpdateTask(request.Task, cancellationToken);

        if (!result)
        {
            throw new Exception($"Task was not created");
        }

        return new UpdateTaskResult(request.Task.Id);
    }
}