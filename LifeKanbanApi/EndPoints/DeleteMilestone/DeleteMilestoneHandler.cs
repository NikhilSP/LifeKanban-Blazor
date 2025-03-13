using LifeKanbanApi.CQRS;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.DeleteMilestone;

public record DeleteMilestoneCommand(Guid Id) : ICommand<DeleteMilestoneResult>;

public record DeleteMilestoneResult(bool IsSuccess);

public class DeleteMileHandler(ProjectRepository projectRepository)
    : ICommandHandler<DeleteMilestoneCommand, DeleteMilestoneResult>
{
    public async Task<DeleteMilestoneResult> Handle(DeleteMilestoneCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = await projectRepository.DeleteMilestone(request.Id, cancellationToken: cancellationToken);

        if (!isSuccess)
        {
            throw new Exception($"Milestone with Id {request.Id} was not deleted");
        }

        return new DeleteMilestoneResult(isSuccess);
    }
}