import '../../../core/network/api_client.dart';
import '../domain/supplier_models.dart';
import '../domain/supplier_repository.dart';
import '../domain/supplier_requests.dart';

final class ApiSupplierRepository implements SupplierRepository {
  const ApiSupplierRepository(this._client);
  final ApiClient _client;

  @override
  Future<SupplierPage<SupplierSummary>> listSuppliers({
    required int page,
    required int pageSize,
    required SupplierFilters filters,
  }) => _client.listSuppliers(page: page, pageSize: pageSize, filters: filters);
  @override
  Future<SupplierDetails> getSupplier(String supplierId) =>
      _client.getSupplier(supplierId);
  @override
  Future<SupplierDetails> createSupplier(SupplierCreateInput input) =>
      _client.createSupplier(input);
  @override
  Future<SupplierDetails> updateSupplier(
    String supplierId,
    SupplierUpdateInput input,
  ) => _client.updateSupplier(supplierId, input);
  @override
  Future<SupplierPage<SupplierPaymentAccount>> listPaymentAccounts(
    String supplierId, {
    required int page,
    required int pageSize,
  }) => _client.listSupplierPaymentAccounts(
    supplierId,
    page: page,
    pageSize: pageSize,
  );
  @override
  Future<SupplierPaymentAccount> createPaymentAccount(
    String supplierId,
    SupplierPaymentAccountInput input,
    String idempotencyKey,
  ) => _client.createSupplierPaymentAccount(supplierId, input, idempotencyKey);
  @override
  Future<SupplierPage<SupplierInvoiceSummary>> listInvoices({
    required int page,
    required int pageSize,
    required SupplierInvoiceFilters filters,
  }) => _client.listSupplierInvoices(
    page: page,
    pageSize: pageSize,
    filters: filters,
  );
  @override
  Future<SupplierInvoiceDetails> getInvoice(String debtId) =>
      _client.getSupplierInvoice(debtId);
  @override
  Future<SupplierInvoiceDetails> createInvoice(
    SupplierInvoiceCreateInput input,
    String idempotencyKey,
  ) => _client.createSupplierInvoice(input, idempotencyKey);
  @override
  Future<SupplierPage<SupplierPaymentSummary>> listPayments({
    required int page,
    required int pageSize,
    required SupplierPaymentFilters filters,
  }) => _client.listSupplierPayments(
    page: page,
    pageSize: pageSize,
    filters: filters,
  );
  @override
  Future<SupplierPaymentDetails> getPayment(String paymentId) =>
      _client.getSupplierPayment(paymentId);
  @override
  Future<SupplierPage<SupplierFundingSource>> listFundingSources({
    required int page,
    required int pageSize,
    String? currencyCode,
    String? paymentMethod,
  }) => _client.listSupplierFundingSources(
    page: page,
    pageSize: pageSize,
    currencyCode: currencyCode,
    paymentMethod: paymentMethod,
  );
  @override
  Future<SupplierPaymentDetails> createPayment(
    SupplierPaymentCreateInput input,
    String idempotencyKey,
  ) => _client.createSupplierPayment(input, idempotencyKey);
  @override
  Future<SupplierPaymentDetails> confirmPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  ) => _client.confirmSupplierPayment(paymentId, input, idempotencyKey);
  @override
  Future<SupplierPaymentDetails> rejectPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  ) => _client.rejectSupplierPayment(paymentId, input, idempotencyKey);
  @override
  Future<SupplierPage<SupplierCreditNoteSummary>> listCreditNotes({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  }) => _client.listSupplierCreditNotes(
    page: page,
    pageSize: pageSize,
    supplierId: supplierId,
    status: status,
    currencyCode: currencyCode,
  );
  @override
  Future<SupplierCreditNoteDetails> getCreditNote(String creditNoteId) =>
      _client.getSupplierCreditNote(creditNoteId);
  @override
  Future<SupplierCreditNoteDetails> createCreditNote(
    SupplierCreditCreateInput input,
    String idempotencyKey,
  ) => _client.createSupplierCreditNote(input, idempotencyKey);
  @override
  Future<SupplierCreditNoteDetails> approveCreditNote(
    String creditNoteId,
    SupplierCreditApproveInput input,
    String idempotencyKey,
  ) => _client.approveSupplierCreditNote(creditNoteId, input, idempotencyKey);
  @override
  Future<SupplierCreditNoteDetails> applyCreditNote(
    String creditNoteId,
    SupplierCreditApplyInput input,
    String idempotencyKey,
  ) => _client.applySupplierCreditNote(creditNoteId, input, idempotencyKey);
  @override
  Future<SupplierPage<SupplierRefundSummary>> listRefunds({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  }) => _client.listSupplierRefunds(
    page: page,
    pageSize: pageSize,
    supplierId: supplierId,
    status: status,
    currencyCode: currencyCode,
  );
  @override
  Future<SupplierStatement> getStatement(
    String supplierId, {
    required int page,
    required int pageSize,
    String? currencyCode,
  }) => _client.getSupplierStatement(
    supplierId,
    page: page,
    pageSize: pageSize,
    currencyCode: currencyCode,
  );
}
