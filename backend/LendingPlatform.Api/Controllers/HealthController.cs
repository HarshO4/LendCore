using Microsoft.AspNetCore.Mvc;

namespace LendingPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "online", timestamp = DateTime.UtcNow });
    }
}
