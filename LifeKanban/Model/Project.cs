namespace LifeKanban.Model;

public class Project
{
    public Project(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; set; }
}