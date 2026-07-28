using Ahdah.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController(IHostEnvironment hostEnvironment) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("health")]
    [ProducesResponseType<SystemHealthResponse>(StatusCodes.Status200OK)]
    public ActionResult<SystemHealthResponse> GetHealth()
    {
        var response = new SystemHealthResponse(
            Service: "Ahdah.Api",
            Status: "Healthy",
            TimestampUtc: DateTimeOffset.UtcNow,
            Environment: hostEnvironment.EnvironmentName);

        return Ok(response);
    }
}
