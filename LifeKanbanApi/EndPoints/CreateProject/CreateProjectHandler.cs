using FluentValidation;
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;

namespace LifeKanbanApi.EndPoints.CreateProject;

public record CreateProjectCommand(Guid Id,string ProjectName) 
    : ICommand<CreateProjectResult>;

public record CreateProjectResult(bool IsSuccess);

public class CreateProjectValidator:AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.ProjectName).NotNull().WithMessage("ProjectName can't be null");
        RuleFor(x => x.ProjectName).NotEmpty().WithMessage("ProjectName can't be empty");
    }
}

public class CreateProjectHandler(IProjectRepository projectRepository): ICommandHandler<CreateProjectCommand, CreateProjectResult>
{
    public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
       var result = await projectRepository.AddProject(new Project(request.Id, request.ProjectName),cancellationToken);
       return new CreateProjectResult(result);
    }
}