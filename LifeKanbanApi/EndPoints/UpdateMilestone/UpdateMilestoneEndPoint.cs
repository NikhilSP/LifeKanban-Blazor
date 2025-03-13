using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.UpdateMilestone;

public record UpdateMilestoneRequest(Milestone Milestone, Guid ProjectGuid);

public record UpdateMilestoneResponse(Guid Id);

public class UpdateMilestoneEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/updateMilestone", async (UpdateMilestoneRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateMilestoneCommand>();
               
                var result = await sender.Send(command);

                var response = result.Adapt<UpdateMilestoneResponse>();

                return Results.Ok(response);
            })
            .WithName("UpdateMilestone")
            .Produces<UpdateMilestoneResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Milestone")
            .WithDescription("Update Milestone");
    }
}