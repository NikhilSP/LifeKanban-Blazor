using LifeKanbanApi.CQRS;
using LifeKanbanApi.Data;
using LifeKanbanApi.EndPoints.CreateProject;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.AddMilestone;

public record AddMilestoneCommand(Milestone Milestone, Guid ProjectGuid)
    : ICommand<AddMilestoneResult>;

public record AddMilestoneResult(Guid Id);

public class AddMilestoneHandler(ProjectRepository projectRepository)
    : ICommandHandler<AddMilestoneCommand, AddMilestoneResult>
{
    public async Task<AddMilestoneResult> Handle(AddMilestoneCommand request, CancellationToken cancellationToken)
    {
        var milestone = new Milestone()
        {
            Id = Guid.NewGuid(),
            Name = request.Milestone.Name,
            ProjectId = request.ProjectGuid,
            Project = request.Milestone.Project,
            Tasks = request.Milestone.Tasks,
        };
        

        var result = await projectRepository.AddMilestone(milestone, request.ProjectGuid, cancellationToken);
        if (!result)
        {
            throw new Exception($"Milestone was not created");
        }
        
        return new AddMilestoneResult(milestone.Id);
    }
}