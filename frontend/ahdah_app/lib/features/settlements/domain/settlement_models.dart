import '../../advances/domain/advance_models.dart' show decimalText;

final class ProjectSettlement {
  ProjectSettlement.fromJson(Map<String, Object?> json)
    : projectId = _text(json['projectId']),
      projectStatus = _text(json['projectStatus']),
      projectVersion = _count(json['projectVersion']),
      evaluatedAt = DateTime.tryParse(_text(json['evaluatedAt']))?.toUtc(),
      visibilityScope = _text(json['visibilityScope']),
      hasKnownFinancialBlockers = json['hasKnownFinancialBlockers'] as bool,
      canSettle = json['canSettle'] as bool?,
      canClose = json['canClose'] as bool?,
      settlementReadiness = _text(json['settlementReadiness']),
      closureReadiness = _text(json['closureReadiness']),
      totalBlockerCount = _count(json['totalBlockerCount']),
      categories = _list(json['categories'], SettlementCategory.fromJson),
      evaluationGaps = _list(json['evaluationGaps'], SettlementGap.fromJson),
      settlementImpediments = List.unmodifiable(
        (json['settlementImpediments'] as List).cast<String>(),
      ),
      closureImpediments = List.unmodifiable(
        (json['closureImpediments'] as List).cast<String>(),
      );

  final String projectId;
  final String projectStatus;
  final int projectVersion;
  final DateTime? evaluatedAt;
  final String visibilityScope;
  final bool hasKnownFinancialBlockers;
  final bool? canSettle;
  final bool? canClose;
  final String settlementReadiness;
  final String closureReadiness;
  final int totalBlockerCount;
  final List<SettlementCategory> categories;
  final List<SettlementGap> evaluationGaps;
  final List<String> settlementImpediments;
  final List<String> closureImpediments;
}

final class SettlementCategory {
  SettlementCategory.fromJson(Map<String, Object?> json)
    : category = _text(json['category']),
      evaluationStatus = _text(json['evaluationStatus']),
      amountMeaning = _text(json['amountMeaning']),
      blockerCount = _count(json['blockerCount']),
      totals = _list(json['totals'], SettlementCurrencyTotal.fromJson),
      blockers = _list(json['blockers'], SettlementBlocker.fromJson);

  final String category;
  final String evaluationStatus;
  final String amountMeaning;
  final int blockerCount;
  final List<SettlementCurrencyTotal> totals;
  final List<SettlementBlocker> blockers;
}

final class SettlementCurrencyTotal {
  SettlementCurrencyTotal.fromJson(Map<String, Object?> json)
    : currencyCode = _text(json['currencyCode']),
      amount = _money(json['amount']);
  final String currencyCode;
  final String amount;
}

final class SettlementBlocker {
  SettlementBlocker.fromJson(Map<String, Object?> json)
    : category = _text(json['category']),
      code = _text(json['code']),
      recordType = _text(json['recordType']),
      recordId = _text(json['recordId']),
      status = _text(json['status']),
      currencyCode = json['currencyCode'] as String?,
      amount = json['amount'] == null ? null : _money(json['amount']),
      resourcePath = json['resourcePath'] as String?;
  final String category;
  final String code;
  final String recordType;
  final String recordId;
  final String status;
  final String? currencyCode;
  final String? amount;
  // API reference only. Never interpreted as a client route or external URL.
  final String? resourcePath;
}

final class SettlementGap {
  SettlementGap.fromJson(Map<String, Object?> json)
    : category = _text(json['category']),
      code = _text(json['code']);
  final String category;
  final String code;
}

String _text(Object? value) => value as String;
int _count(Object? value) {
  if (value is! int || value < 0) throw const FormatException('Invalid count');
  return value;
}

String _money(Object? value) {
  // The shared financial transport protects JSON number tokens before decoding.
  if (value is! String && value is! int) {
    throw const FormatException('Invalid decimal');
  }
  final text = decimalText(value);
  if (!RegExp(r'^-?\d+\.\d{2}$').hasMatch(text)) {
    throw const FormatException('Invalid decimal');
  }
  return text;
}

List<T> _list<T>(Object? value, T Function(Map<String, Object?>) parse) =>
    List.unmodifiable(
      (value as List).map(
        (item) => parse(Map<String, Object?>.from(item as Map)),
      ),
    );
