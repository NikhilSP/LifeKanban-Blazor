using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.EndPoints.CreateProject;
using LifeKanbanApi.Model;

namespace LifeKanbanApi.EndPoints.AddMilestone;

public record AddMilestoneCommand(Milestone Milestone, Guid ProjectGuid)
    : ICommand<AddMilestoneResult>;

public record AddMilestoneResult(bool IsSuccess);


public class AddMilestoneHandler(ProjectRepository projectRepository)
    : ICommandHandler<AddMilestoneCommand, AddMilestoneResult>
{
    public async Task<AddMilestoneResult> Handle(AddMilestoneCommand request, CancellationToken cancellationToken)
    {
        var result = await projectRepository.AddMilestone(request.Milestone,request.ProjectGuid, cancellationToken);
        return new AddMilestoneResult(result);
    }
}