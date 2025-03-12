namespace LifeKanbanApi.DTO;

public class ProjectTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double ColumnPosition { get; set; } = 0;
    public Guid ProjectId { get; set; }
    public Guid? MilestoneId { get; set; }
}