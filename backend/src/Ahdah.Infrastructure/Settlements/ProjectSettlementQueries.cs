using Ahdah.Application.Settlements;
using Ahdah.Infrastructure.Persistence.Generated.Context;

namespace Ahdah.Infrastructure.Settlements;

/// <summary>Every root and relationship is tenant scoped before projection.</summary>
internal static class ProjectSettlementQueries
{
    internal sealed record CategoryQuery(string Category, string AmountMeaning,
        IQueryable<ProjectSettlementBlocker> Records);

    internal static IReadOnlyList<CategoryQuery> Create(
        AhdahDbContext db, Guid companyId, Guid projectId, bool restricted, decimal? documentBoundary)
    {
        var expenses = db.Expenses.Where(e => e.CompanyId == companyId && e.ProjectId == projectId);
        var activeExpenses = expenses.Where(e => e.Status != "Rejected" && e.Status != "Cancelled"
            && e.Status != "Reversed");
        var debts = db.SupplierDebts.Where(d => d.CompanyId == companyId && d.ProjectId == projectId);
        var claims = db.PersonalClaims.Where(c => c.CompanyId == companyId && c.ProjectId == projectId);
        var returns = db.ExpenseReturns.Where(r => r.CompanyId == companyId
            && expenses.Any(e => e.ExpenseId == r.ExpenseId));
        var categories = new List<CategoryQuery>
        {
            new("Expenses", "PendingExpenseAmount", activeExpenses.Where(e => e.Status != "Approved")
                .Select(e => new ProjectSettlementBlocker("Expenses",
                    e.Status == "Draft" ? "ExpenseDraft" : e.Status == "PendingReview" ? "ExpensePendingReview"
                    : e.Status == "CorrectionRequired" ? "ExpenseCorrectionRequired" : "UnknownExpenseStatus",
                    "Expense", e.ExpenseId, e.Status, e.CurrencyCode, e.TotalAmount,
                    "/api/v1/expenses/" + e.ExpenseId))),
            new("ExpenseDocuments", "None", activeExpenses.Where(e =>
                (db.ExpenseCategories.Any(c => c.CompanyId == companyId
                    && c.ExpenseCategoryId == e.ExpenseCategoryId && c.RequiresReceipt)
                    && !db.ExpenseDocuments.Any(d => d.CompanyId == companyId && d.ExpenseId == e.ExpenseId
                        && d.VerificationStatus != "Rejected" && (d.DocumentType == "Receipt" || d.DocumentType == "Invoice")))
                || (documentBoundary != null && e.TotalAmount >= documentBoundary
                    && !db.ExpenseDocuments.Any(d => d.CompanyId == companyId && d.ExpenseId == e.ExpenseId
                        && d.VerificationStatus != "Rejected")))
                .Select(e => new ProjectSettlementBlocker("ExpenseDocuments", "MissingExpenseDocument",
                    "Expense", e.ExpenseId, e.Status, null, null,
                    "/api/v1/expenses/" + e.ExpenseId + "/documents"))),
            new("SupplierDebt", "OutstandingLiability", debts.Where(d => d.Status != "Settled"
                && d.Status != "Cancelled" && d.Status != "Reversed")
                .Select(d => new ProjectSettlementBlocker("SupplierDebt",
                    d.Status != "Open" && d.Status != "PartiallySettled" ? "UnknownSupplierDebtStatus"
                    : d.OutstandingAmount == null ? "OutstandingAmountUnavailable" : "OutstandingSupplierDebt",
                    "SupplierDebt", d.SupplierDebtId, d.Status, d.CurrencyCode, d.OutstandingAmount,
                    "/api/v1/supplier-debts/" + d.SupplierDebtId)))
        };
        if (restricted)
        {
            return categories;
        }

        var unsettledClaims = claims.Where(c => c.Status != "Settled" && c.Status != "Cancelled"
            && c.Status != "Reversed");
        categories.Add(new("Reimbursements", "OutstandingLiability",
            unsettledClaims.Where(c => c.SourceType != "ManagerContribution")
                .Select(c => new ProjectSettlementBlocker("Reimbursements",
                    c.Status != "Open" && c.Status != "PartiallySettled" ? "UnknownClaimStatus"
                    : c.OutstandingAmount == null ? "OutstandingAmountUnavailable" : "UnresolvedReimbursement",
                    "PersonalClaim", c.PersonalClaimId, c.Status, c.CurrencyCode, c.OutstandingAmount,
                    "/api/v1/reimbursements/" + c.PersonalClaimId))));
        categories.Add(new("ManagerContributions", "OutstandingLiability",
            unsettledClaims.Where(c => c.SourceType == "ManagerContribution")
                .Select(c => new ProjectSettlementBlocker("ManagerContributions",
                    c.Status != "Open" && c.Status != "PartiallySettled" ? "UnknownClaimStatus"
                    : c.OutstandingAmount == null ? "OutstandingAmountUnavailable" : "UnsettledManagerContributionClaim",
                    "PersonalClaim", c.PersonalClaimId, c.Status, c.CurrencyCode, c.OutstandingAmount,
                    "/api/v1/reimbursements/" + c.PersonalClaimId))));

        // Sum only allocations to this project. A header may cover several projects.
        var paymentAllocations = db.SupplierPaymentDebtAllocations.Where(a => a.CompanyId == companyId
            && debts.Any(d => d.SupplierDebtId == a.SupplierDebtId));
        categories.Add(new("SupplierPayments", "PendingProjectAllocation", db.SupplierPayments
            .Where(p => p.CompanyId == companyId && p.Status != "Confirmed" && p.Status != "Rejected"
                && p.Status != "Cancelled" && p.Status != "Reversed"
                && paymentAllocations.Any(a => a.SupplierPaymentId == p.SupplierPaymentId))
            .Select(p => new ProjectSettlementBlocker("SupplierPayments",
                p.Status == "Draft" || p.Status == "PendingApproval" ? "PendingSupplierPayment" : "UnknownSupplierPaymentStatus",
                "SupplierPayment", p.SupplierPaymentId, p.Status, p.CurrencyCode,
                paymentAllocations.Where(a => a.SupplierPaymentId == p.SupplierPaymentId).Sum(a => a.AllocatedAmount),
                "/api/v1/supplier-payments/" + p.SupplierPaymentId))));

        var creditAllocations = db.SupplierCreditNoteAllocations.Where(a => a.CompanyId == companyId
            && debts.Any(d => d.SupplierDebtId == a.SupplierDebtId));
        categories.Add(new("SupplierCredits", "PendingProjectAllocation", db.SupplierCreditNotes
            .Where(n => n.CompanyId == companyId && n.Status != "Approved" && n.Status != "Rejected"
                && n.Status != "Cancelled" && n.Status != "Reversed"
                && creditAllocations.Any(a => a.SupplierCreditNoteId == n.SupplierCreditNoteId))
            .Select(n => new ProjectSettlementBlocker("SupplierCredits",
                n.Status == "Draft" || n.Status == "PendingApproval" ? "PendingSupplierCredit" : "UnknownSupplierCreditStatus",
                "SupplierCreditNote", n.SupplierCreditNoteId, n.Status, n.CurrencyCode,
                creditAllocations.Where(a => a.SupplierCreditNoteId == n.SupplierCreditNoteId).Sum(a => a.AllocatedAmount),
                "/api/v1/supplier-credit-notes/" + n.SupplierCreditNoteId))));

        var claimAllocations = db.PersonalClaimPaymentAllocations.Where(a => a.CompanyId == companyId
            && claims.Any(c => c.PersonalClaimId == a.PersonalClaimId));
        categories.Add(new("ReimbursementPayments", "PendingProjectAllocation", db.PersonalClaimPayments
            .Where(p => p.CompanyId == companyId && p.Status != "Confirmed" && p.Status != "Rejected"
                && p.Status != "Cancelled" && p.Status != "Reversed"
                && claimAllocations.Any(a => a.PersonalClaimPaymentId == p.PersonalClaimPaymentId))
            .Select(p => new ProjectSettlementBlocker("ReimbursementPayments",
                p.Status == "Draft" || p.Status == "PendingApproval" ? "PendingReimbursementPayment" : "UnknownClaimPaymentStatus",
                "PersonalClaimPayment", p.PersonalClaimPaymentId, p.Status, p.CurrencyCode,
                claimAllocations.Where(a => a.PersonalClaimPaymentId == p.PersonalClaimPaymentId).Sum(a => a.AllocatedAmount), null))));

        categories.Add(new("ExpenseReturns", "PendingReturnAmount", returns
            .Where(r => r.Status != "Approved" && r.Status != "Rejected" && r.Status != "Cancelled" && r.Status != "Reversed")
            .Select(r => new ProjectSettlementBlocker("ExpenseReturns",
                r.Status == "Draft" || r.Status == "PendingApproval" ? "PendingExpenseReturn" : "UnknownExpenseReturnStatus",
                "ExpenseReturn", r.ExpenseReturnId, r.Status, r.CurrencyCode, r.ReturnAmount,
                "/api/v1/expenses/" + r.ExpenseId))));
        categories.Add(new("SupplierRefunds", "PendingRefundAmount", db.SupplierRefunds
            .Where(r => r.CompanyId == companyId && r.Status != "Confirmed" && r.Status != "Cancelled"
                && r.Status != "Reversed" && returns.Any(e => e.ExpenseReturnId == r.ExpenseReturnId))
            .Select(r => new ProjectSettlementBlocker("SupplierRefunds",
                r.Status == "PendingVerification" ? "PendingSupplierRefund" : "UnknownSupplierRefundStatus",
                "SupplierRefund", r.SupplierRefundId, r.Status, r.CurrencyCode, r.RefundAmount, null))));
        return categories;
    }
}
