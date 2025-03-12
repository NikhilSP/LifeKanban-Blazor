using Carter;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.DeleteTask;


public record DeleteTaskResponse(bool IsSuccess);

public class DeleteTaskEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/deleteTask/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteTaskCommand(id));
                var response = result.Adapt<DeleteTaskResponse>();
                return Results.Ok(response);
            }).WithName("DeleteTask")
            .Produces<DeleteTaskResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Task")
            .WithDescription("Delete Task");
    }
}