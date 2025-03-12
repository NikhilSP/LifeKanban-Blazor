using Carter;
using LifeKanbanApi.DTO;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeKanbanApi.EndPoints.GetProjects;

public record GetProjectsRequest();

public record GetProjectsResponse(List<ProjectDto> Projects);

public class GetProjectsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/projects", async ([AsParameters] GetProjectsRequest request, ISender sender) =>
            {
                var command = request.Adapt<GetProjectsCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<GetProjectsResponse>();
                return Results.Ok(response);
            }).WithName("GetProjects")
            .Produces<GetProjectsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Projects")
            .WithDescription("Get Projects");
    }
}