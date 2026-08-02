import '../../../core/errors/app_exception.dart';
import 'company_member_models.dart';

enum CompanyMemberListPhase {
  initial,
  loading,
  data,
  empty,
  refreshing,
  loadingMore,
  failure,
}

final class CompanyMemberListState {
  const CompanyMemberListState({
    this.phase = CompanyMemberListPhase.initial,
    this.items = const [],
    this.role,
    this.status,
    this.search = '',
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final CompanyMemberListPhase phase;
  final List<CompanyMemberSummary> items;
  final String? role;
  final String? status;
  final String search;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;

  bool get hasMore => page < totalPages;

  CompanyMemberListState copyWith({
    CompanyMemberListPhase? phase,
    List<CompanyMemberSummary>? items,
    Object? role = _notSet,
    Object? status = _notSet,
    String? search,
    int? page,
    int? totalPages,
    Object? error = _notSet,
    Object? loadMoreError = _notSet,
  }) => CompanyMemberListState(
    phase: phase ?? this.phase,
    items: items ?? this.items,
    role: identical(role, _notSet) ? this.role : role as String?,
    status: identical(status, _notSet) ? this.status : status as String?,
    search: search ?? this.search,
    page: page ?? this.page,
    totalPages: totalPages ?? this.totalPages,
    error: identical(error, _notSet) ? this.error : error as AppException?,
    loadMoreError: identical(loadMoreError, _notSet)
        ? this.loadMoreError
        : loadMoreError as AppException?,
  );
}

const _notSet = Object();
