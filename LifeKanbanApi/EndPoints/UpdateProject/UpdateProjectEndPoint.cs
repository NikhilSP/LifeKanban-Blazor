using Carter;
using LifeKanbanApi.Model;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.UpdateProject;

public record UpdateProjectRequest(Project Project);

public record UpdateProjectResponse(Guid Id);

public class UpdateProjectEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/updateProject", async (UpdateProjectRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProjectCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<UpdateProjectResponse>();

                return Results.Ok(response);
            })
            .WithName("UpdateProject")
            .Produces<UpdateProjectResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Project")
            .WithDescription("Update Project");
    }
}