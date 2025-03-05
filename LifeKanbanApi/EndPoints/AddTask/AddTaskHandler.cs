using FluentValidation;
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.Model;

namespace LifeKanbanApi.EndPoints.CreateProject;

public record AddTaskCommand(ProjectTask Task, Guid ProjectGuid)
    : ICommand<AddTaskResult>;

public record AddTaskResult(bool IsSuccess);


public class AddTaskHandler(ProjectRepository projectRepository)
    : ICommandHandler<AddTaskCommand, AddTaskResult>
{
    public async Task<AddTaskResult> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        var result = await projectRepository.AddTask(request.Task,request.ProjectGuid, cancellationToken);
        return new AddTaskResult(result);
    }
}