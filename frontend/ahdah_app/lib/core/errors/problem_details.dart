import 'dart:convert';

final class FieldValidationErrors {
  const FieldValidationErrors(this.values);

  final Map<String, List<String>> values;

  String? firstFor(String field) => values[field]?.firstOrNull;

  static FieldValidationErrors fromJson(Object? json) {
    if (json is! Map) return const FieldValidationErrors({});
    final result = <String, List<String>>{};
    for (final entry in json.entries) {
      final rawKey = entry.key.toString();
      if (rawKey.isEmpty) continue;
      final key = '${rawKey[0].toLowerCase()}${rawKey.substring(1)}';
      final value = entry.value;
      if (value is List) {
        final messages = value.whereType<String>().toList(growable: false);
        if (messages.isNotEmpty) result[key] = messages;
      } else if (value is String && value.isNotEmpty) {
        result[key] = [value];
      }
    }
    return FieldValidationErrors(Map.unmodifiable(result));
  }
}

final class ProblemDetails {
  const ProblemDetails({
    this.type,
    this.title,
    this.status,
    this.detail,
    this.instance,
    this.code,
    this.traceId,
    this.fieldErrors = const FieldValidationErrors({}),
  });

  final String? type;
  final String? title;
  final int? status;
  final String? detail;
  final String? instance;
  final String? code;
  final String? traceId;
  final FieldValidationErrors fieldErrors;

  factory ProblemDetails.fromJson(Object? json) {
    if (json is String) {
      try {
        return ProblemDetails.fromJson(jsonDecode(json));
      } on FormatException {
        return const ProblemDetails();
      }
    }
    if (json is! Map) return const ProblemDetails();
    final map = Map<String, Object?>.from(json);
    return ProblemDetails(
      type: map['type'] as String?,
      title: map['title'] as String?,
      status: switch (map['status']) {
        final int value => value,
        final String value => int.tryParse(value),
        _ => null,
      },
      detail: map['detail'] as String?,
      instance: map['instance'] as String?,
      code: map['code'] as String?,
      traceId: map['traceId'] as String?,
      fieldErrors: FieldValidationErrors.fromJson(map['errors']),
    );
  }

  @override
  String toString() =>
      'ProblemDetails(status: $status, code: $code, hasFieldErrors: ${fieldErrors.values.isNotEmpty})';
}
