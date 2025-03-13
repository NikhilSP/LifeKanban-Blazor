using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.UpdateTask;

public record UpdateTaskRequest(ProjectTask Task, Guid ProjectGuid);

public record UpdateTaskResponse(Guid Id);

public class UpdateTaskEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/updateTask", async (UpdateTaskRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateTaskCommand>();
               
                var result = await sender.Send(command);

                var response = result.Adapt<UpdateTaskResponse>();

                return Results.Ok(response);
            })
            .WithName("UpdateTask")
            .Produces<UpdateTaskResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Task")
            .WithDescription("Update Task");
    }
}