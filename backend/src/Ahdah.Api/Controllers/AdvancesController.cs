using Ahdah.Application.Access.Models;
using Ahdah.Application.Advances;
using Ahdah.Application.Advances.Contracts;
using Ahdah.Application.Advances.Models;
using Ahdah.Application.Advances.Services;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class AdvancesController(IAdvanceService advanceService) : ControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceViewer)]
    [HttpGet("advances")]
    [ProducesResponseType<PagedResult<AdvanceSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<AdvanceSummary>>> List(
        [FromQuery] AdvanceQuery query,
        CancellationToken cancellationToken) =>
        AdvanceResult(await advanceService.ListAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceViewer)]
    [HttpGet("advances/{advanceId:guid}")]
    [ProducesResponseType<AdvanceDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdvanceDetails>> Get(
        Guid advanceId,
        CancellationToken cancellationToken) =>
        AdvanceResult(await advanceService.GetAsync(advanceId, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceViewer)]
    [HttpGet("advances/{advanceId:guid}/movements")]
    [ProducesResponseType<PagedResult<AdvanceMovementSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<AdvanceMovementSummary>>> ListMovements(
        Guid advanceId,
        [FromQuery] AdvanceMovementQuery query,
        CancellationToken cancellationToken) =>
        AdvanceResult(await advanceService.ListMovementsAsync(advanceId, query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceCreator)]
    [HttpPost("advances")]
    [ProducesResponseType<AdvanceDetails>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdvanceDetails>> Create(
        CreateAdvanceRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        var result = await advanceService.CreateAsync(request, idempotencyKey, cancellationToken);
        return result.Status == AccessResultStatus.Success
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : AdvanceResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceDistributor)]
    [HttpPost("advances/{advanceId:guid}/distributions")]
    [ProducesResponseType<AdvanceTransferDetails>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdvanceTransferDetails>> Distribute(
        Guid advanceId,
        CreateAdvanceDistributionRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        var result = await advanceService.DistributeAsync(
            advanceId,
            request,
            idempotencyKey,
            cancellationToken);
        return result.Status == AccessResultStatus.Success
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : AdvanceResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceParticipant)]
    [HttpPost("advances/{advanceId:guid}/returns")]
    [ProducesResponseType<AdvanceTransferDetails>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdvanceTransferDetails>> Return(
        Guid advanceId,
        CreateAdvanceReturnRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        var result = await advanceService.ReturnAsync(
            advanceId,
            request,
            idempotencyKey,
            cancellationToken);
        return result.Status == AccessResultStatus.Success
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : AdvanceResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceParticipant)]
    [HttpPost("advance-transfers/{transferId:guid}/confirm")]
    [ProducesResponseType<AdvanceTransferDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdvanceTransferDetails>> ConfirmTransfer(
        Guid transferId,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        return AdvanceResult(await advanceService.ConfirmTransferAsync(
            transferId,
            idempotencyKey,
            cancellationToken));
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceParticipant)]
    [HttpPost("advance-transfers/{transferId:guid}/reject")]
    [ProducesResponseType<AdvanceTransferDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdvanceTransferDetails>> RejectTransfer(
        Guid transferId,
        RejectAdvanceTransferRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        return AdvanceResult(await advanceService.RejectTransferAsync(
            transferId,
            request,
            idempotencyKey,
            cancellationToken));
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceViewer)]
    [HttpGet("advance-balances/me")]
    [ProducesResponseType<AdvanceBalancePage>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AdvanceBalancePage>> GetMyBalances(
        [FromQuery] AdvanceBalanceQuery query,
        CancellationToken cancellationToken) =>
        AdvanceResult(await advanceService.GetMyBalancesAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceBalanceViewer)]
    [HttpGet("advance-balances/users/{userId:guid}")]
    [ProducesResponseType<AdvanceBalancePage>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdvanceBalancePage>> GetUserBalances(
        Guid userId,
        [FromQuery] AdvanceBalanceQuery query,
        CancellationToken cancellationToken) =>
        AdvanceResult(await advanceService.GetUserBalancesAsync(userId, query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.AdvanceCreator)]
    [HttpGet("advance-funding-sources")]
    [ProducesResponseType<PagedResult<AvailableFundingSourceSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<AvailableFundingSourceSummary>>> ListFundingSources(
        [FromQuery] AvailableFundingSourceQuery query,
        CancellationToken cancellationToken) =>
        AdvanceResult(await advanceService.ListAvailableFundingSourcesAsync(query, cancellationToken));

    private ActionResult<T> AdvanceResult<T>(AccessResult<T> result) => result.Status switch
    {
        AccessResultStatus.Success => Ok(result.Value),
        AccessResultStatus.Invalid => AdvanceProblem(
            StatusCodes.Status400BadRequest,
            "Invalid advance operation",
            "The advance operation is invalid or unsupported in the current lifecycle state.",
            "advances.invalid_operation"),
        AccessResultStatus.NotFound => AdvanceProblem(
            StatusCodes.Status404NotFound,
            "Advance operation unavailable",
            "The requested advance operation was not found.",
            "advances.not_found"),
        AccessResultStatus.Conflict => AdvanceProblem(
            StatusCodes.Status409Conflict,
            "Advance conflict",
            "The advance operation conflicts with the current balance or lifecycle state.",
            "advances.conflict"),
        AccessResultStatus.Forbidden => AdvanceProblem(
            StatusCodes.Status403Forbidden,
            "Forbidden",
            "You are not authorized to perform this advance operation.",
            "advances.forbidden"),
        AccessResultStatus.Unauthorized => AdvanceProblem(
            StatusCodes.Status401Unauthorized,
            "Unauthorized",
            "The access token is no longer valid for an active company member.",
            "authentication.stale_token"),
        _ => AdvanceProblem(
            StatusCodes.Status500InternalServerError,
            "Unexpected server error",
            "An unexpected error occurred.",
            "server.unexpected_error")
    };

    private ObjectResult AdvanceProblem(int status, string title, string detail, string code) =>
        Problem(
            statusCode: status,
            title: title,
            detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code });

    private ObjectResult InvalidIdempotencyKey() => AdvanceProblem(
        StatusCodes.Status400BadRequest,
        "Invalid idempotency key",
        "A 16 to 200 character Idempotency-Key header is required for financial commands.",
        "advances.invalid_idempotency_key");
}
