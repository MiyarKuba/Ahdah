import 'package:dio/dio.dart';

import '../security/access_token_store.dart';

const requiresAuthenticationKey = 'requiresAuthentication';

final class AuthInterceptor extends Interceptor {
  AuthInterceptor(this._tokenStore);

  final AccessTokenStore _tokenStore;

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    if (options.extra[requiresAuthenticationKey] == true) {
      final token = (await _tokenStore.read())?.trim();
      if (token != null && token.isNotEmpty) {
        options.headers['Authorization'] = 'Bearer $token';
      }
    }
    handler.next(options);
  }
}
