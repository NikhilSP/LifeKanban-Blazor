using LifeKanbanApi.Model;

namespace LifeKanbanApi.Data;

public interface IProjectRepository
{
    Task<Project[]> GetProjects(CancellationToken cancellationToken = default);
    Task<Project> GetProject(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AddProject(Project project, CancellationToken cancellationToken = default);
    Task<Project> UpdateProject(Project basket, CancellationToken cancellationToken = default);
    Task<bool> DeleteProject(Guid id, CancellationToken cancellationToken = default);
}