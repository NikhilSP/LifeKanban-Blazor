// LifeKanbanApi/EndPoints/AddQuickTodo/AddQuickTodoHandler.cs
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.AddQuickTodo;

public record AddQuickTodoCommand(QuickTodo QuickTodo) : ICommand<AddQuickTodoResult>;
public record AddQuickTodoResult(Guid Id);

public class AddQuickTodoHandler(ProjectRepository projectRepository)
    : ICommandHandler<AddQuickTodoCommand, AddQuickTodoResult>
{
    public async Task<AddQuickTodoResult> Handle(AddQuickTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new QuickTodo
        {
            Id = Guid.NewGuid(),
            Title = request.QuickTodo.Title,
            IsCompleted = request.QuickTodo.IsCompleted,
            DateCreated = request.QuickTodo.DateCreated,
            DateCompleted = request.QuickTodo.DateCompleted
        };

        var result = await projectRepository.AddQuickTodo(todo, cancellationToken);
        
        if (result == null)
        {
            throw new Exception("Quick todo was not created");
        }
        
        return new AddQuickTodoResult(todo.Id);
    }
}