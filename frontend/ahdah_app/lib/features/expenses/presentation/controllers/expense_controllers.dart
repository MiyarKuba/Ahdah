import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../advances/domain/financial_operation_key.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/expense_filters.dart';
import '../../domain/expense_models.dart';
import '../../domain/expense_repository.dart';
import '../../domain/expense_requests.dart';

const expensePageSize = 20;

enum ExpenseReadPhase {
  initial,
  loading,
  data,
  empty,
  refreshing,
  loadingMore,
  failure,
}

final class ExpenseListState {
  const ExpenseListState({
    this.phase = ExpenseReadPhase.initial,
    this.items = const [],
    this.filters = const ExpenseFilters(),
    this.reference = '',
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final ExpenseReadPhase phase;
  final List<ExpenseSummary> items;
  final ExpenseFilters filters;
  final String reference;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;
  bool get hasMore => page < totalPages;

  ExpenseListState copyWith({
    ExpenseReadPhase? phase,
    List<ExpenseSummary>? items,
    ExpenseFilters? filters,
    String? reference,
    int? page,
    int? totalPages,
    AppException? error,
    bool clearError = false,
    AppException? loadMoreError,
    bool clearLoadMoreError = false,
  }) => ExpenseListState(
    phase: phase ?? this.phase,
    items: items ?? this.items,
    filters: filters ?? this.filters,
    reference: reference ?? this.reference,
    page: page ?? this.page,
    totalPages: totalPages ?? this.totalPages,
    error: clearError ? null : error ?? this.error,
    loadMoreError: clearLoadMoreError
        ? null
        : loadMoreError ?? this.loadMoreError,
  );
}

final expenseListControllerProvider =
    StateNotifierProvider<ExpenseListController, ExpenseListState>(
      (ref) => ExpenseListController(
        ref.watch(expenseRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class ExpenseListController extends StateNotifier<ExpenseListState> {
  ExpenseListController(this._repository, {required this.onUnauthorized})
    : super(const ExpenseListState());

  final ExpenseRepository _repository;
  final Future<void> Function() onUnauthorized;
  int _generation = 0;
  bool _loadingMore = false;
  Timer? _searchTimer;

  Future<void> load() => _replace(ExpenseReadPhase.loading);
  Future<void> refresh() => _replace(ExpenseReadPhase.refreshing);

  Future<void> setFilters(ExpenseFilters filters) async {
    state = state.copyWith(filters: filters, page: 0, totalPages: 0);
    await load();
  }

  void setReference(
    String value, {
    Duration debounce = const Duration(milliseconds: 350),
  }) {
    final bounded = value.length > 50 ? value.substring(0, 50) : value;
    if (bounded == state.reference) return;
    _searchTimer?.cancel();
    state = state.copyWith(reference: bounded, page: 0, totalPages: 0);
    if (bounded.trim().length == 1) {
      _generation++;
      state = state.copyWith(phase: ExpenseReadPhase.empty, items: const []);
      return;
    }
    _searchTimer = Timer(debounce, load);
  }

  ExpenseFilters _requestFilters() => ExpenseFilters(
    status: state.filters.status,
    categoryId: state.filters.categoryId,
    projectId: state.filters.projectId,
    incurredByUserId: state.filters.incurredByUserId,
    submittedByUserId: state.filters.submittedByUserId,
    paymentMode: state.filters.paymentMode,
    reference: state.reference.trim().length >= 2
        ? state.reference.trim()
        : null,
  );

  Future<void> _replace(ExpenseReadPhase phase) async {
    final generation = ++_generation;
    state = state.copyWith(
      phase: phase,
      clearError: true,
      clearLoadMoreError: true,
    );
    try {
      final page = await _repository.listExpenses(
        page: 1,
        pageSize: expensePageSize,
        filters: _requestFilters(),
      );
      if (!mounted || generation != _generation) return;
      state = state.copyWith(
        phase: page.items.isEmpty
            ? ExpenseReadPhase.empty
            : ExpenseReadPhase.data,
        items: page.items,
        page: page.page,
        totalPages: page.totalPages,
        clearError: true,
      );
    } on AppException catch (error) {
      await _expire(error);
      if (mounted && generation == _generation) {
        state = state.copyWith(phase: ExpenseReadPhase.failure, error: error);
      }
    }
  }

  Future<void> loadMore() async {
    if (_loadingMore || !state.hasMore || state.items.isEmpty) return;
    _loadingMore = true;
    final generation = _generation;
    state = state.copyWith(
      phase: ExpenseReadPhase.loadingMore,
      clearLoadMoreError: true,
    );
    try {
      final page = await _repository.listExpenses(
        page: state.page + 1,
        pageSize: expensePageSize,
        filters: _requestFilters(),
      );
      if (!mounted || generation != _generation) return;
      final byId = {for (final item in state.items) item.expenseId: item};
      for (final item in page.items) {
        byId[item.expenseId] = item;
      }
      state = state.copyWith(
        phase: ExpenseReadPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      await _expire(error);
      if (mounted && generation == _generation) {
        state = state.copyWith(
          phase: ExpenseReadPhase.data,
          loadMoreError: error,
        );
      }
    } finally {
      _loadingMore = false;
    }
  }

  Future<void> _expire(AppException error) async {
    if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
  }

  @override
  void dispose() {
    _searchTimer?.cancel();
    super.dispose();
  }
}

enum ExpenseDetailsPhase { initial, loading, data, failure }

final class ExpenseDetailsState {
  const ExpenseDetailsState({
    this.phase = ExpenseDetailsPhase.initial,
    this.details,
    this.error,
  });
  final ExpenseDetailsPhase phase;
  final ExpenseDetails? details;
  final AppException? error;
}

final expenseDetailsControllerProvider = StateNotifierProvider.autoDispose
    .family<ExpenseDetailsController, ExpenseDetailsState, String>(
      (ref, expenseId) => ExpenseDetailsController(
        ref.watch(expenseRepositoryProvider),
        expenseId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class ExpenseDetailsController
    extends StateNotifier<ExpenseDetailsState> {
  ExpenseDetailsController(
    this._repository,
    this.expenseId, {
    required this.onUnauthorized,
  }) : super(const ExpenseDetailsState());

  final ExpenseRepository _repository;
  final String expenseId;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = ExpenseDetailsState(
      phase: ExpenseDetailsPhase.loading,
      details: state.details,
    );
    try {
      final details = await _repository.getExpense(expenseId);
      if (mounted) {
        state = ExpenseDetailsState(
          phase: ExpenseDetailsPhase.data,
          details: details,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = ExpenseDetailsState(
          phase: ExpenseDetailsPhase.failure,
          details: state.details,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }
}

final class ExpenseCategoryState {
  const ExpenseCategoryState({
    this.phase = ExpenseReadPhase.initial,
    this.items = const [],
    this.group,
    this.scope,
    this.error,
  });
  final ExpenseReadPhase phase;
  final List<ExpenseCategory> items;
  final String? group;
  final String? scope;
  final AppException? error;
}

final expenseCategoryControllerProvider =
    StateNotifierProvider<ExpenseCategoryController, ExpenseCategoryState>(
      (ref) => ExpenseCategoryController(
        ref.watch(expenseRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class ExpenseCategoryController
    extends StateNotifier<ExpenseCategoryState> {
  ExpenseCategoryController(this._repository, {required this.onUnauthorized})
    : super(const ExpenseCategoryState());
  final ExpenseRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> setGroup(String? group) {
    state = ExpenseCategoryState(
      phase: state.phase,
      items: state.items,
      group: expenseCategoryGroups.contains(group) ? group : null,
      scope: state.scope,
    );
    return load();
  }

  Future<void> setScope(String? scope) {
    state = ExpenseCategoryState(
      phase: state.phase,
      items: state.items,
      group: state.group,
      scope: expenseCategoryScopes.contains(scope) ? scope : null,
    );
    return load();
  }

  Future<void> load({String? group, String? scope}) async {
    if (_loading) return;
    _loading = true;
    final nextGroup = group ?? state.group;
    final nextScope = scope ?? state.scope;
    state = ExpenseCategoryState(
      phase: ExpenseReadPhase.loading,
      items: state.items,
      group: nextGroup,
      scope: nextScope,
    );
    try {
      final page = await _repository.listCategories(
        page: 1,
        pageSize: 100,
        categoryGroup: nextGroup,
        expenseScope: nextScope,
      );
      if (mounted) {
        state = ExpenseCategoryState(
          phase: page.items.isEmpty
              ? ExpenseReadPhase.empty
              : ExpenseReadPhase.data,
          items: page.items,
          group: nextGroup,
          scope: nextScope,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = ExpenseCategoryState(
          phase: ExpenseReadPhase.failure,
          items: state.items,
          group: nextGroup,
          scope: nextScope,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }
}

final class PagedExpenseState<T> {
  const PagedExpenseState({
    this.phase = ExpenseReadPhase.initial,
    this.items = const [],
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });
  final ExpenseReadPhase phase;
  final List<T> items;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;
  bool get hasMore => page < totalPages;
}

final expenseDocumentControllerProvider = StateNotifierProvider.autoDispose
    .family<
      ExpenseDocumentController,
      PagedExpenseState<ExpenseDocumentMetadata>,
      String
    >(
      (ref, expenseId) => ExpenseDocumentController(
        ref.watch(expenseRepositoryProvider),
        expenseId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final expenseHistoryControllerProvider = StateNotifierProvider.autoDispose
    .family<
      ExpenseHistoryController,
      PagedExpenseState<ExpenseHistoryEvent>,
      String
    >(
      (ref, expenseId) => ExpenseHistoryController(
        ref.watch(expenseRepositoryProvider),
        expenseId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

abstract base class _PagedExpenseController<T>
    extends StateNotifier<PagedExpenseState<T>> {
  _PagedExpenseController({required this.onUnauthorized})
    : super(const PagedExpenseState());
  final Future<void> Function() onUnauthorized;
  bool _loading = false;
  Future<({List<T> items, int page, int totalPages})> request(int page);
  String idOf(T item);

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = PagedExpenseState(
      phase: ExpenseReadPhase.loading,
      items: state.items,
    );
    try {
      final result = await request(1);
      if (mounted) {
        state = PagedExpenseState(
          phase: result.items.isEmpty
              ? ExpenseReadPhase.empty
              : ExpenseReadPhase.data,
          items: result.items,
          page: result.page,
          totalPages: result.totalPages,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = PagedExpenseState(
          phase: ExpenseReadPhase.failure,
          items: state.items,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }

  Future<void> loadMore() async {
    if (_loading || !state.hasMore || state.items.isEmpty) return;
    _loading = true;
    state = PagedExpenseState(
      phase: ExpenseReadPhase.loadingMore,
      items: state.items,
      page: state.page,
      totalPages: state.totalPages,
    );
    try {
      final result = await request(state.page + 1);
      if (!mounted) return;
      final byId = {for (final item in state.items) idOf(item): item};
      for (final item in result.items) {
        byId[idOf(item)] = item;
      }
      state = PagedExpenseState(
        phase: ExpenseReadPhase.data,
        items: byId.values.toList(growable: false),
        page: result.page,
        totalPages: result.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = PagedExpenseState(
          phase: ExpenseReadPhase.data,
          items: state.items,
          page: state.page,
          totalPages: state.totalPages,
          loadMoreError: error,
        );
      }
    } finally {
      _loading = false;
    }
  }
}

final class ExpenseDocumentController
    extends _PagedExpenseController<ExpenseDocumentMetadata> {
  ExpenseDocumentController(
    this._repository,
    this.expenseId, {
    required super.onUnauthorized,
  });
  final ExpenseRepository _repository;
  final String expenseId;

  @override
  String idOf(ExpenseDocumentMetadata item) => item.expenseDocumentId;

  @override
  Future<({List<ExpenseDocumentMetadata> items, int page, int totalPages})>
  request(int page) async {
    final result = await _repository.listDocuments(
      expenseId,
      page: page,
      pageSize: expensePageSize,
    );
    return (
      items: result.items,
      page: result.page,
      totalPages: result.totalPages,
    );
  }
}

final class ExpenseHistoryController
    extends _PagedExpenseController<ExpenseHistoryEvent> {
  ExpenseHistoryController(
    this._repository,
    this.expenseId, {
    required super.onUnauthorized,
  });
  final ExpenseRepository _repository;
  final String expenseId;

  @override
  String idOf(ExpenseHistoryEvent item) => item.eventId;

  @override
  Future<({List<ExpenseHistoryEvent> items, int page, int totalPages})> request(
    int page,
  ) async {
    final result = await _repository.listHistory(
      expenseId,
      page: page,
      pageSize: expensePageSize,
    );
    return (
      items: result.items,
      page: result.page,
      totalPages: result.totalPages,
    );
  }
}

final class ReimbursementListState {
  const ReimbursementListState({
    this.phase = ExpenseReadPhase.initial,
    this.items = const [],
    this.status,
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });
  final ExpenseReadPhase phase;
  final List<ReimbursementSummary> items;
  final String? status;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;
  bool get hasMore => page < totalPages;
}

final reimbursementListControllerProvider =
    StateNotifierProvider<ReimbursementListController, ReimbursementListState>(
      (ref) => ReimbursementListController(
        ref.watch(expenseRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class ReimbursementListController
    extends StateNotifier<ReimbursementListState> {
  ReimbursementListController(this._repository, {required this.onUnauthorized})
    : super(const ReimbursementListState());
  final ExpenseRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> setStatus(String? status) {
    state = ReimbursementListState(
      phase: state.phase,
      items: state.items,
      status: reimbursementStatuses.contains(status) ? status : null,
    );
    return load();
  }

  Future<void> load({String? status}) async {
    if (_loading) return;
    _loading = true;
    final safeStatus = reimbursementStatuses.contains(status)
        ? status
        : state.status;
    state = ReimbursementListState(
      phase: ExpenseReadPhase.loading,
      items: state.items,
      status: safeStatus,
    );
    try {
      final page = await _repository.listReimbursements(
        page: 1,
        pageSize: expensePageSize,
        status: safeStatus,
      );
      if (mounted) {
        state = ReimbursementListState(
          phase: page.items.isEmpty
              ? ExpenseReadPhase.empty
              : ExpenseReadPhase.data,
          items: page.items,
          status: safeStatus,
          page: page.page,
          totalPages: page.totalPages,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = ReimbursementListState(
          phase: ExpenseReadPhase.failure,
          items: state.items,
          status: safeStatus,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }

  Future<void> loadMore() async {
    if (_loading || !state.hasMore || state.items.isEmpty) return;
    _loading = true;
    state = ReimbursementListState(
      phase: ExpenseReadPhase.loadingMore,
      items: state.items,
      status: state.status,
      page: state.page,
      totalPages: state.totalPages,
    );
    try {
      final page = await _repository.listReimbursements(
        page: state.page + 1,
        pageSize: expensePageSize,
        status: state.status,
      );
      if (!mounted) return;
      final byId = {for (final item in state.items) item.reimbursementId: item};
      for (final item in page.items) {
        byId[item.reimbursementId] = item;
      }
      state = ReimbursementListState(
        phase: ExpenseReadPhase.data,
        items: byId.values.toList(growable: false),
        status: state.status,
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = ReimbursementListState(
          phase: ExpenseReadPhase.data,
          items: state.items,
          status: state.status,
          page: state.page,
          totalPages: state.totalPages,
          loadMoreError: error,
        );
      }
    } finally {
      _loading = false;
    }
  }
}

final reimbursementDetailsControllerProvider = FutureProvider.autoDispose
    .family<ReimbursementSummary, String>((ref, reimbursementId) async {
      try {
        return await ref
            .watch(expenseRepositoryProvider)
            .getReimbursement(reimbursementId);
      } on AppException catch (error) {
        if (error.kind == AppExceptionKind.unauthorized) {
          await ref.read(sessionControllerProvider.notifier).expire();
        }
        rethrow;
      }
    });

enum ExpenseCommandPhase {
  idle,
  submitting,
  uncertainSubmission,
  success,
  failure,
}

final class ExpenseCommandState<T> {
  const ExpenseCommandState({
    this.phase = ExpenseCommandPhase.idle,
    this.result,
    this.error,
  });
  final ExpenseCommandPhase phase;
  final T? result;
  final AppException? error;
  bool get isSubmitting => phase == ExpenseCommandPhase.submitting;
  bool get isUncertain => phase == ExpenseCommandPhase.uncertainSubmission;
}

abstract base class ExpenseFinancialCommandController<T>
    extends StateNotifier<ExpenseCommandState<T>> {
  ExpenseFinancialCommandController(
    FinancialOperationKeyFactory keyFactory, {
    required this.onUnauthorized,
  }) : _keySession = FinancialOperationKeySession(keyFactory),
       super(const ExpenseCommandState());

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
    state = const ExpenseCommandState(phase: ExpenseCommandPhase.submitting);
    try {
      final result = await operation(_keySession.keyFor(fingerprint));
      _clearPending();
      if (mounted) {
        state = ExpenseCommandState(
          phase: ExpenseCommandPhase.success,
          result: result,
        );
      }
      return result;
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      final uncertain =
          error.kind == AppExceptionKind.network ||
          error.kind == AppExceptionKind.timeout;
      if (!uncertain) _clearPending();
      if (mounted) {
        state = ExpenseCommandState(
          phase: uncertain
              ? ExpenseCommandPhase.uncertainSubmission
              : ExpenseCommandPhase.failure,
          error: error,
        );
      }
      return null;
    } finally {
      _submitting = false;
    }
  }

  void cancelPending() {
    _clearPending();
    if (mounted) state = const ExpenseCommandState();
  }

  void _clearPending() {
    _keySession.clear();
    _pendingFingerprint = null;
    _pendingOperation = null;
  }

  @override
  void dispose() {
    _clearPending();
    super.dispose();
  }
}

final expenseCreateControllerProvider =
    StateNotifierProvider.autoDispose<
      ExpenseCreateController,
      ExpenseCommandState<ExpenseDetails>
    >(
      (ref) => ExpenseCreateController(
        ref.watch(expenseRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final expenseApprovalControllerProvider =
    StateNotifierProvider.autoDispose<
      ExpenseApprovalController,
      ExpenseCommandState<ExpenseDetails>
    >(
      (ref) => ExpenseApprovalController(
        ref.watch(expenseRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final expenseRejectionControllerProvider =
    StateNotifierProvider.autoDispose<
      ExpenseRejectionController,
      ExpenseCommandState<ExpenseDetails>
    >(
      (ref) => ExpenseRejectionController(
        ref.watch(expenseRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class ExpenseCreateController
    extends ExpenseFinancialCommandController<ExpenseDetails> {
  ExpenseCreateController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final ExpenseRepository _repository;
  Future<ExpenseDetails?> create(CreateExpenseInput input) => execute(
    'create:${input.payloadFingerprint}',
    (key) => _repository.createExpense(input, key),
  );
}

final class ExpenseApprovalController
    extends ExpenseFinancialCommandController<ExpenseDetails> {
  ExpenseApprovalController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final ExpenseRepository _repository;
  Future<ExpenseDetails?> approve(
    String expenseId,
    ApproveExpenseInput input,
  ) => execute(
    'approve:$expenseId:${input.payloadFingerprint}',
    (key) => _repository.approveExpense(expenseId, input, key),
  );
}

final class ExpenseRejectionController
    extends ExpenseFinancialCommandController<ExpenseDetails> {
  ExpenseRejectionController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final ExpenseRepository _repository;
  Future<ExpenseDetails?> reject(String expenseId, RejectExpenseInput input) =>
      execute(
        'reject:$expenseId:${input.payloadFingerprint}',
        (key) => _repository.rejectExpense(expenseId, input, key),
      );
}

enum CategoryCommandPhase { idle, submitting, success, failure }

final class CategoryCommandState {
  const CategoryCommandState({
    this.phase = CategoryCommandPhase.idle,
    this.result,
    this.error,
  });
  final CategoryCommandPhase phase;
  final ExpenseCategory? result;
  final AppException? error;
}

final expenseCategoryCreateControllerProvider =
    StateNotifierProvider.autoDispose<
      ExpenseCategoryCreateController,
      CategoryCommandState
    >(
      (ref) => ExpenseCategoryCreateController(
        ref.watch(expenseRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class ExpenseCategoryCreateController
    extends StateNotifier<CategoryCommandState> {
  ExpenseCategoryCreateController(
    this._repository, {
    required this.onUnauthorized,
  }) : super(const CategoryCommandState());
  final ExpenseRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _submitting = false;

  Future<ExpenseCategory?> create(CreateExpenseCategoryInput input) async {
    if (_submitting) return null;
    _submitting = true;
    state = const CategoryCommandState(phase: CategoryCommandPhase.submitting);
    try {
      final result = await _repository.createCategory(input);
      if (mounted) {
        state = CategoryCommandState(
          phase: CategoryCommandPhase.success,
          result: result,
        );
      }
      return result;
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = CategoryCommandState(
          phase: CategoryCommandPhase.failure,
          error: error,
        );
      }
      return null;
    } finally {
      _submitting = false;
    }
  }
}
