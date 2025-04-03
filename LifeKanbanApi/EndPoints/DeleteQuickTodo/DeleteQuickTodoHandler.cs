// LifeKanbanApi/EndPoints/DeleteQuickTodo/DeleteQuickTodoHandler.cs
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.DeleteQuickTodo;

public record DeleteQuickTodoCommand(Guid Id) : ICommand<DeleteQuickTodoResult>;
public record DeleteQuickTodoResult(bool IsSuccess);

public class DeleteQuickTodoHandler(ProjectRepository projectRepository)
    : ICommandHandler<DeleteQuickTodoCommand, DeleteQuickTodoResult>
{
    public async Task<DeleteQuickTodoResult> Handle(DeleteQuickTodoCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = await projectRepository.DeleteQuickTodo(request.Id, cancellationToken);
        
        if (!isSuccess)
        {
            throw new Exception($"Quick todo with Id {request.Id} was not deleted");
        }
        
        return new DeleteQuickTodoResult(isSuccess);
    }
}