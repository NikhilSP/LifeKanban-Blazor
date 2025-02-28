using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;

namespace LifeKanbanApi.EndPoints.GetProjects;

public record GetProjectsCommand():ICommand<GetProjectsResult>;

public record GetProjectsResult(List<Project> Projects);

public class GetProjectsHandler(ProjectRepository projectRepository):ICommandHandler<GetProjectsCommand,GetProjectsResult>
{
    public async Task<GetProjectsResult> Handle(GetProjectsCommand request, CancellationToken cancellationToken)
    {
        var projects = await projectRepository.GetProjects(cancellationToken);

        return new GetProjectsResult(projects);
    }
}