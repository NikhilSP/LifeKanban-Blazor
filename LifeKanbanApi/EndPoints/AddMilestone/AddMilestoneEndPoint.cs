using Carter;
using LifeKanbanApi.EndPoints.AddTask;
using LifeKanbanApi.EndPoints.CreateProject;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.AddMilestone;

public record AddMilestoneRequest(Milestone Milestone, Guid ProjectGuid);

public record AddMilestoneResponse(Guid Id);

public class AddMilestoneEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/addMilestone", async (AddMilestoneRequest request, ISender sender) =>
            {
                var command = request.Adapt<AddMilestoneCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<AddMilestoneResponse>();

                return Results.Ok(response);
            })
            .WithName("Add Milestone")
            .Produces<AddMilestoneResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Milestone")
            .WithDescription("Add Milestone");
    }
}