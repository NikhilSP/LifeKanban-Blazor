namespace LifeKanban.Model;

public record CreateProjectRequest(ProjectItem Project);

public record DeleteProjectRequest(Guid Id);