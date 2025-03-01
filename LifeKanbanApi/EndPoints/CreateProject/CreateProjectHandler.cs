using FluentValidation;
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;

namespace LifeKanbanApi.EndPoints.CreateProject;

public record CreateProjectCommand(Project Project)
    : ICommand<CreateProjectResult>;

public record CreateProjectResult(bool IsSuccess);

public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Project.Name).NotNull().WithMessage("ProjectName can't be null");
        RuleFor(x => x.Project.Name).NotEmpty().WithMessage("ProjectName can't be empty");
    }
}

public class CreateProjectHandler(ProjectRepository projectRepository)
    : ICommandHandler<CreateProjectCommand, CreateProjectResult>
{
    public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var result = await projectRepository.AddProject(request.Project, cancellationToken);
        return new CreateProjectResult(result);
    }
}