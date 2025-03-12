using Carter;
using LifeKanbanApi.Config;
using LifeKanbanApi.Data;
using LifeKanbanApi.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LifeKanbanApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var assembly = typeof(Program).Assembly;
        builder.Services.AddCarter();
        builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(assembly); });
        builder.Services.AddDbContext<ProjectDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Database")));

        builder.Services.AddScoped<ProjectRepository>();
        MapsterConfig.Configure();
        var app = builder.Build();
        app.MapCarter();

        app.Run();
    }
}