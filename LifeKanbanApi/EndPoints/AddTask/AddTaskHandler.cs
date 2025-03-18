using LifeKanbanApi.CQRS;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.AddTask;

public record AddTaskCommand(ProjectTask Task, Guid ProjectGuid)
    : ICommand<AddTaskResult>;

public record AddTaskResult(Guid Id);

public class AddTaskHandler(ProjectRepository projectRepository)
    : ICommandHandler<AddTaskCommand, AddTaskResult>
{
    public async Task<AddTaskResult> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new ProjectTask()
        {
            Id = Guid.NewGuid(),
            Title = request.Task.Title,
            Description = request.Task.Description,
            Status = request.Task.Status,
            MilestoneId = request.Task.Milestone?.Id,
            Milestone = request.Task.Milestone,
            ProjectId = request.Task.ProjectId,
            Project = request.Task.Project,
        };

        var result = await projectRepository.AddTask(task, request.ProjectGuid, cancellationToken);

        if (!result)
        {
            throw new Exception($"Task was not created");
        }

        return new AddTaskResult(task.Id);
    }
}