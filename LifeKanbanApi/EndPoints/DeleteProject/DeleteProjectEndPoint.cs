using Carter;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.DeleteProject;

public record DeleteProjectRequest(Guid Id);

public record DeleteProjectResponse(bool IsSuccess);

public class DeleteProjectEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/deleteProject/{id}", async (Guid id, ISender sender) =>
            {
                var request = new DeleteProjectRequest(id);
                var command = request.Adapt<DeleteProjectCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<DeleteProjectRequest>();
                return Results.Ok(response);
            }).WithName("DeleteProject")
            .Produces<DeleteProjectResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Project")
            .WithDescription("Delete Project");
    }
}