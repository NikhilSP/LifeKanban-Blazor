namespace LifeKanban.Model;

public record GetProjectsResponse(List<ProjectItem> Projects);

public record IdResponse(Guid Id);

public record BoolResponse(bool IsSuccess);

public record GetProjectResponse(ProjectItem Project);
