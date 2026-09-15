using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Advances;
using Ahdah.Application.Expenses;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Expenses.Models;
using Ahdah.Application.Expenses.Services;
using Ahdah.Application.Identity;
using Ahdah.Application.Suppliers;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Expenses;

public sealed class ExpenseService(
    AhdahDbContext dbContext,
    ICurrentUserContext currentUserContext,
    TimeProvider timeProvider) : IExpenseService
{
    public async Task<AccessResult<PagedResult<ExpenseCategorySummary>>> ListCategoriesAsync(
        ExpenseCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ExpenseCategorySummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (!IsValidPage(query.Page, query.PageSize)
            || query.CategoryGroup is not null
                && !ExpenseConstants.CategoryGroups.Contains(query.CategoryGroup)
            || query.ExpenseScope is not null
                && !ExpenseConstants.CategoryScopes.Contains(query.ExpenseScope))
        {
            return AccessResult<PagedResult<ExpenseCategorySummary>>.Failure(AccessResultStatus.Invalid);
        }

        var categories = dbContext.ExpenseCategories.AsNoTracking().Where(category =>
            category.CompanyId == caller.CompanyId && category.IsActive);
        if (query.CategoryGroup is not null)
        {
            categories = categories.Where(category => category.CategoryGroup == query.CategoryGroup);
        }

        if (query.ExpenseScope is not null)
        {
            categories = categories.Where(category => category.ExpenseScope == query.ExpenseScope);
        }

        var totalCount = await categories.CountAsync(cancellationToken);
        var items = await categories
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.CategoryName)
            .ThenBy(category => category.ExpenseCategoryId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(category => MapCategory(category))
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<ExpenseCategorySummary>>.Success(new(
            items, query.Page, query.PageSize, totalCount, TotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<ExpenseCategorySummary>> CreateCategoryAsync(
        CreateExpenseCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ExpenseCategorySummary>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanManageCategories || !Validate(request))
        {
            return AccessResult<ExpenseCategorySummary>.Failure(
                caller.Capabilities.CanManageCategories
                    ? AccessResultStatus.Invalid
                    : AccessResultStatus.Forbidden);
        }

        if (request.ParentExpenseCategoryId is { } parentId
            && !await dbContext.ExpenseCategories.AsNoTracking().AnyAsync(category =>
                category.CompanyId == caller.CompanyId
                && category.ExpenseCategoryId == parentId
                && category.IsActive,
                cancellationToken))
        {
            return AccessResult<ExpenseCategorySummary>.Failure(AccessResultStatus.Invalid);
        }

        var category = new ExpenseCategory
        {
            ExpenseCategoryId = Guid.NewGuid(),
            CompanyId = caller.CompanyId,
            ParentExpenseCategoryId = request.ParentExpenseCategoryId,
            CategoryCode = NormalizeOptional(request.CategoryCode)?.ToUpperInvariant(),
            CategoryName = request.CategoryName.Trim(),
            CategoryGroup = request.CategoryGroup,
            ExpenseScope = request.ExpenseScope,
            Description = NormalizeOptional(request.Description),
            RequiresSupplier = request.RequiresSupplier,
            RequiresReceipt = request.RequiresReceipt,
            SupportsQuantityDetails = request.SupportsQuantityDetails,
            IsActive = true,
            DisplayOrder = request.DisplayOrder,
            CreatedByUserId = caller.UserId,
            VersionNumber = 1
        };

        dbContext.ExpenseCategories.Add(category);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return AccessResult<ExpenseCategorySummary>.Failure(AccessResultStatus.Conflict);
        }

        return AccessResult<ExpenseCategorySummary>.Success(MapCategory(category));
    }

    public async Task<AccessResult<PagedResult<ExpenseSummary>>> ListAsync(
        ExpenseQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ExpenseSummary>>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!IsValid(query)
            || (query.IncurredByUserId is not null || query.SubmittedByUserId is not null)
                && !caller.Capabilities.CanViewAllCompanyExpenses)
        {
            return AccessResult<PagedResult<ExpenseSummary>>.Failure(
                IsValid(query) ? AccessResultStatus.Forbidden : AccessResultStatus.Invalid);
        }

        var expenses = ApplyVisibility(
            dbContext.Expenses.AsNoTracking().Where(expense => expense.CompanyId == caller.CompanyId),
            caller);
        if (query.Status is not null)
        {
            expenses = expenses.Where(expense => expense.Status == query.Status);
        }

        if (query.CategoryId is { } categoryId)
        {
            expenses = expenses.Where(expense => expense.ExpenseCategoryId == categoryId);
        }

        if (query.ProjectId is { } projectId)
        {
            expenses = expenses.Where(expense => expense.ProjectId == projectId);
        }

        if (query.IncurredByUserId is { } incurredBy)
        {
            expenses = expenses.Where(expense => expense.IncurredByUserId == incurredBy);
        }

        if (query.SubmittedByUserId is { } submittedBy)
        {
            expenses = expenses.Where(expense => expense.SubmittedByUserId == submittedBy);
        }

        if (query.PaymentMode is not null)
        {
            expenses = expenses.Where(expense => expense.PaymentMode == query.PaymentMode);
        }

        if (NormalizeOptional(query.Reference) is { } reference)
        {
            var lowered = reference.ToLower();
            expenses = expenses.Where(expense => expense.ExpenseNumber.ToLower().StartsWith(lowered));
        }

        var totalCount = await expenses.CountAsync(cancellationToken);
        var rows = await SelectExpenseRows(expenses)
            .OrderByDescending(expense => expense.CreatedAt)
            .ThenByDescending(expense => expense.ExpenseId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<ExpenseSummary>>.Success(new(
            rows.Select(MapExpense).ToArray(),
            query.Page,
            query.PageSize,
            totalCount,
            TotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<ExpenseDetails>> GetAsync(
        Guid expenseId,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        return caller is null
            ? AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Unauthorized)
            : await LoadDetailsAsync(caller, expenseId, cancellationToken);
    }

    public async Task<AccessResult<ExpenseDetails>> CreateAsync(
        CreateExpenseRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanCreateExpense)
        {
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Forbidden);
        }

        if (!Validate(request) || !ExpenseRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotencyAsync(
                caller, idempotencyKey!, "expenses.create", "/api/v1/expenses",
                Fingerprint("expenses.create", request), cancellationToken);
            if (attempt.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (attempt.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<ExpenseDetails>(attempt.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Success(replay);
            }

            var category = await dbContext.ExpenseCategories.AsNoTracking().SingleOrDefaultAsync(
                value => value.CompanyId == caller.CompanyId
                    && value.ExpenseCategoryId == request.ExpenseCategoryId
                    && value.IsActive,
                cancellationToken);
            if (category is null
                || !ExpenseRules.ProjectMatchesScope(category.ExpenseScope, request.ProjectId)
                || category.RequiresSupplier)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
            }

            if (request.ProjectId is { } projectId
                && !await dbContext.Projects.AsNoTracking().AnyAsync(project =>
                    project.CompanyId == caller.CompanyId && project.ProjectId == projectId,
                    cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
            }

            if (request.ProjectId is not null && caller.Role == AccessConstants.WorkerRole)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Forbidden);
            }

            if (request.ProjectId is { } supervisorProjectId
                && caller.Role == AccessConstants.SupervisorRole
                && !await dbContext.ProjectSupervisors.AsNoTracking().AnyAsync(assignment =>
                    assignment.CompanyId == caller.CompanyId
                    && assignment.ProjectId == supervisorProjectId
                    && assignment.SupervisorUserId == caller.UserId
                    && assignment.IsActive,
                    cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Forbidden);
            }

            var settings = await dbContext.CompanySettings.AsNoTracking().SingleOrDefaultAsync(value =>
                value.CompanyId == caller.CompanyId && value.IsActive, cancellationToken);
            if (settings?.MaxSingleExpenseAmount is { } maximum && request.Amount > maximum)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
            }

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var expenseId = Guid.NewGuid();
            var expense = new Expense
            {
                ExpenseId = expenseId,
                CompanyId = caller.CompanyId,
                ExpenseNumber = CreateReference("EXP", expenseId),
                ProjectId = request.ProjectId,
                ExpenseCategoryId = category.ExpenseCategoryId,
                IncurredByUserId = caller.UserId,
                ExpenseDate = request.ExpenseDate,
                PaymentMode = request.PaymentMode,
                CurrencyCode = request.CurrencyCode,
                SubtotalAmount = request.Amount,
                DiscountAmount = 0m,
                TaxAmount = 0m,
                TotalAmount = request.Amount,
                InvoiceNumber = NormalizeOptional(request.InvoiceNumber),
                ReceiptNumber = NormalizeOptional(request.ReceiptNumber),
                MerchantName = NormalizeOptional(request.MerchantName),
                Description = request.Description.Trim(),
                ExpenseLocation = NormalizeOptional(request.ExpenseLocation),
                Notes = NormalizeOptional(request.Notes),
                Status = ExpenseConstants.PendingReviewStatus,
                SubmittedByUserId = caller.UserId,
                SubmittedAt = now,
                VersionNumber = 1
            };
            dbContext.Expenses.Add(expense);

            if (request.PaymentMode == ExpenseConstants.AdvanceBalancePaymentMode)
            {
                foreach (var allocationRequest in request.AdvanceAllocations
                    .OrderBy(allocation => allocation.UserAdvanceBalanceId))
                {
                    var balance = await LockBalanceAsync(
                        caller.CompanyId,
                        allocationRequest.UserAdvanceBalanceId,
                        cancellationToken);
                    if (balance is null
                        || balance.UserId != caller.UserId
                        || balance.Status != AdvanceConstants.ActiveBalanceStatus
                        || balance.AvailableAmount < allocationRequest.Amount)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                    }

                    var advance = await dbContext.Advances.AsNoTracking().SingleOrDefaultAsync(value =>
                        value.CompanyId == caller.CompanyId
                        && value.AdvanceId == balance.AdvanceId,
                        cancellationToken);
                    if (advance is null
                        || advance.Status != AdvanceConstants.OpenStatus
                        || advance.CurrencyCode != request.CurrencyCode)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
                    }

                    balance.AvailableAmount -= allocationRequest.Amount;
                    balance.ReservedAmount += allocationRequest.Amount;
                    balance.VersionNumber++;
                    dbContext.ExpenseAdvanceAllocations.Add(new ExpenseAdvanceAllocation
                    {
                        ExpenseAdvanceAllocationId = Guid.NewGuid(),
                        CompanyId = caller.CompanyId,
                        ExpenseId = expenseId,
                        UserAdvanceBalanceId = balance.UserAdvanceBalanceId,
                        AllocatedAmount = allocationRequest.Amount,
                        AllocatedByUserId = caller.UserId,
                        Notes = NormalizeOptional(allocationRequest.Notes)
                    });
                    AddBalanceLedger(
                        balance,
                        ExpenseConstants.ExpenseReservedLedgerType,
                        0m,
                        -allocationRequest.Amount,
                        allocationRequest.Amount,
                        caller.UserId,
                        expenseId,
                        "Expense amount reserved pending review.");
                }
            }
            else
            {
                var claimId = Guid.NewGuid();
                dbContext.PersonalClaims.Add(new PersonalClaim
                {
                    PersonalClaimId = claimId,
                    CompanyId = caller.CompanyId,
                    ClaimNumber = CreateReference("CLM", claimId),
                    ClaimantUserId = caller.UserId,
                    ProjectId = request.ProjectId,
                    SourceType = ExpenseConstants.PersonalExpenseClaimSource,
                    ExpenseId = expenseId,
                    ClaimDate = request.ExpenseDate,
                    DueDate = settings is null
                        ? null
                        : request.ExpenseDate.AddDays(settings.DefaultPersonalClaimDueDays),
                    CurrencyCode = request.CurrencyCode,
                    ClaimAmount = request.Amount,
                    AdjustmentAmount = 0m,
                    PaidAmount = 0m,
                    ReductionAmount = 0m,
                    WrittenOffAmount = 0m,
                    Description = request.Description.Trim(),
                    Notes = NormalizeOptional(request.Notes),
                    Status = ExpenseConstants.OpenClaimStatus,
                    RecordedByUserId = caller.UserId,
                    VersionNumber = 1
                });
            }

            AddExpenseAudit(caller, expense, "ExpenseCreated", "Create", "Expense submitted for review.");
            await dbContext.SaveChangesAsync(cancellationToken);
            var details = await LoadDetailsValueAsync(caller, expenseId, cancellationToken);
            if (details is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
            }

            CompleteIdempotency(attempt.Record!, "Expense", expenseId, expense.VersionNumber, 201);
            SetResponsePayload(attempt.Record!, details);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AccessResult<ExpenseDetails>.Success(details);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
        }
    }

    public Task<AccessResult<ExpenseDetails>> ApproveAsync(
        Guid expenseId,
        ApproveExpenseRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken) => ReviewAsync(
            expenseId, request.ExpectedVersion, null, approve: true, idempotencyKey, cancellationToken);

    public Task<AccessResult<ExpenseDetails>> RejectAsync(
        Guid expenseId,
        RejectExpenseRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken) => ReviewAsync(
            expenseId, request.ExpectedVersion, request.Reason, approve: false,
            idempotencyKey, cancellationToken);

    public async Task<AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>> ListAllocationsAsync(
        Guid expenseId,
        ExpenseSubresourceQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (!IsValidPage(query.Page, query.PageSize))
        {
            return AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>.Failure(
                AccessResultStatus.Invalid);
        }

        if (!await VisibleExpenseExistsAsync(caller, expenseId, cancellationToken))
        {
            return AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>.Failure(
                AccessResultStatus.NotFound);
        }

        var allocations = dbContext.ExpenseAdvanceAllocations.AsNoTracking().Where(allocation =>
            allocation.CompanyId == caller.CompanyId && allocation.ExpenseId == expenseId);
        var totalCount = await allocations.CountAsync(cancellationToken);
        var items = await allocations
            .OrderBy(allocation => allocation.CreatedAt)
            .ThenBy(allocation => allocation.ExpenseAdvanceAllocationId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(allocation => new ExpenseAdvanceAllocationSummary(
                allocation.ExpenseAdvanceAllocationId,
                allocation.UserAdvanceBalance.AdvanceId,
                allocation.UserAdvanceBalance.Advance.AdvanceNumber,
                allocation.AllocatedAmount,
                ToUtc(allocation.CreatedAt)))
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>.Success(new(
            items, query.Page, query.PageSize, totalCount, TotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<PagedResult<ExpenseDocumentSummary>>> ListDocumentsAsync(
        Guid expenseId,
        ExpenseSubresourceQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ExpenseDocumentSummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (!IsValidPage(query.Page, query.PageSize))
        {
            return AccessResult<PagedResult<ExpenseDocumentSummary>>.Failure(AccessResultStatus.Invalid);
        }

        if (!await VisibleExpenseExistsAsync(caller, expenseId, cancellationToken))
        {
            return AccessResult<PagedResult<ExpenseDocumentSummary>>.Failure(AccessResultStatus.NotFound);
        }

        var documents = dbContext.ExpenseDocuments.AsNoTracking().Where(document =>
            document.CompanyId == caller.CompanyId && document.ExpenseId == expenseId);
        var totalCount = await documents.CountAsync(cancellationToken);
        var items = await SelectDocuments(documents)
            .OrderByDescending(document => document.CreatedAtUtc)
            .ThenBy(document => document.ExpenseDocumentId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<ExpenseDocumentSummary>>.Success(new(
            items, query.Page, query.PageSize, totalCount, TotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<ExpenseDocumentSummary>> AddDocumentAsync(
        Guid expenseId,
        AddExpenseDocumentRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanAttachDocuments)
        {
            return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Forbidden);
        }

        if (!Validate(request) || !ExpenseRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotencyAsync(
                caller,
                idempotencyKey!,
                "expenses.document_metadata_add",
                $"/api/v1/expenses/{expenseId}/attachments",
                Fingerprint("expenses.document_metadata_add", new { expenseId, request }),
                cancellationToken);
            if (attempt.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Conflict);
            }

            if (attempt.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<ExpenseDocumentSummary>(attempt.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<ExpenseDocumentSummary>.Success(replay);
            }

            var expense = await LockExpenseAsync(caller.CompanyId, expenseId, cancellationToken);
            if (expense is null || !CanSee(expense, caller, await AssignedProjectIdsAsync(caller, cancellationToken)))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.NotFound);
            }

            if (expense.Status is ExpenseConstants.CancelledStatus or ExpenseConstants.ReversedStatus
                or ExpenseConstants.RejectedStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Conflict);
            }

            var settings = await dbContext.CompanySettings.AsNoTracking().SingleOrDefaultAsync(value =>
                value.CompanyId == caller.CompanyId && value.IsActive, cancellationToken);
            if (settings is null || !DocumentAllowed(settings, request))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Invalid);
            }

            var document = new ExpenseDocument
            {
                ExpenseDocumentId = Guid.NewGuid(),
                CompanyId = caller.CompanyId,
                ExpenseId = expenseId,
                DocumentType = request.DocumentType,
                DocumentNumber = NormalizeOptional(request.DocumentNumber),
                DocumentDate = request.DocumentDate,
                IssuerName = NormalizeOptional(request.IssuerName),
                OriginalFileName = request.OriginalFileName.Trim(),
                FileUrl = request.FileUrl.Trim(),
                MimeType = request.MimeType.Trim().ToLowerInvariant(),
                FileSizeBytes = request.FileSizeBytes,
                Sha256Hash = request.Sha256Hash.ToLowerInvariant(),
                CaptureSource = request.CaptureSource,
                IsPrimary = request.IsPrimary,
                VerificationStatus = ExpenseConstants.PendingVerificationStatus,
                UploadedByUserId = caller.UserId,
                Notes = NormalizeOptional(request.Notes),
                VersionNumber = 1
            };
            dbContext.ExpenseDocuments.Add(document);
            AddExpenseAudit(
                caller,
                expense,
                "ExpenseDocumentMetadataAdded",
                "Attach",
                "Supporting document metadata added to expense.");
            await dbContext.SaveChangesAsync(cancellationToken);

            var response = await SelectDocuments(dbContext.ExpenseDocuments.AsNoTracking().Where(value =>
                    value.CompanyId == caller.CompanyId
                    && value.ExpenseDocumentId == document.ExpenseDocumentId))
                .SingleAsync(cancellationToken);
            CompleteIdempotency(attempt.Record!, "ExpenseDocument", document.ExpenseDocumentId, 1, 201);
            SetResponsePayload(attempt.Record!, response);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AccessResult<ExpenseDocumentSummary>.Success(response);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ExpenseDocumentSummary>.Failure(AccessResultStatus.Conflict);
        }
    }

    public async Task<AccessResult<PagedResult<ExpenseHistoryEntry>>> ListHistoryAsync(
        Guid expenseId,
        ExpenseHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ExpenseHistoryEntry>>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!IsValidPage(query.Page, query.PageSize))
        {
            return AccessResult<PagedResult<ExpenseHistoryEntry>>.Failure(AccessResultStatus.Invalid);
        }

        if (!await VisibleExpenseExistsAsync(caller, expenseId, cancellationToken))
        {
            return AccessResult<PagedResult<ExpenseHistoryEntry>>.Failure(AccessResultStatus.NotFound);
        }

        var events = dbContext.AuditLogs.AsNoTracking().Where(audit =>
            audit.CompanyId == caller.CompanyId
            && audit.EntityType == "Expense"
            && audit.EntityId == expenseId);
        var totalCount = await events.CountAsync(cancellationToken);
        var items = await events
            .OrderBy(audit => audit.OccurredAt)
            .ThenBy(audit => audit.AuditSequence)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(audit => new ExpenseHistoryEntry(
                audit.AuditLogId,
                audit.EventName,
                audit.EventAction,
                audit.Outcome,
                audit.Description,
                audit.ActorUserId == null
                    ? null
                    : new ExpenseUserSummary(
                        audit.ActorUserId.Value,
                        audit.ActorNameSnapshot,
                        audit.ActorRoleSnapshot!),
                audit.EntityVersionNumber,
                ToUtc(audit.OccurredAt)))
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<ExpenseHistoryEntry>>.Success(new(
            items, query.Page, query.PageSize, totalCount, TotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<PagedResult<ReimbursementSummary>>> ListReimbursementsAsync(
        ReimbursementQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ReimbursementSummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (!IsValidPage(query.Page, query.PageSize)
            || query.Status is not null && !ExpenseRules.IsKnownClaimStatus(query.Status)
            || query.ClaimantUserId is not null && !caller.Capabilities.CanViewAllReimbursements)
        {
            return AccessResult<PagedResult<ReimbursementSummary>>.Failure(
                query.ClaimantUserId is not null && !caller.Capabilities.CanViewAllReimbursements
                    ? AccessResultStatus.Forbidden
                    : AccessResultStatus.Invalid);
        }

        var claims = dbContext.PersonalClaims.AsNoTracking().Where(claim =>
            claim.CompanyId == caller.CompanyId
            && claim.SourceType == ExpenseConstants.PersonalExpenseClaimSource
            && claim.ExpenseId != null);
        if (!caller.Capabilities.CanViewAllReimbursements)
        {
            claims = claims.Where(claim => claim.ClaimantUserId == caller.UserId);
        }

        if (query.Status is not null)
        {
            claims = claims.Where(claim => claim.Status == query.Status);
        }

        if (query.ClaimantUserId is { } claimantId)
        {
            claims = claims.Where(claim => claim.ClaimantUserId == claimantId);
        }

        var totalCount = await claims.CountAsync(cancellationToken);
        var items = await SelectReimbursements(claims)
            .OrderByDescending(claim => claim.CreatedAtUtc)
            .ThenByDescending(claim => claim.ReimbursementId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<ReimbursementSummary>>.Success(new(
            items, query.Page, query.PageSize, totalCount, TotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<ReimbursementSummary>> GetReimbursementAsync(
        Guid reimbursementId,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ReimbursementSummary>.Failure(AccessResultStatus.Unauthorized);
        }

        var claims = dbContext.PersonalClaims.AsNoTracking().Where(claim =>
            claim.CompanyId == caller.CompanyId
            && claim.PersonalClaimId == reimbursementId
            && claim.SourceType == ExpenseConstants.PersonalExpenseClaimSource
            && claim.ExpenseId != null);
        if (!caller.Capabilities.CanViewAllReimbursements)
        {
            claims = claims.Where(claim => claim.ClaimantUserId == caller.UserId);
        }

        var item = await SelectReimbursements(claims).SingleOrDefaultAsync(cancellationToken);
        return item is null
            ? AccessResult<ReimbursementSummary>.Failure(AccessResultStatus.NotFound)
            : AccessResult<ReimbursementSummary>.Success(item);
    }

    private async Task<AccessResult<ExpenseDetails>> ReviewAsync(
        Guid expenseId,
        int expectedVersion,
        string? rejectionReason,
        bool approve,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanReviewExpense)
        {
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Forbidden);
        }

        if (expectedVersion < 1
            || !ExpenseRules.IsValidIdempotencyKey(idempotencyKey)
            || !approve && (string.IsNullOrWhiteSpace(rejectionReason)
                || rejectionReason.Trim().Length > 1000))
        {
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
        }

        var operation = approve ? "expenses.approve" : "expenses.reject";
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotencyAsync(
                caller,
                idempotencyKey!,
                operation,
                $"/api/v1/expenses/{expenseId}/{(approve ? "approve" : "reject")}",
                Fingerprint(operation, new { expenseId, expectedVersion, rejectionReason }),
                cancellationToken);
            if (attempt.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (attempt.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<ExpenseDetails>(attempt.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Success(replay);
            }

            var expense = await LockExpenseAsync(caller.CompanyId, expenseId, cancellationToken);
            if (expense is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.NotFound);
            }

            if (expense.Status != ExpenseConstants.PendingReviewStatus
                || expense.VersionNumber != expectedVersion)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
            }

            var settings = await dbContext.CompanySettings.AsNoTracking().SingleOrDefaultAsync(value =>
                value.CompanyId == caller.CompanyId && value.IsActive, cancellationToken);
            if (caller.UserId == expense.SubmittedByUserId
                && (settings is null
                    || !settings.AllowSelfApproval
                    || settings.RequireDistinctSubmitterApprover
                    || settings.RequireDistinctCreatorApprover))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Forbidden);
            }

            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (approve)
            {
                var category = await dbContext.ExpenseCategories.AsNoTracking().SingleAsync(value =>
                    value.CompanyId == caller.CompanyId
                    && value.ExpenseCategoryId == expense.ExpenseCategoryId,
                    cancellationToken);
                var documents = dbContext.ExpenseDocuments.AsNoTracking().Where(document =>
                    document.CompanyId == caller.CompanyId
                    && document.ExpenseId == expenseId
                    && document.VerificationStatus != ExpenseConstants.RejectedDocumentStatus);
                if (category.RequiresReceipt
                    && !await documents.AnyAsync(document =>
                        document.DocumentType == "Receipt" || document.DocumentType == "Invoice",
                        cancellationToken))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
                }

                if (settings is not null
                    && DocumentRequired(settings, expense.TotalAmount)
                    && !await documents.AnyAsync(cancellationToken))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Invalid);
                }

                if (expense.PaymentMode == ExpenseConstants.AdvanceBalancePaymentMode)
                {
                    var allocations = await dbContext.ExpenseAdvanceAllocations.AsNoTracking()
                        .Where(allocation => allocation.CompanyId == caller.CompanyId
                            && allocation.ExpenseId == expenseId)
                        .OrderBy(allocation => allocation.UserAdvanceBalanceId)
                        .ToArrayAsync(cancellationToken);
                    if (allocations.Length == 0
                        || allocations.Sum(allocation => allocation.AllocatedAmount)
                            != expense.TotalAmount)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                    }

                    foreach (var allocation in allocations)
                    {
                        var balance = await LockBalanceAsync(
                            caller.CompanyId, allocation.UserAdvanceBalanceId, cancellationToken);
                        if (balance is null || balance.ReservedAmount < allocation.AllocatedAmount)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                        }

                        balance.ReservedAmount -= allocation.AllocatedAmount;
                        balance.TotalExpensedAmount += allocation.AllocatedAmount;
                        balance.VersionNumber++;
                        AddBalanceLedger(
                            balance,
                            ExpenseConstants.ExpenseConfirmedLedgerType,
                            allocation.AllocatedAmount,
                            0m,
                            -allocation.AllocatedAmount,
                            caller.UserId,
                            expenseId,
                            "Approved expense confirmed against advance balance.");
                    }
                }

                expense.Status = ExpenseConstants.ApprovedStatus;
                expense.ReviewedByUserId = caller.UserId;
                expense.ReviewedAt = now;
                expense.RejectionReason = null;
                expense.VersionNumber++;
                AddExpenseAudit(caller, expense, "ExpenseApproved", "Approve", "Expense approved.");
            }
            else
            {
                if (expense.PaymentMode == ExpenseConstants.AdvanceBalancePaymentMode)
                {
                    var allocations = await dbContext.ExpenseAdvanceAllocations.AsNoTracking()
                        .Where(allocation => allocation.CompanyId == caller.CompanyId
                            && allocation.ExpenseId == expenseId)
                        .OrderBy(allocation => allocation.UserAdvanceBalanceId)
                        .ToArrayAsync(cancellationToken);
                    foreach (var allocation in allocations)
                    {
                        var balance = await LockBalanceAsync(
                            caller.CompanyId, allocation.UserAdvanceBalanceId, cancellationToken);
                        if (balance is null || balance.ReservedAmount < allocation.AllocatedAmount)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                        }

                        balance.ReservedAmount -= allocation.AllocatedAmount;
                        balance.AvailableAmount += allocation.AllocatedAmount;
                        balance.VersionNumber++;
                        AddBalanceLedger(
                            balance,
                            ExpenseConstants.ExpenseReservationReleasedLedgerType,
                            0m,
                            allocation.AllocatedAmount,
                            -allocation.AllocatedAmount,
                            caller.UserId,
                            expenseId,
                            "Rejected expense reservation released.");
                    }
                }
                else if (expense.PaymentMode == ExpenseConstants.PersonalFundsPaymentMode)
                {
                    var claim = await LockPersonalClaimAsync(caller.CompanyId, expenseId, cancellationToken);
                    if (claim is null
                        || claim.Status != ExpenseConstants.OpenClaimStatus
                        || claim.AdjustmentAmount != 0m
                        || claim.PaidAmount != 0m
                        || claim.ReductionAmount != 0m
                        || claim.WrittenOffAmount != 0m)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                    }

                    claim.Status = ExpenseConstants.CancelledClaimStatus;
                    claim.CancelledByUserId = caller.UserId;
                    claim.CancelledAt = now;
                    claim.CancellationReason = rejectionReason!.Trim();
                    claim.VersionNumber++;
                }
                else if (expense.PaymentMode == ExpenseConstants.SupplierCreditPaymentMode)
                {
                    var debt = await LockSupplierDebtAsync(caller.CompanyId, expenseId, cancellationToken);
                    if (debt is null
                        || debt.Status != SupplierConstants.OpenDebtStatus
                        || debt.AdjustmentAmount != 0m
                        || debt.PaidAmount != 0m
                        || debt.CreditNoteAmount != 0m
                        || debt.WrittenOffAmount != 0m)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
                    }

                    debt.Status = SupplierConstants.CancelledStatus;
                    debt.CancelledByUserId = caller.UserId;
                    debt.CancelledAt = now;
                    debt.CancellationReason = rejectionReason!.Trim();
                    debt.VersionNumber++;
                    dbContext.SupplierDebtLedgerEntries.Add(new SupplierDebtLedgerEntry
                    {
                        SupplierDebtLedgerEntryId = Guid.NewGuid(),
                        CompanyId = caller.CompanyId,
                        SupplierDebtId = debt.SupplierDebtId,
                        DebtVersionNumber = debt.VersionNumber,
                        EntryType = "DebtCancelled",
                        DeltaDebtAmount = 0m,
                        DeltaAdjustmentAmount = 0m,
                        DeltaPaidAmount = 0m,
                        DeltaCreditNoteAmount = 0m,
                        DeltaWrittenOffAmount = 0m,
                        DebtAfterAmount = debt.DebtAmount,
                        AdjustmentAfterAmount = debt.AdjustmentAmount,
                        PaidAfterAmount = debt.PaidAmount,
                        CreditNoteAfterAmount = debt.CreditNoteAmount,
                        WrittenOffAfterAmount = debt.WrittenOffAmount,
                        OutstandingAfterAmount = debt.DebtAmount,
                        DebtStatusAfter = debt.Status,
                        CorrelationId = Guid.NewGuid(),
                        Description = "Supplier debt cancelled after expense rejection.",
                        PerformedByUserId = caller.UserId,
                        OccurredAt = now
                    });
                }

                expense.Status = ExpenseConstants.RejectedStatus;
                expense.ReviewedByUserId = caller.UserId;
                expense.ReviewedAt = now;
                expense.RejectionReason = rejectionReason!.Trim();
                expense.VersionNumber++;
                AddExpenseAudit(caller, expense, "ExpenseRejected", "Reject", "Expense rejected.");
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            var details = await LoadDetailsValueAsync(caller, expenseId, cancellationToken);
            if (details is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
            }

            CompleteIdempotency(attempt.Record!, "Expense", expenseId, expense.VersionNumber, 200);
            SetResponsePayload(attempt.Record!, details);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AccessResult<ExpenseDetails>.Success(details);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ExpenseDetails>.Failure(AccessResultStatus.Conflict);
        }
    }

    private async Task<AccessResult<ExpenseDetails>> LoadDetailsAsync(
        Caller caller,
        Guid expenseId,
        CancellationToken cancellationToken)
    {
        var value = await LoadDetailsValueAsync(caller, expenseId, cancellationToken);
        return value is null
            ? AccessResult<ExpenseDetails>.Failure(AccessResultStatus.NotFound)
            : AccessResult<ExpenseDetails>.Success(value);
    }

    private async Task<ExpenseDetails?> LoadDetailsValueAsync(
        Caller caller,
        Guid expenseId,
        CancellationToken cancellationToken)
    {
        var row = await SelectExpenseRows(ApplyVisibility(
                dbContext.Expenses.AsNoTracking().Where(expense =>
                    expense.CompanyId == caller.CompanyId && expense.ExpenseId == expenseId),
                caller))
            .SingleOrDefaultAsync(cancellationToken);
        if (row is null)
        {
            return null;
        }

        var allocationRows = await dbContext.ExpenseAdvanceAllocations.AsNoTracking()
            .Where(allocation => allocation.CompanyId == caller.CompanyId
                && allocation.ExpenseId == expenseId)
            .OrderBy(allocation => allocation.CreatedAt)
            .ThenBy(allocation => allocation.ExpenseAdvanceAllocationId)
            .Select(allocation => new ExpenseAdvanceAllocationSummary(
                allocation.ExpenseAdvanceAllocationId,
                allocation.UserAdvanceBalance.AdvanceId,
                allocation.UserAdvanceBalance.Advance.AdvanceNumber,
                allocation.AllocatedAmount,
                ToUtc(allocation.CreatedAt)))
            .ToArrayAsync(cancellationToken);
        var documents = await SelectDocuments(dbContext.ExpenseDocuments.AsNoTracking().Where(document =>
                document.CompanyId == caller.CompanyId && document.ExpenseId == expenseId))
            .OrderByDescending(document => document.CreatedAtUtc)
            .ThenBy(document => document.ExpenseDocumentId)
            .ToArrayAsync(cancellationToken);
        var items = await dbContext.ExpenseItems.AsNoTracking()
            .Where(item => item.CompanyId == caller.CompanyId && item.ExpenseId == expenseId)
            .OrderBy(item => item.LineNumber)
            .ThenBy(item => item.ExpenseItemId)
            .Select(item => new ExpenseItemSummary(
                item.ExpenseItemId,
                item.LineNumber,
                item.ItemName,
                item.ItemCode,
                item.ItemDescription,
                item.Quantity,
                item.UnitCode,
                item.CustomUnitName,
                item.UnitPrice,
                item.SubtotalAmount ?? 0m,
                item.DiscountAmount,
                item.TaxAmount,
                item.TotalAmount ?? 0m,
                item.Notes))
            .ToArrayAsync(cancellationToken);
        var reimbursement = await SelectReimbursements(dbContext.PersonalClaims.AsNoTracking().Where(claim =>
                claim.CompanyId == caller.CompanyId
                && claim.ExpenseId == expenseId
                && claim.SourceType == ExpenseConstants.PersonalExpenseClaimSource))
            .SingleOrDefaultAsync(cancellationToken);
        var reviewer = row.ReviewerUserId is { } reviewerId
            ? new ExpenseUserSummary(reviewerId, row.ReviewerName!, row.ReviewerRole!)
            : null;

        return new ExpenseDetails(
            MapExpense(row),
            row.SubtotalAmount,
            row.DiscountAmount,
            row.TaxAmount,
            row.MerchantName,
            row.ExpenseLocation,
            row.Notes,
            row.CorrectionReason,
            row.RejectionReason,
            reviewer,
            allocationRows,
            documents,
            items,
            reimbursement);
    }

    private async Task<Caller?> GetActiveCallerAsync(CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated
            || currentUserContext.CompanyId is not { } companyId
            || currentUserContext.UserId is not { } userId
            || currentUserContext.Role is not { } claimRole)
        {
            return null;
        }

        var user = await dbContext.AppUsers.AsNoTracking()
            .Where(value => value.CompanyId == companyId
                && value.UserId == userId
                && value.Status == IdentityConstants.ActiveStatus
                && value.Company.Status == IdentityConstants.ActiveStatus)
            .Select(value => new { value.Role, value.FullName })
            .SingleOrDefaultAsync(cancellationToken);
        return user is null || user.Role != claimRole
            ? null
            : new Caller(
                companyId,
                userId,
                user.FullName,
                user.Role,
                ExpenseRoleCapabilities.For(user.Role));
    }

    private IQueryable<Expense> ApplyVisibility(IQueryable<Expense> expenses, Caller caller)
    {
        if (caller.Capabilities.CanViewAllCompanyExpenses)
        {
            return expenses;
        }

        if (caller.Role == AccessConstants.SupervisorRole)
        {
            return expenses.Where(expense =>
                expense.IncurredByUserId == caller.UserId
                || expense.SubmittedByUserId == caller.UserId
                || expense.ProjectId != null
                    && dbContext.ProjectSupervisors.Any(assignment =>
                        assignment.CompanyId == caller.CompanyId
                        && assignment.ProjectId == expense.ProjectId
                        && assignment.SupervisorUserId == caller.UserId
                        && assignment.IsActive));
        }

        if (caller.Role == AccessConstants.WorkerRole)
        {
            return expenses.Where(expense =>
                expense.IncurredByUserId == caller.UserId
                || expense.SubmittedByUserId == caller.UserId);
        }

        return expenses.Where(_ => false);
    }

    private Task<bool> VisibleExpenseExistsAsync(
        Caller caller,
        Guid expenseId,
        CancellationToken cancellationToken) => ApplyVisibility(
            dbContext.Expenses.AsNoTracking().Where(expense =>
                expense.CompanyId == caller.CompanyId && expense.ExpenseId == expenseId),
            caller).AnyAsync(cancellationToken);

    private async Task<HashSet<Guid>> AssignedProjectIdsAsync(
        Caller caller,
        CancellationToken cancellationToken)
    {
        if (caller.Role != AccessConstants.SupervisorRole)
        {
            return [];
        }

        return (await dbContext.ProjectSupervisors.AsNoTracking()
            .Where(assignment => assignment.CompanyId == caller.CompanyId
                && assignment.SupervisorUserId == caller.UserId
                && assignment.IsActive)
            .Select(assignment => assignment.ProjectId)
            .ToArrayAsync(cancellationToken)).ToHashSet();
    }

    private static bool CanSee(Expense expense, Caller caller, HashSet<Guid> assignedProjects) =>
        caller.Capabilities.CanViewAllCompanyExpenses
        || expense.IncurredByUserId == caller.UserId
        || expense.SubmittedByUserId == caller.UserId
        || caller.Role == AccessConstants.SupervisorRole
            && expense.ProjectId is { } projectId
            && assignedProjects.Contains(projectId);

    private IQueryable<ExpenseRow> SelectExpenseRows(IQueryable<Expense> expenses) =>
        expenses.Select(expense => new ExpenseRow(
            expense.ExpenseId,
            expense.ExpenseNumber,
            expense.ExpenseDate,
            expense.SubtotalAmount,
            expense.DiscountAmount,
            expense.TaxAmount,
            expense.TotalAmount,
            expense.CurrencyCode,
            expense.PaymentMode,
            expense.Description,
            expense.Status,
            expense.ExpenseCategoryId,
            expense.ExpenseCategory.ParentExpenseCategoryId,
            expense.ExpenseCategory.CategoryCode,
            expense.ExpenseCategory.CategoryName,
            expense.ExpenseCategory.CategoryGroup,
            expense.ExpenseCategory.ExpenseScope,
            expense.ExpenseCategory.Description,
            expense.ExpenseCategory.RequiresSupplier,
            expense.ExpenseCategory.RequiresReceipt,
            expense.ExpenseCategory.SupportsQuantityDetails,
            expense.ExpenseCategory.IsActive,
            expense.ExpenseCategory.DisplayOrder,
            expense.ExpenseCategory.VersionNumber,
            expense.ProjectId,
            expense.Project == null ? null : expense.Project.ProjectName,
            expense.IncurredByUserId,
            expense.AppUserNavigation.FullName,
            expense.AppUserNavigation.Role,
            expense.SubmittedByUserId,
            expense.AppUser3.FullName,
            expense.AppUser3.Role,
            expense.ReviewedByUserId,
            expense.AppUser2 == null ? null : expense.AppUser2.FullName,
            expense.AppUser2 == null ? null : expense.AppUser2.Role,
            expense.ReceiptNumber,
            expense.InvoiceNumber,
            dbContext.ExpenseDocuments.Any(document =>
                document.CompanyId == expense.CompanyId
                && document.ExpenseId == expense.ExpenseId
                && (document.DocumentType == "Receipt" || document.DocumentType == "Invoice")
                && document.VerificationStatus != ExpenseConstants.RejectedDocumentStatus),
            expense.PersonalClaim == null ? null : expense.PersonalClaim.Status,
            expense.MerchantName,
            expense.ExpenseLocation,
            expense.Notes,
            expense.CorrectionReason,
            expense.RejectionReason,
            expense.VersionNumber,
            expense.CreatedAt,
            expense.SubmittedAt,
            expense.ReviewedAt));

    private static ExpenseSummary MapExpense(ExpenseRow row) => new(
        row.ExpenseId,
        row.ExpenseNumber,
        row.ExpenseDate,
        row.TotalAmount,
        row.CurrencyCode,
        row.PaymentMode,
        row.Description,
        row.Status,
        new ExpenseCategorySummary(
            row.CategoryId,
            row.ParentCategoryId,
            row.CategoryCode,
            row.CategoryName,
            row.CategoryGroup,
            row.CategoryScope,
            row.CategoryDescription,
            row.CategoryRequiresSupplier,
            row.CategoryRequiresReceipt,
            row.CategorySupportsQuantity,
            row.CategoryIsActive,
            row.CategoryDisplayOrder,
            row.CategoryVersion),
        row.ProjectId is { } projectId
            ? new ExpenseProjectSummary(projectId, row.ProjectName!)
            : null,
        new ExpenseUserSummary(row.IncurredByUserId, row.IncurredByName, row.IncurredByRole),
        new ExpenseUserSummary(row.SubmittedByUserId, row.SubmittedByName, row.SubmittedByRole),
        row.ReceiptNumber,
        row.InvoiceNumber,
        row.HasReceiptDocument,
        row.ReimbursementStatus,
        row.VersionNumber,
        ToUtc(row.CreatedAt),
        ToNullableUtc(row.SubmittedAt),
        ToNullableUtc(row.ReviewedAt));

    private static ExpenseCategorySummary MapCategory(ExpenseCategory category) => new(
        category.ExpenseCategoryId,
        category.ParentExpenseCategoryId,
        category.CategoryCode,
        category.CategoryName,
        category.CategoryGroup,
        category.ExpenseScope,
        category.Description,
        category.RequiresSupplier,
        category.RequiresReceipt,
        category.SupportsQuantityDetails,
        category.IsActive,
        category.DisplayOrder,
        category.VersionNumber);

    private static IQueryable<ExpenseDocumentSummary> SelectDocuments(
        IQueryable<ExpenseDocument> documents) => documents.Select(document =>
            new ExpenseDocumentSummary(
                document.ExpenseDocumentId,
                document.DocumentType,
                document.DocumentNumber,
                document.DocumentDate,
                document.IssuerName,
                document.OriginalFileName,
                document.MimeType,
                document.FileSizeBytes,
                document.CaptureSource,
                document.IsPrimary,
                document.VerificationStatus,
                new ExpenseUserSummary(
                    document.UploadedByUserId,
                    document.AppUser.FullName,
                    document.AppUser.Role),
                document.VerifiedByUserId == null
                    ? null
                    : new ExpenseUserSummary(
                        document.VerifiedByUserId.Value,
                        document.AppUserNavigation!.FullName,
                        document.AppUserNavigation.Role),
                ToUtc(document.CreatedAt),
                ToNullableUtc(document.VerifiedAt),
                document.RejectionReason,
                document.Notes,
                document.VersionNumber));

    private static IQueryable<ReimbursementSummary> SelectReimbursements(
        IQueryable<PersonalClaim> claims) => claims.Select(claim => new ReimbursementSummary(
            claim.PersonalClaimId,
            claim.ClaimNumber,
            claim.ExpenseId!.Value,
            claim.Expense!.ExpenseNumber,
            new ExpenseUserSummary(
                claim.ClaimantUserId,
                claim.AppUserNavigation.FullName,
                claim.AppUserNavigation.Role),
            claim.ProjectId == null
                ? null
                : new ExpenseProjectSummary(claim.ProjectId.Value, claim.Project!.ProjectName),
            claim.ClaimDate,
            claim.DueDate,
            claim.ClaimAmount,
            claim.OutstandingAmount ?? 0m,
            claim.CurrencyCode,
            claim.Description,
            claim.Status,
            claim.VersionNumber,
            ToUtc(claim.CreatedAt)));

    private Task<Expense?> LockExpenseAsync(
        Guid companyId,
        Guid expenseId,
        CancellationToken cancellationToken) => dbContext.Expenses
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.expenses WHERE company_id = {companyId} AND expense_id = {expenseId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<UserAdvanceBalance?> LockBalanceAsync(
        Guid companyId,
        Guid balanceId,
        CancellationToken cancellationToken) => dbContext.UserAdvanceBalances
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.user_advance_balances WHERE company_id = {companyId} AND user_advance_balance_id = {balanceId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<PersonalClaim?> LockPersonalClaimAsync(
        Guid companyId,
        Guid expenseId,
        CancellationToken cancellationToken) => dbContext.PersonalClaims
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.personal_claims WHERE company_id = {companyId} AND expense_id = {expenseId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<SupplierDebt?> LockSupplierDebtAsync(
        Guid companyId,
        Guid expenseId,
        CancellationToken cancellationToken) => dbContext.SupplierDebts
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.supplier_debts WHERE company_id = {companyId} AND expense_id = {expenseId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private void AddBalanceLedger(
        UserAdvanceBalance balance,
        string entryType,
        decimal expensedDelta,
        decimal availableDelta,
        decimal reservedDelta,
        Guid actorUserId,
        Guid expenseId,
        string description)
    {
        dbContext.BalanceLedgerEntries.Add(new BalanceLedgerEntry
        {
            BalanceLedgerEntryId = Guid.NewGuid(),
            CompanyId = balance.CompanyId,
            UserAdvanceBalanceId = balance.UserAdvanceBalanceId,
            BalanceVersionNumber = balance.VersionNumber,
            EntryType = entryType,
            DeltaReceivedAmount = 0m,
            DeltaRestoredAmount = 0m,
            DeltaExpensedAmount = expensedDelta,
            DeltaTransferredOutAmount = 0m,
            DeltaReturnedAmount = 0m,
            DeltaAdjustmentOutAmount = 0m,
            DeltaAvailableAmount = availableDelta,
            DeltaReservedAmount = reservedDelta,
            ReceivedAfterAmount = balance.TotalReceivedAmount,
            RestoredAfterAmount = balance.TotalRestoredAmount,
            ExpensedAfterAmount = balance.TotalExpensedAmount,
            TransferredOutAfterAmount = balance.TotalTransferredOutAmount,
            ReturnedAfterAmount = balance.TotalReturnedAmount,
            AdjustmentOutAfterAmount = balance.TotalAdjustmentOutAmount,
            AvailableAfterAmount = balance.AvailableAmount,
            ReservedAfterAmount = balance.ReservedAmount,
            ReferenceType = "Expense",
            ReferenceId = expenseId,
            CorrelationId = Guid.NewGuid(),
            Description = description,
            PerformedByUserId = actorUserId
        });
    }

    private void AddExpenseAudit(
        Caller caller,
        Expense expense,
        string eventName,
        string action,
        string description)
    {
        dbContext.AuditLogs.Add(new AuditLog
        {
            AuditLogId = Guid.NewGuid(),
            CompanyId = caller.CompanyId,
            EventCategory = "Expense",
            EventName = eventName,
            EventAction = action,
            Severity = "Information",
            Outcome = "Success",
            ActorType = "User",
            ActorUserId = caller.UserId,
            ActorNameSnapshot = caller.FullName,
            ActorRoleSnapshot = caller.Role,
            EntityType = "Expense",
            EntityId = expense.ExpenseId,
            EntityVersionNumber = expense.VersionNumber,
            Description = description,
            ChangedFields = "[]",
            Metadata = "{}",
            CorrelationId = Guid.NewGuid(),
            SourceType = "Application",
            OccurredAt = timeProvider.GetUtcNow().UtcDateTime
        });
    }

    private async Task<IdempotencyAttempt> BeginIdempotencyAsync(
        Caller caller,
        string idempotencyKey,
        string operationName,
        string requestPath,
        string fingerprint,
        CancellationToken cancellationToken)
    {
        var existing = await dbContext.IdempotencyRecords
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.idempotency_records WHERE company_id = {caller.CompanyId} AND idempotency_key = {idempotencyKey} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
        if (existing is not null)
        {
            if (existing.OperationName != operationName
                || existing.RequestFingerprintHash != fingerprint
                || existing.ActorType != "User"
                || existing.ActorUserId != caller.UserId
                || existing.Status != "Completed"
                || existing.ResourceId is null)
            {
                return new(null, null, Conflict: true);
            }

            existing.ReplayCount++;
            existing.LastReplayedAt = timeProvider.GetUtcNow().UtcDateTime;
            return new(existing, existing.ResourceId, Conflict: false);
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var record = new IdempotencyRecord
        {
            IdempotencyRecordId = Guid.NewGuid(),
            CompanyId = caller.CompanyId,
            IdempotencyKey = idempotencyKey,
            OperationName = operationName,
            RequestMethod = "POST",
            RequestPath = requestPath,
            RequestSource = "Application",
            RequestFingerprintHash = fingerprint,
            RequestPayloadHash = fingerprint,
            ActorType = "User",
            ActorUserId = caller.UserId,
            Status = "InProgress",
            AttemptCount = 1,
            LockToken = Guid.NewGuid(),
            LockedBy = "Ahdah.Api",
            LockAcquiredAt = now,
            LeaseExpiresAt = now.AddMinutes(5),
            ReplayCount = 0,
            CorrelationId = Guid.NewGuid(),
            StartedAt = now,
            ExpiresAt = now.AddHours(24)
        };
        dbContext.IdempotencyRecords.Add(record);
        return new(record, null, Conflict: false);
    }

    private void CompleteIdempotency(
        IdempotencyRecord record,
        string resourceType,
        Guid resourceId,
        int? resourceVersion,
        int responseStatus)
    {
        record.Status = "Completed";
        record.LockToken = null;
        record.LockedBy = null;
        record.LockAcquiredAt = null;
        record.LeaseExpiresAt = null;
        record.ResponseHttpStatus = responseStatus;
        record.ResponseContentType = "application/json";
        record.ResourceType = resourceType;
        record.ResourceId = resourceId;
        record.ResourceVersionNumber = resourceVersion;
        record.CompletedAt = timeProvider.GetUtcNow().UtcDateTime;
    }

    private static void SetResponsePayload<T>(IdempotencyRecord record, T value) =>
        record.ResponsePayload = JsonSerializer.Serialize(value);

    private static T? DeserializeReplay<T>(string? payload)
    {
        try
        {
            return string.IsNullOrWhiteSpace(payload) ? default : JsonSerializer.Deserialize<T>(payload);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    private static string Fingerprint(string operationName, object payload)
    {
        var bytes = Encoding.UTF8.GetBytes($"{operationName}|{JsonSerializer.Serialize(payload)}");
        return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }

    private static bool DocumentRequired(CompanySetting settings, decimal amount) =>
        ExpenseDocumentRules.IsRequired(
            settings.ExpenseDocumentMode, settings.ExpenseDocumentThresholdAmount, amount);

    private static bool DocumentAllowed(CompanySetting settings, AddExpenseDocumentRequest request)
    {
        if (request.FileSizeBytes > settings.MaxDocumentSizeBytes)
        {
            return false;
        }

        var mimeTypes = ParseJsonArray(settings.AllowedMimeTypes);
        var extensions = ParseJsonArray(settings.AllowedFileExtensions);
        return mimeTypes.Contains(request.MimeType.Trim(), StringComparer.OrdinalIgnoreCase)
            && extensions.Contains(
                Path.GetExtension(request.OriginalFileName),
                StringComparer.OrdinalIgnoreCase);
    }

    private static string[] ParseJsonArray(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static bool IsValid(ExpenseQuery query) =>
        IsValidPage(query.Page, query.PageSize)
        && (query.Status is null || ExpenseRules.IsKnownStatus(query.Status))
        && (query.PaymentMode is null || ExpenseRules.IsKnownPaymentMode(query.PaymentMode))
        && (query.CategoryId is null || query.CategoryId != Guid.Empty)
        && (query.ProjectId is null || query.ProjectId != Guid.Empty)
        && (query.IncurredByUserId is null || query.IncurredByUserId != Guid.Empty)
        && (query.SubmittedByUserId is null || query.SubmittedByUserId != Guid.Empty)
        && (query.Reference is null
            || NormalizeOptional(query.Reference) is
            { Length: >= 2 and <= ExpenseConstants.MaximumSearchLength });

    private static bool Validate(object value)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(value, new ValidationContext(value), results, true);
    }

    private static bool IsValidPage(int page, int pageSize) =>
        page >= 1 && pageSize is >= 1 and <= AccessConstants.MaximumPageSize;

    private static int TotalPages(int count, int pageSize) =>
        count == 0 ? 0 : (count + pageSize - 1) / pageSize;

    private static string CreateReference(string prefix, Guid id) => $"{prefix}-{id:N}";

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTimeOffset ToUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static DateTimeOffset? ToNullableUtc(DateTime? value) =>
        value is null ? null : ToUtc(value.Value);

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private sealed record Caller(
        Guid CompanyId,
        Guid UserId,
        string FullName,
        string Role,
        ExpenseRoleCapabilities Capabilities);

    private sealed record IdempotencyAttempt(
        IdempotencyRecord? Record,
        Guid? ReplayResourceId,
        bool Conflict);

    private sealed record ExpenseRow(
        Guid ExpenseId,
        string ExpenseNumber,
        DateOnly ExpenseDate,
        decimal SubtotalAmount,
        decimal DiscountAmount,
        decimal TaxAmount,
        decimal TotalAmount,
        string CurrencyCode,
        string PaymentMode,
        string Description,
        string Status,
        Guid CategoryId,
        Guid? ParentCategoryId,
        string? CategoryCode,
        string CategoryName,
        string CategoryGroup,
        string CategoryScope,
        string? CategoryDescription,
        bool CategoryRequiresSupplier,
        bool CategoryRequiresReceipt,
        bool CategorySupportsQuantity,
        bool CategoryIsActive,
        int CategoryDisplayOrder,
        int CategoryVersion,
        Guid? ProjectId,
        string? ProjectName,
        Guid IncurredByUserId,
        string IncurredByName,
        string IncurredByRole,
        Guid SubmittedByUserId,
        string SubmittedByName,
        string SubmittedByRole,
        Guid? ReviewerUserId,
        string? ReviewerName,
        string? ReviewerRole,
        string? ReceiptNumber,
        string? InvoiceNumber,
        bool HasReceiptDocument,
        string? ReimbursementStatus,
        string? MerchantName,
        string? ExpenseLocation,
        string? Notes,
        string? CorrectionReason,
        string? RejectionReason,
        int VersionNumber,
        DateTime CreatedAt,
        DateTime? SubmittedAt,
        DateTime? ReviewedAt);
}
