import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../company_members/domain/company_member_models.dart';
import '../../../company_members/domain/company_member_repository.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/advance_list_state.dart';
import '../../domain/advance_models.dart';
import '../../domain/advance_repository.dart';
import '../../domain/advance_requests.dart';
import '../../domain/financial_operation_key.dart';

const advancePageSize = 20;

final advanceListControllerProvider =
    StateNotifierProvider<AdvanceListController, AdvanceListState>(
      (ref) => AdvanceListController(
        ref.watch(advanceRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceDetailsControllerProvider = StateNotifierProvider.autoDispose
    .family<AdvanceDetailsController, AdvanceDetailsState, String>(
      (ref, advanceId) => AdvanceDetailsController(
        ref.watch(advanceRepositoryProvider),
        advanceId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceMovementsControllerProvider = StateNotifierProvider.autoDispose
    .family<AdvanceMovementsController, AdvanceMovementsState, String>(
      (ref, advanceId) => AdvanceMovementsController(
        ref.watch(advanceRepositoryProvider),
        advanceId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final personalAdvanceBalanceControllerProvider =
    StateNotifierProvider<
      PersonalAdvanceBalanceController,
      AdvanceBalancesState
    >(
      (ref) => PersonalAdvanceBalanceController(
        ref.watch(advanceRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final authorizedAdvanceBalanceControllerProvider = StateNotifierProvider
    .autoDispose
    .family<AuthorizedAdvanceBalanceController, AdvanceBalancesState, String>(
      (ref, userId) => AuthorizedAdvanceBalanceController(
        ref.watch(advanceRepositoryProvider),
        userId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final fundingSourceControllerProvider =
    StateNotifierProvider.autoDispose<
      FundingSourceController,
      FundingSourceState
    >(
      (ref) => FundingSourceController(
        ref.watch(advanceRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceRecipientControllerProvider = StateNotifierProvider.autoDispose
    .family<AdvanceRecipientController, AdvanceRecipientState, String>(
      (ref, senderRole) => AdvanceRecipientController(
        ref.watch(companyMemberRepositoryProvider),
        senderRole,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceMemberDirectoryControllerProvider =
    StateNotifierProvider.autoDispose<
      AdvanceMemberDirectoryController,
      AdvanceRecipientState
    >(
      (ref) => AdvanceMemberDirectoryController(
        ref.watch(companyMemberRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceCreateControllerProvider =
    StateNotifierProvider.autoDispose<
      AdvanceCreateController,
      FinancialCommandState<AdvanceDetails>
    >(
      (ref) => AdvanceCreateController(
        ref.watch(advanceRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceDistributionControllerProvider =
    StateNotifierProvider.autoDispose<
      AdvanceDistributionController,
      FinancialCommandState<AdvanceTransferDetails>
    >(
      (ref) => AdvanceDistributionController(
        ref.watch(advanceRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceReturnControllerProvider =
    StateNotifierProvider.autoDispose<
      AdvanceReturnController,
      FinancialCommandState<AdvanceTransferDetails>
    >(
      (ref) => AdvanceReturnController(
        ref.watch(advanceRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceConfirmationControllerProvider =
    StateNotifierProvider.autoDispose<
      AdvanceConfirmationController,
      FinancialCommandState<AdvanceTransferDetails>
    >(
      (ref) => AdvanceConfirmationController(
        ref.watch(advanceRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final advanceRejectionControllerProvider =
    StateNotifierProvider.autoDispose<
      AdvanceRejectionController,
      FinancialCommandState<AdvanceTransferDetails>
    >(
      (ref) => AdvanceRejectionController(
        ref.watch(advanceRepositoryProvider),
        ref.watch(financialOperationKeyFactoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class AdvanceListController extends StateNotifier<AdvanceListState> {
  AdvanceListController(this._repository, {required this.onUnauthorized})
    : super(const AdvanceListState());

  final AdvanceRepository _repository;
  final Future<void> Function() onUnauthorized;
  int _generation = 0;
  bool _loadingMore = false;
  Timer? _searchTimer;

  Future<void> load() => _replace(AdvanceListPhase.loading);
  Future<void> refresh() => _replace(AdvanceListPhase.refreshing);

  Future<void> setStatus(String? status) async {
    final safeStatus = knownAdvanceStatuses.contains(status) ? status : null;
    if (state.status == safeStatus) return;
    state = state.copyWith(status: safeStatus, page: 0, totalPages: 0);
    await load();
  }

  Future<void> setUser(String? userId) async {
    if (state.userId == userId) return;
    state = state.copyWith(userId: userId, page: 0, totalPages: 0);
    await load();
  }

  void setReference(
    String value, {
    Duration debounce = const Duration(milliseconds: 350),
  }) {
    final bounded = value.length > 50 ? value.substring(0, 50) : value;
    if (state.reference == bounded) return;
    _searchTimer?.cancel();
    state = state.copyWith(reference: bounded, page: 0, totalPages: 0);
    if (bounded.trim().length == 1) {
      _generation++;
      state = state.copyWith(phase: AdvanceListPhase.empty, items: const []);
      return;
    }
    _searchTimer = Timer(debounce, load);
  }

  Future<void> _replace(AdvanceListPhase phase) async {
    final generation = ++_generation;
    state = state.copyWith(phase: phase, error: null, loadMoreError: null);
    try {
      final reference = state.reference.trim();
      final page = await _repository.listAdvances(
        page: 1,
        pageSize: advancePageSize,
        status: state.status,
        userId: state.userId,
        reference: reference.length >= 2 ? reference : null,
      );
      if (!mounted || generation != _generation) return;
      state = state.copyWith(
        phase: page.items.isEmpty
            ? AdvanceListPhase.empty
            : AdvanceListPhase.data,
        items: page.items,
        page: page.page,
        totalPages: page.totalPages,
        error: null,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted && generation == _generation) {
        state = state.copyWith(phase: AdvanceListPhase.failure, error: error);
      }
    }
  }

  Future<void> loadMore() async {
    if (_loadingMore || !state.hasMore || state.items.isEmpty) return;
    _loadingMore = true;
    final generation = _generation;
    state = state.copyWith(
      phase: AdvanceListPhase.loadingMore,
      loadMoreError: null,
    );
    try {
      final reference = state.reference.trim();
      final page = await _repository.listAdvances(
        page: state.page + 1,
        pageSize: advancePageSize,
        status: state.status,
        userId: state.userId,
        reference: reference.length >= 2 ? reference : null,
      );
      if (!mounted || generation != _generation) return;
      final byId = {for (final item in state.items) item.advanceId: item};
      for (final item in page.items) {
        byId[item.advanceId] = item;
      }
      state = state.copyWith(
        phase: AdvanceListPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted && generation == _generation) {
        state = state.copyWith(
          phase: AdvanceListPhase.data,
          loadMoreError: error,
        );
      }
    } finally {
      _loadingMore = false;
    }
  }

  @override
  void dispose() {
    _searchTimer?.cancel();
    super.dispose();
  }
}

enum AdvanceDetailsPhase { initial, loading, data, failure }

final class AdvanceDetailsState {
  const AdvanceDetailsState({
    this.phase = AdvanceDetailsPhase.initial,
    this.details,
    this.error,
  });

  final AdvanceDetailsPhase phase;
  final AdvanceDetails? details;
  final AppException? error;
}

final class AdvanceDetailsController
    extends StateNotifier<AdvanceDetailsState> {
  AdvanceDetailsController(
    this._repository,
    this.advanceId, {
    required this.onUnauthorized,
  }) : super(const AdvanceDetailsState());

  final AdvanceRepository _repository;
  final String advanceId;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = AdvanceDetailsState(
      phase: AdvanceDetailsPhase.loading,
      details: state.details,
    );
    try {
      final details = await _repository.getAdvance(advanceId);
      if (mounted) {
        state = AdvanceDetailsState(
          phase: AdvanceDetailsPhase.data,
          details: details,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = AdvanceDetailsState(
          phase: AdvanceDetailsPhase.failure,
          details: state.details,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }
}

enum PagedReadPhase { initial, loading, data, empty, loadingMore, failure }

final class AdvanceMovementsState {
  const AdvanceMovementsState({
    this.phase = PagedReadPhase.initial,
    this.items = const [],
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final PagedReadPhase phase;
  final List<AdvanceMovement> items;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;
  bool get hasMore => page < totalPages;
}

final class AdvanceMovementsController
    extends StateNotifier<AdvanceMovementsState> {
  AdvanceMovementsController(
    this._repository,
    this.advanceId, {
    required this.onUnauthorized,
  }) : super(const AdvanceMovementsState());

  final AdvanceRepository _repository;
  final String advanceId;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = const AdvanceMovementsState(phase: PagedReadPhase.loading);
    try {
      final page = await _repository.listMovements(
        advanceId,
        page: 1,
        pageSize: advancePageSize,
      );
      if (mounted) {
        state = AdvanceMovementsState(
          phase: page.items.isEmpty
              ? PagedReadPhase.empty
              : PagedReadPhase.data,
          items: page.items,
          page: page.page,
          totalPages: page.totalPages,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = AdvanceMovementsState(
          phase: PagedReadPhase.failure,
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
    state = AdvanceMovementsState(
      phase: PagedReadPhase.loadingMore,
      items: state.items,
      page: state.page,
      totalPages: state.totalPages,
    );
    try {
      final page = await _repository.listMovements(
        advanceId,
        page: state.page + 1,
        pageSize: advancePageSize,
      );
      if (!mounted) return;
      final byId = {
        for (final item in state.items)
          '${item.movementId}:${item.operationType}': item,
      };
      for (final item in page.items) {
        byId['${item.movementId}:${item.operationType}'] = item;
      }
      state = AdvanceMovementsState(
        phase: PagedReadPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = AdvanceMovementsState(
          phase: PagedReadPhase.data,
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

final class AdvanceBalancesState {
  const AdvanceBalancesState({
    this.phase = PagedReadPhase.initial,
    this.items = const [],
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final PagedReadPhase phase;
  final List<AdvanceBalanceSummary> items;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;
  bool get hasMore => page < totalPages;
}

abstract base class _AdvanceBalanceController
    extends StateNotifier<AdvanceBalancesState> {
  _AdvanceBalanceController({required this.onUnauthorized})
    : super(const AdvanceBalancesState());

  final Future<void> Function() onUnauthorized;
  bool _loading = false;
  Future<AdvanceBalancePage> fetch(int page);

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = const AdvanceBalancesState(phase: PagedReadPhase.loading);
    try {
      final result = await fetch(1);
      if (mounted) {
        state = AdvanceBalancesState(
          phase: result.page.items.isEmpty
              ? PagedReadPhase.empty
              : PagedReadPhase.data,
          items: result.page.items,
          page: result.page.page,
          totalPages: result.page.totalPages,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = AdvanceBalancesState(
          phase: PagedReadPhase.failure,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }

  Future<void> refresh() => load();

  Future<void> loadMore() async {
    if (_loading || !state.hasMore || state.items.isEmpty) return;
    _loading = true;
    try {
      final result = await fetch(state.page + 1);
      if (!mounted) return;
      final byAdvance = {for (final item in state.items) item.advanceId: item};
      for (final item in result.page.items) {
        byAdvance[item.advanceId] = item;
      }
      state = AdvanceBalancesState(
        phase: PagedReadPhase.data,
        items: byAdvance.values.toList(growable: false),
        page: result.page.page,
        totalPages: result.page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = AdvanceBalancesState(
          phase: PagedReadPhase.data,
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

final class PersonalAdvanceBalanceController extends _AdvanceBalanceController {
  PersonalAdvanceBalanceController(
    this._repository, {
    required super.onUnauthorized,
  });

  final AdvanceRepository _repository;

  @override
  Future<AdvanceBalancePage> fetch(int page) =>
      _repository.getMyBalances(page: page, pageSize: advancePageSize);
}

final class AuthorizedAdvanceBalanceController
    extends _AdvanceBalanceController {
  AuthorizedAdvanceBalanceController(
    this._repository,
    this.userId, {
    required super.onUnauthorized,
  });

  final AdvanceRepository _repository;
  final String userId;

  @override
  Future<AdvanceBalancePage> fetch(int page) => _repository.getUserBalances(
    userId,
    page: page,
    pageSize: advancePageSize,
  );
}

final class FundingSourceState {
  const FundingSourceState({
    this.phase = PagedReadPhase.initial,
    this.items = const [],
    this.page = 0,
    this.totalPages = 0,
    this.error,
  });

  final PagedReadPhase phase;
  final List<AvailableFundingSource> items;
  final int page;
  final int totalPages;
  final AppException? error;
  bool get hasMore => page < totalPages;
}

final class FundingSourceController extends StateNotifier<FundingSourceState> {
  FundingSourceController(this._repository, {required this.onUnauthorized})
    : super(const FundingSourceState());

  final AdvanceRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = const FundingSourceState(phase: PagedReadPhase.loading);
    try {
      final page = await _repository.listFundingSources(
        page: 1,
        pageSize: advancePageSize,
      );
      if (mounted) {
        state = FundingSourceState(
          phase: page.items.isEmpty
              ? PagedReadPhase.empty
              : PagedReadPhase.data,
          items: page.items,
          page: page.page,
          totalPages: page.totalPages,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = FundingSourceState(phase: PagedReadPhase.failure, error: error);
      }
    } finally {
      _loading = false;
    }
  }

  Future<void> loadMore() async {
    if (_loading || !state.hasMore) return;
    _loading = true;
    try {
      final page = await _repository.listFundingSources(
        page: state.page + 1,
        pageSize: advancePageSize,
      );
      if (!mounted) return;
      final byId = {for (final item in state.items) item.fundingSourceId: item};
      for (final item in page.items) {
        byId[item.fundingSourceId] = item;
      }
      state = FundingSourceState(
        phase: PagedReadPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = FundingSourceState(
          phase: PagedReadPhase.data,
          items: state.items,
          page: state.page,
          totalPages: state.totalPages,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }
}

final class AdvanceRecipientState {
  const AdvanceRecipientState({
    this.phase = PagedReadPhase.initial,
    this.items = const [],
    this.search = '',
    this.page = 0,
    this.totalPages = 0,
    this.error,
  });

  final PagedReadPhase phase;
  final List<CompanyMemberSummary> items;
  final String search;
  final int page;
  final int totalPages;
  final AppException? error;
  bool get hasMore => page < totalPages;
}

final class AdvanceRecipientController
    extends StateNotifier<AdvanceRecipientState> {
  AdvanceRecipientController(
    this._repository,
    this.senderRole, {
    required this.onUnauthorized,
  }) : super(const AdvanceRecipientState());

  final CompanyMemberRepository _repository;
  final String senderRole;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;
  Timer? _searchTimer;

  List<String> get _roles => senderRole == 'Manager'
      ? const ['Deputy']
      : senderRole == 'Deputy'
      ? const ['Supervisor', 'Worker']
      : const [];

  Future<void> load() => _fetch(page: 1, replace: true);

  void setSearch(
    String value, {
    Duration debounce = const Duration(milliseconds: 350),
  }) {
    final bounded = value.length > 100 ? value.substring(0, 100) : value;
    if (state.search == bounded) return;
    _searchTimer?.cancel();
    state = AdvanceRecipientState(
      phase: state.phase,
      items: state.items,
      search: bounded,
      page: state.page,
      totalPages: state.totalPages,
    );
    if (bounded.trim().length == 1) {
      state = AdvanceRecipientState(
        phase: PagedReadPhase.empty,
        search: bounded,
      );
      return;
    }
    _searchTimer = Timer(debounce, load);
  }

  Future<void> loadMore() => state.hasMore
      ? _fetch(page: state.page + 1, replace: false)
      : Future.value();

  Future<void> _fetch({required int page, required bool replace}) async {
    if (_loading) return;
    _loading = true;
    final search = state.search.trim();
    state = AdvanceRecipientState(
      phase: replace ? PagedReadPhase.loading : PagedReadPhase.loadingMore,
      items: replace ? const [] : state.items,
      search: state.search,
      page: state.page,
      totalPages: state.totalPages,
    );
    try {
      final results = await Future.wait([
        for (final role in _roles)
          _repository.listMembers(
            page: page,
            pageSize: advancePageSize,
            role: role,
            status: 'Active',
            search: search.length >= 2 ? search : null,
          ),
      ]);
      final byId = replace
          ? <String, CompanyMemberSummary>{}
          : {for (final item in state.items) item.id: item};
      var totalPages = 0;
      for (final result in results) {
        if (result.totalPages > totalPages) totalPages = result.totalPages;
        for (final item in result.items) {
          if (_roles.contains(item.role) && item.status == 'Active') {
            byId[item.id] = item;
          }
        }
      }
      if (mounted) {
        state = AdvanceRecipientState(
          phase: byId.isEmpty ? PagedReadPhase.empty : PagedReadPhase.data,
          items: byId.values.toList(growable: false),
          search: state.search,
          page: page,
          totalPages: totalPages,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = AdvanceRecipientState(
          phase: PagedReadPhase.failure,
          items: replace ? const [] : state.items,
          search: state.search,
          page: state.page,
          totalPages: state.totalPages,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }

  @override
  void dispose() {
    _searchTimer?.cancel();
    super.dispose();
  }
}

final class AdvanceMemberDirectoryController
    extends StateNotifier<AdvanceRecipientState> {
  AdvanceMemberDirectoryController(
    this._repository, {
    required this.onUnauthorized,
  }) : super(const AdvanceRecipientState());

  final CompanyMemberRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = const AdvanceRecipientState(phase: PagedReadPhase.loading);
    try {
      final page = await _repository.listMembers(page: 1, pageSize: 100);
      if (mounted) {
        state = AdvanceRecipientState(
          phase: page.items.isEmpty
              ? PagedReadPhase.empty
              : PagedReadPhase.data,
          items: page.items,
        );
      }
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (mounted) {
        state = AdvanceRecipientState(
          phase: PagedReadPhase.failure,
          error: error,
        );
      }
    } finally {
      _loading = false;
    }
  }
}

enum FinancialCommandPhase {
  idle,
  submitting,
  uncertainSubmission,
  success,
  failure,
}

final class FinancialCommandState<T> {
  const FinancialCommandState({
    this.phase = FinancialCommandPhase.idle,
    this.result,
    this.error,
  });

  final FinancialCommandPhase phase;
  final T? result;
  final AppException? error;
  bool get isSubmitting => phase == FinancialCommandPhase.submitting;
  bool get isUncertain => phase == FinancialCommandPhase.uncertainSubmission;
}

abstract base class FinancialCommandController<T>
    extends StateNotifier<FinancialCommandState<T>> {
  FinancialCommandController(
    FinancialOperationKeyFactory keyFactory, {
    required this.onUnauthorized,
  }) : _keySession = FinancialOperationKeySession(keyFactory),
       super(const FinancialCommandState());

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
    if (mounted) {
      state = const FinancialCommandState(
        phase: FinancialCommandPhase.submitting,
      );
    }
    try {
      final result = await operation(_keySession.keyFor(fingerprint));
      _clearPending();
      if (mounted) {
        state = FinancialCommandState(
          phase: FinancialCommandPhase.success,
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
        state = FinancialCommandState(
          phase: uncertain
              ? FinancialCommandPhase.uncertainSubmission
              : FinancialCommandPhase.failure,
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
    if (mounted) state = const FinancialCommandState();
  }

  void reset() {
    if (state.phase != FinancialCommandPhase.uncertainSubmission) {
      if (mounted) state = const FinancialCommandState();
    }
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

final class AdvanceCreateController
    extends FinancialCommandController<AdvanceDetails> {
  AdvanceCreateController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final AdvanceRepository _repository;

  Future<AdvanceDetails?> create(CreateAdvanceInput input) => execute(
    'create:${input.payloadFingerprint}',
    (key) => _repository.createAdvance(input, key),
  );
}

final class AdvanceDistributionController
    extends FinancialCommandController<AdvanceTransferDetails> {
  AdvanceDistributionController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final AdvanceRepository _repository;

  Future<AdvanceTransferDetails?> distribute(
    String advanceId,
    CreateAdvanceDistributionInput input,
  ) => execute(
    'distribute:$advanceId:${input.payloadFingerprint}',
    (key) => _repository.distribute(advanceId, input, key),
  );
}

final class AdvanceReturnController
    extends FinancialCommandController<AdvanceTransferDetails> {
  AdvanceReturnController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final AdvanceRepository _repository;

  Future<AdvanceTransferDetails?> returnMoney(
    String advanceId,
    CreateAdvanceReturnInput input,
  ) => execute(
    'return:$advanceId:${input.payloadFingerprint}',
    (key) => _repository.returnMoney(advanceId, input, key),
  );
}

final class AdvanceConfirmationController
    extends FinancialCommandController<AdvanceTransferDetails> {
  AdvanceConfirmationController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final AdvanceRepository _repository;

  Future<AdvanceTransferDetails?> confirm(String transferId) => execute(
    'confirm:$transferId',
    (key) => _repository.confirmTransfer(transferId, key),
  );
}

final class AdvanceRejectionController
    extends FinancialCommandController<AdvanceTransferDetails> {
  AdvanceRejectionController(
    this._repository,
    super.keyFactory, {
    required super.onUnauthorized,
  });
  final AdvanceRepository _repository;

  Future<AdvanceTransferDetails?> reject(
    String transferId,
    RejectAdvanceTransferInput input,
  ) => execute(
    'reject:$transferId:${input.payloadFingerprint}',
    (key) => _repository.rejectTransfer(transferId, input, key),
  );
}
