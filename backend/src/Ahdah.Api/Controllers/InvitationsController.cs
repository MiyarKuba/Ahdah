using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Access.Services;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/invitations")]
public sealed class InvitationsController(IInvitationService invitationService) : ControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpPost]
    [ProducesResponseType<CreateInvitationResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateInvitationResult>> Create(
        CreateInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await invitationService.CreateAsync(request, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => StatusCode(StatusCodes.Status201Created, result.Value),
            AccessResultStatus.Invalid => AccessProblem(
                StatusCodes.Status400BadRequest,
                "Invalid invitation",
                "The invitation details are invalid.",
                "invitations.invalid_request"),
            AccessResultStatus.Conflict => AccessProblem(
                StatusCodes.Status409Conflict,
                "Invitation conflict",
                "An invitation cannot be created with the supplied details.",
                "invitations.creation_conflict"),
            AccessResultStatus.Unauthorized => AccessProblem(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The access token is no longer valid for an active manager and company.",
                "authentication.stale_token"),
            _ => UnexpectedProblem()
        };
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpGet]
    [ProducesResponseType<PagedResult<InvitationSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<InvitationSummary>>> List(
        [FromQuery] InvitationQuery query,
        CancellationToken cancellationToken)
    {
        var result = await invitationService.ListAsync(query, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => AccessProblem(
                StatusCodes.Status400BadRequest,
                "Invalid invitation query",
                "The invitation query is invalid.",
                "invitations.invalid_query"),
            AccessResultStatus.Unauthorized => AccessProblem(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The access token is no longer valid for an active manager and company.",
                "authentication.stale_token"),
            _ => UnexpectedProblem()
        };
    }

    [AllowAnonymous]
    [HttpPost("accept")]
    [ProducesResponseType<InvitationAcceptanceResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InvitationAcceptanceResult>> Accept(
        AcceptInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await invitationService.AcceptAsync(request, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => AccessProblem(
                StatusCodes.Status400BadRequest,
                "Invalid invitation",
                "The invitation token is invalid or cannot be accepted.",
                "invitations.invalid_token"),
            AccessResultStatus.Conflict => AccessProblem(
                StatusCodes.Status409Conflict,
                "Invitation conflict",
                "The invitation cannot be accepted with the supplied account details.",
                "invitations.acceptance_conflict"),
            _ => UnexpectedProblem()
        };
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpPost("{invitationId:guid}/cancel")]
    [ProducesResponseType<InvitationSummary>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InvitationSummary>> Cancel(
        Guid invitationId,
        CancellationToken cancellationToken)
    {
        var result = await invitationService.CancelAsync(invitationId, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => AccessProblem(
                StatusCodes.Status400BadRequest,
                "Invalid invitation transition",
                "The invitation cannot be cancelled in its current state.",
                "invitations.invalid_transition"),
            AccessResultStatus.NotFound => AccessProblem(
                StatusCodes.Status404NotFound,
                "Invitation not found",
                "The invitation was not found.",
                "invitations.not_found"),
            AccessResultStatus.Conflict => AccessProblem(
                StatusCodes.Status409Conflict,
                "Invitation conflict",
                "The invitation was changed by another request.",
                "invitations.concurrency_conflict"),
            AccessResultStatus.Unauthorized => AccessProblem(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The access token is no longer valid for an active manager and company.",
                "authentication.stale_token"),
            _ => UnexpectedProblem()
        };
    }

    private ObjectResult AccessProblem(int status, string title, string detail, string code) =>
        Problem(
            statusCode: status,
            title: title,
            detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code });

    private ObjectResult UnexpectedProblem() => AccessProblem(
        StatusCodes.Status500InternalServerError,
        "Unexpected server error",
        "An unexpected error occurred.",
        "server.unexpected_error");
}
