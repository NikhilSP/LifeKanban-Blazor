namespace LifeKanbanApi.Model;

public class Milestone
{
    public Milestone(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; set; }
}