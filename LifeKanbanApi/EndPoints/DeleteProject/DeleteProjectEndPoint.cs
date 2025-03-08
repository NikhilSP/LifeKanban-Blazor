using Carter;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.DeleteProject;


public record DeleteProjectResponse(bool IsSuccess);

public class DeleteProjectEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/deleteProject/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProjectCommand(id));
                var response = result.Adapt<DeleteProjectResponse>();
                return Results.Ok(response);
            }).WithName("DeleteProject")
            .Produces<DeleteProjectResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Project")
            .WithDescription("Delete Project");
    }
}