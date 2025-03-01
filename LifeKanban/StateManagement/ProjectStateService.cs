namespace LifeKanban.StateManagement;

public class ProjectStateService
{
    public event Action? ProjectsChanged;
    public void NotifyStateChanged() => ProjectsChanged?.Invoke();
}