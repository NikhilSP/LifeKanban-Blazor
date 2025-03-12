using LifeKanbanApi.CQRS;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.DeleteTask;

public record DeleteTaskCommand(Guid Id):ICommand<DeleteTaskResult>;

public record DeleteTaskResult(bool IsSuccess);

public class DeleteTaskHandler(ProjectRepository projectRepository):ICommandHandler<DeleteTaskCommand,DeleteTaskResult>
{
    public async Task<DeleteTaskResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = await projectRepository.DeleteTask(request.Id, cancellationToken);

        if (!isSuccess)
        {
            throw new Exception($"Task with Id {request.Id} was not deleted");
        }
        
        return new DeleteTaskResult(isSuccess);
    }
}