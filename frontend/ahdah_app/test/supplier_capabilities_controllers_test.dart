import 'dart:async';

import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/access/domain/role_capabilities.dart';
import 'package:ahdah_app/features/advances/domain/financial_operation_key.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_models.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_repository.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_requests.dart';
import 'package:ahdah_app/features/suppliers/presentation/controllers/supplier_controllers.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('supplier role capabilities match backend policies conservatively', () {
    final manager = RoleCapabilities.forRole('Manager');
    expect(manager.canViewSuppliers, isTrue);
    expect(manager.canManageSuppliers, isTrue);
    expect(manager.canRecordSupplierPayment, isTrue);
    expect(manager.canManageSupplierCredits, isTrue);

    final deputy = RoleCapabilities.forRole('Deputy');
    expect(deputy.canViewSupplierFinancials, isTrue);
    expect(deputy.canCreateSupplierInvoice, isTrue);
    expect(deputy.canRecordSupplierPayment, isFalse);

    final accountant = RoleCapabilities.forRole('Accountant');
    expect(accountant.canManageSuppliers, isFalse);
    expect(accountant.canCreateSupplierPaymentAccount, isTrue);
    expect(accountant.canManageSupplierCredits, isTrue);

    final supervisor = RoleCapabilities.forRole('Supervisor');
    expect(supervisor.canViewSuppliers, isTrue);
    expect(supervisor.requiresAssignedSupplierProjects, isTrue);
    expect(supervisor.canViewSupplierFinancials, isFalse);
    expect(RoleCapabilities.forRole('Worker').canViewSuppliers, isFalse);
    expect(RoleCapabilities.forRole('FutureRole').canViewSuppliers, isFalse);
  });

  test('supplier list filters, paginates, and deduplicates stably', () async {
    final repository = _FakeSupplierRepository()
      ..supplierPage = SupplierPage(
        items: [_supplier],
        page: 1,
        pageSize: 20,
        totalCount: 2,
        totalPages: 2,
      );
    final controller = SupplierListController(
      repository,
      onUnauthorized: () async {},
    );
    await controller.load();
    await controller.applyFilters(
      const SupplierFilters(isActive: false, search: 'Su'),
    );
    expect(repository.lastSupplierFilters.isActive, isFalse);
    repository.supplierPage = SupplierPage(
      items: [_supplier],
      page: 2,
      pageSize: 20,
      totalCount: 2,
      totalPages: 2,
    );
    await controller.loadMore();
    expect(controller.state.items, hasLength(1));
  });

  test('load-more error preserves already loaded supplier records', () async {
    final repository = _FakeSupplierRepository()
      ..supplierPage = SupplierPage(
        items: [_supplier],
        page: 1,
        pageSize: 20,
        totalCount: 2,
        totalPages: 2,
      );
    final controller = SupplierListController(
      repository,
      onUnauthorized: () async {},
    );
    await controller.load();
    repository.readError = const AppException(AppExceptionKind.network);
    await controller.loadMore();
    expect(controller.state.items.single.supplierId, 'supplier-1');
    expect(controller.state.loadMoreError?.kind, AppExceptionKind.network);
  });

  test('duplicate payment submission is suppressed', () async {
    final repository = _FakeSupplierRepository();
    final completer = Completer<SupplierPaymentDetails>();
    repository.paymentCompleter = completer;
    final controller = SupplierPaymentCreateController(
      repository,
      _SequenceKeys(),
      onUnauthorized: () async {},
    );
    final first = controller.create(_paymentInput('100.00'));
    expect(await controller.create(_paymentInput('100.00')), isNull);
    expect(repository.paymentCreateCalls, 1);
    completer.complete(_paymentDetails);
    expect(await first, _paymentDetails);
  });

  test('uncertain payment retry reuses key and success clears it', () async {
    final repository = _FakeSupplierRepository()
      ..writeError = const AppException(AppExceptionKind.timeout);
    final controller = SupplierPaymentCreateController(
      repository,
      _SequenceKeys(),
      onUnauthorized: () async {},
    );
    await controller.create(_paymentInput('100.00'));
    final firstKey = repository.lastKey;
    expect(controller.state.phase, SupplierCommandPhase.uncertainSubmission);
    repository.writeError = null;
    expect(await controller.retrySameOperation(), _paymentDetails);
    expect(repository.lastKey, firstKey);
    expect(controller.state.phase, SupplierCommandPhase.success);
  });

  test('changed payment payload invalidates uncertain key', () async {
    final repository = _FakeSupplierRepository()
      ..writeError = const AppException(AppExceptionKind.network);
    final controller = SupplierPaymentCreateController(
      repository,
      _SequenceKeys(),
      onUnauthorized: () async {},
    );
    await controller.create(_paymentInput('100.00'));
    final firstKey = repository.lastKey;
    repository.writeError = null;
    await controller.create(_paymentInput('100.01'));
    expect(repository.lastKey, isNot(firstKey));
  });

  test(
    'payment review and credit commands preserve expected version payloads',
    () async {
      final repository = _FakeSupplierRepository();
      final keys = _SequenceKeys();
      final review = SupplierPaymentReviewController(
        repository,
        keys,
        onUnauthorized: () async {},
      );
      await review.confirm(
        'payment-1',
        const SupplierPaymentReviewInput(expectedVersion: 4),
      );
      expect(repository.lastPaymentReview?.expectedVersion, 4);
      await review.reject(
        'payment-1',
        const SupplierPaymentReviewInput(
          expectedVersion: 5,
          rejectionReason: 'Duplicate',
        ),
      );
      expect(repository.lastPaymentReview?.rejectionReason, 'Duplicate');

      final credit = SupplierCreditCommandController(
        repository,
        keys,
        onUnauthorized: () async {},
      );
      await credit.apply(
        'credit-1',
        const SupplierCreditApplyInput(
          expectedVersion: 3,
          allocations: [
            SupplierCreditAllocationInput(
              supplierDebtId: 'debt-1',
              amount: '10.00',
            ),
          ],
        ),
      );
      expect(repository.lastCreditApply?.expectedVersion, 3);
    },
  );
}

