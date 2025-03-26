namespace LifeKanban.Model;

public class SubTaskItem
{
    public Guid id { get; set; } = Guid.NewGuid();
    public string title { get; set; } = string.Empty;
    public bool isCompleted { get; set; }
}