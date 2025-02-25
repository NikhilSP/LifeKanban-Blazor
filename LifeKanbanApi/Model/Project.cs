namespace LifeKanbanApi.Model;

public class Project
{
    public Project(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; set; }
    
    public IReadOnlyList<ProjectTask> Tasks { get; set; }
}