import '../../../core/errors/app_exception.dart';

enum AccessListPhase {
  initial,
  loading,
  data,
  empty,
  refreshing,
  loadingMore,
  failure,
}

final class AccessListState<T> {
  const AccessListState({
    this.phase = AccessListPhase.initial,
    this.items = const [],
    this.filter,
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final AccessListPhase phase;
  final List<T> items;
  final String? filter;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;

  bool get hasMore => page < totalPages;

  AccessListState<T> copyWith({
    AccessListPhase? phase,
    List<T>? items,
    Object? filter = _notSet,
    int? page,
    int? totalPages,
    Object? error = _notSet,
    Object? loadMoreError = _notSet,
  }) => AccessListState(
    phase: phase ?? this.phase,
    items: items ?? this.items,
    filter: identical(filter, _notSet) ? this.filter : filter as String?,
    page: page ?? this.page,
    totalPages: totalPages ?? this.totalPages,
    error: identical(error, _notSet) ? this.error : error as AppException?,
    loadMoreError: identical(loadMoreError, _notSet)
        ? this.loadMoreError
        : loadMoreError as AppException?,
  );
}

const _notSet = Object();

enum AccessSubmitPhase { idle, submitting, success, failure }

final class AccessSubmitState {
  const AccessSubmitState({this.phase = AccessSubmitPhase.idle, this.error});

  final AccessSubmitPhase phase;
  final AppException? error;
  bool get isSubmitting => phase == AccessSubmitPhase.submitting;
}
