namespace LifeKanban.Model.ViewModels;

// Define task status as an enum instead of strings
public enum TaskStatus
{
    ToDo,
    InProgress,
    Done,
    // Easy to add new statuses
    // Review,
    // Testing,
    // Deployed,
}

// Define column type as an enum instead of magic numbers
public enum ColumnType
{
    Milestones = 0,
    ToDo = 1,
    InProgress = 2,
    Done = 4,
    // Easy to add new column types
    // Review = 5,
    // Testing = 6,
}

// Class to define column configuration
public class ColumnDefinition
{
    public ColumnType Type { get; set; }
    public string Title { get; set; }
    public string CssClass { get; set; }
    public string BorderColor { get; set; }
    public TaskStatus? MappedStatus { get; set; }
    public bool AllowTasks { get; set; } = true;
    public int SortOrder { get; set; }
}

// Extending the KanbanTask model
public class KanbanTask
{
    public ColumnType ColumnType { get; set; }
    public ProjectTaskItem Task { get; set; }
}

// Extending the KanbanMilestone model
public class KanbanMilestone
{
    public MilestoneItem Milestone { get; set; }
}

// Static configuration provider
public static class BoardConfiguration
{
    // Default column definitions
    public static readonly List<ColumnDefinition> DefaultColumns = new()
    {
        new ColumnDefinition 
        { 
            Type = ColumnType.Milestones, 
            Title = "Milestones", 
            CssClass = "column-milestones", 
            BorderColor = "transparent",
            AllowTasks = false,
            SortOrder = 0
        },
        new ColumnDefinition 
        { 
            Type = ColumnType.ToDo, 
            Title = "To Do", 
            CssClass = "column-todo", 
            BorderColor = "var(--column-todo-color)",
            MappedStatus = TaskStatus.ToDo,
            SortOrder = 1
        },
        new ColumnDefinition 
        { 
            Type = ColumnType.InProgress, 
            Title = "In Progress", 
            CssClass = "column-inprogress", 
            BorderColor = "var(--column-inprogress-color)",
            MappedStatus = TaskStatus.InProgress,
            SortOrder = 2
        },
        new ColumnDefinition 
        { 
            Type = ColumnType.Done, 
            Title = "Done", 
            CssClass = "column-done", 
            BorderColor = "var(--column-done-color)",
            MappedStatus = TaskStatus.Done,
            SortOrder = 3
        },
    };
    
    // Converters for task status
    public static TaskStatus ToTaskStatus(string statusString)
    {
        return statusString switch
        {
            "To Do" => TaskStatus.ToDo,
            "In Progress" => TaskStatus.InProgress,
            "Done" => TaskStatus.Done,
            _ => TaskStatus.ToDo // Default
        };
    }

    public static string FromTaskStatus(TaskStatus status)
    {
        return status switch
        {
            TaskStatus.ToDo => "To Do",
            TaskStatus.InProgress => "In Progress",
            TaskStatus.Done => "Done",
            _ => "To Do" // Default
        };
    }
    
    // Mapping between column types and task statuses
    public static TaskStatus? GetMappedStatus(ColumnType columnType)
    {
        return DefaultColumns.FirstOrDefault(c => c.Type == columnType)?.MappedStatus;
    }
    
    public static ColumnType GetColumnForStatus(TaskStatus status)
    {
        var column = DefaultColumns.FirstOrDefault(c => c.MappedStatus == status);
        return column?.Type ?? ColumnType.ToDo; // Default to ToDo if no mapping found
    }
}