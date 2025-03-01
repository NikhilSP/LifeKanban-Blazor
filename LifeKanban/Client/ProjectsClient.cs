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

    public async Task<bool> AddProject(ProjectItem newProjectItem)
    {
        try
        {
            var request = new CreateProjectRequest(newProjectItem);
        
            var response = await httpClient.PostAsJsonAsync("/addProject", request);
        
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateProjectResponse>();
                return result?.IsSuccess ?? false;
            }
        
            return false;
        }
        catch (Exception)
        {
            // Consider logging the exception
            return false;
        }
    }
    
    public async Task<bool> DeleteProjects(Guid id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"/deleteProject/{id}");
        
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DeleteProjectResponse>();
                return result?.IsSuccess ?? false;
            }
        
            return false;
        }
        catch (Exception)
        {
            // Consider logging the exception
            return false;
        }
    }
}