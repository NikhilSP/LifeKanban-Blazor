using Carter;
using LifeKanbanApi.EndPoints.CreateProject;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.AddTask;

public record AddTaskRequest(ProjectTask Task, Guid ProjectGuid);

public record AddTaskResponse(bool IsSuccess);

public class AddTaskEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/addTask", async (AddTaskRequest request, ISender sender) =>
            {
                var command = request.Adapt<AddTaskCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<AddTaskResponse>();

                return Results.Ok(response);
            })
            .WithName("Add Task")
            .Produces<AddTaskResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Checkout Basket")
            .WithDescription("Checkout Basket");
    }
}