import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/access_list_state.dart';
import '../../domain/access_models.dart';
import '../../domain/access_repository.dart';

const accessPageSize = 20;

final invitationListControllerProvider =
    StateNotifierProvider<
      InvitationListController,
      AccessListState<Invitation>
    >(
      (ref) => InvitationListController(
        ref.watch(accessRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final joinRequestListControllerProvider =
    StateNotifierProvider<
      JoinRequestListController,
      AccessListState<JoinRequest>
    >(
      (ref) => JoinRequestListController(
        ref.watch(accessRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final class InvitationListController
    extends StateNotifier<AccessListState<Invitation>> {
  InvitationListController(this._repository, {required this.onUnauthorized})
    : super(const AccessListState());

  final AccessRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _requestInFlight = false;
  int _requestGeneration = 0;

  Future<void> load() => _replace(AccessListPhase.loading);
  Future<void> refresh() => _replace(AccessListPhase.refreshing);

  Future<void> setFilter(String? filter) async {
    if (state.filter == filter) return;
    state = AccessListState(filter: filter);
    await load();
  }

  Future<void> _replace(AccessListPhase phase) async {
    if (_requestInFlight) return;
    _requestInFlight = true;
    final generation = ++_requestGeneration;
    state = state.copyWith(phase: phase, error: null, loadMoreError: null);
    try {
      final page = await _repository.listInvitations(
        page: 1,
        pageSize: accessPageSize,
        status: state.filter,
      );
      if (generation != _requestGeneration) return;
      state = state.copyWith(
        phase: page.items.isEmpty
            ? AccessListPhase.empty
            : AccessListPhase.data,
        items: page.items,
        page: page.page,
        totalPages: page.totalPages,
        error: null,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) {
        await onUnauthorized();
      }
      if (generation == _requestGeneration) {
        state = state.copyWith(phase: AccessListPhase.failure, error: error);
      }
    } finally {
      _requestInFlight = false;
    }
  }

  Future<void> loadMore() async {
    if (_requestInFlight || !state.hasMore || state.items.isEmpty) return;
    _requestInFlight = true;
    state = state.copyWith(
      phase: AccessListPhase.loadingMore,
      loadMoreError: null,
    );
    try {
      final page = await _repository.listInvitations(
        page: state.page + 1,
        pageSize: accessPageSize,
        status: state.filter,
      );
      final byId = {for (final item in state.items) item.id: item};
      for (final item in page.items) {
        byId[item.id] = item;
      }
      state = state.copyWith(
        phase: AccessListPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) {
        await onUnauthorized();
      }
      state = state.copyWith(phase: AccessListPhase.data, loadMoreError: error);
    } finally {
      _requestInFlight = false;
    }
  }
}

final class JoinRequestListController
    extends StateNotifier<AccessListState<JoinRequest>> {
  JoinRequestListController(this._repository, {required this.onUnauthorized})
    : super(const AccessListState());

  final AccessRepository _repository;
  final Future<void> Function() onUnauthorized;
  bool _requestInFlight = false;
  int _requestGeneration = 0;

  Future<void> load() => _replace(AccessListPhase.loading);
  Future<void> refresh() => _replace(AccessListPhase.refreshing);

  Future<void> setFilter(String? filter) async {
    if (state.filter == filter) return;
    state = AccessListState(filter: filter);
    await load();
  }

  Future<void> _replace(AccessListPhase phase) async {
    if (_requestInFlight) return;
    _requestInFlight = true;
    final generation = ++_requestGeneration;
    state = state.copyWith(phase: phase, error: null, loadMoreError: null);
    try {
      final page = await _repository.listJoinRequests(
        page: 1,
        pageSize: accessPageSize,
        status: state.filter,
      );
      if (generation != _requestGeneration) return;
      state = state.copyWith(
        phase: page.items.isEmpty
            ? AccessListPhase.empty
            : AccessListPhase.data,
        items: page.items,
        page: page.page,
        totalPages: page.totalPages,
        error: null,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) {
        await onUnauthorized();
      }
      if (generation == _requestGeneration) {
        state = state.copyWith(phase: AccessListPhase.failure, error: error);
      }
    } finally {
      _requestInFlight = false;
    }
  }

  Future<void> loadMore() async {
    if (_requestInFlight || !state.hasMore || state.items.isEmpty) return;
    _requestInFlight = true;
    state = state.copyWith(
      phase: AccessListPhase.loadingMore,
      loadMoreError: null,
    );
    try {
      final page = await _repository.listJoinRequests(
        page: state.page + 1,
        pageSize: accessPageSize,
        status: state.filter,
      );
      final byId = {for (final item in state.items) item.id: item};
      for (final item in page.items) {
        byId[item.id] = item;
      }
      state = state.copyWith(
        phase: AccessListPhase.data,
        items: byId.values.toList(growable: false),
        page: page.page,
        totalPages: page.totalPages,
      );
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) {
        await onUnauthorized();
      }
      state = state.copyWith(phase: AccessListPhase.data, loadMoreError: error);
    } finally {
      _requestInFlight = false;
    }
  }
}
