using System.ComponentModel.DataAnnotations;
namespace LifeKanban.Model;

public class QuickTodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    public bool IsCompleted { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime? DateCompleted { get; set; }
}