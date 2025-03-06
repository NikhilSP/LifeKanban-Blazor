namespace LifeKanban.Model;

public record GetProjectsResponse(List<ProjectItem> Projects);

public record CreateProjectResponse(bool IsSuccess);

public record DeleteProjectResponse(bool IsSuccess);

public record GetProjectResponse(ProjectItem Project);

public record AddTaskResponse(bool IsSuccess);
public record AddMilestoneResponse(bool IsSuccess);