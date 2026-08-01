import 'access_token_store.dart';

final class WebAccessTokenStore implements AccessTokenStore {
  String? _token;

  @override
  Future<String?> read() async => _token;

  @override
  Future<void> write(String token) async {
    final normalized = token.trim();
    _token = normalized.isEmpty ? null : normalized;
  }

  @override
  Future<void> clear() async => _token = null;
}

AccessTokenStore createPlatformTokenStore() => WebAccessTokenStore();
