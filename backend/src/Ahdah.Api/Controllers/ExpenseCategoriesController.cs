using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Expenses.Models;
using Ahdah.Application.Expenses.Services;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/expense-categories")]
public sealed class ExpenseCategoriesController(IExpenseService expenseService) : ControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseViewer)]
    [HttpGet]
    [ProducesResponseType<PagedResult<ExpenseCategorySummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ExpenseCategorySummary>>> List(
        [FromQuery] ExpenseCategoryQuery query,
        CancellationToken cancellationToken) =>
        Result(await expenseService.ListCategoriesAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.ExpenseCategoryManager)]
    [HttpPost]
    [ProducesResponseType<ExpenseCategorySummary>(StatusCodes.Status201Created)]
    public async Task<ActionResult<ExpenseCategorySummary>> Create(
        CreateExpenseCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await expenseService.CreateCategoryAsync(request, cancellationToken);
        return result.Status == AccessResultStatus.Success
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : Result(result);
    }

    private ActionResult<T> Result<T>(AccessResult<T> result) => result.Status switch
    {
        AccessResultStatus.Success => Ok(result.Value),
        AccessResultStatus.Invalid => Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Invalid expense category",
            detail: "The expense category request is invalid.",
            extensions: new Dictionary<string, object?> { ["code"] = "expense_categories.invalid" }),
        AccessResultStatus.Conflict => Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Expense category conflict",
            detail: "The expense category conflicts with existing data.",
            extensions: new Dictionary<string, object?> { ["code"] = "expense_categories.conflict" }),
        AccessResultStatus.Forbidden => Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Forbidden",
            detail: "You are not authorized to manage expense categories.",
            extensions: new Dictionary<string, object?> { ["code"] = "expense_categories.forbidden" }),
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