final _supplier = SupplierSummary.fromJson(const {
  'supplierId': 'supplier-1',
  'supplierName': 'Supplier One',
  'supplierType': 'GeneralSupplier',
  'defaultCurrencyCode': 'LYD',
  'transactionMode': 'CashAndCredit',
  'defaultPaymentTermsDays': 30,
  'isActive': true,
  'versionNumber': 1,
});
final _supplierDetails = SupplierDetails(
  supplier: _supplier,
  balances: const [],
);
final _invoice = SupplierInvoiceSummary.fromJson({
  'supplierDebtId': 'debt-1',
  'debtNumber': 'SDEBT-1',
  'expenseId': 'expense-1',
  'expenseNumber': 'EXP-1',
  'supplier': {
    'supplierId': 'supplier-1',
    'supplierName': 'Supplier One',
    'supplierType': 'GeneralSupplier',
    'defaultCurrencyCode': 'LYD',
    'transactionMode': 'CashAndCredit',
    'defaultPaymentTermsDays': 30,
    'isActive': true,
    'versionNumber': 1,
  },
  'amount': 100,
  'paidAmount': 0,
  'creditNoteAmount': 0,
  'writtenOffAmount': 0,
  'outstandingAmount': 100,
  'currencyCode': 'LYD',
  'expenseStatus': 'Approved',
  'debtStatus': 'Open',
  'description': 'Materials',
  'expenseVersionNumber': 1,
  'debtVersionNumber': 1,
});
final _invoiceDetails = SupplierInvoiceDetails(
  invoice: _invoice,
  adjustmentAmount: '0.00',
  items: const [],
);
final _paymentSummary = SupplierPaymentSummary.fromJson(const {
  'supplierPaymentId': 'payment-1',
  'paymentNumber': 'SPAY-1',
  'supplierId': 'supplier-1',
  'supplierName': 'Supplier One',
  'paymentAmount': 100,
  'currencyCode': 'LYD',
  'paymentMethod': 'Cash',
  'hasProof': false,
  'status': 'PendingApproval',
  'createdBy': {'userId': 'user-1', 'fullName': 'Manager', 'role': 'Manager'},
  'versionNumber': 1,
});
final _paymentDetails = SupplierPaymentDetails(
  payment: _paymentSummary,
  debtAllocations: const [],
  fundingSources: const [],
);
final _creditSummary = SupplierCreditNoteSummary.fromJson(const {
  'supplierCreditNoteId': 'credit-1',
  'creditNoteNumber': 'SCN-1',
  'supplierId': 'supplier-1',
  'supplierName': 'Supplier One',
  'creditNoteAmount': 10,
  'appliedAmount': 0,
  'availableAmount': 10,
  'currencyCode': 'LYD',
  'reasonType': 'Other',
  'description': 'Credit',
  'status': 'Approved',
  'versionNumber': 1,
});
final _creditDetails = SupplierCreditNoteDetails(
  creditNote: _creditSummary,
  allocations: const [],
);

SupplierPaymentCreateInput _paymentInput(String amount) =>
    SupplierPaymentCreateInput(
      supplierId: 'supplier-1',
      paymentDate: DateTime(2026, 8, 4),
      paymentAmount: amount,
      currencyCode: 'LYD',
      paymentMethod: 'Cash',
      debtAllocations: [
        SupplierPaymentDebtAllocationInput(
          supplierDebtId: 'debt-1',
          amount: amount,
        ),
      ],
      fundingAllocations: [
        SupplierPaymentFundingAllocationInput(
          fundingSourceId: 'source-1',
          amount: amount,
        ),
      ],
    );

