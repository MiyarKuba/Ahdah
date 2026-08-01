import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/session/presentation/session_controller.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

void main() {
  test('no token becomes unauthenticated', () async {
    final store = FakeAccessTokenStore();
    final repository = FakeAuthRepository();
    final controller = SessionController(repository, store);

    await controller.bootstrap();

    expect(controller.state.status, SessionStatus.unauthenticated);
    expect(repository.currentCalls, 0);
  });

  test('valid token and me result becomes authenticated', () async {
    final store = FakeAccessTokenStore('valid-token');
    final repository = FakeAuthRepository();
    final controller = SessionController(repository, store);

    await controller.bootstrap();

    expect(controller.state.status, SessionStatus.authenticated);
    expect(controller.state.current, testCurrentSession);
  });

  test('401 clears token and becomes session expired', () async {
    final store = FakeAccessTokenStore('stale-token');
    final repository = FakeAuthRepository()
      ..currentError = const AppException(AppExceptionKind.unauthorized);
    final controller = SessionController(repository, store);

    await controller.bootstrap();

    expect(controller.state.status, SessionStatus.sessionExpired);
    expect(store.token, isNull);
    expect(store.clearCalls, 1);
  });

  test(
    'network failure keeps potentially valid token and allows retry',
    () async {
      final store = FakeAccessTokenStore('potentially-valid-token');
      final repository = FakeAuthRepository()
        ..currentError = const AppException(AppExceptionKind.network);
      final controller = SessionController(repository, store);

      await controller.bootstrap();

      expect(controller.state.status, SessionStatus.temporarilyUnavailable);
      expect(store.token, 'potentially-valid-token');
      expect(store.clearCalls, 0);
    },
  );

  test('establish stores token then verifies me', () async {
    final store = FakeAccessTokenStore();
    final repository = FakeAuthRepository();
    final controller = SessionController(repository, store);

    await controller.establish(testAuthentication);

    expect(store.token, 'test-token-never-logged');
    expect(controller.state.status, SessionStatus.authenticated);
    expect(repository.currentCalls, 1);
  });

  test('logout clears token and in-memory session', () async {
    final store = FakeAccessTokenStore('valid-token');
    final controller = SessionController(FakeAuthRepository(), store);
    await controller.bootstrap();

    await controller.logout();

    expect(store.token, isNull);
    expect(controller.state.status, SessionStatus.unauthenticated);
    expect(controller.state.current, isNull);
  });
}
