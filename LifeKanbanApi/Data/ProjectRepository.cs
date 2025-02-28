using LifeKanbanApi.Model;

namespace LifeKanbanApi.Data;

public class ProjectRepository : IProjectRepository
{
    public List<Project> _projects =
    [
        new Project() { Id = Guid.NewGuid(), Name = "Project 1" },
        new Project() { Id = Guid.NewGuid(), Name = "Project 2" },
        new Project() { Id = Guid.NewGuid(), Name = "Project 3" }
    ];

    public async Task<List<Project>> GetProjects(CancellationToken cancellationToken = default)
    {
        return _projects;
    }

    public Task<Project> GetProject(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddProject(Project project, CancellationToken cancellationToken = default)
    {
        _projects.Add(project);
        return true;
    }

    public Task<Project> UpdateProject(Project project, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteProject(Guid id, CancellationToken cancellationToken = default)
    {
        _projects.RemoveAll(project => project.Id == id);
        return true;
    }
}