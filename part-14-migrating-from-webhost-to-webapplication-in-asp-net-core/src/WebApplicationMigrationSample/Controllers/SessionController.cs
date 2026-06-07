using Microsoft.AspNetCore.Mvc;

namespace WebApplicationMigrationSample.Controllers;

[ApiController]
[Route("session")]
public sealed class SessionController : ControllerBase
{
    private const string VisitCountKey = "visit-count";

    [HttpGet("visit")]
    public IActionResult Visit()
    {
        var count = HttpContext.Session.GetInt32(VisitCountKey) ?? 0;
        count++;

        HttpContext.Session.SetInt32(VisitCountKey, count);

        return Ok(new
        {
            visits = count
        });
    }
}
