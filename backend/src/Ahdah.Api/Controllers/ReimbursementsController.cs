using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Expenses.Models;
using Ahdah.Application.Expenses.Services;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/reimbursements")]
[Authorize(Policy = AhdahAuthorizationPolicies.ReimbursementViewer)]
public sealed class ReimbursementsController(IExpenseService expenseService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<ReimbursementSummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ReimbursementSummary>>> List(
        [FromQuery] ReimbursementQuery query,
        CancellationToken cancellationToken) =>
        Result(await expenseService.ListReimbursementsAsync(query, cancellationToken));

    [HttpGet("{reimbursementId:guid}")]
    [ProducesResponseType<ReimbursementSummary>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReimbursementSummary>> Get(
        Guid reimbursementId,
        CancellationToken cancellationToken) =>
        Result(await expenseService.GetReimbursementAsync(reimbursementId, cancellationToken));

    private ActionResult<T> Result<T>(AccessResult<T> result) => result.Status switch
    {
        AccessResultStatus.Success => Ok(result.Value),
        AccessResultStatus.Invalid => Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Invalid reimbursement query",
            detail: "The reimbursement query is invalid.",
            extensions: new Dictionary<string, object?> { ["code"] = "reimbursements.invalid" }),
        AccessResultStatus.NotFound => Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Reimbursement unavailable",
            detail: "The requested reimbursement was not found.",
            extensions: new Dictionary<string, object?> { ["code"] = "reimbursements.not_found" }),
        AccessResultStatus.Forbidden => Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Forbidden",
            detail: "You are not authorized to view this reimbursement.",
            extensions: new Dictionary<string, object?> { ["code"] = "reimbursements.forbidden" }),
        AccessResultStatus.Unauthorized => Problem(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Unauthorized",
            detail: "The access token is no longer valid for an active company member.",
            extensions: new Dictionary<string, object?> { ["code"] = "authentication.stale_token" }),
        _ => Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unexpected server error",
            detail: "An unexpected error occurred.",
            extensions: new Dictionary<string, object?> { ["code"] = "server.unexpected_error" })
    };
}
