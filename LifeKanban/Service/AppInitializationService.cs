using System.Text.Json;
using Microsoft.JSInterop;

namespace LifeKanban.Services;

public class AppInitializationService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AppInitializationService> _logger;
    private const string SEEDED_FLAG_KEY = "lifekanban_app_seeded";
    private bool _isInitialized = false;

    public AppInitializationService(HttpClient httpClient, IJSRuntime jsRuntime, ILogger<AppInitializationService> logger)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task<bool> InitializeAppIfNeeded()
    {
        // Prevent multiple initializations
        if (_isInitialized) return true;

        try
        {
            // Check if app has been seeded before
            var isSeeded = await GetSeededFlag();
            
            if (isSeeded)
            {
                _logger.LogInformation("App was already initialized previously");
                _isInitialized = true;
                return true;
            }

            _logger.LogInformation("First time user detected - initializing app with seed data");

            // Call the seeding API (fire and forget style)
            _ = Task.Run(async () =>
            {
                try
                {
                    var response = await _httpClient.PostAsync("initializeApp", null);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<SeedResponse>(jsonResponse, new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });

                        if (result?.Success == true)
                        {
                            // Mark as seeded in localStorage
                            await SetSeededFlag();
                            _logger.LogInformation("App initialization completed successfully");
                        }
                        else
                        {
                            _logger.LogError("App initialization failed: {Message}", result?.Message);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to initialize app. Status: {StatusCode}", response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background initialization failed");
                }
            });

            // Mark as initialized immediately (don't wait for seeding to complete)
            _isInitialized = true;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during app initialization");
            return false;
        }
    }

    public async Task<bool> GetSeededFlag()
    {
        try
        {
            var flagValue = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", SEEDED_FLAG_KEY);
            return !string.IsNullOrEmpty(flagValue) && bool.TryParse(flagValue, out var result) && result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not read seeded flag from localStorage");
            return false;
        }
    }

    public async Task SetSeededFlag()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", SEEDED_FLAG_KEY, "true");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not set seeded flag in localStorage");
        }
    }

    // Method to manually trigger seeding (for development/testing)
    public async Task<bool> ForceSeedData()
    {
        try
        {
            var response = await _httpClient.PostAsync("initializeApp", null);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SeedResponse>(jsonResponse, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (result?.Success == true)
                {
                    await SetSeededFlag();
                    _logger.LogInformation("Manual seeding completed successfully");
                    return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual seeding");
            return false;
        }
    }

    public async Task ClearSeededFlag()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", SEEDED_FLAG_KEY);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not clear seeded flag from localStorage");
        }
    }

    // For development/testing purposes
    public async Task<AppStatusResponse?> GetAppStatus()
    {
        try
        {
            var response = await _httpClient.GetAsync("appStatus");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AppStatusResponse>(jsonResponse, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting app status");
        }
        
        return null;
    }

    public record SeedResponse(bool Success, string Message, DateTime? Timestamp = null);
    public record AppStatusResponse(bool HasData, int ProjectCount, int QuickTodoCount);
}