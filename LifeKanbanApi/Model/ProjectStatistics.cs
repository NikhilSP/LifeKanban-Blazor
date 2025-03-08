namespace LifeKanbanApi.Model;

public class ProjectStatistics
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public Dictionary<string, int> TasksByStatus { get; set; } = new();
    public int MilestoneCount { get; set; }
    public int TasksWithNoMilestone { get; set; }
    
    // Calculated properties
    public double CompletionPercentage => TotalTasks > 0 ? 
        Math.Round((double)CompletedTasks / TotalTasks * 100, 2) : 0;
}