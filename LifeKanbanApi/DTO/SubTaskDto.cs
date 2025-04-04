namespace LifeKanbanApi.DTO;

public class SubTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Position { get; set; } = 0;
}