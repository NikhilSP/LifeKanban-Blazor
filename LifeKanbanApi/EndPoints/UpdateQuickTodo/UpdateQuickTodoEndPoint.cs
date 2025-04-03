// LifeKanbanApi/EndPoints/UpdateQuickTodo/UpdateQuickTodoEndPoint.cs
using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.UpdateQuickTodo;

public record UpdateQuickTodoRequest(QuickTodo QuickTodo);
public record UpdateQuickTodoResponse(bool IsSuccess);

public class UpdateQuickTodoEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/updateQuickTodo", async (UpdateQuickTodoRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateQuickTodoCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateQuickTodoResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateQuickTodo")
            .Produces<UpdateQuickTodoResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Quick Todo")
            .WithDescription("Update an existing quick todo item");
    }
}