using System.Text.Json;
using LifeKanban.Model;

namespace LifeKanban.Client;

public class ProjectsClient(HttpClient httpClient)
{
    public async Task<List<ProjectItem>> GetProjects()
    {
        var options = new JsonSerializerOptions 
        {
            PropertyNameCaseInsensitive = true // This ignores case differences
        };

        var res = await httpClient.GetFromJsonAsync<GetProjectsResponse>("projects", options);

        return res?.Projects ??[];
    }

    public void AddProject(ProjectItem newProjectItem)
    {
    }
}