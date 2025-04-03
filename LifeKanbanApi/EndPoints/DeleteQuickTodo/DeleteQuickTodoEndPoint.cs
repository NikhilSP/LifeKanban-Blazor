// LifeKanbanApi/EndPoints/DeleteQuickTodo/DeleteQuickTodoEndPoint.cs
using Carter;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.DeleteQuickTodo;

public record DeleteQuickTodoResponse(bool IsSuccess);

public class DeleteQuickTodoEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/deleteQuickTodo/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteQuickTodoCommand(id));
                var response = result.Adapt<DeleteQuickTodoResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteQuickTodo")
            .Produces<DeleteQuickTodoResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Quick Todo")
            .WithDescription("Delete a quick todo item");
    }
}