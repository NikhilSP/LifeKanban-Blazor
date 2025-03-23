using System.Text.Json.Serialization;

namespace LifeKanban.Model;

public class ProjectItem
{
    public Guid id { get; set; } = Guid.Empty;
    
    public string name { get; set; } = string.Empty;
    
    public int position { get; set; } = 0; // Add this property
    
    public List<ProjectTaskItem> tasks { get; set; } = new();
    
    public List<MilestoneItem> milestones { get; set; } = new();
}