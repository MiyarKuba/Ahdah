import 'package:flutter/foundation.dart';

final class AppConfig {
  AppConfig._(this.apiBaseUri);

  static const _environmentValue = String.fromEnvironment('API_BASE_URL');

  final Uri apiBaseUri;

  String get apiBaseUrl => apiBaseUri.toString();

  static AppConfig fromEnvironment() =>
      parse(_environmentValue, isRelease: kReleaseMode);

  @visibleForTesting
  static AppConfig parse(String value, {bool isRelease = false}) {
    final normalized = value.trim().replaceFirst(RegExp(r'/+$'), '');
    if (normalized.isEmpty) {
      throw const AppConfigException('API_BASE_URL is required.');
    }

    final uri = Uri.tryParse(normalized);
    final validScheme = uri?.scheme == 'http' || uri?.scheme == 'https';
    final isAbsoluteHost =
        uri != null && uri.hasAuthority && uri.host.isNotEmpty;
    final hasUnsupportedParts =
        uri != null &&
        (uri.userInfo.isNotEmpty || uri.hasQuery || uri.hasFragment);
    if (!validScheme || !isAbsoluteHost || hasUnsupportedParts) {
      throw const AppConfigException(
        'API_BASE_URL must be an absolute HTTP(S) origin without credentials, query, or fragment.',
      );
    }

    final isLoopback =
        uri.host == 'localhost' || uri.host == '127.0.0.1' || uri.host == '::1';
    if (isRelease && uri.scheme != 'https' && !isLoopback) {
      throw const AppConfigException(
        'Production API_BASE_URL values must use HTTPS.',
      );
    }

    return AppConfig._(uri);
  }
}

final class AppConfigException implements Exception {
  const AppConfigException(this.message);

  final String message;

  @override
  String toString() => 'AppConfigException: $message';
}
