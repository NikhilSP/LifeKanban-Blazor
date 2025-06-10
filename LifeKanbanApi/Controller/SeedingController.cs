using LifeKanbanApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace LifeKanbanApi.Controller
{
    [ApiController]
    [Route("")]
    public class SeedingController : ControllerBase
    {
        private readonly SeedDataService _seedService;
        private readonly ILogger<SeedingController> _logger;

        public SeedingController(SeedDataService seedService, ILogger<SeedingController> logger)
        {
            _seedService = seedService;
            _logger = logger;
        }

        [HttpPost("initializeApp")]
        public async Task<IActionResult> InitializeApp(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Initializing app with seed data...");
                
                await _seedService.SeedDefaultDataAsync();
                await _seedService.SeedQuickTodosAsync();
                
                _logger.LogInformation("App initialization completed successfully");
                
                return Ok(new { 
                    Success = true, 
                    Message = "App initialized successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during app initialization");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = "Failed to initialize app",
                    Error = ex.Message 
                });
            }
        }

        [HttpGet("appStatus")]
        public async Task<IActionResult> GetAppStatus(CancellationToken cancellationToken = default)
        {
            try
            {
                // This endpoint can be used to check if the app has any data
                var projectCount = await _seedService.GetProjectCount();
                var quickTodoCount = await _seedService.GetQuickTodoCount();
                
                return Ok(new { 
                    HasData = projectCount > 0 || quickTodoCount > 0,
                    ProjectCount = projectCount,
                    QuickTodoCount = quickTodoCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking app status");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = "Failed to check app status" 
                });
            }
        }
    }
}