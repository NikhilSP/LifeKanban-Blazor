using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.DeleteProject;

public record DeleteProjectCommand(Guid Id):ICommand<DeleteProjectResult>;

public record DeleteProjectResult(bool IsSuccess);

public class DeleteProjectHandler(ProjectRepository projectRepository):ICommandHandler<DeleteProjectCommand,DeleteProjectResult>
{
    public async Task<DeleteProjectResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = await projectRepository.DeleteProject(request.Id, cancellationToken);

        if (!isSuccess)
        {
            throw new Exception($"Project with Id {request.Id} was not deleted");
        }
        
        return new DeleteProjectResult(isSuccess);
    }
}