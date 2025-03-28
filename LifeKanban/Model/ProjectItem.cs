using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LifeKanban.Model;

public class ProjectItem
{
    public Guid id { get; set; } = Guid.Empty;
    
    [Required(ErrorMessage = "Project name is required")]
    [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
    public string name { get; set; } = string.Empty;
    
    public int position { get; set; } = 0;
    
    public List<ProjectTaskItem> tasks { get; set; } = new();
    
    public List<MilestoneItem> milestones { get; set; } = new();
}