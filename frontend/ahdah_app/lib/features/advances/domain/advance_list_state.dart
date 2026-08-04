import '../../../core/errors/app_exception.dart';
import 'advance_models.dart';

enum AdvanceListPhase {
  initial,
  loading,
  data,
  empty,
  refreshing,
  loadingMore,
  failure,
}

final class AdvanceListState {
  const AdvanceListState({
    this.phase = AdvanceListPhase.initial,
    this.items = const [],
    this.status,
    this.userId,
    this.reference = '',
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final AdvanceListPhase phase;
  final List<AdvanceSummary> items;
  final String? status;
  final String? userId;
  final String reference;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;

  bool get hasMore => page < totalPages;

  AdvanceListState copyWith({
    AdvanceListPhase? phase,
    List<AdvanceSummary>? items,
    Object? status = _unset,
    Object? userId = _unset,
    String? reference,
    int? page,
    int? totalPages,
    Object? error = _unset,
    Object? loadMoreError = _unset,
  }) => AdvanceListState(
    phase: phase ?? this.phase,
    items: items ?? this.items,
    status: identical(status, _unset) ? this.status : status as String?,
    userId: identical(userId, _unset) ? this.userId : userId as String?,
    reference: reference ?? this.reference,
    page: page ?? this.page,
    totalPages: totalPages ?? this.totalPages,
    error: identical(error, _unset) ? this.error : error as AppException?,
    loadMoreError: identical(loadMoreError, _unset)
        ? this.loadMoreError
        : loadMoreError as AppException?,
  );
}

const knownAdvanceStatuses = <String>[
  'Draft',
  'PendingConfirmation',
  'Open',
  'InSettlement',
  'ReadyToClose',
  'Closed',
  'Cancelled',
  'Reversed',
];

const knownTransferMethods = <String>[
  'Cash',
  'BankTransfer',
  'Cheque',
  'Card',
  'MobileWallet',
  'BalanceTransfer',
  'Other',
];

const _unset = Object();
