using LifeKanban.Model;

namespace LifeKanban.Client;

public class ProjectsClient
{
    private List<Project> _projects = [
        new Project(id: 1, name: "Project A"),
        new Project(id: 2, name: "Project B"),
        new Project(id: 3, name: "Project C"),];
    
    
    public Project[] GetProjects()
    {
        return _projects.ToArray();
    }

    public void AddProject(Project newProject)
    {
        _projects.Add(newProject);
    }
}