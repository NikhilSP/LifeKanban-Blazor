// LifeKanbanApi/EndPoints/GetQuickTodos/GetQuickTodosEndPoint.cs
using Carter;
using LifeKanbanApi.DTO;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.GetQuickTodos;

public record GetQuickTodosRequest();
public record GetQuickTodosResponse(List<QuickTodoDto> QuickTodos);

public class GetQuickTodosEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/quickTodos", async (ISender sender) =>
        {
            var command = new GetQuickTodosCommand();
            var result = await sender.Send(command);
            var response = result.Adapt<GetQuickTodosResponse>();
            return Results.Ok(response);
        });
    }
}