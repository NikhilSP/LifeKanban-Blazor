using System.ComponentModel.DataAnnotations;

namespace LifeKanban.Model;

public class ProjectTaskItem
{
    public Guid id { get; set; }
    
    [Required(ErrorMessage = "Task title is required")]
    [StringLength(200, ErrorMessage = "Task title cannot exceed 200 characters")]
    public string title { get; set; } = string.Empty;
    
    public string description { get; set; } = string.Empty;
    
    public string status { get; set; } = string.Empty;
    
    public double columnPosition { get; set; } = 0;
    
    public MilestoneItem? milestone { get; set; }
    
    public List<SubTaskItem> subtasks { get; set; } = new();

    public ProjectTaskItem Clone()
    {
        return new ProjectTaskItem()
        {
            id = id,
            title = title,
            description = description,
            status = status,
            columnPosition = columnPosition,
            milestone = milestone?.Clone(),
            subtasks = subtasks.Select(x=>x.Clone()).ToList()
        };
    }
}

