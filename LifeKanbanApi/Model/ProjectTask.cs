using System.Diagnostics.CodeAnalysis;

namespace LifeKanbanApi.Model;

public class ProjectTask
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public Milestone Milestone { get; set; }
}