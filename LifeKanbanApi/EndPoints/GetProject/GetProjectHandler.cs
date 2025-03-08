using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.GetProject;

public record GetProjectCommand(Guid Id):ICommand<GetProjectResult>;

public record GetProjectResult(Project Project);

public class GetProjectHandler(ProjectRepository projectRepository):ICommandHandler<GetProjectCommand,GetProjectResult>
{
    public async Task<GetProjectResult> Handle(GetProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetProject(request.Id, cancellationToken);
       
        if (project is null)
        {
            throw new Exception($"Project with Id {request.Id} Not found");
        }
        
        return new GetProjectResult( project);
    }
}