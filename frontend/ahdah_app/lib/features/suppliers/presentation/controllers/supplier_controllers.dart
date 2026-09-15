import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../advances/domain/financial_operation_key.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/supplier_models.dart';
import '../../domain/supplier_repository.dart';
import '../../domain/supplier_requests.dart';

final class SupplierListState<T> {
  const SupplierListState({
    this.items = const [],
    this.loading = false,
    this.refreshing = false,
    this.loadingMore = false,
    this.hasMore = true,
    this.error,
    this.loadMoreError,
  });
  final List<T> items;
  final bool loading;
  final bool refreshing;
  final bool loadingMore;
  final bool hasMore;
  final AppException? error;
  final AppException? loadMoreError;
}

abstract base class SupplierPagedController<T>
    extends StateNotifier<SupplierListState<T>> {
  SupplierPagedController({required this.onUnauthorized})
    : super(const SupplierListState());
  final Future<void> Function() onUnauthorized;
  int _page = 0;
  int _generation = 0;
  bool _busy = false;
  String idOf(T item);
  Future<SupplierPage<T>> fetch(int page);

  Future<void> load() async {
    if (_busy) return;
    _busy = true;
    final generation = ++_generation;
    state = const SupplierListState(loading: true);
    try {
      final result = await fetch(1);
      if (generation != _generation || !mounted) return;
      _page = 1;
      state = SupplierListState(
        items: _deduplicate(result.items),
        hasMore: result.hasMore,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (generation == _generation && mounted) {
        state = SupplierListState(error: error, hasMore: false);
      }
    } finally {
      _busy = false;
    }
  }

  Future<void> refresh() async {
    if (_busy) return;
    _busy = true;
    final generation = ++_generation;
    state = SupplierListState(
      items: state.items,
      refreshing: true,
      hasMore: state.hasMore,
    );
    try {
      final result = await fetch(1);
      if (generation != _generation || !mounted) return;
      _page = 1;
      state = SupplierListState(
        items: _deduplicate(result.items),
        hasMore: result.hasMore,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (generation == _generation && mounted) {
        state = SupplierListState(
          items: state.items,
          error: error,
          hasMore: state.hasMore,
        );
      }
    } finally {
      _busy = false;
    }
  }

  Future<void> loadMore() async {
    if (_busy || !state.hasMore || state.items.isEmpty) return;
    _busy = true;
    final generation = _generation;
    state = SupplierListState(
      items: state.items,
      loadingMore: true,
      hasMore: state.hasMore,
    );
    try {
      final result = await fetch(_page + 1);
      if (generation != _generation || !mounted) return;
      _page++;
      state = SupplierListState(
        items: _deduplicate([...state.items, ...result.items]),
        hasMore: result.hasMore,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (generation == _generation && mounted) {
        state = SupplierListState(
          items: state.items,
          hasMore: state.hasMore,
          loadMoreError: error,
        );
      }
    } finally {
      _busy = false;
    }
  }

  List<T> _deduplicate(Iterable<T> values) {
    final seen = <String>{};
    return values
        .where((value) => seen.add(idOf(value)))
        .toList(growable: false);
  }
}

final supplierListControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierListController,
      SupplierListState<SupplierSummary>
    >((ref) {
      final controller = SupplierListController(
        ref.watch(supplierRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      );
      Future.microtask(controller.load);
      return controller;
    });

final class SupplierListController
    extends SupplierPagedController<SupplierSummary> {
  SupplierListController(this._repository, {required super.onUnauthorized});
  final SupplierRepository _repository;
  SupplierFilters filters = const SupplierFilters(isActive: true);
  @override
  String idOf(SupplierSummary item) => item.supplierId;
  @override
  Future<SupplierPage<SupplierSummary>> fetch(int page) =>
      _repository.listSuppliers(page: page, pageSize: 20, filters: filters);
  Future<void> applyFilters(SupplierFilters value) async {
    filters = value;
    await load();
  }
}

final supplierInvoiceListControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierInvoiceListController,
      SupplierListState<SupplierInvoiceSummary>
    >((ref) {
      final controller = SupplierInvoiceListController(
        ref.watch(supplierRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      );
      Future.microtask(controller.load);
      return controller;
    });

final class SupplierInvoiceListController
    extends SupplierPagedController<SupplierInvoiceSummary> {
  SupplierInvoiceListController(
    this._repository, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  SupplierInvoiceFilters filters = const SupplierInvoiceFilters();
  @override
  String idOf(SupplierInvoiceSummary item) => item.supplierDebtId;
  @override
  Future<SupplierPage<SupplierInvoiceSummary>> fetch(int page) =>
      _repository.listInvoices(page: page, pageSize: 20, filters: filters);
  Future<void> applyFilters(SupplierInvoiceFilters value) async {
    filters = value;
    await load();
  }
}

final supplierPaymentListControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierPaymentListController,
      SupplierListState<SupplierPaymentSummary>
    >((ref) {
      final controller = SupplierPaymentListController(
        ref.watch(supplierRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      );
      Future.microtask(controller.load);
      return controller;
    });

final class SupplierPaymentListController
    extends SupplierPagedController<SupplierPaymentSummary> {
  SupplierPaymentListController(
    this._repository, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  SupplierPaymentFilters filters = const SupplierPaymentFilters();
  @override
  String idOf(SupplierPaymentSummary item) => item.supplierPaymentId;
  @override
  Future<SupplierPage<SupplierPaymentSummary>> fetch(int page) =>
      _repository.listPayments(page: page, pageSize: 20, filters: filters);
}

final supplierCreditListControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierCreditListController,
      SupplierListState<SupplierCreditNoteSummary>
    >((ref) {
      final controller = SupplierCreditListController(
        ref.watch(supplierRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      );
      Future.microtask(controller.load);
      return controller;
    });

final class SupplierCreditListController
    extends SupplierPagedController<SupplierCreditNoteSummary> {
  SupplierCreditListController(
    this._repository, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  String? supplierId;
  String? status;
  String? currencyCode;
  @override
  String idOf(SupplierCreditNoteSummary item) => item.supplierCreditNoteId;
  @override
  Future<SupplierPage<SupplierCreditNoteSummary>> fetch(int page) =>
      _repository.listCreditNotes(
        page: page,
        pageSize: 20,
        supplierId: supplierId,
        status: status,
        currencyCode: currencyCode,
      );
}

final supplierRefundListControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierRefundListController,
      SupplierListState<SupplierRefundSummary>
    >((ref) {
      final controller = SupplierRefundListController(
        ref.watch(supplierRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      );
      Future.microtask(controller.load);
      return controller;
    });

final class SupplierRefundListController
    extends SupplierPagedController<SupplierRefundSummary> {
  SupplierRefundListController(
    this._repository, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  @override
  String idOf(SupplierRefundSummary item) => item.supplierRefundId;
  @override
  Future<SupplierPage<SupplierRefundSummary>> fetch(int page) =>
      _repository.listRefunds(page: page, pageSize: 20);
}

final supplierDetailsProvider = FutureProvider.autoDispose
    .family<SupplierDetails, String>(
      (ref, id) => ref.watch(supplierRepositoryProvider).getSupplier(id),
    );
final supplierInvoiceDetailsProvider = FutureProvider.autoDispose
    .family<SupplierInvoiceDetails, String>(
      (ref, id) => ref.watch(supplierRepositoryProvider).getInvoice(id),
    );
final supplierPaymentDetailsProvider = FutureProvider.autoDispose
    .family<SupplierPaymentDetails, String>(
      (ref, id) => ref.watch(supplierRepositoryProvider).getPayment(id),
    );
final supplierCreditDetailsProvider = FutureProvider.autoDispose
    .family<SupplierCreditNoteDetails, String>(
      (ref, id) => ref.watch(supplierRepositoryProvider).getCreditNote(id),
    );
final supplierPaymentAccountsProvider = FutureProvider.autoDispose
    .family<SupplierPage<SupplierPaymentAccount>, String>(
      (ref, id) => ref
          .watch(supplierRepositoryProvider)
          .listPaymentAccounts(id, page: 1, pageSize: 100),
    );
final supplierStatementProvider = FutureProvider.autoDispose
    .family<SupplierStatement, String>(
      (ref, id) => ref
          .watch(supplierRepositoryProvider)
          .getStatement(id, page: 1, pageSize: 100),
    );
final supplierFundingSourcesProvider = FutureProvider.autoDispose
    .family<
      SupplierPage<SupplierFundingSource>,
      ({String currency, String method})
    >(
      (ref, key) => ref
          .watch(supplierRepositoryProvider)
          .listFundingSources(
            page: 1,
            pageSize: 100,
            currencyCode: key.currency,
            paymentMethod: key.method,
          ),
    );
final activeSupplierOptionsProvider =
    FutureProvider.autoDispose<SupplierPage<SupplierSummary>>(
      (ref) => ref
          .watch(supplierRepositoryProvider)
          .listSuppliers(
            page: 1,
            pageSize: 100,
            filters: const SupplierFilters(isActive: true),
          ),
    );
final eligibleSupplierDebtsProvider = FutureProvider.autoDispose
    .family<
      SupplierPage<SupplierInvoiceSummary>,
      ({String supplierId, String currency})
    >((ref, key) async {
      final page = await ref
          .watch(supplierRepositoryProvider)
          .listInvoices(
            page: 1,
            pageSize: 100,
            filters: SupplierInvoiceFilters(
              supplierId: key.supplierId,
              currencyCode: key.currency,
              unpaidOnly: true,
            ),
          );
      return SupplierPage(
        items: page.items
            .where(
              (item) =>
                  item.expenseStatus == 'Approved' &&
                  (item.debtStatus == 'Open' ||
                      item.debtStatus == 'PartiallySettled'),
            )
            .toList(growable: false),
        page: page.page,
        pageSize: page.pageSize,
        totalCount: page.totalCount,
        totalPages: page.totalPages,
      );
    });

enum SupplierCommandPhase {
  idle,
  submitting,
  uncertainSubmission,
  success,
  failure,
}

final class SupplierCommandState<T> {
  const SupplierCommandState({
    this.phase = SupplierCommandPhase.idle,
    this.result,
    this.error,
  });
  final SupplierCommandPhase phase;
  final T? result;
  final AppException? error;
  bool get isSubmitting => phase == SupplierCommandPhase.submitting;
  bool get isUncertain => phase == SupplierCommandPhase.uncertainSubmission;
}

abstract base class SupplierFinancialCommandController<T>
    extends StateNotifier<SupplierCommandState<T>> {
  SupplierFinancialCommandController(
    FinancialOperationKeyFactory keyFactory, {
    required this.onUnauthorized,
  }) : _keySession = FinancialOperationKeySession(keyFactory),
       super(const SupplierCommandState());
  final FinancialOperationKeySession _keySession;
  final Future<void> Function() onUnauthorized;
  bool _submitting = false;
  String? _pendingFingerprint;
  Future<T> Function(String key)? _pendingOperation;

  Future<T?> execute(
    String fingerprint,
    Future<T> Function(String key) operation,
  ) async {
    if (_submitting) return null;
    _pendingFingerprint = fingerprint;
    _pendingOperation = operation;
    return _run(fingerprint, operation);
  }

  Future<T?> retrySameOperation() async {
    final fingerprint = _pendingFingerprint;
    final operation = _pendingOperation;
    if (fingerprint == null ||
        operation == null ||
        !_keySession.matches(fingerprint)) {
      return null;
    }
    return _run(fingerprint, operation);
  }

  Future<T?> _run(
    String fingerprint,
    Future<T> Function(String key) operation,
  ) async {
    if (_submitting) return null;
    _submitting = true;
    state = const SupplierCommandState(phase: SupplierCommandPhase.submitting);
    try {
      final result = await operation(_keySession.keyFor(fingerprint));
      _clear();
      if (mounted) {
        state = SupplierCommandState(
          phase: SupplierCommandPhase.success,
          result: result,
        );
      }
      return result;
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      final uncertain =
          error.kind == AppExceptionKind.network ||
          error.kind == AppExceptionKind.timeout;
      if (!uncertain) _clear();
      if (mounted) {
        state = SupplierCommandState(
          phase: uncertain
              ? SupplierCommandPhase.uncertainSubmission
              : SupplierCommandPhase.failure,
          error: error,
        );
      }
      return null;
    } finally {
      _submitting = false;
    }
  }

  void cancelPending() {
    _clear();
    if (mounted) state = const SupplierCommandState();
  }

  void _clear() {
    _keySession.clear();
    _pendingFingerprint = null;
    _pendingOperation = null;
  }

  @override
  void dispose() {
    _clear();
    super.dispose();
  }
}

final class SupplierMutationController
    extends StateNotifier<SupplierCommandState<SupplierDetails>> {
  SupplierMutationController(this._repository, {required this.onUnauthorized})
    : super(const SupplierCommandState());
  final SupplierRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _busy = false;
  Future<SupplierDetails?> create(SupplierCreateInput input) =>
      _run(() => _repository.createSupplier(input));
  Future<SupplierDetails?> update(String id, SupplierUpdateInput input) =>
      _run(() => _repository.updateSupplier(id, input));
  Future<SupplierDetails?> _run(
    Future<SupplierDetails> Function() operation,
  ) async {
    if (_busy) return null;
    _busy = true;
    state = const SupplierCommandState(phase: SupplierCommandPhase.submitting);
    try {
      final result = await operation();
      if (mounted) {
        state = SupplierCommandState(
          phase: SupplierCommandPhase.success,
          result: result,
        );
      }
      return result;
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = SupplierCommandState(
          phase: SupplierCommandPhase.failure,
          error: error,
        );
      }
      return null;
    } finally {
      _busy = false;
    }
  }
}

final supplierMutationControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierMutationController,
      SupplierCommandState<SupplierDetails>
    >(
      (ref) => SupplierMutationController(
        ref.watch(supplierRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class SupplierPaymentAccountCreateController
    extends SupplierFinancialCommandController<SupplierPaymentAccount> {
  SupplierPaymentAccountCreateController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  Future<SupplierPaymentAccount?> create(
    String supplierId,
    SupplierPaymentAccountInput input,
  ) => execute(
    'account:$supplierId:${input.payloadFingerprint}',
    (key) => _repository.createPaymentAccount(supplierId, input, key),
  );
}

final class SupplierInvoiceCreateController
    extends SupplierFinancialCommandController<SupplierInvoiceDetails> {
  SupplierInvoiceCreateController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  Future<SupplierInvoiceDetails?> create(SupplierInvoiceCreateInput input) =>
      execute(
        'invoice:${input.payloadFingerprint}',
        (key) => _repository.createInvoice(input, key),
      );
}

final class SupplierPaymentCreateController
    extends SupplierFinancialCommandController<SupplierPaymentDetails> {
  SupplierPaymentCreateController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  Future<SupplierPaymentDetails?> create(SupplierPaymentCreateInput input) =>
      execute(
        'payment:${input.payloadFingerprint}',
        (key) => _repository.createPayment(input, key),
      );
}

final class SupplierPaymentReviewController
    extends SupplierFinancialCommandController<SupplierPaymentDetails> {
  SupplierPaymentReviewController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  Future<SupplierPaymentDetails?> confirm(
    String id,
    SupplierPaymentReviewInput input,
  ) => execute(
    'payment-confirm:$id:${input.payloadFingerprint}',
    (key) => _repository.confirmPayment(id, input, key),
  );
  Future<SupplierPaymentDetails?> reject(
    String id,
    SupplierPaymentReviewInput input,
  ) => execute(
    'payment-reject:$id:${input.payloadFingerprint}',
    (key) => _repository.rejectPayment(id, input, key),
  );
}

final class SupplierCreditCommandController
    extends SupplierFinancialCommandController<SupplierCreditNoteDetails> {
  SupplierCreditCommandController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final SupplierRepository _repository;
  Future<SupplierCreditNoteDetails?> create(SupplierCreditCreateInput input) =>
      execute(
        'credit:${input.payloadFingerprint}',
        (key) => _repository.createCreditNote(input, key),
      );
  Future<SupplierCreditNoteDetails?> approve(
    String id,
    SupplierCreditApproveInput input,
  ) => execute(
    'credit-approve:$id:${input.payloadFingerprint}',
    (key) => _repository.approveCreditNote(id, input, key),
  );
  Future<SupplierCreditNoteDetails?> apply(
    String id,
    SupplierCreditApplyInput input,
  ) => execute(
    'credit-apply:$id:${input.payloadFingerprint}',
    (key) => _repository.applyCreditNote(id, input, key),
  );
}

T _financialController<T>(
  T Function(
    SupplierRepository,
    FinancialOperationKeyFactory,
    Future<void> Function(),
  )
  build,
  Ref ref,
) => build(
  ref.watch(supplierRepositoryProvider),
  ref.watch(financialOperationKeyFactoryProvider),
  ref.read(sessionControllerProvider.notifier).expire,
);

final supplierPaymentAccountCreateControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierPaymentAccountCreateController,
      SupplierCommandState<SupplierPaymentAccount>
    >(
      (ref) => _financialController(
        (repository, keys, expire) => SupplierPaymentAccountCreateController(
          repository,
          keys,
          onUnauthorized: expire,
        ),
        ref,
      ),
    );
final supplierInvoiceCreateControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierInvoiceCreateController,
      SupplierCommandState<SupplierInvoiceDetails>
    >(
      (ref) => _financialController(
        (repository, keys, expire) => SupplierInvoiceCreateController(
          repository,
          keys,
          onUnauthorized: expire,
        ),
        ref,
      ),
    );
final supplierPaymentCreateControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierPaymentCreateController,
      SupplierCommandState<SupplierPaymentDetails>
    >(
      (ref) => _financialController(
        (repository, keys, expire) => SupplierPaymentCreateController(
          repository,
          keys,
          onUnauthorized: expire,
        ),
        ref,
      ),
    );
final supplierPaymentReviewControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierPaymentReviewController,
      SupplierCommandState<SupplierPaymentDetails>
    >(
      (ref) => _financialController(
        (repository, keys, expire) => SupplierPaymentReviewController(
          repository,
          keys,
          onUnauthorized: expire,
        ),
        ref,
      ),
    );
final supplierCreditCommandControllerProvider =
    StateNotifierProvider.autoDispose<
      SupplierCreditCommandController,
      SupplierCommandState<SupplierCreditNoteDetails>
    >(
      (ref) => _financialController(
        (repository, keys, expire) => SupplierCreditCommandController(
          repository,
          keys,
          onUnauthorized: expire,
        ),
        ref,
      ),
    );
