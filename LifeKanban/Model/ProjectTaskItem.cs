namespace LifeKanban.Model;

public class ProjectTaskItem
{
    public Guid id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public MilestoneItem milestone { get; set; }
}