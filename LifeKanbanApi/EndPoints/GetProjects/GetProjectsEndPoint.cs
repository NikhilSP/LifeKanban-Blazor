using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.GetProjects;

public record GetProjectsRequest();

public record GetProjectsResponse(Project[] Projects);

public class GetProjectsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/projects", async (GetProjectsRequest request, ISender sender) =>
        {
            var command = request.Adapt<GetProjectsCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<GetProjectsRequest>();
            return Results.Ok(response);
        }).WithName("GetProjects")
        .Produces<GetProjectsResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Projects")
        .WithDescription("Get Projects");
    }
}