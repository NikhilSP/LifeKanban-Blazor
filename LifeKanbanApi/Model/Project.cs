namespace LifeKanbanApi.Model;

public class Project
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public List<ProjectTask> Tasks { get; set; } = new ();
    public List<Milestone> Milestones { get; set; } = new ();
}