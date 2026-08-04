using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Expenses.Models;

namespace Ahdah.Application.Expenses.Services;

public interface IExpenseService
{
    Task<AccessResult<PagedResult<ExpenseCategorySummary>>> ListCategoriesAsync(ExpenseCategoryQuery query, CancellationToken cancellationToken);
    Task<AccessResult<ExpenseCategorySummary>> CreateCategoryAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<ExpenseSummary>>> ListAsync(ExpenseQuery query, CancellationToken cancellationToken);
    Task<AccessResult<ExpenseDetails>> GetAsync(Guid expenseId, CancellationToken cancellationToken);
    Task<AccessResult<ExpenseDetails>> CreateAsync(CreateExpenseRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<ExpenseDetails>> ApproveAsync(Guid expenseId, ApproveExpenseRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<ExpenseDetails>> RejectAsync(Guid expenseId, RejectExpenseRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>> ListAllocationsAsync(Guid expenseId, ExpenseSubresourceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<ExpenseDocumentSummary>>> ListDocumentsAsync(Guid expenseId, ExpenseSubresourceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<ExpenseDocumentSummary>> AddDocumentAsync(Guid expenseId, AddExpenseDocumentRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<ExpenseHistoryEntry>>> ListHistoryAsync(Guid expenseId, ExpenseHistoryQuery query, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<ReimbursementSummary>>> ListReimbursementsAsync(ReimbursementQuery query, CancellationToken cancellationToken);
    Task<AccessResult<ReimbursementSummary>> GetReimbursementAsync(Guid reimbursementId, CancellationToken cancellationToken);
}
