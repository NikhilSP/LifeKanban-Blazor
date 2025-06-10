using LifeKanbanApi.Config;
using LifeKanbanApi.Data;
using LifeKanbanApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace LifeKanbanApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var assembly = typeof(Program).Assembly;
        
        // Add services
        builder.Services.AddControllers();
        builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(assembly); });
        builder.Services.AddDbContext<ProjectDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Database")));

        // Register repositories and services
        builder.Services.AddScoped<ProjectRepository>();
        builder.Services.AddScoped<SeedDataService>();
        
        // Configure Mapster
        MapsterConfig.Configure();
        
        var app = builder.Build();

        // Only ensure database exists - no automatic seeding
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ProjectDbContext>();
            var repository = services.GetRequiredService<ProjectRepository>();
            
            // Ensure database is created
            context.Database.EnsureCreated();
            
            // Initialize project positions for existing data
            repository.InitializeProjectPositions().GetAwaiter().GetResult();
        }

        // Configure middleware
        app.MapControllers();
        app.Run();
    }
}