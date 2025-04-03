// LifeKanbanApi/EndPoints/AddQuickTodo/AddQuickTodoEndPoint.cs
using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.AddQuickTodo;

public record AddQuickTodoRequest(QuickTodo QuickTodo);
public record AddQuickTodoResponse(Guid Id);

public class AddQuickTodoEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/addQuickTodo", async (AddQuickTodoRequest request, ISender sender) =>
            {
                var command = request.Adapt<AddQuickTodoCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<AddQuickTodoResponse>();
                return Results.Ok(response);
            })
            .WithName("AddQuickTodo")
            .Produces<AddQuickTodoResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Quick Todo")
            .WithDescription("Add a new quick todo item");
    }
}