// LifeKanbanApi/EndPoints/GetQuickTodos/GetQuickTodosHandler.cs
using LifeKanbanApi.CQRS;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;

namespace LifeKanbanApi.EndPoints.GetQuickTodos;

public record GetQuickTodosCommand() : ICommand<GetQuickTodosResult>;
public record GetQuickTodosResult(List<QuickTodo> QuickTodos);

public class GetQuickTodosHandler(ProjectRepository projectRepository) 
    : ICommandHandler<GetQuickTodosCommand, GetQuickTodosResult>
{
    public async Task<GetQuickTodosResult> Handle(GetQuickTodosCommand request, CancellationToken cancellationToken)
    {
        var quickTodos = await projectRepository.GetQuickTodos(cancellationToken);
        return new GetQuickTodosResult(quickTodos);
    }
}