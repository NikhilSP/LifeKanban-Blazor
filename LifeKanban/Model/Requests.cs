namespace LifeKanban.Model;

public record CreateProjectRequest(ProjectItem Project);
public record UpdateProjectRequest(ProjectItem Project);
public record AddTaskRequest(ProjectTaskItem Task, Guid ProjectGuid);
public record UpdateTaskRequest(ProjectTaskItem Task, Guid ProjectGuid);
public record AddMilestoneRequest(MilestoneItem Milestone, Guid ProjectGuid);
public record UpdateMilestoneRequest(MilestoneItem Milestone, Guid ProjectGuid);