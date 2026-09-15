using Ahdah.Application.Access.Models;
using Ahdah.Application.Suppliers.Contracts;
using Ahdah.Application.Suppliers.Models;

namespace Ahdah.Application.Suppliers.Services;

public interface ISupplierService
{
    Task<AccessResult<PagedResult<SupplierSummary>>> ListSuppliersAsync(SupplierQuery query, CancellationToken cancellationToken);
    Task<AccessResult<SupplierDetails>> GetSupplierAsync(Guid supplierId, CancellationToken cancellationToken);
    Task<AccessResult<SupplierDetails>> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken);
    Task<AccessResult<SupplierDetails>> UpdateSupplierAsync(Guid supplierId, UpdateSupplierRequest request, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<SupplierPaymentAccountSummary>>> ListPaymentAccountsAsync(Guid supplierId, SupplierSubresourceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<SupplierPaymentAccountSummary>> CreatePaymentAccountAsync(Guid supplierId, CreateSupplierPaymentAccountRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<SupplierInvoiceSummary>>> ListInvoicesAsync(SupplierInvoiceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<SupplierInvoiceDetails>> GetInvoiceAsync(Guid debtId, CancellationToken cancellationToken);
    Task<AccessResult<SupplierInvoiceDetails>> CreateInvoiceAsync(CreateSupplierInvoiceRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<SupplierPaymentSummary>>> ListPaymentsAsync(SupplierPaymentQuery query, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<SupplierFundingSourceSummary>>> ListFundingSourcesAsync(SupplierFundingSourceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<SupplierPaymentDetails>> GetPaymentAsync(Guid paymentId, CancellationToken cancellationToken);
    Task<AccessResult<SupplierPaymentDetails>> CreatePaymentAsync(CreateSupplierPaymentRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<SupplierPaymentDetails>> ConfirmPaymentAsync(Guid paymentId, ReviewSupplierPaymentRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<SupplierPaymentDetails>> RejectPaymentAsync(Guid paymentId, ReviewSupplierPaymentRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<SupplierRefundSummary>>> ListRefundsAsync(SupplierRefundQuery query, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<SupplierCreditNoteSummary>>> ListCreditNotesAsync(SupplierCreditNoteQuery query, CancellationToken cancellationToken);
    Task<AccessResult<SupplierCreditNoteDetails>> GetCreditNoteAsync(Guid creditNoteId, CancellationToken cancellationToken);
    Task<AccessResult<SupplierCreditNoteDetails>> CreateCreditNoteAsync(CreateSupplierCreditNoteRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<SupplierCreditNoteDetails>> ApproveCreditNoteAsync(Guid creditNoteId, ApproveSupplierCreditNoteRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<SupplierCreditNoteDetails>> ApplyCreditNoteAsync(Guid creditNoteId, ApplySupplierCreditNoteRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<SupplierStatement>> GetStatementAsync(Guid supplierId, SupplierStatementQuery query, CancellationToken cancellationToken);
}
