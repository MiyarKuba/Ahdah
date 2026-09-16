import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/settlement_models.dart';
import '../../domain/settlement_repository.dart';

final class SettlementState {
  const SettlementState({this.value, this.loading = false, this.error});
  final ProjectSettlement? value;
  final bool loading;
  final AppException? error;
}

final settlementControllerProvider = StateNotifierProvider.autoDispose
    .family<SettlementController, SettlementState, String>((ref, id) {
      ref.watch(sessionControllerProvider);
      final controller = SettlementController(
        ref.watch(settlementRepositoryProvider),
        id,
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      );
      Future.microtask(controller.load);
      return controller;
    });

final class SettlementController extends StateNotifier<SettlementState> {
  SettlementController(
    this._repository,
    this.projectId, {
    required this.onUnauthorized,
  }) : super(const SettlementState(loading: true));
  final SettlementRepository _repository;
  final String projectId;
  final Future<void> Function() onUnauthorized;
  bool _busy = false;

  Future<void> load() async {
    if (_busy || !mounted) return;
    _busy = true;
    state = SettlementState(value: state.value, loading: true);
    try {
      final value = await _repository.getProjectSettlement(projectId);
      if (!mounted) return;
      if (value.projectId != projectId) {
        throw const AppException(AppExceptionKind.server);
      }
      state = SettlementState(value: value);
    } on AppException catch (error) {
      if (!mounted) return;
      // Do not retain a stale financial snapshot after a failed refresh or lost access.
      state = SettlementState(error: error);
      if (error.kind == AppExceptionKind.unauthorized) await onUnauthorized();
    } catch (_) {
      if (mounted) {
        state = const SettlementState(
          error: AppException(AppExceptionKind.server),
        );
      }
    } finally {
      _busy = false;
    }
  }
}
