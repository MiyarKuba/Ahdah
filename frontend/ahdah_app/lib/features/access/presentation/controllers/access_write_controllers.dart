import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/access_list_state.dart';
import '../../domain/access_models.dart';
import '../../domain/access_repository.dart';

final invitationCreationControllerProvider =
    StateNotifierProvider<InvitationCreationController, AccessSubmitState>(
      (ref) => InvitationCreationController(
        ref.watch(accessRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final invitationCancellationControllerProvider =
    StateNotifierProvider<InvitationCancellationController, AccessSubmitState>(
      (ref) => InvitationCancellationController(
        ref.watch(accessRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final joinApprovalControllerProvider =
    StateNotifierProvider<JoinApprovalController, AccessSubmitState>(
      (ref) => JoinApprovalController(
        ref.watch(accessRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

final joinRejectionControllerProvider =
    StateNotifierProvider<JoinRejectionController, AccessSubmitState>(
      (ref) => JoinRejectionController(
        ref.watch(accessRepositoryProvider),
        onUnauthorized: ref.read(sessionControllerProvider.notifier).expire,
      ),
    );

abstract base class _WriteController extends StateNotifier<AccessSubmitState> {
  _WriteController({required this.onUnauthorized})
    : super(const AccessSubmitState());

  final Future<void> Function() onUnauthorized;
  bool _submitting = false;

  Future<T?> submit<T>(Future<T> Function() operation) async {
    if (_submitting) return null;
    _submitting = true;
    state = const AccessSubmitState(phase: AccessSubmitPhase.submitting);
    try {
      final result = await operation();
      state = const AccessSubmitState(phase: AccessSubmitPhase.success);
      return result;
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) {
        await onUnauthorized();
      }
      state = AccessSubmitState(phase: AccessSubmitPhase.failure, error: error);
      return null;
    } finally {
      _submitting = false;
    }
  }

  void reset() => state = const AccessSubmitState();
}

final class InvitationCreationController extends _WriteController {
  InvitationCreationController(
    this._repository, {
    required super.onUnauthorized,
  });
  final AccessRepository _repository;

  Future<CreatedInvitation?> create(CreateInvitationInput input) =>
      submit(() => _repository.createInvitation(input));
}

final class InvitationCancellationController extends _WriteController {
  InvitationCancellationController(
    this._repository, {
    required super.onUnauthorized,
  });
  final AccessRepository _repository;

  Future<Invitation?> cancel(String id) =>
      submit(() => _repository.cancelInvitation(id));
}

final class JoinApprovalController extends _WriteController {
  JoinApprovalController(this._repository, {required super.onUnauthorized});
  final AccessRepository _repository;

  Future<JoinRequestDecision?> approve(
    String id,
    ApproveJoinRequestInput input,
  ) => submit(() => _repository.approveJoinRequest(id, input));
}

final class JoinRejectionController extends _WriteController {
  JoinRejectionController(this._repository, {required super.onUnauthorized});
  final AccessRepository _repository;

  Future<JoinRequestDecision?> reject(
    String id,
    RejectJoinRequestInput input,
  ) => submit(() => _repository.rejectJoinRequest(id, input));
}
