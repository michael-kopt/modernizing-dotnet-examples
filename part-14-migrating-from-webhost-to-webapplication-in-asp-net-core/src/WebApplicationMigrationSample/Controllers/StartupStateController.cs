using Microsoft.AspNetCore.Mvc;
using WebApplicationMigrationSample.Services;

namespace WebApplicationMigrationSample.Controllers;

[ApiController]
[Route("startup")]
public sealed class StartupStateController(StartupState state) : ControllerBase
{
    [HttpGet("state")]
    public IActionResult GetState()
    {
        return Ok(new
        {
            initialized = state.Initialized,
            initializedAtUtc = state.InitializedAtUtc
        });
    }
}
