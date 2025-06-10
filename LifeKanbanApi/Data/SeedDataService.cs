using LifeKanbanApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LifeKanbanApi.Data;

public class SeedDataService
{
    private readonly ProjectDbContext _context;
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(ProjectDbContext context, ILogger<SeedDataService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Remove the automatic checks - we'll call this only when needed
    public async Task SeedDefaultDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting data seeding...");

            // Create sample projects
            var projects = CreateSampleProjects();
            
            _logger.LogInformation($"Created {projects.Count} sample projects");
            
            await _context.Projects.AddRangeAsync(projects);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully seeded default project data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding default data");
            throw;
        }
    }

    public async Task SeedQuickTodosAsync()
    {
        try
        {
            _logger.LogInformation("Starting quick todos seeding...");

            var quickTodos = new List<QuickTodo>
            {
                new QuickTodo
                {
                    Id = Guid.NewGuid(),
                    Title = "Welcome! Try adding your own quick todo",
                    IsCompleted = false,
                    DateCreated = DateTime.UtcNow
                },
                new QuickTodo
                {
                    Id = Guid.NewGuid(),
                    Title = "Check out the projects section",
                    IsCompleted = false,
                    DateCreated = DateTime.UtcNow.AddMinutes(-30)
                }
            };

            await _context.QuickTodos.AddRangeAsync(quickTodos);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Successfully seeded {quickTodos.Count} quick todos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding quick todos");
            throw;
        }
    }

    // Helper methods for the controller
    public async Task<int> GetProjectCount()
    {
        return await _context.Projects.CountAsync();
    }

    public async Task<int> GetQuickTodoCount()
    {
        return await _context.QuickTodos.CountAsync();
    }

    private List<Project> CreateSampleProjects()
    {
        var welcomeProjectId = Guid.NewGuid();
        var personalProjectId = Guid.NewGuid();
        var learningProjectId = Guid.NewGuid();

        // Welcome/Getting Started Project
        var welcomeProject = new Project
        {
            Id = welcomeProjectId,
            Name = "🚀 Welcome to LifeKanban",
            Description = "Get started with your personal project management system. This project shows you the basics!",
            Position = 10,
            Tasks = new List<ProjectTask>(),
            Milestones = new List<Milestone>()
        };

        // Personal Development Project
        var personalProject = new Project
        {
            Id = personalProjectId,
            Name = "🌱 Personal Development",
            Description = "Track your personal growth, habits, and self-improvement goals.",
            Position = 20,
            Tasks = new List<ProjectTask>(),
            Milestones = new List<Milestone>()
        };

        // Learning Project
        var learningProject = new Project
        {
            Id = learningProjectId,
            Name = "📚 Learning & Skills",
            Description = "Organize your learning goals, courses, and skill development.",
            Position = 30,
            Tasks = new List<ProjectTask>(),
            Milestones = new List<Milestone>()
        };

        // Add milestones
        var welcomeMilestone = new Milestone
        {
            Id = Guid.NewGuid(),
            Name = "Setup Complete",
            ProjectId = welcomeProjectId,
            Tasks = new List<ProjectTask>()
        };

        var personalMilestone = new Milestone
        {
            Id = Guid.NewGuid(),
            Name = "Month 1 Goals",
            ProjectId = personalProjectId,
            Tasks = new List<ProjectTask>()
        };

        var learningMilestone = new Milestone
        {
            Id = Guid.NewGuid(),
            Name = "Q1 Learning Goals",
            ProjectId = learningProjectId,
            Tasks = new List<ProjectTask>()
        };

        welcomeProject.Milestones.Add(welcomeMilestone);
        personalProject.Milestones.Add(personalMilestone);
        learningProject.Milestones.Add(learningMilestone);

        // Add sample tasks to Welcome Project
        var task1 = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "Explore the Interface",
            Description = "Take a tour of LifeKanban's features: projects, tasks, milestones, and quick todos.",
            Status = "To Do",
            ColumnPosition = 1,
            ProjectId = welcomeProjectId,
            MilestoneId = welcomeMilestone.Id,
            SubTasks = new List<SubTask>
            {
                new SubTask
                {
                    Id = Guid.NewGuid(),
                    Title = "Check out the project view",
                    IsCompleted = false,
                    Position = 10
                },
                new SubTask
                {
                    Id = Guid.NewGuid(),
                    Title = "Try creating a new task",
                    IsCompleted = false,
                    Position = 20
                },
                new SubTask
                {
                    Id = Guid.NewGuid(),
                    Title = "Add a quick todo",
                    IsCompleted = false,
                    Position = 30
                }
            }
        };

        var task2 = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "Organize Your First Project",
            Description = "Create your own project and start adding tasks that matter to you.",
            Status = "To Do",
            ColumnPosition = 2,
            ProjectId = welcomeProjectId,
            MilestoneId = welcomeMilestone.Id,
            SubTasks = new List<SubTask>()
        };

        var task3 = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "Learn About Drag & Drop",
            Description = "Tasks can be moved between different status columns and reordered within columns.",
            Status = "In Progress",
            ColumnPosition = 1,
            ProjectId = welcomeProjectId,
            SubTasks = new List<SubTask>()
        };

        // Add sample tasks to Personal Development Project
        var personalTask1 = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "Morning Routine",
            Description = "Establish a consistent morning routine to start each day productively.",
            Status = "To Do",
            ColumnPosition = 1,
            ProjectId = personalProjectId,
            MilestoneId = personalMilestone.Id,
            SubTasks = new List<SubTask>
            {
                new SubTask
                {
                    Id = Guid.NewGuid(),
                    Title = "Wake up at 6:30 AM",
                    IsCompleted = false,
                    Position = 10
                },
                new SubTask
                {
                    Id = Guid.NewGuid(),
                    Title = "10 minutes meditation",
                    IsCompleted = false,
                    Position = 20
                },
                new SubTask
                {
                    Id = Guid.NewGuid(),
                    Title = "Review daily goals",
                    IsCompleted = false,
                    Position = 30
                }
            }
        };

        var personalTask2 = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "Exercise Plan",
            Description = "Create and stick to a regular exercise routine.",
            Status = "To Do",
            ColumnPosition = 2,
            ProjectId = personalProjectId,
            SubTasks = new List<SubTask>()
        };

        // Add sample tasks to Learning Project
        var learningTask1 = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "Learn a New Programming Language",
            Description = "Pick a programming language you've always wanted to learn and create a study plan.",
            Status = "To Do",
            ColumnPosition = 1,
            ProjectId = learningProjectId,
            MilestoneId = learningMilestone.Id,
            SubTasks = new List<SubTask>()
        };

        var learningTask2 = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "Online Course Progress",
            Description = "Track progress on online courses and certifications.",
            Status = "To Do",
            ColumnPosition = 2,
            ProjectId = learningProjectId,
            SubTasks = new List<SubTask>()
        };

        // Add tasks to projects
        welcomeProject.Tasks.AddRange(new[] { task1, task2, task3 });
        personalProject.Tasks.AddRange(new[] { personalTask1, personalTask2 });
        learningProject.Tasks.AddRange(new[] { learningTask1, learningTask2 });

        // Set navigation properties for subtasks
        foreach (var task in welcomeProject.Tasks.Concat(personalProject.Tasks).Concat(learningProject.Tasks))
        {
            foreach (var subtask in task.SubTasks)
            {
                subtask.ProjectTaskId = task.Id;
            }
        }

        return new List<Project> { welcomeProject, personalProject, learningProject };
    }
}