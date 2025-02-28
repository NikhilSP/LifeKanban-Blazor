namespace LifeKanban.Model;

public class ProjectTaskItem
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public MilestoneItem milestoneitem { get; set; }
}