// LifeKanbanApi/EndPoints/UpdateQuickTodo/UpdateQuickTodoHandler.cs
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.UpdateQuickTodo;

public record UpdateQuickTodoCommand(QuickTodo QuickTodo) : ICommand<UpdateQuickTodoResult>;
public record UpdateQuickTodoResult(bool IsSuccess);

public class UpdateQuickTodoHandler(ProjectRepository projectRepository)
    : ICommandHandler<UpdateQuickTodoCommand, UpdateQuickTodoResult>
{
    public async Task<UpdateQuickTodoResult> Handle(UpdateQuickTodoCommand request, CancellationToken cancellationToken)
    {
        var result = await projectRepository.UpdateQuickTodo(request.QuickTodo, cancellationToken);
        
        if (!result)
        {
            throw new Exception($"Quick todo with Id {request.QuickTodo.Id} was not updated");
        }
        
        return new UpdateQuickTodoResult(true);
    }
}