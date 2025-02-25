using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;

namespace LifeKanbanApi.EndPoints.GetProject;

public record GetProjectCommand(Guid Id):ICommand<GetProjectResult>;

public record GetProjectResult(Project Project);

public class GetProjectHandler(IProjectRepository projectRepository):ICommandHandler<GetProjectCommand,GetProjectResult>
{
    public async Task<GetProjectResult> Handle(GetProjectCommand request, CancellationToken cancellationToken)
    {
        var projects = await projectRepository.GetProject(request.Id, cancellationToken);
        return new GetProjectResult(projects);
    }
}