namespace LifeKanban.Model;

public class ProjectTaskItem
{
    public Guid id { get; set; }
    public string title { get; set; }
    public string description { get; set; } = string.Empty;
    public string status { get; set; }  = string.Empty;
    public double columnPosition { get; set; } = 0;
    public MilestoneItem? milestone { get; set; }
}

