namespace LifeKanbanApi.DTO;

public class MilestoneDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
}