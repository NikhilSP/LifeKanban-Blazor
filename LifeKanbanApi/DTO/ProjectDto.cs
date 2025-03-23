namespace LifeKanbanApi.DTO;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; } = 0; // Add this property
    public List<ProjectTaskDto> Tasks { get; set; } = new();
    public List<MilestoneDto> Milestones { get; set; } = new();
}