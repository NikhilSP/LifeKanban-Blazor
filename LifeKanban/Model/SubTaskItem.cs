using System.ComponentModel.DataAnnotations;

namespace LifeKanban.Model;

public class SubTaskItem
{
    public Guid id { get; set; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "Subtask title is required")]
    [StringLength(200, ErrorMessage = "Subtask title cannot exceed 200 characters")]
    public string title { get; set; } = string.Empty;
    
    public bool isCompleted { get; set; }
}