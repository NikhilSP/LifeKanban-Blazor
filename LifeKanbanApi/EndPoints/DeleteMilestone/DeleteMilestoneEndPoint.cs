using Carter;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.DeleteMilestone;


public record DeleteMilestoneResponse(bool IsSuccess);

public class DeleteMilestoneEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/deleteMilestone/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteMilestoneCommand(id));
                var response = result.Adapt<DeleteMilestoneResponse>();
                return Results.Ok(response);
            }).WithName("DeleteMilestone")
            .Produces<DeleteMilestoneResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Milestone")
            .WithDescription("Delete Milestone");
    }
}