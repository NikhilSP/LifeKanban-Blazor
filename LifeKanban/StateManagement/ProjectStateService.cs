namespace LifeKanban.StateManagement;

public class ProjectStateService
{
    public event Func<Task>? ProjectsChanged;
    public void NotifyStateChanged() => ProjectsChanged?.Invoke();
}