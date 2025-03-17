using FluentValidation;
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.CreateProject;

public record CreateProjectCommand(Project Project) : ICommand<CreateProjectResult>;

public record CreateProjectResult(Guid Id);

public class CreateProjectHandler(ProjectRepository projectRepository)
    : ICommandHandler<CreateProjectCommand, CreateProjectResult>
{
    public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = request.Project.Name,
            Tasks = [],
            Milestones = [],
        };

        var result = await projectRepository.AddProject(project, cancellationToken);

        if (result is null)
        {
            throw new Exception($"New project was not created");
        }
        
        return new CreateProjectResult(result.Value);
    }
}