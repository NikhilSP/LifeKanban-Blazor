namespace LifeKanbanApi.DTO;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ProjectTaskDto> Tasks { get; set; } = new();
    public List<MilestoneDto> Milestones { get; set; } = new();
}