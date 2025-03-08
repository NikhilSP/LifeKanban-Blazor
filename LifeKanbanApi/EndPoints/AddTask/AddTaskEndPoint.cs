using Carter;
using LifeKanbanApi.EndPoints.CreateProject;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.AddTask;

public record AddTaskRequest(ProjectTask Task, Guid ProjectGuid);

public record AddTaskResponse(Guid Id);

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
            .WithName("AddTask")
            .Produces<AddTaskResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Task")
            .WithDescription("Add Task");
    }
}