using LifeKanbanApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LifeKanbanApi.Data;

public sealed class ProjectDbContext(DbContextOptions<ProjectDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<SubTask> SubTasks { get; set; }
    public DbSet<QuickTodo> QuickTodos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure one-to-many relationship between Project and Tasks
        modelBuilder.Entity<ProjectTask>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure one-to-many relationship between Project and Milestones
        modelBuilder.Entity<Milestone>()
            .HasOne(m => m.Project)
            .WithMany(p => p.Milestones)
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure one-to-many relationship between Milestone and Tasks
        modelBuilder.Entity<ProjectTask>()
            .HasOne(t => t.Milestone)
            .WithMany(m => m.Tasks)
            .HasForeignKey(t => t.MilestoneId)
            .IsRequired(false); // A task may not have a milestone
            
        // Configure one-to-many relationship between ProjectTask and SubTasks
        modelBuilder.Entity<SubTask>()
            .HasOne(s => s.ProjectTask)
            .WithMany(t => t.SubTasks)
            .HasForeignKey(s => s.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}