import '../../../core/errors/app_exception.dart';
import 'project_models.dart';

enum ProjectListPhase {
  initial,
  loading,
  data,
  empty,
  refreshing,
  loadingMore,
  failure,
}

final class ProjectListState {
  const ProjectListState({
    this.phase = ProjectListPhase.initial,
    this.items = const [],
    this.status,
    this.search = '',
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final ProjectListPhase phase;
  final List<ProjectDetails> items;
  final String? status;
  final String search;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;

  bool get hasMore => page < totalPages;

  ProjectListState copyWith({
    ProjectListPhase? phase,
    List<ProjectDetails>? items,
    Object? status = _notSet,
    String? search,
    int? page,
    int? totalPages,
    Object? error = _notSet,
    Object? loadMoreError = _notSet,
  }) => ProjectListState(
    phase: phase ?? this.phase,
    items: items ?? this.items,
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
