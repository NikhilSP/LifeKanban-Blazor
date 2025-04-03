// LifeKanbanApi/DTO/QuickTodoDto.cs
namespace LifeKanbanApi.DTO;

public class QuickTodoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateCompleted { get; set; }
}