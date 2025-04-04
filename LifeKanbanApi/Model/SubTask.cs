namespace LifeKanbanApi.Model;

public class SubTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Position { get; set; } = 0;
    
    // Foreign key to ProjectTask
    public Guid ProjectTaskId { get; set; }
    
    // Navigation property back to parent ProjectTask
    public ProjectTask ProjectTask { get; set; }
}