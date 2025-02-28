using Carter;
using Mapster;
using MediatR;

namespace LifeKanbanApi.EndPoints.CreateProject;

public record CreateProjectRequest(Guid Id, string ProjectName);

public record CreateProjectResponse(bool IsSuccess);

public class CreateProjectEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/addProject", async (CreateProjectRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateProjectCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<CreateProjectResponse>();

                return Results.Ok(response);
            })
            .WithName("CreateProject")
            .Produces<CreateProjectResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Checkout Basket")
            .WithDescription("Checkout Basket");
    }
}