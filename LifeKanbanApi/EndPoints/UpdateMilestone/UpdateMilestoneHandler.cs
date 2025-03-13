using LifeKanbanApi.CQRS;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.UpdateMilestone;

public record UpdateMilestoneCommand(Milestone Milestone, Guid ProjectGuid)
    : ICommand<UpdateMilestoneResult>;

public record UpdateMilestoneResult(Guid Id);

public class UpdateMilestoneHandler(ProjectRepository projectRepository)
    : ICommandHandler<UpdateMilestoneCommand, UpdateMilestoneResult>
{
    public async Task<UpdateMilestoneResult> Handle(UpdateMilestoneCommand request, CancellationToken cancellationToken)
    {
        request.Milestone.ProjectId = request.ProjectGuid;
        
        var result = await projectRepository.UpdateMilestone(request.Milestone, cancellationToken);

        if (!result)
        {
            throw new Exception($"Milestone was not created");
        }

        return new UpdateMilestoneResult(request.Milestone.Id);
    }
}