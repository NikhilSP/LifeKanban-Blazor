using LifeKanbanApi.Model;

namespace LifeKanbanApi.Data;

public class ProjectRepository : IProjectRepository
{
    public List<Project> _projects =
    [
        new Project() { Id = Guid.NewGuid(), Name = "Project 1" },
        new Project() { Id = Guid.NewGuid(), Name = "Project 2" },
        new Project()
        {
            Id = Guid.NewGuid(),
            Name = "Project 3",
            Tasks =
            [
                new ProjectTask()
                {
                    Id = Guid.NewGuid(),
                    Title = "Task 1",
                    Description = "Description",
                    Status = "Status",
                    Milestone = new Milestone()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Milestone 1"
                    }
                },
                new ProjectTask()
                {
                    Id = Guid.NewGuid(),
                    Title = "Task 2",
                    Description = "Description 2",
                    Status = "Status 2",
                    Milestone = new Milestone()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Milestone 2"
                    }
                }
                
            ]
        }
    ];

    public async Task<List<Project>> GetProjects(CancellationToken cancellationToken = default)
    {
        return _projects;
    }

    public async Task<Project> GetProject(Guid id, CancellationToken cancellationToken = default)
    {
        return _projects.First(x => x.Id == id);
    }

    public async Task<bool> AddProject(Project project, CancellationToken cancellationToken = default)
    {
        _projects.Add(project);
        return true;
    }
    
    public async Task<bool> AddTask(ProjectTask projectTask,Guid projectGuid, CancellationToken cancellationToken = default)
    {
        var project = await GetProject(projectGuid,cancellationToken);
        project.Tasks.Add(projectTask);
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