using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.GetProject;

public record GetProjectRequest(Guid Id);

public record GetProjectResponse(Project[] Projects);

public class GetProjectEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/project", async ([AsParameters]GetProjectRequest request, ISender sender) =>
            {
                var command = request.Adapt<GetProjectCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<GetProjectRequest>();
                return Results.Ok(response);
            }).WithName("GetProject")
            .Produces<GetProjectResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Project")
            .WithDescription("Get Project");
    }
}