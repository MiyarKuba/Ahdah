using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Expenses.Models;
using Ahdah.Application.Expenses.Services;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/expenses")]
public sealed class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseViewer)]
    [HttpGet]
    [ProducesResponseType<PagedResult<ExpenseSummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ExpenseSummary>>> List(
        [FromQuery] ExpenseQuery query,
        CancellationToken cancellationToken) =>
        ExpenseResult(await expenseService.ListAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseViewer)]
    [HttpGet("{expenseId:guid}")]
    [ProducesResponseType<ExpenseDetails>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDetails>> Get(
        Guid expenseId,
        CancellationToken cancellationToken) =>
        ExpenseResult(await expenseService.GetAsync(expenseId, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseCreator)]
    [HttpPost]
    [ProducesResponseType<ExpenseDetails>(StatusCodes.Status201Created)]
    public async Task<ActionResult<ExpenseDetails>> Create(
        CreateExpenseRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!ExpenseRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        var result = await expenseService.CreateAsync(request, idempotencyKey, cancellationToken);
        return result.Status == AccessResultStatus.Success
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : ExpenseResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseViewer)]
    [HttpGet("{expenseId:guid}/allocations")]
    [ProducesResponseType<PagedResult<ExpenseAdvanceAllocationSummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ExpenseAdvanceAllocationSummary>>> ListAllocations(
        Guid expenseId,
        [FromQuery] ExpenseSubresourceQuery query,
        CancellationToken cancellationToken) =>
        ExpenseResult(await expenseService.ListAllocationsAsync(expenseId, query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseViewer)]
    [HttpGet("{expenseId:guid}/attachments")]
    [ProducesResponseType<PagedResult<ExpenseDocumentSummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ExpenseDocumentSummary>>> ListAttachments(
        Guid expenseId,
        [FromQuery] ExpenseSubresourceQuery query,
        CancellationToken cancellationToken) =>
        ExpenseResult(await expenseService.ListDocumentsAsync(expenseId, query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseDocumentContributor)]
    [HttpPost("{expenseId:guid}/attachments")]
    [ProducesResponseType<ExpenseDocumentSummary>(StatusCodes.Status201Created)]
    public async Task<ActionResult<ExpenseDocumentSummary>> AddAttachment(
        Guid expenseId,
        AddExpenseDocumentRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!ExpenseRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        var result = await expenseService.AddDocumentAsync(
            expenseId, request, idempotencyKey, cancellationToken);
        return result.Status == AccessResultStatus.Success
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : ExpenseResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseViewer)]
    [HttpGet("{expenseId:guid}/history")]
    [ProducesResponseType<PagedResult<ExpenseHistoryEntry>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ExpenseHistoryEntry>>> ListHistory(
        Guid expenseId,
        [FromQuery] ExpenseHistoryQuery query,
        CancellationToken cancellationToken) =>
        ExpenseResult(await expenseService.ListHistoryAsync(expenseId, query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseReviewer)]
    [HttpPost("{expenseId:guid}/approve")]
    [ProducesResponseType<ExpenseDetails>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDetails>> Approve(
        Guid expenseId,
        ApproveExpenseRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!ExpenseRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        return ExpenseResult(await expenseService.ApproveAsync(
            expenseId, request, idempotencyKey, cancellationToken));
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseReviewer)]
    [HttpPost("{expenseId:guid}/reject")]
    [ProducesResponseType<ExpenseDetails>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDetails>> Reject(
        Guid expenseId,
        RejectExpenseRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!ExpenseRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return InvalidIdempotencyKey();
        }

        return ExpenseResult(await expenseService.RejectAsync(
            expenseId, request, idempotencyKey, cancellationToken));
    }

    private ActionResult<T> ExpenseResult<T>(AccessResult<T> result) => result.Status switch
    {
        AccessResultStatus.Success => Ok(result.Value),
        AccessResultStatus.Invalid => ExpenseProblem(
            StatusCodes.Status400BadRequest,
            "Invalid expense operation",
            "The expense operation is invalid or unsupported in the current lifecycle state.",
            "expenses.invalid_operation"),
        AccessResultStatus.NotFound => ExpenseProblem(
            StatusCodes.Status404NotFound,
            "Expense operation unavailable",
            "The requested expense operation was not found.",
            "expenses.not_found"),
        AccessResultStatus.Conflict => ExpenseProblem(
            StatusCodes.Status409Conflict,
            "Expense conflict",
            "The expense operation conflicts with the current balance or lifecycle state.",
            "expenses.conflict"),
        AccessResultStatus.Forbidden => ExpenseProblem(
            StatusCodes.Status403Forbidden,
            "Forbidden",
            "You are not authorized to perform this expense operation.",
            "expenses.forbidden"),
        AccessResultStatus.Unauthorized => ExpenseProblem(
            StatusCodes.Status401Unauthorized,
            "Unauthorized",
            "The access token is no longer valid for an active company member.",
            "authentication.stale_token"),
        _ => ExpenseProblem(
            StatusCodes.Status500InternalServerError,
            "Unexpected server error",
            "An unexpected error occurred.",
            "server.unexpected_error")
    };

    private ObjectResult ExpenseProblem(int status, string title, string detail, string code) =>
        Problem(
            statusCode: status,
            title: title,
            detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code });

    private ObjectResult InvalidIdempotencyKey() => ExpenseProblem(
        StatusCodes.Status400BadRequest,
        "Invalid idempotency key",
        "A 16 to 200 character Idempotency-Key header is required for expense commands.",
        "expenses.invalid_idempotency_key");
}
