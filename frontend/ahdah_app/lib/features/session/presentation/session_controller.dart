import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../app/providers.dart';
import '../../../core/errors/app_exception.dart';
import '../../../core/security/access_token_store.dart';
import '../../authentication/data/auth_repository.dart';
import '../../authentication/domain/identity_models.dart';

enum SessionStatus {
  bootstrapping,
  unauthenticated,
  authenticated,
  temporarilyUnavailable,
  sessionExpired,
}

final class SessionState {
  const SessionState._(this.status, {this.current, this.error});

  const SessionState.bootstrapping() : this._(SessionStatus.bootstrapping);
  const SessionState.unauthenticated() : this._(SessionStatus.unauthenticated);
  const SessionState.authenticated(CurrentSessionResult current)
    : this._(SessionStatus.authenticated, current: current);
  const SessionState.temporarilyUnavailable(AppException error)
    : this._(SessionStatus.temporarilyUnavailable, error: error);
  const SessionState.sessionExpired() : this._(SessionStatus.sessionExpired);

  final SessionStatus status;
  final CurrentSessionResult? current;
  final AppException? error;
}

final sessionControllerProvider =
    StateNotifierProvider<SessionController, SessionState>((ref) {
      return SessionController(
        ref.watch(authRepositoryProvider),
        ref.watch(accessTokenStoreProvider),
      );
    });

final class SessionController extends StateNotifier<SessionState> {
  SessionController(this._repository, this._tokenStore)
    : super(const SessionState.bootstrapping());

  final AuthRepository _repository;
  final AccessTokenStore _tokenStore;
  bool _bootstrapStarted = false;

  Future<void> bootstrap({bool force = false}) async {
    if (_bootstrapStarted && !force) return;
    _bootstrapStarted = true;
    state = const SessionState.bootstrapping();

    final token = (await _tokenStore.read())?.trim();
    if (token == null || token.isEmpty) {
      state = const SessionState.unauthenticated();
      return;
    }

    await _loadAuthoritativeSession();
  }

  Future<void> establish(AuthenticationResult authentication) async {
    await _tokenStore.write(authentication.accessToken);
    await _loadAuthoritativeSession();
  }

  Future<void> _loadAuthoritativeSession() async {
    try {
      final current = await _repository.currentSession();
      state = SessionState.authenticated(current);
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) {
        await _tokenStore.clear();
        state = const SessionState.sessionExpired();
      } else if (error.kind == AppExceptionKind.network ||
          error.kind == AppExceptionKind.timeout) {
        state = SessionState.temporarilyUnavailable(error);
      } else {
        state = SessionState.temporarilyUnavailable(error);
      }
    }
  }

  Future<void> retry() => bootstrap(force: true);

  Future<void> logout() async {
    await _tokenStore.clear();
    state = const SessionState.unauthenticated();
  }
}
