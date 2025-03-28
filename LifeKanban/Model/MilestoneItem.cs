using System.ComponentModel.DataAnnotations;

namespace LifeKanban.Model;

public class MilestoneItem
{
    public Guid id { get; set; }
    
    [Required(ErrorMessage = "Milestone name is required")]
    [StringLength(100, ErrorMessage = "Milestone name cannot exceed 100 characters")]
    public string name { get; set; } = string.Empty;
    
    public int position { get; set; } = 0;
}