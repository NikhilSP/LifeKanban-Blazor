using LifeKanbanApi.CQRS;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.UpdateProject;

public record UpdateProjectCommand(Project Project) : ICommand<UpdateProjectResult>;

public record UpdateProjectResult(Guid Id);

public class UpdateProjectHandler(ProjectRepository projectRepository)
    : ICommandHandler<UpdateProjectCommand, UpdateProjectResult>
{
    public async Task<UpdateProjectResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var result = await projectRepository.UpdateProject(request.Project, cancellationToken);

        if (result is null)
        {
            throw new Exception($"Project was not updated");
        }
        
        return new UpdateProjectResult(result.Value);
    }
}