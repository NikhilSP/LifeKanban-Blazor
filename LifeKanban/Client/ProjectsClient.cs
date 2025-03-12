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

        return res?.Projects ?? [];
    }

    public async Task<ProjectItem?> GetProjectById(Guid id)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var res = await httpClient.GetFromJsonAsync<GetProjectResponse>($"project/{id}", options);

            return res!.Project;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Guid?> AddProject(ProjectItem newProjectItem)
    {
        var request = new CreateProjectRequest(newProjectItem);

        var response = await httpClient.PostAsJsonAsync("/addProject", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CreateProjectResponse>();
            return result!.Id;
        }

        return null;
    }

    public async Task<Guid?> AddTask(ProjectTaskItem newProjectItem, Guid projectId)
    {
        try
        {
            var request = new AddTaskRequest(newProjectItem, projectId);

            var response = await httpClient.PostAsJsonAsync("/addTask", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AddTaskResponse>();
                return result!.Id;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<Guid?> UpdateTask(ProjectTaskItem taskItem, Guid projectId)
    {
        try
        {
            var request = new UpdateTaskRequest(taskItem, projectId);

            var response = await httpClient.PostAsJsonAsync("/updateTask", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AddTaskResponse>();
                return result!.Id;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Guid?> AddMilestone(MilestoneItem newProjectItem, Guid projectId)
    {
        try
        {
            var request = new AddMilestoneRequest(newProjectItem, projectId);

            var response = await httpClient.PostAsJsonAsync("/addMilestone", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AddMilestoneResponse>();
                return result!.Id;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
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
    
    public async Task<bool> DeleteTask(Guid taskId)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"/deleteTask/{taskId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DeleteTaskResponse>();
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