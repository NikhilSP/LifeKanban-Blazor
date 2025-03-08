namespace LifeKanbanApi.Model;

public class Milestone
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    // Foreign key to Project
    public Guid ProjectId { get; set; }
    
    // Navigation property back to parent Project
    public Project Project { get; set; }
    
    // Collection of tasks associated with this milestone
    public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}