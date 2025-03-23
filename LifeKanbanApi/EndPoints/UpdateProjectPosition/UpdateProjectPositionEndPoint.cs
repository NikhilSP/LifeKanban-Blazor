using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.UpdateProjectPosition;

public record UpdateProjectPositionRequest(Guid ProjectId, int NewPosition);

public record UpdateProjectPositionResponse(bool Success);

public class UpdateProjectPositionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/updateProjectPosition", async (UpdateProjectPositionRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProjectPositionCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateProjectPositionResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateProjectPosition")
            .Produces<UpdateProjectPositionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Project Position")
            .WithDescription("Update Project Position in Navigation Menu");
    }
}