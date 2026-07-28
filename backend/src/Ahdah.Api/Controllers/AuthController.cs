using Ahdah.Application.Identity;
using Ahdah.Application.Identity.Contracts;
using Ahdah.Application.Identity.Models;
using Ahdah.Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register-company")]
    [ProducesResponseType<RegisterCompanyResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterCompanyResult>> RegisterCompany(
        RegisterCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterCompanyAsync(request, cancellationToken);

        return result.Status switch
        {
            IdentityResultStatus.Success => StatusCode(StatusCodes.Status201Created, result.Value),
            IdentityResultStatus.Conflict => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Registration conflict",
                detail: "The company or manager cannot be registered with the supplied details.",
                extensions: ProblemExtensions("identity.registration_conflict")),
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected server error",
                detail: "An unexpected error occurred.",
                extensions: ProblemExtensions("server.unexpected_error"))
        };
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthenticationResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResult>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await identityService.LoginAsync(request, cancellationToken);

        return result.Status == IdentityResultStatus.Success
            ? Ok(result.Value)
            : Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid credentials",
                detail: "The supplied credentials are invalid.",
                extensions: ProblemExtensions("authentication.invalid_credentials"));
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.CompanyMember)]
    [HttpGet("me")]
    [ProducesResponseType<CurrentUserResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CurrentUserResult>> Me(CancellationToken cancellationToken)
    {
        var result = await identityService.GetCurrentUserAsync(cancellationToken);

        return result.Status == IdentityResultStatus.Success
            ? Ok(result.Value)
            : Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: "The access token is no longer valid for an active user and company.",
                extensions: ProblemExtensions("authentication.stale_token"));
    }

    private static Dictionary<string, object?> ProblemExtensions(string code) => new()
    {
        ["code"] = code
    };
}
