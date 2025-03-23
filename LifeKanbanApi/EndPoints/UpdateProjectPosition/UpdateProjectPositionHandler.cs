using LifeKanbanApi.CQRS;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.UpdateProjectPosition;

public record UpdateProjectPositionCommand(Guid ProjectId, int NewPosition) : ICommand<UpdateProjectPositionResult>;

public record UpdateProjectPositionResult(bool Success);

public class UpdateProjectPositionHandler(ProjectRepository projectRepository) : ICommandHandler<UpdateProjectPositionCommand, UpdateProjectPositionResult>
{
    public async Task<UpdateProjectPositionResult> Handle(UpdateProjectPositionCommand request, CancellationToken cancellationToken)
    {
        var success = await projectRepository.UpdateProjectPosition(request.ProjectId, request.NewPosition, cancellationToken);
        return new UpdateProjectPositionResult(success);
    }
}