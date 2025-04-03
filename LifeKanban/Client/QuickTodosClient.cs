// LifeKanban/Client/QuickTodosClient.cs
using System.Text.Json;
using LifeKanban.Model;

namespace LifeKanban.Client;

public class QuickTodosClient(HttpClient httpClient)
{
    public async Task<List<QuickTodoItem>> GetQuickTodos()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true 
        };

        try
        {
            var res = await httpClient.GetFromJsonAsync<GetQuickTodosResponse>("quickTodos", options);
            return res?.QuickTodos ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting quick todos: {ex.Message}");
            return [];
        }
    }

    public async Task<Guid?> AddQuickTodo(QuickTodoItem todoItem)
    {
        try
        {
            var request = new { QuickTodo = todoItem };
            var response = await httpClient.PostAsJsonAsync("/addQuickTodo", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IdResponse>();
                return result!.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding quick todo: {ex.Message}");
            return null;
        }
    }
    
    public async Task<bool> UpdateQuickTodo(QuickTodoItem todoItem)
    {
        try
        {
            var request = new { QuickTodo = todoItem };
            var response = await httpClient.PostAsJsonAsync("/updateQuickTodo", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<BoolResponse>();
                return result?.IsSuccess ?? false;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating quick todo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteQuickTodo(Guid id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"/deleteQuickTodo/{id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<BoolResponse>();
                return result?.IsSuccess ?? false;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting quick todo: {ex.Message}");
            return false;
        }
    }
}

public record GetQuickTodosResponse(List<QuickTodoItem> QuickTodos);