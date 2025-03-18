namespace LifeKanbanApi.Model;

public class Milestone
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    // Foreign key to Project
    public Guid ProjectId { get; set; }
    
    public Project Project { get; set; }
    
    public List<ProjectTask> Tasks { get; set; } = new ();
}