final class _SequenceKeys implements FinancialOperationKeyFactory {
  int count = 0;
  @override
  String create() => 'supplier-test-key-${++count}-secure';
}

final class _FakeSupplierRepository implements SupplierRepository {
  SupplierPage<SupplierSummary> supplierPage = const SupplierPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  AppException? readError;
  AppException? writeError;
  Completer<SupplierPaymentDetails>? paymentCompleter;
  int paymentCreateCalls = 0;
  String? lastKey;
  SupplierFilters lastSupplierFilters = const SupplierFilters();
  SupplierPaymentReviewInput? lastPaymentReview;
  SupplierCreditApplyInput? lastCreditApply;

  @override
  Future<SupplierPage<SupplierSummary>> listSuppliers({
    required int page,
    required int pageSize,
    required SupplierFilters filters,
  }) async {
    lastSupplierFilters = filters;
    if (readError != null) throw readError!;
    return supplierPage;
  }

  @override
  Future<SupplierPaymentDetails> createPayment(
    SupplierPaymentCreateInput input,
    String idempotencyKey,
  ) async {
    paymentCreateCalls++;
    lastKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return paymentCompleter?.future ?? _paymentDetails;
  }

  @override
  Future<SupplierPaymentDetails> confirmPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  ) async {
    lastKey = idempotencyKey;
    lastPaymentReview = input;
    return _paymentDetails;
  }

  @override
  Future<SupplierPaymentDetails> rejectPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  ) async {
    lastKey = idempotencyKey;
    lastPaymentReview = input;
    return _paymentDetails;
  }

  @override
  Future<SupplierCreditNoteDetails> applyCreditNote(
    String creditNoteId,
    SupplierCreditApplyInput input,
    String idempotencyKey,
  ) async {
    lastKey = idempotencyKey;
    lastCreditApply = input;
    return _creditDetails;
  }

  @override
  Future<SupplierDetails> createSupplier(SupplierCreateInput input) async =>
      _supplierDetails;
  @override
  Future<SupplierDetails> updateSupplier(
    String supplierId,
    SupplierUpdateInput input,
  ) async => _supplierDetails;
  @override
  Future<SupplierDetails> getSupplier(String supplierId) async =>
      _supplierDetails;
  @override
  Future<SupplierPage<SupplierInvoiceSummary>> listInvoices({
    required int page,
    required int pageSize,
    required SupplierInvoiceFilters filters,
  }) async => SupplierPage(
    items: [_invoice],
    page: page,
    pageSize: pageSize,
    totalCount: 1,
    totalPages: 1,
  );
  @override
  Future<SupplierInvoiceDetails> getInvoice(String debtId) async =>
      _invoiceDetails;
  @override
  Future<SupplierInvoiceDetails> createInvoice(
    SupplierInvoiceCreateInput input,
    String idempotencyKey,
  ) async => _invoiceDetails;
  @override
  Future<SupplierPage<SupplierPaymentSummary>> listPayments({
    required int page,
    required int pageSize,
    required SupplierPaymentFilters filters,
  }) async => SupplierPage(
    items: [_paymentSummary],
    page: page,
    pageSize: pageSize,
    totalCount: 1,
    totalPages: 1,
  );
  @override
  Future<SupplierPaymentDetails> getPayment(String paymentId) async =>
      _paymentDetails;
  @override
  Future<SupplierPage<SupplierFundingSource>> listFundingSources({
    required int page,
    required int pageSize,
    String? currencyCode,
    String? paymentMethod,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierPage<SupplierPaymentAccount>> listPaymentAccounts(
    String supplierId, {
    required int page,
    required int pageSize,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierPaymentAccount> createPaymentAccount(
    String supplierId,
    SupplierPaymentAccountInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierPage<SupplierCreditNoteSummary>> listCreditNotes({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  }) async => SupplierPage(
    items: [_creditSummary],
    page: page,
    pageSize: pageSize,
    totalCount: 1,
    totalPages: 1,
  );
  @override
  Future<SupplierCreditNoteDetails> getCreditNote(String creditNoteId) async =>
      _creditDetails;
  @override
  Future<SupplierCreditNoteDetails> createCreditNote(
    SupplierCreditCreateInput input,
    String idempotencyKey,
  ) async => _creditDetails;
  @override
  Future<SupplierCreditNoteDetails> approveCreditNote(
    String creditNoteId,
    SupplierCreditApproveInput input,
    String idempotencyKey,
  ) async => _creditDetails;
  @override
  Future<SupplierPage<SupplierRefundSummary>> listRefunds({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierStatement> getStatement(
    String supplierId, {
    required int page,
    required int pageSize,
    String? currencyCode,
  }) async => SupplierStatement(
    supplier: _supplier,
    balances: const [],
    entries: SupplierPage(
      items: const [],
      page: page,
      pageSize: pageSize,
      totalCount: 0,
      totalPages: 0,
    ),
  );
}
