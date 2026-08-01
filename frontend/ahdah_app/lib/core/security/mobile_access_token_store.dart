import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import 'access_token_store.dart';

const _tokenKey = 'ahdah.access_token';

final class MobileAccessTokenStore implements AccessTokenStore {
  MobileAccessTokenStore([FlutterSecureStorage? storage])
    : _storage = storage ?? const FlutterSecureStorage();

  final FlutterSecureStorage _storage;

  @override
  Future<String?> read() => _storage.read(key: _tokenKey);

  @override
  Future<void> write(String token) async {
    final normalized = token.trim();
    if (normalized.isEmpty) {
      await clear();
      return;
    }
    await _storage.write(key: _tokenKey, value: normalized);
  }

  @override
  Future<void> clear() => _storage.delete(key: _tokenKey);
}

AccessTokenStore createPlatformTokenStore() => MobileAccessTokenStore();
