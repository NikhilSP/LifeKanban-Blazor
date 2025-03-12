namespace LifeKanban.Model;

public record GetProjectsResponse(List<ProjectItem> Projects);

public record CreateProjectResponse(Guid Id);

public record DeleteProjectResponse(bool IsSuccess);
public record DeleteTaskResponse(bool IsSuccess);

public record GetProjectResponse(ProjectItem Project);

public record AddTaskResponse(Guid Id);
public record AddMilestoneResponse(Guid Id);