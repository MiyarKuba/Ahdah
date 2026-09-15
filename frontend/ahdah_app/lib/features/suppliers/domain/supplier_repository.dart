import 'supplier_models.dart';
import 'supplier_requests.dart';

abstract interface class SupplierRepository {
  Future<SupplierPage<SupplierSummary>> listSuppliers({
    required int page,
    required int pageSize,
    required SupplierFilters filters,
  });
  Future<SupplierDetails> getSupplier(String supplierId);
  Future<SupplierDetails> createSupplier(SupplierCreateInput input);
  Future<SupplierDetails> updateSupplier(
    String supplierId,
    SupplierUpdateInput input,
  );
  Future<SupplierPage<SupplierPaymentAccount>> listPaymentAccounts(
    String supplierId, {
    required int page,
    required int pageSize,
  });
  Future<SupplierPaymentAccount> createPaymentAccount(
    String supplierId,
    SupplierPaymentAccountInput input,
    String idempotencyKey,
  );
  Future<SupplierPage<SupplierInvoiceSummary>> listInvoices({
    required int page,
    required int pageSize,
    required SupplierInvoiceFilters filters,
  });
  Future<SupplierInvoiceDetails> getInvoice(String debtId);
  Future<SupplierInvoiceDetails> createInvoice(
    SupplierInvoiceCreateInput input,
    String idempotencyKey,
  );
  Future<SupplierPage<SupplierPaymentSummary>> listPayments({
    required int page,
    required int pageSize,
    required SupplierPaymentFilters filters,
  });
  Future<SupplierPaymentDetails> getPayment(String paymentId);
  Future<SupplierPage<SupplierFundingSource>> listFundingSources({
    required int page,
    required int pageSize,
    String? currencyCode,
    String? paymentMethod,
  });
  Future<SupplierPaymentDetails> createPayment(
    SupplierPaymentCreateInput input,
    String idempotencyKey,
  );
  Future<SupplierPaymentDetails> confirmPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  );
  Future<SupplierPaymentDetails> rejectPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  );
  Future<SupplierPage<SupplierCreditNoteSummary>> listCreditNotes({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  });
  Future<SupplierCreditNoteDetails> getCreditNote(String creditNoteId);
  Future<SupplierCreditNoteDetails> createCreditNote(
    SupplierCreditCreateInput input,
    String idempotencyKey,
  );
  Future<SupplierCreditNoteDetails> approveCreditNote(
    String creditNoteId,
    SupplierCreditApproveInput input,
    String idempotencyKey,
  );
  Future<SupplierCreditNoteDetails> applyCreditNote(
    String creditNoteId,
    SupplierCreditApplyInput input,
    String idempotencyKey,
  );
  Future<SupplierPage<SupplierRefundSummary>> listRefunds({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  });
  Future<SupplierStatement> getStatement(
    String supplierId, {
    required int page,
    required int pageSize,
    String? currencyCode,
  });
}
