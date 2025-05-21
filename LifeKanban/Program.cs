using LifeKanban.Client;
using LifeKanban.Components;
using LifeKanban.Services;
using LifeKanban.StateManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.
    Services.
    AddRazorComponents().
    AddInteractiveServerComponents();

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5190/") });

builder.Services.AddSingleton<ProjectsClient>();
builder.Services.AddSingleton<ProjectStateService>();
builder.Services.AddSingleton<QuickTodosClient>();
builder.Services.AddSingleton<KanbanService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();