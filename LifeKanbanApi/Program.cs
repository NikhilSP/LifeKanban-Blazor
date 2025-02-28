using Carter;
using LifeKanbanApi.Data;

namespace LifeKanbanApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var assembly = typeof(Program).Assembly;
        builder.Services.AddCarter();
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });
        
        builder.Services.AddScoped<ProjectRepository>();
        
        var app = builder.Build();
        app.MapCarter();

        app.Run();
    }
}