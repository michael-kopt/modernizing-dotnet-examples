using Microsoft.AspNetCore.Mvc;
using WebApplicationMigrationSample.Services;

namespace WebApplicationMigrationSample.Controllers;

[ApiController]
[Route("api")]
public sealed class PingController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        var middlewareSeen = HttpContext.Items.TryGetValue(ApiLoggingMiddleware.FlagKey, out var value)
            && value is true;

        return Ok(new
        {
            message = "pong",
            middlewareSeen
        });
    }
}
