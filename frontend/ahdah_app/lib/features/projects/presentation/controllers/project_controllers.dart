import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/project_list_state.dart';
import '../../domain/project_models.dart';
import '../../domain/project_repository.dart';
import '../../domain/project_requests.dart';

const projectPageSize = 20;

final projectListControllerProvider =
    StateNotifierProvider<ProjectListController, ProjectListState>(
      (ref) => ProjectListController(
        ref.watch(projectRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final projectDetailsControllerProvider = StateNotifierProvider.autoDispose
    .family<ProjectDetailsController, ProjectDetailsState, String>(
      (ref, projectId) => ProjectDetailsController(
        ref.watch(projectRepositoryProvider),
        projectId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final projectMembersControllerProvider = StateNotifierProvider.autoDispose
    .family<ProjectMembersController, ProjectMembersState, String>(
      (ref, projectId) => ProjectMembersController(
        ref.watch(projectRepositoryProvider),
        projectId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final projectCreateControllerProvider =
    StateNotifierProvider<ProjectCreateController, ProjectSubmitState>(
      (ref) => ProjectCreateController(
        ref.watch(projectRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final projectUpdateControllerProvider =
    StateNotifierProvider<ProjectUpdateController, ProjectSubmitState>(
      (ref) => ProjectUpdateController(
        ref.watch(projectRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final supervisorAssignmentControllerProvider =
    StateNotifierProvider<SupervisorAssignmentController, ProjectSubmitState>(
      (ref) => SupervisorAssignmentController(
        ref.watch(projectRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class ProjectListController extends StateNotifier<ProjectListState> {
  ProjectListController(this._repository, {required this.onUnauthorized})
    : super(const ProjectListState());

  final ProjectRepository _repository;
  final Future<void> Function() onUnauthorized;
  int _replaceGeneration = 0;
  bool _loadingMore = false;
  Timer? _searchTimer;

  Future<void> load() => _replace(ProjectListPhase.loading);
  Future<void> refresh() => _replace(ProjectListPhase.refreshing);

  Future<void> setStatus(String? status) async {
    if (state.status == status) return;
    _searchTimer?.cancel();
    state = state.copyWith(status: status, page: 0, totalPages: 0);
    await load();
  }

  void setSearch(
    String value, {
    Duration debounce = const Duration(milliseconds: 350),
  }) {
    final bounded = value.length > 100 ? value.substring(0, 100) : value;
    if (state.search == bounded) return;
    _searchTimer?.cancel();
    state = state.copyWith(search: bounded, page: 0, totalPages: 0);
    final normalized = bounded.trim();
    if (normalized.length == 1) {
      _replaceGeneration++;
      state = state.copyWith(phase: ProjectListPhase.empty, items: const []);
      return;
    }
    _searchTimer = Timer(debounce, load);
  }

  Future<void> _replace(ProjectListPhase phase) async {
    final generation = ++_replaceGeneration;
    state = state.copyWith(phase: phase, error: null, loadMoreError: null);
    try {
      final normalizedSearch = state.search.trim();
      final page = await _repository.listProjects(
        page: 1,
        pageSize: projectPageSize,
        status: state.status,
        search: normalizedSearch.length >= 2 ? normalizedSearch : null,
      );
      if (generation != _replaceGeneration) return;
      state = state.copyWith(
        phase: page.items.isEmpty
            ? ProjectListPhase.empty
            : ProjectListPhase.data,
        items: page.items,
        page: page.page,
        totalPages: page.totalPages,
        error: null,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (generation == _replaceGeneration) {
        state = state.copyWith(phase: ProjectListPhase.failure, error: error);
      }
    }
  }

  Future<void> loadMore() async {
    if (_loadingMore || !state.hasMore || state.items.isEmpty) return;
    _loadingMore = true;
    final generation = _replaceGeneration;
    state = state.copyWith(
      phase: ProjectListPhase.loadingMore,
      loadMoreError: null,
    );
    try {
      final normalizedSearch = state.search.trim();
      final page = await _repository.listProjects(
        page: state.page + 1,
        pageSize: projectPageSize,
        status: state.status,
        search: normalizedSearch.length >= 2 ? normalizedSearch : null,
      );
      if (generation != _replaceGeneration) return;
      final byId = {for (final item in state.items) item.id: item};
      for (final item in page.items) {
        byId[item.id] = item;
      }
      state = state.copyWith(
        phase: ProjectListPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (generation == _replaceGeneration) {
        state = state.copyWith(
          phase: ProjectListPhase.data,
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

enum ProjectDetailsPhase { initial, loading, data, failure }

final class ProjectDetailsState {
  const ProjectDetailsState({
    this.phase = ProjectDetailsPhase.initial,
    this.project,
    this.error,
  });

  final ProjectDetailsPhase phase;
  final ProjectDetails? project;
  final AppException? error;
}

final class ProjectDetailsController
    extends StateNotifier<ProjectDetailsState> {
  ProjectDetailsController(
    this._repository,
    this.projectId, {
    required this.onUnauthorized,
  }) : super(const ProjectDetailsState());

  final ProjectRepository _repository;
  final String projectId;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = ProjectDetailsState(
      phase: ProjectDetailsPhase.loading,
      project: state.project,
    );
    try {
      state = ProjectDetailsState(
        phase: ProjectDetailsPhase.data,
        project: await _repository.getProject(projectId),
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      state = ProjectDetailsState(
        phase: ProjectDetailsPhase.failure,
        project: state.project,
        error: error,
      );
    } finally {
      _loading = false;
    }
  }
}

enum ProjectMembersPhase { initial, loading, data, empty, loadingMore, failure }

final class ProjectMembersState {
  const ProjectMembersState({
    this.phase = ProjectMembersPhase.initial,
    this.items = const [],
    this.page = 0,
    this.totalPages = 0,
    this.error,
    this.loadMoreError,
  });

  final ProjectMembersPhase phase;
  final List<ProjectMemberSummary> items;
  final int page;
  final int totalPages;
  final AppException? error;
  final AppException? loadMoreError;

  bool get hasMore => page < totalPages;

  ProjectMembersState copyWith({
    ProjectMembersPhase? phase,
    List<ProjectMemberSummary>? items,
    int? page,
    int? totalPages,
    Object? error = _sentinel,
    Object? loadMoreError = _sentinel,
  }) => ProjectMembersState(
    phase: phase ?? this.phase,
    items: items ?? this.items,
    page: page ?? this.page,
    totalPages: totalPages ?? this.totalPages,
    error: identical(error, _sentinel) ? this.error : error as AppException?,
    loadMoreError: identical(loadMoreError, _sentinel)
        ? this.loadMoreError
        : loadMoreError as AppException?,
  );
}

final class ProjectMembersController
    extends StateNotifier<ProjectMembersState> {
  ProjectMembersController(
    this._repository,
    this.projectId, {
    required this.onUnauthorized,
  }) : super(const ProjectMembersState());

  final ProjectRepository _repository;
  final String projectId;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = const ProjectMembersState(phase: ProjectMembersPhase.loading);
    try {
      final page = await _repository.listProjectMembers(
        projectId,
        page: 1,
        pageSize: projectPageSize,
      );
      state = ProjectMembersState(
        phase: page.items.isEmpty
            ? ProjectMembersPhase.empty
            : ProjectMembersPhase.data,
        items: page.items,
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      state = ProjectMembersState(
        phase: ProjectMembersPhase.failure,
        error: error,
      );
    } finally {
      _loading = false;
    }
  }

  Future<void> loadMore() async {
    if (_loading || !state.hasMore || state.items.isEmpty) return;
    _loading = true;
    state = state.copyWith(
      phase: ProjectMembersPhase.loadingMore,
      loadMoreError: null,
    );
    try {
      final page = await _repository.listProjectMembers(
        projectId,
        page: state.page + 1,
        pageSize: projectPageSize,
      );
      final byId = {for (final item in state.items) item.id: item};
      for (final item in page.items) {
        byId[item.id] = item;
      }
      state = state.copyWith(
        phase: ProjectMembersPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      state = state.copyWith(
        phase: ProjectMembersPhase.data,
        loadMoreError: error,
      );
    } finally {
      _loading = false;
    }
  }
}

enum ProjectSubmitPhase { idle, submitting, success, failure }

final class ProjectSubmitState {
  const ProjectSubmitState({this.phase = ProjectSubmitPhase.idle, this.error});

  final ProjectSubmitPhase phase;
  final AppException? error;
  bool get isSubmitting => phase == ProjectSubmitPhase.submitting;
  bool get isConflict => error?.kind == AppExceptionKind.conflict;
}

abstract base class _ProjectWriteController
    extends StateNotifier<ProjectSubmitState> {
  _ProjectWriteController({required this.onUnauthorized})
    : super(const ProjectSubmitState());

  final Future<void> Function() onUnauthorized;
  bool _submitting = false;

  Future<ProjectDetails?> submit(
    Future<ProjectDetails> Function() operation,
  ) async {
    if (_submitting) return null;
    _submitting = true;
    state = const ProjectSubmitState(phase: ProjectSubmitPhase.submitting);
    try {
      final result = await operation();
      state = const ProjectSubmitState(phase: ProjectSubmitPhase.success);
      return result;
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      state = ProjectSubmitState(
        phase: ProjectSubmitPhase.failure,
        error: error,
      );
      return null;
    } finally {
      _submitting = false;
    }
  }

  void reset() => state = const ProjectSubmitState();
}

final class ProjectCreateController extends _ProjectWriteController {
  ProjectCreateController(this._repository, {required super.onUnauthorized});
  final ProjectRepository _repository;

  Future<ProjectDetails?> create(ProjectCreateInput input) =>
      submit(() => _repository.createProject(input));
}

final class ProjectUpdateController extends _ProjectWriteController {
  ProjectUpdateController(this._repository, {required super.onUnauthorized});
  final ProjectRepository _repository;

  Future<ProjectDetails?> update(String id, ProjectUpdateInput input) =>
      submit(() => _repository.updateProject(id, input));
}

final class SupervisorAssignmentController extends _ProjectWriteController {
  SupervisorAssignmentController(
    this._repository, {
    required super.onUnauthorized,
  });
  final ProjectRepository _repository;

  Future<ProjectDetails?> assign(String id, SupervisorAssignmentInput input) =>
      submit(() => _repository.assignSupervisor(id, input));
}

const _sentinel = Object();
