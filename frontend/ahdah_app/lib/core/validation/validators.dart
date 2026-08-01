import 'package:flutter/services.dart';

abstract final class Validators {
  static final _phone = RegExp(r'^\+[1-9][0-9]{7,14}$');
  static final _companyCode = RegExp(r'^[A-Za-z0-9]{6,20}$');
  static final _email = RegExp(r'^[^\s@]+@[^\s@]+\.[^\s@]+$');
  static final _invitationToken = RegExp(r'^[A-Za-z0-9_-]{32,256}$');

  static String? required(String? value, String message) =>
      value == null || value.trim().isEmpty ? message : null;

  static String? phone(String? value, String required, String invalid) {
    final missing = Validators.required(value, required);
    if (missing != null) return missing;
    return _phone.hasMatch(value!.trim()) ? null : invalid;
  }

  static String? companyCode(String? value, String required, String invalid) {
    final missing = Validators.required(value, required);
    if (missing != null) return missing;
    return _companyCode.hasMatch(value!.trim()) ? null : invalid;
  }

  static String? email(String? value, String invalid) {
    final normalized = value?.trim() ?? '';
    if (normalized.isEmpty) return null;
    return normalized.length <= 254 && _email.hasMatch(normalized)
        ? null
        : invalid;
  }

  static String? onboardingPassword(
    String? value,
    String required,
    String invalid,
  ) {
    final missing = Validators.required(value, required);
    if (missing != null) return missing;
    return value!.length >= 12 && value.length <= 128 ? null : invalid;
  }

  static String? loginPassword(String? value, String message) =>
      value == null || value.isEmpty || value.length > 128 ? message : null;

  static String? name(String? value, String required, String invalid) {
    final normalized = value?.trim() ?? '';
    if (normalized.isEmpty) return required;
    return normalized.length <= 200 &&
            !normalized.contains('\n') &&
            !normalized.contains('\r')
        ? null
        : invalid;
  }

  static String? message(String? value, String invalid) {
    final normalized = value?.trim() ?? '';
    if (normalized.isEmpty) return null;
    return normalized.length <= 1000 &&
            !normalized.contains('\n') &&
            !normalized.contains('\r')
        ? null
        : invalid;
  }

  static String? rejectionReason(
    String? value,
    String required,
    String invalid,
  ) {
    final normalized = value?.trim() ?? '';
    if (normalized.isEmpty) return required;
    return normalized.length <= 500 &&
            !normalized.contains('\n') &&
            !normalized.contains('\r')
        ? null
        : invalid;
  }

  static String? invitationToken(
    String? value,
    String required,
    String invalid,
  ) {
    final missing = Validators.required(value, required);
    if (missing != null) return missing;
    return _invitationToken.hasMatch(value!.trim()) ? null : invalid;
  }

  static String? role(String? value, String message) =>
      const {'Deputy', 'Accountant', 'Supervisor', 'Worker'}.contains(value)
      ? null
      : message;
}

final class UpperCaseTextFormatter extends TextInputFormatter {
  const UpperCaseTextFormatter();

  @override
  TextEditingValue formatEditUpdate(
    TextEditingValue oldValue,
    TextEditingValue newValue,
  ) => newValue.copyWith(text: newValue.text.toUpperCase());
}
