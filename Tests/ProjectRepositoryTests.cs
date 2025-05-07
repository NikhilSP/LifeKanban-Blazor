using LifeKanbanApi.Data;
using LifeKanbanApi.Model;
using LifeKanbanApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class ProjectRepositoryTests : IDisposable
{
    private readonly ProjectDbContext _context;
    private readonly ProjectRepository _repository;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ProjectRepositoryTests()
    {
        // Create a new unique database name for each test run
        var dbName = $"ProjectDb_{Guid.NewGuid()}";

        // Configure in-memory database
        var options = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        _context = new ProjectDbContext(options);
        _repository = new ProjectRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // Utility methods to create test data
    private Project CreateTestProject(string name = "Test Project")
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = "Test Description",
            Position = 10,
            Tasks = new List<ProjectTask>(),
            Milestones = new List<Milestone>()
        };
    }

    private ProjectTask CreateTestTask(Guid projectId, string title = "Test Task")
    {
        return new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = "Test Description",
            Status = "ToDo",
            ProjectId = projectId,
            ColumnPosition = 1.0,
            SubTasks = new List<SubTask>()
        };
    }

    private Milestone CreateTestMilestone(Guid projectId, string name = "Test Milestone")
    {
        return new Milestone
        {
            Id = Guid.NewGuid(),
            Name = name,
            ProjectId = projectId,
            Tasks = new List<ProjectTask>()
        };
    }

    private SubTask CreateTestSubTask(Guid taskId, string title = "Test SubTask")
    {
        return new SubTask
        {
            Id = Guid.NewGuid(),
            Title = title,
            IsCompleted = false,
            Position = 10,
            ProjectTaskId = taskId
        };
    }

    private QuickTodo CreateTestQuickTodo(string title = "Test QuickTodo")
    {
        return new QuickTodo
        {
            Id = Guid.NewGuid(),
            Title = title,
            IsCompleted = false,
            DateCreated = DateTime.UtcNow,
            DateCompleted = null
        };
    }

    #region Project Tests

    [Fact]
    public async Task GetProjects_ShouldReturnAllProjects()
    {
        // Arrange
        var project1 = CreateTestProject("Project 1");
        var project2 = CreateTestProject("Project 2");

        _context.Projects.Add(project1);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProjects(_cancellationToken);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == project1.Id);
        Assert.Contains(result, p => p.Id == project2.Id);
    }

    [Fact]
    public async Task GetProject_WithValidId_ShouldReturnProject()
    {
        // Arrange
        var project = CreateTestProject();
        var task = CreateTestTask(project.Id);
        var milestone = CreateTestMilestone(project.Id);

        project.Tasks.Add(task);
        project.Milestones.Add(milestone);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProject(project.Id, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(project.Id, result.Id);
        Assert.Equal(project.Name, result.Name);
        Assert.Equal(1, result.Tasks.Count);
        Assert.Equal(1, result.Milestones.Count);
    }

    [Fact]
    public async Task GetProject_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.GetProject(invalidId, _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddProject_ShouldAddProjectAndReturnId()
    {
        // Arrange
        var project = CreateTestProject();

        // Act
        var result = await _repository.AddProject(project, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(project.Id, result);

        var storedProject = await _context.Projects.FindAsync(project.Id);
        Assert.NotNull(storedProject);
        Assert.Equal(project.Name, storedProject.Name);
    }

    [Fact]
    public async Task UpdateProject_ShouldUpdateProjectProperties()
    {
        // Arrange
        var project = CreateTestProject();
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Update the project
        var updatedProject = new Project
        {
            Id = project.Id,
            Name = "Updated Name",
            Description = "Updated Description",
            Position = 20
        };

        // Act
        var result = await _repository.UpdateProject(updatedProject, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(project.Id, result);

        var storedProject = await _context.Projects.FindAsync(project.Id);
        Assert.NotNull(storedProject);
        Assert.Equal("Updated Name", storedProject.Name);
        Assert.Equal("Updated Description", storedProject.Description);
        Assert.Equal(20, storedProject.Position);
    }

    [Fact]
    public async Task DeleteProject_WithValidId_ShouldDeleteProjectAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteProject(project.Id, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedProject = await _context.Projects.FindAsync(project.Id);
        Assert.Null(storedProject);
    }

    [Fact]
    public async Task DeleteProject_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteProject(invalidId, _cancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task InitializeProjectPositions_ShouldSetProjectPositions()
    {
        // Arrange
        var project1 = CreateTestProject("Project 1");
        var project2 = CreateTestProject("Project 2");

        project1.Position = 0; // Uninitialized position
        project2.Position = 0; // Uninitialized position

        _context.Projects.Add(project1);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync();

        // Act
        await _repository.InitializeProjectPositions(_cancellationToken);

        // Assert
        var projects = await _context.Projects.ToListAsync();
        Assert.All(projects, p => Assert.NotEqual(0, p.Position));
    }

    [Fact]
    public async Task GetMaxProjectPosition_WithProjects_ShouldReturnMaxPosition()
    {
        // Arrange
        var project1 = CreateTestProject("Project 1");
        var project2 = CreateTestProject("Project 2");

        project1.Position = 10;
        project2.Position = 20;

        _context.Projects.Add(project1);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetMaxProjectPosition(_cancellationToken);

        // Assert
        Assert.Equal(20, result);
    }

    [Fact]
    public async Task GetMaxProjectPosition_WithNoProjects_ShouldReturnZero()
    {
        // Act
        var result = await _repository.GetMaxProjectPosition(_cancellationToken);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task UpdateProjectPosition_WithValidId_ShouldUpdatePositionAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        project.Position = 10;

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UpdateProjectPosition(project.Id, 30, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedProject = await _context.Projects.FindAsync(project.Id);
        Assert.NotNull(storedProject);
        Assert.Equal(30, storedProject.Position);
    }

    [Fact]
    public async Task UpdateProjectPosition_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.UpdateProjectPosition(invalidId, 30, _cancellationToken);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Task Tests

    [Fact]
    public async Task GetTaskById_WithValidId_ShouldReturnTask()
    {
        // Arrange
        var project = CreateTestProject();
        var task = CreateTestTask(project.Id);
        var subtask = CreateTestSubTask(task.Id);

        task.SubTasks.Add(subtask);
        project.Tasks.Add(task);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTaskById(task.Id, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
        Assert.Equal(task.Title, result.Title);
        Assert.Equal(1, result.SubTasks.Count);
    }

    [Fact]
    public async Task AddTask_WithValidProjectId_ShouldAddTaskAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var task = CreateTestTask(project.Id);

        // Act
        var result = await _repository.AddTask(task, project.Id, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.NotNull(storedTask);
        Assert.Equal(task.Title, storedTask.Title);
        Assert.Equal(project.Id, storedTask.ProjectId);
    }

    [Fact]
    public async Task AddTask_WithInvalidProjectId_ShouldReturnFalse()
    {
        // Arrange
        var invalidProjectId = Guid.NewGuid();
        var task = CreateTestTask(invalidProjectId);

        // Act
        var result = await _repository.AddTask(task, invalidProjectId, _cancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTask_WithValidTask_ShouldUpdateTaskAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var task = CreateTestTask(project.Id);

        project.Tasks.Add(task);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Update the task
        var updatedTask = new ProjectTask
        {
            Id = task.Id,
            Title = "Updated Title",
            Description = "Updated Description",
            Status = "Done",
            ProjectId = project.Id,
            ColumnPosition = 2.0
        };

        // Act
        var result = await _repository.UpdateTask(updatedTask, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.NotNull(storedTask);
        Assert.Equal("Updated Title", storedTask.Title);
        Assert.Equal("Updated Description", storedTask.Description);
        Assert.Equal("Done", storedTask.Status);
        Assert.Equal(2.0, storedTask.ColumnPosition);
    }

    [Fact]
    public async Task DeleteTask_WithValidId_ShouldDeleteTaskAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var task = CreateTestTask(project.Id);

        project.Tasks.Add(task);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteTask(task.Id, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.Null(storedTask);
    }

    [Fact]
    public async Task DeleteTask_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteTask(invalidId, _cancellationToken);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Milestone Tests

    [Fact]
    public async Task AddMilestone_WithValidProjectId_ShouldAddMilestoneAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var milestone = CreateTestMilestone(project.Id);

        // Act
        var result = await _repository.AddMilestone(milestone, project.Id, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedMilestone = await _context.Milestones.FindAsync(milestone.Id);
        Assert.NotNull(storedMilestone);
        Assert.Equal(milestone.Name, storedMilestone.Name);
        Assert.Equal(project.Id, storedMilestone.ProjectId);
    }

    [Fact]
    public async Task AddMilestone_WithInvalidProjectId_ShouldReturnFalse()
    {
        // Arrange
        var invalidProjectId = Guid.NewGuid();
        var milestone = CreateTestMilestone(invalidProjectId);

        // Act
        var result = await _repository.AddMilestone(milestone, invalidProjectId, _cancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateMilestone_WithValidMilestone_ShouldUpdateMilestoneAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var milestone = CreateTestMilestone(project.Id);

        project.Milestones.Add(milestone);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Update the milestone
        var updatedMilestone = new Milestone
        {
            Id = milestone.Id,
            Name = "Updated Milestone",
            ProjectId = project.Id
        };

        // Act
        var result = await _repository.UpdateMilestone(updatedMilestone, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedMilestone = await _context.Milestones.FindAsync(milestone.Id);
        Assert.NotNull(storedMilestone);
        Assert.Equal("Updated Milestone", storedMilestone.Name);
    }

    [Fact]
    public async Task DeleteMilestone_WithValidId_ShouldDeleteMilestoneAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var milestone = CreateTestMilestone(project.Id);

        project.Milestones.Add(milestone);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteMilestone(milestone.Id, false, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedMilestone = await _context.Milestones.FindAsync(milestone.Id);
        Assert.Null(storedMilestone);
    }

    [Fact]
    public async Task DeleteMilestone_WithTasksAndDeleteTasksTrue_ShouldDeleteMilestoneAndTasksAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var milestone = CreateTestMilestone(project.Id);
        var task = CreateTestTask(project.Id);

        task.MilestoneId = milestone.Id;
        task.Milestone = milestone;
        milestone.Tasks.Add(task);

        project.Milestones.Add(milestone);
        project.Tasks.Add(task);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteMilestone(milestone.Id, true, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedMilestone = await _context.Milestones.FindAsync(milestone.Id);
        Assert.Null(storedMilestone);

        var storedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.Null(storedTask);
    }

    [Fact]
    public async Task DeleteMilestone_WithTasksAndDeleteTasksFalse_ShouldDetachTasksFromMilestone()
    {
        // Arrange
        var project = CreateTestProject();
        var milestone = CreateTestMilestone(project.Id);
        var task = CreateTestTask(project.Id);

        task.MilestoneId = milestone.Id;
        task.Milestone = milestone;
        milestone.Tasks.Add(task);

        project.Milestones.Add(milestone);
        project.Tasks.Add(task);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteMilestone(milestone.Id, false, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedMilestone = await _context.Milestones.FindAsync(milestone.Id);
        Assert.Null(storedMilestone);

        var storedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.NotNull(storedTask);
        Assert.Null(storedTask.MilestoneId);
    }

    #endregion

    #region SubTask Tests

    [Fact]
    public async Task AddSubTask_WithValidTaskId_ShouldAddSubTaskAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var task = CreateTestTask(project.Id);

        project.Tasks.Add(task);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var subtask = CreateTestSubTask(task.Id);

        // Act
        var result = await _repository.AddSubTask(subtask, task.Id, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedSubtask = await _context.SubTasks.FindAsync(subtask.Id);
        Assert.NotNull(storedSubtask);
        Assert.Equal(subtask.Title, storedSubtask.Title);
        Assert.Equal(task.Id, storedSubtask.ProjectTaskId);
    }

    [Fact]
    public async Task AddSubTask_WithInvalidTaskId_ShouldReturnFalse()
    {
        // Arrange
        var invalidTaskId = Guid.NewGuid();
        var subtask = CreateTestSubTask(invalidTaskId);

        // Act
        var result = await _repository.AddSubTask(subtask, invalidTaskId, _cancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateSubTask_WithValidSubTask_ShouldUpdateSubTaskAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var task = CreateTestTask(project.Id);
        var subtask = CreateTestSubTask(task.Id);

        task.SubTasks.Add(subtask);
        project.Tasks.Add(task);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Update the subtask
        subtask.Title = "Updated SubTask";
        subtask.IsCompleted = true;
        subtask.Position = 20;

        // Act
        var result = await _repository.UpdateSubTask(subtask, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedSubtask = await _context.SubTasks.FindAsync(subtask.Id);
        Assert.NotNull(storedSubtask);
        Assert.Equal("Updated SubTask", storedSubtask.Title);
        Assert.True(storedSubtask.IsCompleted);
        Assert.Equal(20, storedSubtask.Position);
    }

    [Fact]
    public async Task DeleteSubTask_WithValidId_ShouldDeleteSubTaskAndReturnTrue()
    {
        // Arrange
        var project = CreateTestProject();
        var task = CreateTestTask(project.Id);
        var subtask = CreateTestSubTask(task.Id);

        task.SubTasks.Add(subtask);
        project.Tasks.Add(task);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteSubTask(subtask.Id, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedSubtask = await _context.SubTasks.FindAsync(subtask.Id);
        Assert.Null(storedSubtask);
    }

    #endregion

    #region QuickTodo Tests

    [Fact]
    public async Task GetQuickTodos_ShouldReturnAllQuickTodos()
    {
        // Arrange
        var todo1 = CreateTestQuickTodo("Todo 1");
        var todo2 = CreateTestQuickTodo("Todo 2");

        _context.QuickTodos.Add(todo1);
        _context.QuickTodos.Add(todo2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetQuickTodos(_cancellationToken);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Id == todo1.Id);
        Assert.Contains(result, t => t.Id == todo2.Id);
    }

    [Fact]
    public async Task AddQuickTodo_ShouldAddQuickTodoAndReturnId()
    {
        // Arrange
        var todo = CreateTestQuickTodo();

        // Act
        var result = await _repository.AddQuickTodo(todo, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todo.Id, result);

        var storedTodo = await _context.QuickTodos.FindAsync(todo.Id);
        Assert.NotNull(storedTodo);
        Assert.Equal(todo.Title, storedTodo.Title);
    }

    [Fact]
    public async Task UpdateQuickTodo_ShouldUpdateQuickTodoAndReturnTrue()
    {
        // Arrange
        var todo = CreateTestQuickTodo();
        _context.QuickTodos.Add(todo);
        await _context.SaveChangesAsync();

        // Update the todo
        todo.Title = "Updated Todo";
        todo.IsCompleted = true;
        todo.DateCompleted = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateQuickTodo(todo, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedTodo = await _context.QuickTodos.FindAsync(todo.Id);
        Assert.NotNull(storedTodo);
        Assert.Equal("Updated Todo", storedTodo.Title);
        Assert.True(storedTodo.IsCompleted);
        Assert.NotNull(storedTodo.DateCompleted);
    }

    [Fact]
    public async Task DeleteQuickTodo_WithValidId_ShouldDeleteQuickTodoAndReturnTrue()
    {
        // Arrange
        var todo = CreateTestQuickTodo();
        _context.QuickTodos.Add(todo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteQuickTodo(todo.Id, _cancellationToken);

        // Assert
        Assert.True(result);

        var storedTodo = await _context.QuickTodos.FindAsync(todo.Id);
        Assert.Null(storedTodo);
    }

    [Fact]
    public async Task DeleteQuickTodo_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteQuickTodo(invalidId, _cancellationToken);

        // Assert
        Assert.False(result);
    }

    #endregion
}