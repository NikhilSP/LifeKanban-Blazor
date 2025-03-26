namespace LifeKanbanApi.Model;

public class ProjectTask
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } 
    public string Status { get; set; }

    public double ColumnPosition { get; set; } = 0;
    // Foreign key property
    public Guid? MilestoneId { get; set; }
    
    // Navigation property to Milestone
    public Milestone? Milestone { get; set; }
    
    // Foreign key to Project
    public Guid ProjectId { get; set; }
    
    // Navigation property back to parent Project
    public Project Project { get; set; }
    
    public List<SubTask> SubTasks { get; set; } = new();
}