import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/company_member_list_state.dart';
import '../../domain/company_member_models.dart';
import '../../domain/company_member_repository.dart';

const companyMemberPageSize = 20;

final companyMemberListControllerProvider =
    StateNotifierProvider<CompanyMemberListController, CompanyMemberListState>(
      (ref) => CompanyMemberListController(
        ref.watch(companyMemberRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final companyMemberDetailsControllerProvider = StateNotifierProvider.autoDispose
    .family<CompanyMemberDetailsController, CompanyMemberDetailsState, String>(
      (ref, memberId) => CompanyMemberDetailsController(
        ref.watch(companyMemberRepositoryProvider),
        memberId,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class CompanyMemberListController
    extends StateNotifier<CompanyMemberListState> {
  CompanyMemberListController(this._repository, {required this.onUnauthorized})
    : super(const CompanyMemberListState());

  final CompanyMemberRepository _repository;
  final Future<void> Function() onUnauthorized;
  int _replaceGeneration = 0;
  bool _loadingMore = false;
  Timer? _searchTimer;

  Future<void> load() => _replace(CompanyMemberListPhase.loading);
  Future<void> refresh() => _replace(CompanyMemberListPhase.refreshing);

  Future<void> setRole(String? role) async {
    if (state.role == role) return;
    state = state.copyWith(role: role, page: 0, totalPages: 0);
    await load();
  }

  Future<void> setStatus(String? status) async {
    if (state.status == status) return;
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
      state = state.copyWith(
        phase: CompanyMemberListPhase.empty,
        items: const [],
      );
      return;
    }
    _searchTimer = Timer(debounce, load);
  }

  Future<void> _replace(CompanyMemberListPhase phase) async {
    final generation = ++_replaceGeneration;
    state = state.copyWith(phase: phase, error: null, loadMoreError: null);
    try {
      final search = state.search.trim();
      final page = await _repository.listMembers(
        page: 1,
        pageSize: companyMemberPageSize,
        role: state.role,
        status: state.status,
        search: search.length >= 2 ? search : null,
      );
      if (generation != _replaceGeneration) return;
      state = state.copyWith(
        phase: page.items.isEmpty
            ? CompanyMemberListPhase.empty
            : CompanyMemberListPhase.data,
        items: page.items,
        page: page.page,
        totalPages: page.totalPages,
        error: null,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (generation == _replaceGeneration) {
        state = state.copyWith(
          phase: CompanyMemberListPhase.failure,
          error: error,
        );
      }
    }
  }

  Future<void> loadMore() async {
    if (_loadingMore || !state.hasMore || state.items.isEmpty) return;
    _loadingMore = true;
    final generation = _replaceGeneration;
    state = state.copyWith(
      phase: CompanyMemberListPhase.loadingMore,
      loadMoreError: null,
    );
    try {
      final search = state.search.trim();
      final page = await _repository.listMembers(
        page: state.page + 1,
        pageSize: companyMemberPageSize,
        role: state.role,
        status: state.status,
        search: search.length >= 2 ? search : null,
      );
      if (generation != _replaceGeneration) return;
      final byId = {for (final item in state.items) item.id: item};
      for (final item in page.items) {
        byId[item.id] = item;
      }
      state = state.copyWith(
        phase: CompanyMemberListPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      if (generation == _replaceGeneration) {
        state = state.copyWith(
          phase: CompanyMemberListPhase.data,
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

enum CompanyMemberDetailsPhase { initial, loading, data, failure }

final class CompanyMemberDetailsState {
  const CompanyMemberDetailsState({
    this.phase = CompanyMemberDetailsPhase.initial,
    this.member,
    this.error,
  });

  final CompanyMemberDetailsPhase phase;
  final CompanyMemberDetails? member;
  final AppException? error;
}

final class CompanyMemberDetailsController
    extends StateNotifier<CompanyMemberDetailsState> {
  CompanyMemberDetailsController(
    this._repository,
    this.memberId, {
    required this.onUnauthorized,
  }) : super(const CompanyMemberDetailsState());

  final CompanyMemberRepository _repository;
  final String memberId;
  final Future<void> Function() onUnauthorized;
  bool _loading = false;

  Future<void> load() async {
    if (_loading) return;
    _loading = true;
    state = const CompanyMemberDetailsState(
      phase: CompanyMemberDetailsPhase.loading,
    );
    try {
      state = CompanyMemberDetailsState(
        phase: CompanyMemberDetailsPhase.data,
        member: await _repository.getMember(memberId),
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
      state = CompanyMemberDetailsState(
        phase: CompanyMemberDetailsPhase.failure,
        error: error,
      );
    } finally {
      _loading = false;
    }
  }
}
