namespace LifeKanban.Model;

public record GetProjectsResponse(List<ProjectItem> Projects);

public record CreateProjectResponse(bool IsSuccess);

public record DeleteProjectResponse(bool IsSuccess);