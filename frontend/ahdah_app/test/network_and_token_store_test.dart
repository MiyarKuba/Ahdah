import 'package:ahdah_app/core/network/auth_interceptor.dart';
import 'package:ahdah_app/core/security/web_access_token_store.dart';
import 'package:ahdah_app/features/authentication/domain/identity_requests.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

void main() {
  test('Authorization header is attached only when token exists', () async {
    final store = FakeAccessTokenStore('  access-token  ');
    final adapter = RecordingAdapter();
    final dio = Dio(BaseOptions(baseUrl: 'http://localhost:5231'))
      ..httpClientAdapter = adapter
      ..interceptors.add(AuthInterceptor(store));

    await dio.get<void>(
      '/private',
      options: Options(extra: const {requiresAuthenticationKey: true}),
    );

    expect(
      adapter.lastRequest!.headers['Authorization'],
      'Bearer access-token',
    );
  });

  test(
    'Authorization header is absent for public request and empty token',
    () async {
      final store = FakeAccessTokenStore('');
      final adapter = RecordingAdapter();
      final dio = Dio(BaseOptions(baseUrl: 'http://localhost:5231'))
        ..httpClientAdapter = adapter
        ..interceptors.add(AuthInterceptor(store));

      await dio.get<void>('/public');

      expect(adapter.lastRequest!.headers, isNot(contains('Authorization')));
    },
  );

  test(
    'web token store is memory-only and clears on logout contract',
    () async {
      final firstRuntime = WebAccessTokenStore();
      await firstRuntime.write('runtime-token');
      expect(await firstRuntime.read(), 'runtime-token');

      final refreshedRuntime = WebAccessTokenStore();
      expect(await refreshedRuntime.read(), isNull);

      await firstRuntime.clear();
      expect(await firstRuntime.read(), isNull);
    },
  );

  test('password and invitation token are absent from diagnostics', () {
    const password = 'not-for-diagnostics';
    const token = 'abcdefghijklmnopqrstuvwxyz_123456789';
    const login = LoginRequest(
      phoneNumber: '+218912345678',
      password: password,
    );
    const invitation = AcceptInvitationRequest(
      token: token,
      fullName: 'Test User',
      password: password,
      email: null,
    );

    expect(login.toString(), isNot(contains(password)));
    expect(invitation.toString(), isNot(contains(password)));
    expect(invitation.toString(), isNot(contains(token)));
  });
}
