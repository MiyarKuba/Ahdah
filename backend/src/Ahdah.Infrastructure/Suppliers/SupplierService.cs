using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses;
using Ahdah.Application.Identity;
using Ahdah.Application.Suppliers;
using Ahdah.Application.Suppliers.Contracts;
using Ahdah.Application.Suppliers.Models;
using Ahdah.Application.Suppliers.Services;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Suppliers;

public sealed class SupplierService(
    AhdahDbContext dbContext,
    ICurrentUserContext currentUserContext,
    TimeProvider timeProvider) : ISupplierService
{
    public async Task<AccessResult<PagedResult<SupplierSummary>>> ListSuppliersAsync(
        SupplierQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<PagedResult<SupplierSummary>>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewSuppliers) return Failure<PagedResult<SupplierSummary>>(AccessResultStatus.Forbidden);
        if (!ValidPage(query) || query.SupplierType is not null
                && !SupplierConstants.SupplierTypes.Contains(query.SupplierType)
            || query.Search is not null && Normalize(query.Search) is not { Length: >= 2 and <= 50 })
            return Failure<PagedResult<SupplierSummary>>(AccessResultStatus.Invalid);

        var suppliers = VisibleSuppliers(caller, dbContext.Suppliers.AsNoTracking());
        if (query.IsActive is { } isActive) suppliers = suppliers.Where(value => value.IsActive == isActive);
        if (query.SupplierType is not null) suppliers = suppliers.Where(value => value.SupplierType == query.SupplierType);
        if (Normalize(query.Search) is { } search)
        {
            var lowered = search.ToLower();
            suppliers = suppliers.Where(value => value.SupplierName.ToLower().StartsWith(lowered)
                || value.SupplierCode != null && value.SupplierCode.ToLower().StartsWith(lowered)
                || value.ContactPersonName != null && value.ContactPersonName.ToLower().StartsWith(lowered)
                || value.PhoneNumber != null && value.PhoneNumber.StartsWith(search));
        }

        var count = await suppliers.CountAsync(cancellationToken);
        var supplierRows = await suppliers.OrderBy(value => value.SupplierName).ThenBy(value => value.SupplierId)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToArrayAsync(cancellationToken);
        var items = supplierRows.Select(MapSupplier).ToArray();
        return Success(new PagedResult<SupplierSummary>(items, query.Page, query.PageSize, count,
            TotalPages(count, query.PageSize)));
    }

    public async Task<AccessResult<SupplierDetails>> GetSupplierAsync(
        Guid supplierId,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewSuppliers) return Failure<SupplierDetails>(AccessResultStatus.Forbidden);
        var supplier = await VisibleSuppliers(caller, dbContext.Suppliers.AsNoTracking())
            .SingleOrDefaultAsync(value => value.SupplierId == supplierId, cancellationToken);
        return supplier is null
            ? Failure<SupplierDetails>(AccessResultStatus.NotFound)
            : Success(await MapSupplierDetailsAsync(supplier, caller, cancellationToken));
    }

    public async Task<AccessResult<SupplierDetails>> CreateSupplierAsync(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanManageSuppliers) return Failure<SupplierDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request)) return Failure<SupplierDetails>(AccessResultStatus.Invalid);

        var supplier = new Supplier
        {
            SupplierId = Guid.NewGuid(), CompanyId = caller.CompanyId,
            SupplierCode = Normalize(request.SupplierCode)?.ToUpperInvariant(),
            SupplierName = request.SupplierName.Trim(), SupplierType = request.SupplierType,
            ContactPersonName = Normalize(request.ContactPersonName), PhoneNumber = Normalize(request.PhoneNumber),
            SecondaryPhoneNumber = Normalize(request.SecondaryPhoneNumber), Email = Normalize(request.Email)?.ToLowerInvariant(),
            CommercialRegistrationNumber = Normalize(request.CommercialRegistrationNumber),
            TaxRegistrationNumber = Normalize(request.TaxRegistrationNumber), Address = Normalize(request.Address),
            City = Normalize(request.City), DefaultCurrencyCode = request.DefaultCurrencyCode,
            TransactionMode = request.TransactionMode, DefaultPaymentTermsDays = request.DefaultPaymentTermsDays,
            CreditLimit = request.CreditLimit, PreferredPaymentMethod = request.PreferredPaymentMethod,
            Notes = Normalize(request.Notes), IsActive = true, CreatedByUserId = caller.UserId, VersionNumber = 1
        };
        dbContext.Suppliers.Add(supplier);
        AddAudit(caller, "Supplier", supplier.SupplierId, supplier.VersionNumber,
            "SupplierCreated", "Create", "Supplier directory entry created.");
        try { await dbContext.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return Failure<SupplierDetails>(AccessResultStatus.Conflict); }
        return Success(await MapSupplierDetailsAsync(supplier, caller, cancellationToken));
    }

    public async Task<AccessResult<SupplierDetails>> UpdateSupplierAsync(
        Guid supplierId,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanManageSuppliers) return Failure<SupplierDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request)) return Failure<SupplierDetails>(AccessResultStatus.Invalid);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var supplier = await LockSupplierAsync(caller.CompanyId, supplierId, cancellationToken);
            if (supplier is null) return await Rollback<SupplierDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            if (supplier.VersionNumber != request.ExpectedVersion || request.IsActive == true && !supplier.IsActive)
                return await Rollback<SupplierDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);

            if (request.ContactPersonName is not null) supplier.ContactPersonName = Normalize(request.ContactPersonName);
            if (request.PhoneNumber is not null) supplier.PhoneNumber = Normalize(request.PhoneNumber);
            if (request.SecondaryPhoneNumber is not null) supplier.SecondaryPhoneNumber = Normalize(request.SecondaryPhoneNumber);
            if (request.Email is not null) supplier.Email = Normalize(request.Email)?.ToLowerInvariant();
            if (request.Address is not null) supplier.Address = Normalize(request.Address);
            if (request.City is not null) supplier.City = Normalize(request.City);
            if (request.Notes is not null) supplier.Notes = Normalize(request.Notes);
            if (request.IsActive == false && supplier.IsActive)
            {
                supplier.IsActive = false;
                supplier.DeactivatedByUserId = caller.UserId;
                supplier.DeactivatedAt = Now();
                supplier.DeactivationReason = request.DeactivationReason!.Trim();
            }
            supplier.VersionNumber++;
            AddAudit(caller, "Supplier", supplier.SupplierId, supplier.VersionNumber,
                supplier.IsActive ? "SupplierUpdated" : "SupplierDeactivated", "Update",
                supplier.IsActive ? "Supplier metadata updated." : "Supplier deactivated without deleting history.");
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(await MapSupplierDetailsAsync(supplier, caller, cancellationToken));
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public async Task<AccessResult<PagedResult<SupplierPaymentAccountSummary>>> ListPaymentAccountsAsync(
        Guid supplierId, SupplierSubresourceQuery query, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<PagedResult<SupplierPaymentAccountSummary>>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewCompanyFinancials) return Failure<PagedResult<SupplierPaymentAccountSummary>>(AccessResultStatus.Forbidden);
        if (!ValidPage(query)) return Failure<PagedResult<SupplierPaymentAccountSummary>>(AccessResultStatus.Invalid);
        if (!await dbContext.Suppliers.AsNoTracking().AnyAsync(value => value.CompanyId == caller.CompanyId
            && value.SupplierId == supplierId, cancellationToken))
            return Failure<PagedResult<SupplierPaymentAccountSummary>>(AccessResultStatus.NotFound);
        var values = dbContext.SupplierPaymentAccounts.AsNoTracking().Where(value =>
            value.CompanyId == caller.CompanyId && value.SupplierId == supplierId);
        var count = await values.CountAsync(cancellationToken);
        var rows = await values.OrderByDescending(value => value.IsDefault).ThenBy(value => value.AccountLabel)
            .ThenBy(value => value.SupplierPaymentAccountId).Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize).ToArrayAsync(cancellationToken);
        return Success(new PagedResult<SupplierPaymentAccountSummary>(rows.Select(MapAccount).ToArray(),
            query.Page, query.PageSize, count, TotalPages(count, query.PageSize)));
    }

    public async Task<AccessResult<SupplierPaymentAccountSummary>> CreatePaymentAccountAsync(
        Guid supplierId, CreateSupplierPaymentAccountRequest request, string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierPaymentAccountSummary>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanManagePaymentAccounts) return Failure<SupplierPaymentAccountSummary>(AccessResultStatus.Forbidden);
        if (!Validate(request) || !SupplierRules.IsValidIdempotencyKey(idempotencyKey))
            return Failure<SupplierPaymentAccountSummary>(AccessResultStatus.Invalid);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotency(caller, idempotencyKey!, "supplier_accounts.create",
                $"/api/v1/suppliers/{supplierId}/payment-accounts", new { supplierId, request }, cancellationToken);
            var replay = await Replay<SupplierPaymentAccountSummary>(attempt, transaction, cancellationToken);
            if (replay is not null) return replay;
            var supplier = await LockSupplierAsync(caller.CompanyId, supplierId, cancellationToken);
            if (supplier is null || !supplier.IsActive)
                return await Rollback<SupplierPaymentAccountSummary>(transaction, AccessResultStatus.NotFound, cancellationToken);
            var account = new SupplierPaymentAccount
            {
                SupplierPaymentAccountId = Guid.NewGuid(), CompanyId = caller.CompanyId, SupplierId = supplierId,
                AccountType = request.AccountType, AccountLabel = request.AccountLabel.Trim(),
                AccountHolderName = Normalize(request.AccountHolderName), BankName = Normalize(request.BankName),
                BankBranchName = Normalize(request.BankBranchName), AccountNumber = Normalize(request.AccountNumber),
                Iban = Normalize(request.Iban)?.Replace(" ", "", StringComparison.Ordinal).ToUpperInvariant(),
                WalletProvider = Normalize(request.WalletProvider), WalletNumber = Normalize(request.WalletNumber),
                CurrencyCode = request.CurrencyCode, IsDefault = false,
                VerificationStatus = SupplierConstants.PendingVerificationStatus, Notes = Normalize(request.Notes),
                IsActive = true, CreatedByUserId = caller.UserId, VersionNumber = 1
            };
            dbContext.SupplierPaymentAccounts.Add(account);
            AddAudit(caller, "SupplierPaymentAccount", account.SupplierPaymentAccountId, 1,
                "SupplierPaymentAccountCreated", "Create", "Supplier payment account metadata recorded pending verification.");
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = MapAccount(account);
            Complete(attempt.Record!, "SupplierPaymentAccount", account.SupplierPaymentAccountId, 1, 201, result);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(result);
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierPaymentAccountSummary>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierPaymentAccountSummary>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public async Task<AccessResult<PagedResult<SupplierInvoiceSummary>>> ListInvoicesAsync(
        SupplierInvoiceQuery query, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<PagedResult<SupplierInvoiceSummary>>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewSuppliers) return Failure<PagedResult<SupplierInvoiceSummary>>(AccessResultStatus.Forbidden);
        if (!ValidInvoiceQuery(query)) return Failure<PagedResult<SupplierInvoiceSummary>>(AccessResultStatus.Invalid);
        var values = VisibleDebts(caller, dbContext.SupplierDebts.AsNoTracking());
        if (query.SupplierId is { } supplierId) values = values.Where(value => value.SupplierId == supplierId);
        if (query.ProjectId is { } projectId) values = values.Where(value => value.ProjectId == projectId);
        if (query.Status is not null) values = values.Where(value => value.Status == query.Status);
        if (query.CurrencyCode is not null) values = values.Where(value => value.CurrencyCode == query.CurrencyCode);
        if (query.DueFrom is { } dueFrom) values = values.Where(value => value.DueDate >= dueFrom);
        if (query.DueTo is { } dueTo) values = values.Where(value => value.DueDate <= dueTo);
        if (query.UnpaidOnly == true) values = values.Where(value => value.OutstandingAmount > 0m);
        if (query.OverdueOnly == true)
        {
            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
            values = values.Where(value => value.DueDate < today && value.OutstandingAmount > 0m);
        }
        if (Normalize(query.Reference) is { } reference)
        {
            var lowered = reference.ToLower();
            values = values.Where(value => value.DebtNumber.ToLower().StartsWith(lowered)
                || value.Expense.ExpenseNumber.ToLower().StartsWith(lowered)
                || value.Expense.InvoiceNumber != null && value.Expense.InvoiceNumber.ToLower().StartsWith(lowered));
        }
        var count = await values.CountAsync(cancellationToken);
        var debtRows = await values.Include(value => value.Supplier).Include(value => value.Expense)
            .Include(value => value.Project).OrderByDescending(value => value.DebtDate)
            .ThenByDescending(value => value.SupplierDebtId).Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize).ToArrayAsync(cancellationToken);
        var rows = debtRows.Select(InvoiceProjection).ToArray();
        return Success(new PagedResult<SupplierInvoiceSummary>(rows, query.Page, query.PageSize, count,
            TotalPages(count, query.PageSize)));
    }

    public async Task<AccessResult<SupplierInvoiceDetails>> GetInvoiceAsync(Guid debtId, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierInvoiceDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewSuppliers) return Failure<SupplierInvoiceDetails>(AccessResultStatus.Forbidden);
        var debt = await VisibleDebts(caller, dbContext.SupplierDebts.AsNoTracking())
            .SingleOrDefaultAsync(value => value.SupplierDebtId == debtId, cancellationToken);
        return debt is null ? Failure<SupplierInvoiceDetails>(AccessResultStatus.NotFound)
            : Success(await InvoiceDetails(debt, cancellationToken));
    }

    public async Task<AccessResult<SupplierInvoiceDetails>> CreateInvoiceAsync(
        CreateSupplierInvoiceRequest request, string? idempotencyKey, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierInvoiceDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanCreateInvoices) return Failure<SupplierInvoiceDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request) || !SupplierRules.IsValidIdempotencyKey(idempotencyKey))
            return Failure<SupplierInvoiceDetails>(AccessResultStatus.Invalid);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotency(caller, idempotencyKey!, "supplier_invoices.create",
                "/api/v1/supplier-invoices", request, cancellationToken);
            var replay = await Replay<SupplierInvoiceDetails>(attempt, transaction, cancellationToken);
            if (replay is not null) return replay;
            var supplier = await LockSupplierAsync(caller.CompanyId, request.SupplierId, cancellationToken);
            if (supplier is null || !supplier.IsActive || supplier.TransactionMode == SupplierConstants.CashOnlyTransactionMode)
                return await Rollback<SupplierInvoiceDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);
            var category = await dbContext.ExpenseCategories.AsNoTracking().SingleOrDefaultAsync(value =>
                value.CompanyId == caller.CompanyId && value.ExpenseCategoryId == request.ExpenseCategoryId
                && value.IsActive, cancellationToken);
            if (category is null || !category.RequiresSupplier
                || !ExpenseRules.ProjectMatchesScope(category.ExpenseScope, request.ProjectId)
                || request.Items.Count > 0 && !category.SupportsQuantityDetails)
                return await Rollback<SupplierInvoiceDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);
            if (request.ProjectId is { } projectId && !await dbContext.Projects.AsNoTracking().AnyAsync(value =>
                value.CompanyId == caller.CompanyId && value.ProjectId == projectId, cancellationToken))
                return await Rollback<SupplierInvoiceDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);

            var now = Now();
            var expenseId = Guid.NewGuid();
            var debtId = Guid.NewGuid();
            var expense = new Expense
            {
                ExpenseId = expenseId, CompanyId = caller.CompanyId, ExpenseNumber = Reference("EXP", expenseId),
                ProjectId = request.ProjectId, ExpenseCategoryId = request.ExpenseCategoryId,
                SupplierId = request.SupplierId, IncurredByUserId = caller.UserId, ExpenseDate = request.InvoiceDate,
                PaymentMode = SupplierConstants.SupplierCreditPaymentMode, CurrencyCode = request.CurrencyCode,
                SubtotalAmount = request.Amount, DiscountAmount = 0m, TaxAmount = 0m, TotalAmount = request.Amount,
                CreditDueDate = request.DueDate, InvoiceNumber = Normalize(request.InvoiceNumber),
                MerchantName = supplier.SupplierName, Description = request.Description.Trim(), Notes = Normalize(request.Notes),
                Status = ExpenseConstants.PendingReviewStatus, SubmittedByUserId = caller.UserId,
                SubmittedAt = now, VersionNumber = 1
            };
            dbContext.Expenses.Add(expense);
            for (var index = 0; index < request.Items.Count; index++)
            {
                var item = request.Items[index];
                dbContext.ExpenseItems.Add(new ExpenseItem
                {
                    ExpenseItemId = Guid.NewGuid(), CompanyId = caller.CompanyId, ExpenseId = expenseId,
                    LineNumber = index + 1, ItemName = item.ItemName.Trim(), ItemCode = Normalize(item.ItemCode),
                    ItemDescription = Normalize(item.ItemDescription), Quantity = item.Quantity, UnitCode = item.UnitCode,
                    CustomUnitName = Normalize(item.CustomUnitName), UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount, TaxAmount = item.TaxAmount, Notes = Normalize(item.Notes),
                    CreatedByUserId = caller.UserId
                });
            }
            var debt = new SupplierDebt
            {
                SupplierDebtId = debtId, CompanyId = caller.CompanyId, DebtNumber = Reference("SDEBT", debtId),
                SupplierId = request.SupplierId, ExpenseId = expenseId, ProjectId = request.ProjectId,
                ExpenseVersionNumber = 1, DebtDate = request.InvoiceDate, DueDate = request.DueDate,
                CurrencyCode = request.CurrencyCode, DebtAmount = request.Amount, AdjustmentAmount = 0m,
                PaidAmount = 0m, CreditNoteAmount = 0m, WrittenOffAmount = 0m,
                Status = SupplierConstants.OpenDebtStatus, Description = request.Description.Trim(),
                Notes = Normalize(request.Notes), RecordedByUserId = caller.UserId, VersionNumber = 1
            };
            dbContext.SupplierDebts.Add(debt);
            AddDebtLedger(debt, SupplierConstants.DebtCreatedLedgerType, request.Amount, 0m, 0m,
                expenseId, "Expense", caller.UserId, "Supplier debt created from supplier-credit expense.");
            AddAudit(caller, "SupplierDebt", debtId, 1, "SupplierInvoiceRecorded", "Create",
                "Supplier-credit expense and debt recorded pending expense review.");
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Entry(debt).ReloadAsync(cancellationToken);
            var result = await InvoiceDetails(debt, cancellationToken);
            Complete(attempt.Record!, "SupplierDebt", debtId, debt.VersionNumber, 201, result);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(result);
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierInvoiceDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierInvoiceDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public async Task<AccessResult<PagedResult<SupplierPaymentSummary>>> ListPaymentsAsync(
        SupplierPaymentQuery query, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<PagedResult<SupplierPaymentSummary>>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewCompanyFinancials) return Failure<PagedResult<SupplierPaymentSummary>>(AccessResultStatus.Forbidden);
        if (!ValidPaymentQuery(query)) return Failure<PagedResult<SupplierPaymentSummary>>(AccessResultStatus.Invalid);
        var values = dbContext.SupplierPayments.AsNoTracking().Where(value => value.CompanyId == caller.CompanyId);
        if (query.SupplierId is { } supplierId) values = values.Where(value => value.SupplierId == supplierId);
        if (query.SupplierDebtId is { } debtId) values = values.Where(value => value.SupplierPaymentDebtAllocations.Any(allocation => allocation.SupplierDebtId == debtId));
        if (query.FundingSourceId is { } sourceId) values = values.Where(value => value.SupplierPaymentFundingSources.Any(allocation => allocation.FundingSourceId == sourceId));
        if (query.Status is not null) values = values.Where(value => value.Status == query.Status);
        if (query.PaymentMethod is not null) values = values.Where(value => value.PaymentMethod == query.PaymentMethod);
        if (query.CurrencyCode is not null) values = values.Where(value => value.CurrencyCode == query.CurrencyCode);
        if (query.DateFrom is { } from) values = values.Where(value => value.PaymentDate >= from);
        if (query.DateTo is { } to) values = values.Where(value => value.PaymentDate <= to);
        var count = await values.CountAsync(cancellationToken);
        var paymentRows = await values.Include(value => value.Supplier).Include(value => value.AppUser1)
            .Include(value => value.AppUserNavigation).OrderByDescending(value => value.PaymentDate)
            .ThenByDescending(value => value.SupplierPaymentId).Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize).ToArrayAsync(cancellationToken);
        var items = paymentRows.Select(PaymentProjection).ToArray();
        return Success(new PagedResult<SupplierPaymentSummary>(items, query.Page, query.PageSize, count,
            TotalPages(count, query.PageSize)));
    }

    public async Task<AccessResult<SupplierPaymentDetails>> GetPaymentAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierPaymentDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewCompanyFinancials) return Failure<SupplierPaymentDetails>(AccessResultStatus.Forbidden);
        var value = await LoadPayment(caller.CompanyId, paymentId, cancellationToken);
        return value is null ? Failure<SupplierPaymentDetails>(AccessResultStatus.NotFound) : Success(value);
    }

    public async Task<AccessResult<SupplierPaymentDetails>> CreatePaymentAsync(
        CreateSupplierPaymentRequest request, string? idempotencyKey, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierPaymentDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanRecordPayments) return Failure<SupplierPaymentDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request) || !SupplierRules.IsValidIdempotencyKey(idempotencyKey))
            return Failure<SupplierPaymentDetails>(AccessResultStatus.Invalid);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotency(caller, idempotencyKey!, "supplier_payments.create",
                "/api/v1/supplier-payments", request, cancellationToken);
            var replay = await Replay<SupplierPaymentDetails>(attempt, transaction, cancellationToken);
            if (replay is not null) return replay;
            var supplier = await LockSupplierAsync(caller.CompanyId, request.SupplierId, cancellationToken);
            if (supplier is null || !supplier.IsActive)
                return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            if (request.SupplierPaymentAccountId is { } accountId && !await dbContext.SupplierPaymentAccounts.AsNoTracking()
                .AnyAsync(value => value.CompanyId == caller.CompanyId && value.SupplierPaymentAccountId == accountId
                    && value.SupplierId == request.SupplierId && value.IsActive
                    && value.VerificationStatus == SupplierConstants.VerifiedStatus
                    && value.CurrencyCode == request.CurrencyCode, cancellationToken))
                return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);
            var settings = await Settings(caller.CompanyId, cancellationToken);
            if (settings?.MaxSingleSupplierPaymentAmount is { } maximum && request.PaymentAmount > maximum)
                return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);
            var requiresApproval = RequiresApproval(settings, request.PaymentAmount);
            if (!requiresApproval && !CanConfirm(request.PaymentMethod, request.ReferenceNumber,
                request.ProofFileUrl, settings, request.PaymentAmount))
                return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);

            var paymentId = Guid.NewGuid();
            var now = Now();
            var payment = new SupplierPayment
            {
                SupplierPaymentId = paymentId, CompanyId = caller.CompanyId,
                PaymentNumber = Reference("SPAY", paymentId), SupplierId = request.SupplierId,
                SupplierPaymentAccountId = request.SupplierPaymentAccountId, PaymentDate = request.PaymentDate,
                CurrencyCode = request.CurrencyCode, PaymentAmount = request.PaymentAmount, FeeAmount = 0m,
                PaymentMethod = request.PaymentMethod, PayerBankName = Normalize(request.PayerBankName),
                ReferenceNumber = Normalize(request.ReferenceNumber), ProofFileUrl = Normalize(request.ProofFileUrl),
                Description = Normalize(request.Description), Notes = Normalize(request.Notes),
                Status = requiresApproval ? SupplierConstants.PendingApprovalStatus : SupplierConstants.ConfirmedStatus,
                CreatedByUserId = caller.UserId, SubmittedByUserId = caller.UserId, SubmittedAt = now,
                ConfirmedByUserId = requiresApproval ? null : caller.UserId,
                ConfirmedAt = requiresApproval ? null : now, VersionNumber = 1
            };
            dbContext.SupplierPayments.Add(payment);

            foreach (var allocationRequest in request.DebtAllocations.OrderBy(value => value.SupplierDebtId))
            {
                var debt = await LockDebtAsync(caller.CompanyId, allocationRequest.SupplierDebtId, cancellationToken);
                if (debt is null || debt.SupplierId != request.SupplierId || debt.CurrencyCode != request.CurrencyCode
                    || debt.Status is not (SupplierConstants.OpenDebtStatus or SupplierConstants.PartiallySettledDebtStatus)
                    || !await ApprovedExpense(caller.CompanyId, debt.ExpenseId, cancellationToken))
                    return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);
                var pending = await PendingDebtAllocations(caller.CompanyId, debt.SupplierDebtId, null, cancellationToken);
                if ((debt.OutstandingAmount ?? 0m) - pending < allocationRequest.Amount)
                    return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                dbContext.SupplierPaymentDebtAllocations.Add(new SupplierPaymentDebtAllocation
                {
                    SupplierPaymentDebtAllocationId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                    SupplierPaymentId = paymentId, SupplierDebtId = debt.SupplierDebtId,
                    DebtVersionNumber = debt.VersionNumber, AllocatedAmount = allocationRequest.Amount,
                    AllocatedByUserId = caller.UserId, Notes = Normalize(allocationRequest.Notes)
                });
                if (!requiresApproval) ApplyPaymentToDebt(debt, allocationRequest.Amount, paymentId, caller.UserId);
            }

            foreach (var allocationRequest in request.FundingAllocations.OrderBy(value => value.FundingSourceId))
            {
                var source = await LockFundingSource(caller.CompanyId, allocationRequest.FundingSourceId, cancellationToken);
                if (source is null || source.CurrencyCode != request.CurrencyCode
                    || source.Status is not (SupplierConstants.AvailableFundingStatus or SupplierConstants.PartiallyUsedFundingStatus)
                    || source.AvailableAmount < allocationRequest.Amount
                    || !await SourceAllowed(caller, source, cancellationToken))
                    return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                source.AvailableAmount -= allocationRequest.Amount;
                source.ReservedAmount += allocationRequest.Amount;
                source.VersionNumber++;
                SetFundingStatus(source);
                AddFundingLedger(source, SupplierConstants.AmountReservedLedgerType, allocationRequest.Amount,
                    -allocationRequest.Amount, allocationRequest.Amount, 0m, paymentId, caller.UserId,
                    "Supplier payment amount reserved.");
                if (!requiresApproval)
                {
                    source.ReservedAmount -= allocationRequest.Amount;
                    source.UsedAmount += allocationRequest.Amount;
                    source.VersionNumber++;
                    SetFundingStatus(source);
                    AddFundingLedger(source, SupplierConstants.SupplierPaymentLedgerType, allocationRequest.Amount,
                        0m, -allocationRequest.Amount, allocationRequest.Amount, paymentId, caller.UserId,
                        "Confirmed supplier payment consumed reserved funding.");
                }
                dbContext.SupplierPaymentFundingSources.Add(new SupplierPaymentFundingSource
                {
                    SupplierPaymentFundingSourceId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                    SupplierPaymentId = paymentId, FundingSourceId = source.FundingSourceId,
                    AllocatedAmount = allocationRequest.Amount, AllocatedByUserId = caller.UserId,
                    Notes = Normalize(allocationRequest.Notes)
                });
            }
            AddAudit(caller, "SupplierPayment", paymentId, 1, "SupplierPaymentRecorded", "Create",
                requiresApproval ? "Supplier payment recorded pending approval with funding reserved."
                    : "Supplier payment confirmed and allocated.");
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadPayment(caller.CompanyId, paymentId, cancellationToken);
            if (result is null) return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            Complete(attempt.Record!, "SupplierPayment", paymentId, 1, 201, result);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(result);
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public Task<AccessResult<SupplierPaymentDetails>> ConfirmPaymentAsync(
        Guid paymentId, ReviewSupplierPaymentRequest request, string? idempotencyKey,
        CancellationToken cancellationToken) => ReviewPayment(paymentId, request, true, idempotencyKey, cancellationToken);

    public Task<AccessResult<SupplierPaymentDetails>> RejectPaymentAsync(
        Guid paymentId, ReviewSupplierPaymentRequest request, string? idempotencyKey,
        CancellationToken cancellationToken) => ReviewPayment(paymentId, request, false, idempotencyKey, cancellationToken);

    private async Task<AccessResult<SupplierPaymentDetails>> ReviewPayment(
        Guid paymentId, ReviewSupplierPaymentRequest request, bool confirm, string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierPaymentDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanRecordPayments) return Failure<SupplierPaymentDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request) || !SupplierRules.IsValidIdempotencyKey(idempotencyKey)
            || !confirm && string.IsNullOrWhiteSpace(request.RejectionReason))
            return Failure<SupplierPaymentDetails>(AccessResultStatus.Invalid);
        var operation = confirm ? "supplier_payments.confirm" : "supplier_payments.reject";
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotency(caller, idempotencyKey!, operation,
                $"/api/v1/supplier-payments/{paymentId}/{(confirm ? "confirm" : "reject")}",
                new { paymentId, request.ExpectedVersion, request.RejectionReason }, cancellationToken);
            var replay = await Replay<SupplierPaymentDetails>(attempt, transaction, cancellationToken);
            if (replay is not null) return replay;
            var payment = await LockPayment(caller.CompanyId, paymentId, cancellationToken);
            if (payment is null) return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            if (payment.Status != SupplierConstants.PendingApprovalStatus || payment.VersionNumber != request.ExpectedVersion)
                return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            var settings = await Settings(caller.CompanyId, cancellationToken);
            if (confirm && caller.UserId == payment.SubmittedByUserId && (settings is null
                || !settings.AllowSelfApproval || settings.RequireDistinctCreatorApprover
                || settings.RequireDistinctSubmitterApprover))
                return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Forbidden, cancellationToken);
            if (confirm && !CanConfirm(payment.PaymentMethod, payment.ReferenceNumber, payment.ProofFileUrl,
                settings, payment.PaymentAmount))
                return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);
            var funding = await dbContext.SupplierPaymentFundingSources.AsNoTracking().Where(value =>
                value.CompanyId == caller.CompanyId && value.SupplierPaymentId == paymentId)
                .OrderBy(value => value.FundingSourceId).ToArrayAsync(cancellationToken);
            foreach (var allocation in funding)
            {
                var source = await LockFundingSource(caller.CompanyId, allocation.FundingSourceId, cancellationToken);
                if (source is null || source.ReservedAmount < allocation.AllocatedAmount)
                    return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                source.ReservedAmount -= allocation.AllocatedAmount;
                if (confirm) source.UsedAmount += allocation.AllocatedAmount;
                else source.AvailableAmount += allocation.AllocatedAmount;
                source.VersionNumber++;
                SetFundingStatus(source);
                AddFundingLedger(source, confirm ? SupplierConstants.SupplierPaymentLedgerType : "ReservationReleased",
                    allocation.AllocatedAmount, confirm ? 0m : allocation.AllocatedAmount,
                    -allocation.AllocatedAmount, confirm ? allocation.AllocatedAmount : 0m,
                    paymentId, caller.UserId, confirm ? "Approved supplier payment consumed funding."
                        : "Rejected supplier payment released funding reservation.");
            }
            if (confirm)
            {
                var debts = await dbContext.SupplierPaymentDebtAllocations.AsNoTracking().Where(value =>
                    value.CompanyId == caller.CompanyId && value.SupplierPaymentId == paymentId)
                    .OrderBy(value => value.SupplierDebtId).ToArrayAsync(cancellationToken);
                foreach (var allocation in debts)
                {
                    var debt = await LockDebtAsync(caller.CompanyId, allocation.SupplierDebtId, cancellationToken);
                    if (debt is null || debt.SupplierId != payment.SupplierId || debt.CurrencyCode != payment.CurrencyCode
                        || !await ApprovedExpense(caller.CompanyId, debt.ExpenseId, cancellationToken))
                        return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                    var otherPending = await PendingDebtAllocations(caller.CompanyId, debt.SupplierDebtId, paymentId, cancellationToken);
                    if ((debt.OutstandingAmount ?? 0m) - otherPending < allocation.AllocatedAmount)
                        return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                    ApplyPaymentToDebt(debt, allocation.AllocatedAmount, paymentId, caller.UserId);
                }
                payment.Status = SupplierConstants.ConfirmedStatus;
                payment.ConfirmedByUserId = caller.UserId;
                payment.ConfirmedAt = Now();
            }
            else
            {
                payment.Status = SupplierConstants.RejectedStatus;
                payment.RejectedByUserId = caller.UserId;
                payment.RejectedAt = Now();
                payment.RejectionReason = request.RejectionReason!.Trim();
            }
            payment.VersionNumber++;
            AddAudit(caller, "SupplierPayment", paymentId, payment.VersionNumber,
                confirm ? "SupplierPaymentConfirmed" : "SupplierPaymentRejected",
                confirm ? "Confirm" : "Reject", confirm ? "Supplier payment confirmed and allocated."
                    : "Supplier payment rejected; funding reservation released.");
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadPayment(caller.CompanyId, paymentId, cancellationToken);
            if (result is null) return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            Complete(attempt.Record!, "SupplierPayment", paymentId, payment.VersionNumber, 200, result);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(result);
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierPaymentDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public async Task<AccessResult<PagedResult<SupplierRefundSummary>>> ListRefundsAsync(
        SupplierRefundQuery query, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<PagedResult<SupplierRefundSummary>>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewCompanyFinancials) return Failure<PagedResult<SupplierRefundSummary>>(AccessResultStatus.Forbidden);
        if (!ValidPage(query) || query.Status is not null && !SupplierConstants.RefundStatuses.Contains(query.Status))
            return Failure<PagedResult<SupplierRefundSummary>>(AccessResultStatus.Invalid);
        var values = dbContext.SupplierRefunds.AsNoTracking().Where(value => value.CompanyId == caller.CompanyId);
        if (query.SupplierId is { } supplierId) values = values.Where(value => value.SupplierId == supplierId);
        if (query.Status is not null) values = values.Where(value => value.Status == query.Status);
        if (query.CurrencyCode is not null) values = values.Where(value => value.CurrencyCode == query.CurrencyCode);
        var count = await values.CountAsync(cancellationToken);
        var refundRows = await values.Include(value => value.Supplier).OrderByDescending(value => value.RefundDate)
            .ThenByDescending(value => value.SupplierRefundId).Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize).ToArrayAsync(cancellationToken);
        var items = refundRows.Select(value => new SupplierRefundSummary(
                value.SupplierRefundId, value.RefundNumber, value.SupplierId, value.Supplier.SupplierName,
                value.ExpenseReturnId, value.RefundDate, value.RefundAmount, value.FeeAmount,
                value.NetReceivedAmount ?? 0m, value.CurrencyCode, value.RefundMethod,
                value.SupplierReferenceNumber, value.TransactionReferenceNumber, value.ProofFileUrl != null,
                value.Description, value.Status, value.VersionNumber, ToUtc(value.CreatedAt))).ToArray();
        return Success(new PagedResult<SupplierRefundSummary>(items, query.Page, query.PageSize, count,
            TotalPages(count, query.PageSize)));
    }

    public async Task<AccessResult<PagedResult<SupplierCreditNoteSummary>>> ListCreditNotesAsync(
        SupplierCreditNoteQuery query, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<PagedResult<SupplierCreditNoteSummary>>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewCompanyFinancials) return Failure<PagedResult<SupplierCreditNoteSummary>>(AccessResultStatus.Forbidden);
        if (!ValidPage(query) || query.Status is not null && !SupplierConstants.CreditNoteStatuses.Contains(query.Status))
            return Failure<PagedResult<SupplierCreditNoteSummary>>(AccessResultStatus.Invalid);
        var values = dbContext.SupplierCreditNotes.AsNoTracking().Where(value => value.CompanyId == caller.CompanyId);
        if (query.SupplierId is { } supplierId) values = values.Where(value => value.SupplierId == supplierId);
        if (query.Status is not null) values = values.Where(value => value.Status == query.Status);
        if (query.CurrencyCode is not null) values = values.Where(value => value.CurrencyCode == query.CurrencyCode);
        var count = await values.CountAsync(cancellationToken);
        var creditRows = await values.Include(value => value.Supplier)
            .Include(value => value.SupplierCreditNoteAllocations).OrderByDescending(value => value.CreditNoteDate)
            .ThenByDescending(value => value.SupplierCreditNoteId).Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize).ToArrayAsync(cancellationToken);
        var items = creditRows.Select(CreditProjection).ToArray();
        return Success(new PagedResult<SupplierCreditNoteSummary>(items, query.Page, query.PageSize, count,
            TotalPages(count, query.PageSize)));
    }

    public async Task<AccessResult<SupplierCreditNoteDetails>> GetCreditNoteAsync(Guid creditNoteId, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewCompanyFinancials) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Forbidden);
        var value = await LoadCredit(caller.CompanyId, creditNoteId, cancellationToken);
        return value is null ? Failure<SupplierCreditNoteDetails>(AccessResultStatus.NotFound) : Success(value);
    }

    public async Task<AccessResult<SupplierCreditNoteDetails>> CreateCreditNoteAsync(
        CreateSupplierCreditNoteRequest request, string? idempotencyKey, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanManageCredits) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request) || !SupplierRules.IsValidIdempotencyKey(idempotencyKey))
            return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Invalid);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotency(caller, idempotencyKey!, "supplier_credit_notes.create",
                "/api/v1/supplier-credit-notes", request, cancellationToken);
            var replay = await Replay<SupplierCreditNoteDetails>(attempt, transaction, cancellationToken);
            if (replay is not null) return replay;
            var supplier = await LockSupplierAsync(caller.CompanyId, request.SupplierId, cancellationToken);
            if (supplier is null || !supplier.IsActive)
                return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            var id = Guid.NewGuid();
            var note = new SupplierCreditNote
            {
                SupplierCreditNoteId = id, CompanyId = caller.CompanyId,
                CreditNoteNumber = Reference("SCN", id), SupplierId = request.SupplierId,
                SupplierReferenceNumber = Normalize(request.SupplierReferenceNumber),
                CreditNoteDate = request.CreditNoteDate, CurrencyCode = request.CurrencyCode,
                CreditNoteAmount = request.Amount, ReasonType = request.ReasonType,
                Description = request.Description.Trim(), Notes = Normalize(request.Notes),
                Status = SupplierConstants.PendingApprovalStatus, CreatedByUserId = caller.UserId,
                SubmittedByUserId = caller.UserId, SubmittedAt = Now(), VersionNumber = 1
            };
            dbContext.SupplierCreditNotes.Add(note);
            AddAudit(caller, "SupplierCreditNote", id, 1, "SupplierCreditNoteCreated", "Create",
                "Supplier credit note recorded pending approval.");
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadCredit(caller.CompanyId, id, cancellationToken);
            if (result is null) return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            Complete(attempt.Record!, "SupplierCreditNote", id, 1, 201, result);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(result);
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public async Task<AccessResult<SupplierCreditNoteDetails>> ApproveCreditNoteAsync(
        Guid creditNoteId, ApproveSupplierCreditNoteRequest request, string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanManageCredits) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request) || !SupplierRules.IsValidIdempotencyKey(idempotencyKey))
            return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Invalid);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotency(caller, idempotencyKey!, "supplier_credit_notes.approve",
                $"/api/v1/supplier-credit-notes/{creditNoteId}/approve", new { creditNoteId, request.ExpectedVersion }, cancellationToken);
            var replay = await Replay<SupplierCreditNoteDetails>(attempt, transaction, cancellationToken);
            if (replay is not null) return replay;
            var note = await LockCredit(caller.CompanyId, creditNoteId, cancellationToken);
            if (note is null) return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            if (note.Status != SupplierConstants.PendingApprovalStatus || note.VersionNumber != request.ExpectedVersion)
                return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            var settings = await Settings(caller.CompanyId, cancellationToken);
            if (caller.UserId == note.SubmittedByUserId && (settings is null || !settings.AllowSelfApproval
                || settings.RequireDistinctCreatorApprover || settings.RequireDistinctSubmitterApprover))
                return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Forbidden, cancellationToken);
            note.Status = SupplierConstants.ApprovedStatus;
            note.ApprovedByUserId = caller.UserId;
            note.ApprovedAt = Now();
            note.VersionNumber++;
            AddAudit(caller, "SupplierCreditNote", creditNoteId, note.VersionNumber,
                "SupplierCreditNoteApproved", "Approve", "Supplier credit note approved for allocation.");
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadCredit(caller.CompanyId, creditNoteId, cancellationToken);
            if (result is null) return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            Complete(attempt.Record!, "SupplierCreditNote", creditNoteId, note.VersionNumber, 200, result);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(result);
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public async Task<AccessResult<SupplierCreditNoteDetails>> ApplyCreditNoteAsync(
        Guid creditNoteId, ApplySupplierCreditNoteRequest request, string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanManageCredits) return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Forbidden);
        if (!Validate(request) || !SupplierRules.IsValidIdempotencyKey(idempotencyKey))
            return Failure<SupplierCreditNoteDetails>(AccessResultStatus.Invalid);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var attempt = await BeginIdempotency(caller, idempotencyKey!, "supplier_credit_notes.allocate",
                $"/api/v1/supplier-credit-notes/{creditNoteId}/allocations", new { creditNoteId, request }, cancellationToken);
            var replay = await Replay<SupplierCreditNoteDetails>(attempt, transaction, cancellationToken);
            if (replay is not null) return replay;
            var note = await LockCredit(caller.CompanyId, creditNoteId, cancellationToken);
            if (note is null) return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            if (note.Status != SupplierConstants.ApprovedStatus || note.VersionNumber != request.ExpectedVersion)
                return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            var applied = await dbContext.SupplierCreditNoteAllocations.AsNoTracking().Where(value =>
                value.CompanyId == caller.CompanyId && value.SupplierCreditNoteId == creditNoteId)
                .SumAsync(value => (decimal?)value.AllocatedAmount, cancellationToken) ?? 0m;
            var requested = request.Allocations.Sum(value => value.Amount);
            if (applied + requested > note.CreditNoteAmount)
                return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            foreach (var allocationRequest in request.Allocations.OrderBy(value => value.SupplierDebtId))
            {
                if (await dbContext.SupplierCreditNoteAllocations.AsNoTracking().AnyAsync(value =>
                    value.CompanyId == caller.CompanyId && value.SupplierCreditNoteId == creditNoteId
                    && value.SupplierDebtId == allocationRequest.SupplierDebtId, cancellationToken))
                    return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                var debt = await LockDebtAsync(caller.CompanyId, allocationRequest.SupplierDebtId, cancellationToken);
                if (debt is null || debt.SupplierId != note.SupplierId || debt.CurrencyCode != note.CurrencyCode
                    || debt.Status is not (SupplierConstants.OpenDebtStatus or SupplierConstants.PartiallySettledDebtStatus)
                    || !await ApprovedExpense(caller.CompanyId, debt.ExpenseId, cancellationToken))
                    return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Invalid, cancellationToken);
                var pending = await PendingDebtAllocations(caller.CompanyId, debt.SupplierDebtId, null, cancellationToken);
                if ((debt.OutstandingAmount ?? 0m) - pending < allocationRequest.Amount)
                    return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                dbContext.SupplierCreditNoteAllocations.Add(new SupplierCreditNoteAllocation
                {
                    SupplierCreditNoteAllocationId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                    SupplierCreditNoteId = creditNoteId, SupplierDebtId = debt.SupplierDebtId,
                    DebtVersionNumber = debt.VersionNumber, AllocatedAmount = allocationRequest.Amount,
                    AllocatedByUserId = caller.UserId, Notes = Normalize(allocationRequest.Notes)
                });
                debt.CreditNoteAmount += allocationRequest.Amount;
                debt.VersionNumber++;
                UpdateDebtStatus(debt);
                AddDebtLedger(debt, SupplierConstants.CreditNoteAppliedLedgerType, 0m, 0m,
                    allocationRequest.Amount, creditNoteId, "SupplierCreditNote", caller.UserId,
                    "Approved supplier credit note applied to debt.");
            }
            note.VersionNumber++;
            AddAudit(caller, "SupplierCreditNote", creditNoteId, note.VersionNumber,
                "SupplierCreditNoteAllocated", "Allocate", "Approved supplier credit allocated to debt.");
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadCredit(caller.CompanyId, creditNoteId, cancellationToken);
            if (result is null) return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            Complete(attempt.Record!, "SupplierCreditNote", creditNoteId, note.VersionNumber, 200, result);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Success(result);
        }
        catch (DbUpdateConcurrencyException)
        { return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
        catch (DbUpdateException exception) when (Unique(exception))
        { return await Rollback<SupplierCreditNoteDetails>(transaction, AccessResultStatus.Conflict, cancellationToken); }
    }

    public async Task<AccessResult<SupplierStatement>> GetStatementAsync(
        Guid supplierId, SupplierStatementQuery query, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return Failure<SupplierStatement>(AccessResultStatus.Unauthorized);
        if (!caller.Capabilities.CanViewCompanyFinancials) return Failure<SupplierStatement>(AccessResultStatus.Forbidden);
        if (!ValidPage(query)) return Failure<SupplierStatement>(AccessResultStatus.Invalid);
        var supplier = await dbContext.Suppliers.AsNoTracking().SingleOrDefaultAsync(value =>
            value.CompanyId == caller.CompanyId && value.SupplierId == supplierId, cancellationToken);
        if (supplier is null) return Failure<SupplierStatement>(AccessResultStatus.NotFound);
        var balances = await BalanceRows(caller.CompanyId, supplierId, cancellationToken);
        var ledgers = dbContext.SupplierDebtLedgerEntries.AsNoTracking().Where(value =>
            value.CompanyId == caller.CompanyId && value.SupplierDebt.SupplierId == supplierId);
        if (query.CurrencyCode is not null) ledgers = ledgers.Where(value => value.SupplierDebt.CurrencyCode == query.CurrencyCode);
        var count = await ledgers.CountAsync(cancellationToken);
        var ledgerRows = await ledgers.Include(value => value.SupplierDebt)
            .OrderByDescending(value => value.OccurredAt)
            .ThenByDescending(value => value.SupplierDebtLedgerEntryId)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToArrayAsync(cancellationToken);
        var rows = ledgerRows.Select(value => new SupplierStatementEntry(
                value.ReferenceId ?? value.SupplierDebtId, value.EntryType,
                value.SupplierDebt.DebtNumber, DateOnly.FromDateTime(value.OccurredAt),
                StatementAmount(value), value.SupplierDebt.CurrencyCode, value.DebtStatusAfter,
                value.Description ?? value.EntryType, ToUtc(value.OccurredAt)))
            .ToArray();
        return Success(new SupplierStatement(MapSupplier(supplier), balances,
            new PagedResult<SupplierStatementEntry>(rows, query.Page, query.PageSize, count,
                TotalPages(count, query.PageSize))));
    }

    private async Task<Caller?> GetCallerAsync(CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.CompanyId is not { } companyId
            || currentUserContext.UserId is not { } userId || currentUserContext.Role is not { } claimRole)
            return null;
        var user = await dbContext.AppUsers.AsNoTracking().Where(value => value.CompanyId == companyId
            && value.UserId == userId && value.Status == IdentityConstants.ActiveStatus
            && value.Company.Status == IdentityConstants.ActiveStatus)
            .Select(value => new { value.FullName, value.Role }).SingleOrDefaultAsync(cancellationToken);
        return user is null || user.Role != claimRole ? null
            : new Caller(companyId, userId, user.FullName, user.Role, SupplierRoleCapabilities.For(user.Role));
    }

    private IQueryable<Supplier> VisibleSuppliers(Caller caller, IQueryable<Supplier> values)
    {
        values = values.Where(value => value.CompanyId == caller.CompanyId);
        if (caller.Capabilities.CanViewCompanyFinancials) return values;
        if (!caller.Capabilities.CanViewAssignedProjectSuppliers) return values.Where(_ => false);
        return values.Where(supplier => supplier.SupplierDebts.Any(debt => debt.CompanyId == caller.CompanyId
            && debt.ProjectId != null && dbContext.ProjectSupervisors.Any(assignment =>
                assignment.CompanyId == caller.CompanyId && assignment.ProjectId == debt.ProjectId
                && assignment.SupervisorUserId == caller.UserId && assignment.IsActive)));
    }

    private IQueryable<SupplierDebt> VisibleDebts(Caller caller, IQueryable<SupplierDebt> values)
    {
        values = values.Where(value => value.CompanyId == caller.CompanyId);
        if (caller.Capabilities.CanViewCompanyFinancials) return values;
        if (!caller.Capabilities.CanViewAssignedProjectSuppliers) return values.Where(_ => false);
        return values.Where(debt => debt.ProjectId != null && dbContext.ProjectSupervisors.Any(assignment =>
            assignment.CompanyId == caller.CompanyId && assignment.ProjectId == debt.ProjectId
            && assignment.SupervisorUserId == caller.UserId && assignment.IsActive));
    }

    private async Task<SupplierDetails> MapSupplierDetailsAsync(Supplier supplier, Caller caller, CancellationToken cancellationToken) =>
        new(MapSupplier(supplier), supplier.SecondaryPhoneNumber, supplier.CommercialRegistrationNumber,
            supplier.TaxRegistrationNumber, supplier.Address, supplier.Notes,
            caller.Capabilities.CanViewCompanyFinancials
                ? await BalanceRows(caller.CompanyId, supplier.SupplierId, cancellationToken) : []);

    private async Task<IReadOnlyList<SupplierBalanceSummary>> BalanceRows(
        Guid companyId, Guid supplierId, CancellationToken cancellationToken) =>
        await dbContext.SupplierDebts.AsNoTracking().Where(value => value.CompanyId == companyId
                && value.SupplierId == supplierId && value.OutstandingAmount > 0m
                && (value.Status == SupplierConstants.OpenDebtStatus
                    || value.Status == SupplierConstants.PartiallySettledDebtStatus))
            .GroupBy(value => value.CurrencyCode)
            .Select(group => new SupplierBalanceSummary(group.Key, group.Sum(value => value.OutstandingAmount ?? 0m), group.Count()))
            .OrderBy(value => value.CurrencyCode).ToArrayAsync(cancellationToken);

    private static SupplierSummary MapSupplier(Supplier value) => new(
        value.SupplierId, value.SupplierCode, value.SupplierName, value.SupplierType,
        value.ContactPersonName, value.PhoneNumber, value.Email, value.City,
        value.DefaultCurrencyCode, value.TransactionMode, value.DefaultPaymentTermsDays,
        value.CreditLimit, value.PreferredPaymentMethod, value.IsActive, value.VersionNumber,
        ToUtc(value.CreatedAt), ToUtc(value.UpdatedAt));

    private static SupplierPaymentAccountSummary MapAccount(SupplierPaymentAccount value) => new(
        value.SupplierPaymentAccountId, value.AccountType, value.AccountLabel, value.AccountHolderName,
        value.BankName, value.BankBranchName, Mask(value.AccountNumber), Mask(value.Iban), value.WalletProvider,
        Mask(value.WalletNumber), value.CurrencyCode, value.IsDefault, value.VerificationStatus,
        value.IsActive, value.Notes, value.VersionNumber, ToUtc(value.CreatedAt));

    private static SupplierInvoiceSummary InvoiceProjection(SupplierDebt value) => new(
        value.SupplierDebtId, value.DebtNumber, value.ExpenseId, value.Expense.ExpenseNumber,
        value.Expense.InvoiceNumber, MapSupplier(value.Supplier),
        value.ProjectId == null ? null : new SupplierProjectSummary(value.ProjectId.Value, value.Project!.ProjectName),
        value.DebtDate, value.DueDate, value.DebtAmount, value.PaidAmount, value.CreditNoteAmount,
        value.WrittenOffAmount, value.OutstandingAmount ?? 0m, value.CurrencyCode, value.Expense.Status,
        value.Status, value.Description ?? value.Expense.Description, value.ExpenseVersionNumber,
        value.VersionNumber, ToUtc(value.CreatedAt));

    private async Task<SupplierInvoiceDetails> InvoiceDetails(SupplierDebt debt, CancellationToken cancellationToken)
    {
        var loaded = await dbContext.SupplierDebts.AsNoTracking().Include(value => value.Supplier)
            .Include(value => value.Expense).Include(value => value.Project).SingleAsync(value => value.CompanyId == debt.CompanyId
            && value.SupplierDebtId == debt.SupplierDebtId, cancellationToken);
        var items = await dbContext.ExpenseItems.AsNoTracking().Where(value => value.CompanyId == debt.CompanyId
            && value.ExpenseId == debt.ExpenseId).OrderBy(value => value.LineNumber).Select(value => new SupplierInvoiceItemSummary(
                value.ExpenseItemId, value.LineNumber, value.ItemName, value.ItemCode, value.ItemDescription,
                value.Quantity, value.UnitCode, value.CustomUnitName, value.UnitPrice, value.SubtotalAmount ?? 0m,
                value.DiscountAmount, value.TaxAmount, value.TotalAmount ?? 0m, value.Notes)).ToArrayAsync(cancellationToken);
        return new(InvoiceProjection(loaded), loaded.AdjustmentAmount, loaded.Notes, items);
    }

    private static SupplierPaymentSummary PaymentProjection(SupplierPayment value) => new(
        value.SupplierPaymentId, value.PaymentNumber, value.SupplierId, value.Supplier.SupplierName,
        value.PaymentDate, value.PaymentAmount, value.CurrencyCode, value.PaymentMethod,
        value.ReferenceNumber, value.ProofFileUrl != null, value.Status,
        new SupplierUserSummary(value.CreatedByUserId, value.AppUser1.FullName, value.AppUser1.Role),
        value.ConfirmedByUserId == null ? null : new SupplierUserSummary(value.ConfirmedByUserId.Value,
            value.AppUserNavigation!.FullName, value.AppUserNavigation.Role), value.VersionNumber,
        ToUtc(value.CreatedAt), ToNullableUtc(value.ConfirmedAt));

    private async Task<SupplierPaymentDetails?> LoadPayment(Guid companyId, Guid paymentId, CancellationToken cancellationToken)
    {
        var payment = await dbContext.SupplierPayments.AsNoTracking().Include(value => value.Supplier)
            .Include(value => value.AppUser1).Include(value => value.AppUserNavigation).SingleOrDefaultAsync(value =>
            value.CompanyId == companyId && value.SupplierPaymentId == paymentId, cancellationToken);
        if (payment is null) return null;
        var debts = await dbContext.SupplierPaymentDebtAllocations.AsNoTracking().Where(value =>
            value.CompanyId == companyId && value.SupplierPaymentId == paymentId)
            .OrderBy(value => value.SupplierDebt.DebtNumber).Select(value => new SupplierPaymentDebtAllocationSummary(
                value.SupplierPaymentDebtAllocationId, value.SupplierDebtId, value.SupplierDebt.DebtNumber,
                value.AllocatedAmount, ToUtc(value.CreatedAt))).ToArrayAsync(cancellationToken);
        var funding = await dbContext.SupplierPaymentFundingSources.AsNoTracking().Where(value =>
            value.CompanyId == companyId && value.SupplierPaymentId == paymentId)
            .OrderBy(value => value.FundingSourceId).Select(value => new SupplierPaymentFundingSummary(
                value.SupplierPaymentFundingSourceId, value.FundingSourceId, value.FundingSource.SourceType,
                value.AllocatedAmount, ToUtc(value.CreatedAt))).ToArrayAsync(cancellationToken);
        return new(PaymentProjection(payment), payment.SupplierPaymentAccountId, payment.PayerBankName,
            payment.Description, payment.Notes, debts, funding);
    }

    private static SupplierCreditNoteSummary CreditProjection(SupplierCreditNote value)
    {
        var applied = value.SupplierCreditNoteAllocations.Sum(allocation => allocation.AllocatedAmount);
        return new(value.SupplierCreditNoteId, value.CreditNoteNumber, value.SupplierId,
            value.Supplier.SupplierName, value.SupplierReferenceNumber, value.CreditNoteDate,
            value.CreditNoteAmount, applied, value.CreditNoteAmount - applied, value.CurrencyCode,
            value.ReasonType, value.Description, value.Status, value.VersionNumber, ToUtc(value.CreatedAt));
    }

    private async Task<SupplierCreditNoteDetails?> LoadCredit(Guid companyId, Guid creditNoteId, CancellationToken cancellationToken)
    {
        var note = await dbContext.SupplierCreditNotes.AsNoTracking().Include(value => value.Supplier)
            .Include(value => value.SupplierCreditNoteAllocations).SingleOrDefaultAsync(value =>
            value.CompanyId == companyId && value.SupplierCreditNoteId == creditNoteId, cancellationToken);
        if (note is null) return null;
        var allocations = await dbContext.SupplierCreditNoteAllocations.AsNoTracking().Where(value =>
            value.CompanyId == companyId && value.SupplierCreditNoteId == creditNoteId)
            .OrderBy(value => value.CreatedAt).ThenBy(value => value.SupplierCreditNoteAllocationId)
            .Select(value => new SupplierCreditAllocationSummary(value.SupplierCreditNoteAllocationId,
                value.SupplierDebtId, value.SupplierDebt.DebtNumber, value.AllocatedAmount, ToUtc(value.CreatedAt)))
            .ToArrayAsync(cancellationToken);
        var applied = allocations.Sum(value => value.AllocatedAmount);
        var summary = new SupplierCreditNoteSummary(note.SupplierCreditNoteId, note.CreditNoteNumber,
            note.SupplierId, note.Supplier.SupplierName, note.SupplierReferenceNumber, note.CreditNoteDate,
            note.CreditNoteAmount, applied, note.CreditNoteAmount - applied, note.CurrencyCode,
            note.ReasonType, note.Description, note.Status, note.VersionNumber, ToUtc(note.CreatedAt));
        return new(summary, note.Notes, allocations);
    }

    private void ApplyPaymentToDebt(SupplierDebt debt, decimal amount, Guid paymentId, Guid actor)
    {
        debt.PaidAmount += amount;
        debt.VersionNumber++;
        UpdateDebtStatus(debt);
        AddDebtLedger(debt, SupplierConstants.PaymentAppliedLedgerType, 0m, amount, 0m,
            paymentId, "SupplierPayment", actor, "Confirmed supplier payment applied to debt.");
    }

    private void UpdateDebtStatus(SupplierDebt debt)
    {
        var outstanding = debt.DebtAmount + debt.AdjustmentAmount - debt.PaidAmount
            - debt.CreditNoteAmount - debt.WrittenOffAmount;
        if (outstanding == 0m)
        {
            debt.Status = SupplierConstants.SettledDebtStatus;
            debt.SettledAt = Now();
        }
        else
        {
            debt.Status = debt.PaidAmount > 0m || debt.CreditNoteAmount > 0m || debt.WrittenOffAmount > 0m
                ? SupplierConstants.PartiallySettledDebtStatus : SupplierConstants.OpenDebtStatus;
            debt.SettledAt = null;
        }
    }

    private void AddDebtLedger(SupplierDebt debt, string entryType, decimal debtDelta, decimal paidDelta,
        decimal creditDelta, Guid referenceId, string referenceType, Guid actor, string description)
    {
        var outstanding = debt.DebtAmount + debt.AdjustmentAmount - debt.PaidAmount
            - debt.CreditNoteAmount - debt.WrittenOffAmount;
        dbContext.SupplierDebtLedgerEntries.Add(new SupplierDebtLedgerEntry
        {
            SupplierDebtLedgerEntryId = Guid.NewGuid(), CompanyId = debt.CompanyId,
            SupplierDebtId = debt.SupplierDebtId, DebtVersionNumber = debt.VersionNumber,
            EntryType = entryType, DeltaDebtAmount = debtDelta, DeltaAdjustmentAmount = 0m,
            DeltaPaidAmount = paidDelta, DeltaCreditNoteAmount = creditDelta, DeltaWrittenOffAmount = 0m,
            DebtAfterAmount = debt.DebtAmount, AdjustmentAfterAmount = debt.AdjustmentAmount,
            PaidAfterAmount = debt.PaidAmount, CreditNoteAfterAmount = debt.CreditNoteAmount,
            WrittenOffAfterAmount = debt.WrittenOffAmount, OutstandingAfterAmount = outstanding,
            DebtStatusAfter = debt.Status, ReferenceType = referenceType, ReferenceId = referenceId,
            CorrelationId = Guid.NewGuid(), Description = description, PerformedByUserId = actor,
            OccurredAt = Now()
        });
    }

    private void AddFundingLedger(FundingSource source, string type, decimal amount,
        decimal availableDelta, decimal reservedDelta, decimal usedDelta, Guid paymentId,
        Guid actor, string description) => dbContext.FundingSourceLedgerEntries.Add(new FundingSourceLedgerEntry
        {
            FundingSourceLedgerEntryId = Guid.NewGuid(), CompanyId = source.CompanyId,
            FundingSourceId = source.FundingSourceId, SourceVersionNumber = source.VersionNumber,
            EntryType = type, Amount = amount, AvailableDelta = availableDelta,
            ReservedDelta = reservedDelta, UsedDelta = usedDelta, ReversedDelta = 0m,
            AvailableAfter = source.AvailableAmount, ReservedAfter = source.ReservedAmount,
            UsedAfter = source.UsedAmount, ReversedAfter = source.ReversedAmount,
            ReferenceType = "SupplierPayment", ReferenceId = paymentId, PerformedByUserId = actor,
            CorrelationId = Guid.NewGuid(), Description = description
        });

    private static void SetFundingStatus(FundingSource source) => source.Status = source.UsedAmount switch
    {
        0m => SupplierConstants.AvailableFundingStatus,
        _ when source.UsedAmount == source.NetAmount => SupplierConstants.FullyUsedFundingStatus,
        _ => SupplierConstants.PartiallyUsedFundingStatus
    };

    private async Task<bool> SourceAllowed(Caller caller, FundingSource source, CancellationToken cancellationToken)
    {
        if (source.SourceType != SupplierConstants.ManagerContributionSourceType) return true;
        if (caller.Role != IdentityConstants.ManagerRole) return false;
        return await dbContext.ManagerContributions.AsNoTracking().AnyAsync(value =>
            value.CompanyId == caller.CompanyId && value.FundingSourceId == source.FundingSourceId
            && value.ManagerUserId == caller.UserId, cancellationToken);
    }

    private Task<CompanySetting?> Settings(Guid companyId, CancellationToken cancellationToken) =>
        dbContext.CompanySettings.AsNoTracking().SingleOrDefaultAsync(value =>
            value.CompanyId == companyId && value.IsActive, cancellationToken);

    private static bool RequiresApproval(CompanySetting? settings, decimal amount) => settings is null
        || settings.SupplierPaymentApprovalMode == "Always"
        || settings.SupplierPaymentApprovalMode == "Threshold"
            && settings.SupplierPaymentApprovalThresholdAmount is { } threshold && amount >= threshold;

    private static bool CanConfirm(string method, string? reference, string? proof,
        CompanySetting? settings, decimal amount)
    {
        if (method is "BankTransfer" or "Cheque" or "Card" or "MobileWallet"
            && string.IsNullOrWhiteSpace(proof)) return false;
        if (method is not ("Cash" or "Other") && string.IsNullOrWhiteSpace(reference)) return false;
        if (settings is null) return false;
        return settings.PaymentProofMode switch
        {
            "Always" => !string.IsNullOrWhiteSpace(proof),
            "Threshold" when settings.PaymentProofThresholdAmount is { } threshold && amount >= threshold
                => !string.IsNullOrWhiteSpace(proof),
            _ => true
        };
    }

    private Task<bool> ApprovedExpense(Guid companyId, Guid expenseId, CancellationToken cancellationToken) =>
        dbContext.Expenses.AsNoTracking().AnyAsync(value => value.CompanyId == companyId
            && value.ExpenseId == expenseId && value.PaymentMode == SupplierConstants.SupplierCreditPaymentMode
            && value.Status == ExpenseConstants.ApprovedStatus, cancellationToken);

    private async Task<decimal> PendingDebtAllocations(Guid companyId, Guid debtId, Guid? excludedPaymentId,
        CancellationToken cancellationToken) => await dbContext.SupplierPaymentDebtAllocations.AsNoTracking()
        .Where(value => value.CompanyId == companyId && value.SupplierDebtId == debtId
            && value.SupplierPayment.Status == SupplierConstants.PendingApprovalStatus
            && (excludedPaymentId == null || value.SupplierPaymentId != excludedPaymentId))
        .SumAsync(value => (decimal?)value.AllocatedAmount, cancellationToken) ?? 0m;

    private Task<Supplier?> LockSupplierAsync(Guid companyId, Guid supplierId, CancellationToken cancellationToken) =>
        dbContext.Suppliers.FromSqlInterpolated(
            $"SELECT * FROM ahdah.suppliers WHERE company_id = {companyId} AND supplier_id = {supplierId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<SupplierDebt?> LockDebtAsync(Guid companyId, Guid debtId, CancellationToken cancellationToken) =>
        dbContext.SupplierDebts.FromSqlInterpolated(
            $"SELECT * FROM ahdah.supplier_debts WHERE company_id = {companyId} AND supplier_debt_id = {debtId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<FundingSource?> LockFundingSource(Guid companyId, Guid sourceId, CancellationToken cancellationToken) =>
        dbContext.FundingSources.FromSqlInterpolated(
            $"SELECT * FROM ahdah.funding_sources WHERE company_id = {companyId} AND funding_source_id = {sourceId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<SupplierPayment?> LockPayment(Guid companyId, Guid paymentId, CancellationToken cancellationToken) =>
        dbContext.SupplierPayments.FromSqlInterpolated(
            $"SELECT * FROM ahdah.supplier_payments WHERE company_id = {companyId} AND supplier_payment_id = {paymentId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<SupplierCreditNote?> LockCredit(Guid companyId, Guid creditNoteId, CancellationToken cancellationToken) =>
        dbContext.SupplierCreditNotes.FromSqlInterpolated(
            $"SELECT * FROM ahdah.supplier_credit_notes WHERE company_id = {companyId} AND supplier_credit_note_id = {creditNoteId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private async Task<IdempotencyAttempt> BeginIdempotency(Caller caller, string key, string operation,
        string path, object payload, CancellationToken cancellationToken)
    {
        var fingerprint = Fingerprint(operation, payload);
        var existing = await dbContext.IdempotencyRecords.FromSqlInterpolated(
            $"SELECT * FROM ahdah.idempotency_records WHERE company_id = {caller.CompanyId} AND idempotency_key = {key} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
        if (existing is not null)
        {
            if (existing.OperationName != operation || existing.RequestFingerprintHash != fingerprint
                || existing.ActorType != "User" || existing.ActorUserId != caller.UserId
                || existing.Status != "Completed" || existing.ResourceId is null)
                return new(null, true, null);
            existing.ReplayCount++;
            existing.LastReplayedAt = Now();
            return new(existing, false, existing.ResourceId);
        }
        var now = Now();
        var record = new IdempotencyRecord
        {
            IdempotencyRecordId = Guid.NewGuid(), CompanyId = caller.CompanyId, IdempotencyKey = key,
            OperationName = operation, RequestMethod = "POST", RequestPath = path,
            RequestSource = "Application", RequestFingerprintHash = fingerprint,
            RequestPayloadHash = fingerprint, ActorType = "User", ActorUserId = caller.UserId,
            Status = "InProgress", AttemptCount = 1, LockToken = Guid.NewGuid(), LockedBy = "Ahdah.Api",
            LockAcquiredAt = now, LeaseExpiresAt = now.AddMinutes(5), ReplayCount = 0,
            CorrelationId = Guid.NewGuid(), StartedAt = now, ExpiresAt = now.AddHours(24)
        };
        dbContext.IdempotencyRecords.Add(record);
        return new(record, false, null);
    }

    private async Task<AccessResult<T>?> Replay<T>(IdempotencyAttempt attempt,
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction,
        CancellationToken cancellationToken)
    {
        if (attempt.Conflict)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Failure<T>(AccessResultStatus.Conflict);
        }
        if (attempt.ReplayResourceId is null) return null;
        T? value;
        try { value = JsonSerializer.Deserialize<T>(attempt.Record!.ResponsePayload ?? string.Empty); }
        catch (JsonException) { value = default; }
        if (value is null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Failure<T>(AccessResultStatus.Conflict);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Success(value);
    }

    private void Complete<T>(IdempotencyRecord record, string resourceType, Guid resourceId,
        int version, int status, T response)
    {
        record.Status = "Completed"; record.LockToken = null; record.LockedBy = null;
        record.LockAcquiredAt = null; record.LeaseExpiresAt = null; record.ResponseHttpStatus = status;
        record.ResponseContentType = "application/json"; record.ResourceType = resourceType;
        record.ResourceId = resourceId; record.ResourceVersionNumber = version;
        record.ResponsePayload = JsonSerializer.Serialize(response); record.CompletedAt = Now();
    }

    private void AddAudit(Caller caller, string entityType, Guid entityId, int version,
        string eventName, string action, string description) => dbContext.AuditLogs.Add(new AuditLog
        {
            AuditLogId = Guid.NewGuid(), CompanyId = caller.CompanyId,
            EventCategory = entityType is "Supplier" or "SupplierPaymentAccount" ? "Supplier" : "SupplierDebt",
            EventName = eventName, EventAction = action, Severity = "Information", Outcome = "Success",
            ActorType = "User", ActorUserId = caller.UserId, ActorNameSnapshot = caller.FullName,
            ActorRoleSnapshot = caller.Role, EntityType = entityType, EntityId = entityId,
            EntityVersionNumber = version, Description = description, ChangedFields = "[]", Metadata = "{}",
            CorrelationId = Guid.NewGuid(), SourceType = "Application", OccurredAt = Now()
        });

    private static bool ValidInvoiceQuery(SupplierInvoiceQuery query) => ValidPage(query)
        && (query.Status is null || SupplierConstants.DebtStatuses.Contains(query.Status))
        && (query.SupplierId is null || query.SupplierId != Guid.Empty)
        && (query.ProjectId is null || query.ProjectId != Guid.Empty)
        && (query.CurrencyCode is null || System.Text.RegularExpressions.Regex.IsMatch(query.CurrencyCode, "^[A-Z]{3}$"))
        && (query.DueFrom is null || query.DueTo is null || query.DueFrom <= query.DueTo)
        && (query.Reference is null || Normalize(query.Reference) is { Length: >= 2 and <= 50 });

    private static bool ValidPaymentQuery(SupplierPaymentQuery query) => ValidPage(query)
        && (query.Status is null || SupplierConstants.PaymentStatuses.Contains(query.Status))
        && (query.PaymentMethod is null || SupplierConstants.PaymentMethods.Contains(query.PaymentMethod))
        && (query.DateFrom is null || query.DateTo is null || query.DateFrom <= query.DateTo);

    private static bool ValidPage(PagedAccessQuery query) => query.Page >= 1
        && query.PageSize is >= 1 and <= AccessConstants.MaximumPageSize;
    private static bool Validate(object value)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(value, new ValidationContext(value), results, true);
    }
    private static bool Unique(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    private static string Fingerprint(string operation, object payload) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes($"{operation}|{JsonSerializer.Serialize(payload)}"))).ToLowerInvariant();
    private static string Reference(string prefix, Guid id) => $"{prefix}-{id:N}";
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? Mask(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Replace(" ", "", StringComparison.Ordinal);
        return normalized.Length <= 4 ? new string('*', normalized.Length) : $"****{normalized[^4..]}";
    }
    private static decimal StatementAmount(SupplierDebtLedgerEntry value) =>
        Math.Abs(value.DeltaDebtAmount)
        + Math.Abs(value.DeltaAdjustmentAmount)
        + Math.Abs(value.DeltaPaidAmount)
        + Math.Abs(value.DeltaCreditNoteAmount)
        + Math.Abs(value.DeltaWrittenOffAmount);
    private DateTime Now() => timeProvider.GetUtcNow().UtcDateTime;
    private static DateTimeOffset ToUtc(DateTime value) => new(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    private static DateTimeOffset? ToNullableUtc(DateTime? value) => value is null ? null : ToUtc(value.Value);
    private static int TotalPages(int count, int pageSize) => count == 0 ? 0 : (count + pageSize - 1) / pageSize;
    private static AccessResult<T> Success<T>(T value) => AccessResult<T>.Success(value);
    private static AccessResult<T> Failure<T>(AccessResultStatus status) => AccessResult<T>.Failure(status);
    private static async Task<AccessResult<T>> Rollback<T>(
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction,
        AccessResultStatus status, CancellationToken cancellationToken)
    {
        await transaction.RollbackAsync(cancellationToken);
        return Failure<T>(status);
    }

    private sealed record Caller(Guid CompanyId, Guid UserId, string FullName, string Role,
        SupplierRoleCapabilities Capabilities);
    private sealed record IdempotencyAttempt(IdempotencyRecord? Record, bool Conflict, Guid? ReplayResourceId);
}
