import 'package:ahdah_app/core/config/app_config.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('AppConfig', () {
    test('requires API_BASE_URL', () {
      expect(() => AppConfig.parse(''), throwsA(isA<AppConfigException>()));
    });

    test('normalizes trailing slashes', () {
      final config = AppConfig.parse('http://localhost:5231///');
      expect(config.apiBaseUrl, 'http://localhost:5231');
    });

    test('accepts documented local origins', () {
      expect(AppConfig.parse('http://localhost:5231').apiBaseUri.port, 5231);
      expect(
        AppConfig.parse('http://10.0.2.2:5231').apiBaseUri.host,
        '10.0.2.2',
      );
    });

    test('rejects invalid and insecure production URLs safely', () {
      expect(
        () => AppConfig.parse('not a url'),
        throwsA(isA<AppConfigException>()),
      );
      expect(
        () => AppConfig.parse('http://api.example.com', isRelease: true),
        throwsA(isA<AppConfigException>()),
      );
      expect(
        () => AppConfig.parse('http://10.0.2.2:5231', isRelease: true),
        throwsA(isA<AppConfigException>()),
      );
    });
  });
}
