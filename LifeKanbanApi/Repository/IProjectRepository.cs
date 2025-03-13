using LifeKanbanApi.Model;

namespace LifeKanbanApi.Repository;

public interface IProjectRepository
{
    Task<List<Project>> GetProjects(CancellationToken cancellationToken = default);
    Task<Project?> GetProject(Guid id, CancellationToken cancellationToken = default);
    Task<Guid?> AddProject(Project project, CancellationToken cancellationToken = default);
    Task<Guid?> UpdateProject(Project basket, CancellationToken cancellationToken = default);
    Task<bool> DeleteProject(Guid id, CancellationToken cancellationToken = default);
}