using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LifeKanbanApi.Data;

public class ProjectDbContextFactory : IDesignTimeDbContextFactory<ProjectDbContext>
{
    public ProjectDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProjectDbContext>();
            
        // Use the same database provider and connection string that you use in your application
        optionsBuilder.UseSqlite("Data Source=Data/ProductDB");
            
        return new ProjectDbContext(optionsBuilder.Options);
    }
}