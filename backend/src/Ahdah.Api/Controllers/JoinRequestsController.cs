using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Access.Services;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/join-requests")]
public sealed class JoinRequestsController(IJoinRequestService joinRequestService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType<JoinRequestSubmissionResult>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<JoinRequestSubmissionResult>> Submit(
        SubmitJoinRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await joinRequestService.SubmitAsync(request, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => StatusCode(StatusCodes.Status202Accepted, result.Value),
            AccessResultStatus.Invalid => AccessProblem(
                StatusCodes.Status400BadRequest,
                "Join request unavailable",
                "The join request cannot be submitted with the supplied details.",
                "join_requests.submission_unavailable"),
            AccessResultStatus.Conflict => AccessProblem(
                StatusCodes.Status409Conflict,
                "Join-request conflict",
                "The join request cannot be submitted with the supplied details.",
                "join_requests.submission_conflict"),
            _ => UnexpectedProblem()
        };
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpGet]
    [ProducesResponseType<PagedResult<JoinRequestSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<JoinRequestSummary>>> List(
        [FromQuery] JoinRequestQuery query,
        CancellationToken cancellationToken)
    {
        var result = await joinRequestService.ListAsync(query, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => AccessProblem(
                StatusCodes.Status400BadRequest,
                "Invalid join-request query",
                "The join-request query is invalid.",
                "join_requests.invalid_query"),
            AccessResultStatus.Unauthorized => AccessProblem(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The access token is no longer valid for an active manager and company.",
                "authentication.stale_token"),
            _ => UnexpectedProblem()
        };
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpPost("{joinRequestId:guid}/approve")]
    [ProducesResponseType<JoinRequestDecisionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<JoinRequestDecisionResult>> Approve(
        Guid joinRequestId,
        ApproveJoinRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await joinRequestService.ApproveAsync(joinRequestId, request, cancellationToken);
        return DecisionResult(result, "approved");
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpPost("{joinRequestId:guid}/reject")]
    [ProducesResponseType<JoinRequestDecisionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<JoinRequestDecisionResult>> Reject(
        Guid joinRequestId,
        RejectJoinRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await joinRequestService.RejectAsync(joinRequestId, request, cancellationToken);
        return DecisionResult(result, "rejected");
    }

    private ActionResult<JoinRequestDecisionResult> DecisionResult(
        AccessResult<JoinRequestDecisionResult> result,
        string operation) => result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => AccessProblem(
                StatusCodes.Status400BadRequest,
                "Invalid join-request transition",
                $"The join request cannot be {operation} with the supplied details.",
                "join_requests.invalid_transition"),
            AccessResultStatus.NotFound => AccessProblem(
                StatusCodes.Status404NotFound,
                "Join request not found",
                "The join request was not found.",
                "join_requests.not_found"),
            AccessResultStatus.Conflict => AccessProblem(
                StatusCodes.Status409Conflict,
                "Join-request conflict",
                "The join request was changed by another request.",
                "join_requests.concurrency_conflict"),
            AccessResultStatus.Unauthorized => AccessProblem(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The access token is no longer valid for an active manager and company.",
                "authentication.stale_token"),
            _ => UnexpectedProblem()
        };

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
