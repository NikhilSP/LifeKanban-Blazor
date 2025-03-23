namespace LifeKanbanApi.Model;

public class Project
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Position { get; set; } = 0; 
    
    public List<ProjectTask> Tasks { get; set; }
    
    public List<Milestone> Milestones { get; set; }
}