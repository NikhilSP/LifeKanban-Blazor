using System.Text.Json.Serialization;

namespace LifeKanban.Model;

public class ProjectItem
{
    public Guid id { get; set; } = Guid.Empty;
    
    public string name { get; set; } = string.Empty;
    
    public List<ProjectTaskItem> tasks { get; set; } = new();
}