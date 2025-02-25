using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;

namespace LifeKanbanApi.EndPoints.DeleteProject;

public record DeleteProjectCommand(Guid Id):ICommand<DeleteProjectResult>;

public record DeleteProjectResult(bool IsSuccess);

public class DeleteProjectHandler(IProjectRepository projectRepository):ICommandHandler<DeleteProjectCommand,DeleteProjectResult>
{
    public async Task<DeleteProjectResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = await projectRepository.DeleteProject(request.Id, cancellationToken);
        return new DeleteProjectResult(isSuccess);
    }
}