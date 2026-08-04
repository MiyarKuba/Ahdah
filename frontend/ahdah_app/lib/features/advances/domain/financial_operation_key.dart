import 'dart:convert';
import 'dart:math';

abstract interface class FinancialOperationKeyFactory {
  String create();
}

final class SecureFinancialOperationKeyFactory
    implements FinancialOperationKeyFactory {
  SecureFinancialOperationKeyFactory({Random? random})
    : _random = random ?? Random.secure();

  final Random _random;

  @override
  String create() {
    final bytes = List<int>.generate(32, (_) => _random.nextInt(256));
    return base64UrlEncode(bytes).replaceAll('=', '');
  }
}

final class FinancialOperationKeySession {
  FinancialOperationKeySession(this._factory);

  final FinancialOperationKeyFactory _factory;
  String? _key;
  String? _payloadFingerprint;

  bool get hasPendingOperation => _key != null;

  String keyFor(String payloadFingerprint) {
    if (_key == null || _payloadFingerprint != payloadFingerprint) {
      _key = _factory.create();
      _payloadFingerprint = payloadFingerprint;
    }
    return _key!;
  }

  bool matches(String payloadFingerprint) =>
      _key != null && _payloadFingerprint == payloadFingerprint;

  void clear() {
    _key = null;
    _payloadFingerprint = null;
  }
}
