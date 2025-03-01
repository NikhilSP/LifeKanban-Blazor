using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.GetProject;

public record GetProjectRequest(Guid Id);

public record GetProjectResponse(Project Project);

public class GetProjectEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/project/{id}", async (Guid id, ISender sender) =>
            {
                var request = new GetProjectRequest(id);
                var command = request.Adapt<GetProjectCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<GetProjectResponse>();
                return Results.Ok(response);
            }).WithName("GetProject")
            .Produces<GetProjectResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Project")
            .WithDescription("Get Project");
    }
}