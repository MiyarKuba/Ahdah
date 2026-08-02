final class CompanyMemberPage {
  const CompanyMemberPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<CompanyMemberSummary> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasMore => page < totalPages;

  factory CompanyMemberPage.fromJson(Map<String, Object?> json) {
    final rawItems = json['items'];
    return CompanyMemberPage(
      items: rawItems is List
          ? rawItems
                .whereType<Map>()
                .map(
                  (item) => CompanyMemberSummary.fromJson(
                    Map<String, Object?>.from(item),
                  ),
                )
                .toList(growable: false)
          : const [],
      page: _integer(json['page'], fallback: 1),
      pageSize: _integer(json['pageSize'], fallback: 20),
      totalCount: _integer(json['totalCount']),
      totalPages: _integer(json['totalPages']),
    );
  }
}

final class CompanyMemberSummary {
  const CompanyMemberSummary({
    required this.id,
    required this.fullName,
    required this.role,
    required this.status,
    required this.identityVerificationStatus,
    this.createdAtUtc,
  });

  final String id;
  final String fullName;
  final String role;
  final String status;
  final String identityVerificationStatus;
  final DateTime? createdAtUtc;

  bool get isEligibleSupervisor => role == 'Supervisor' && status == 'Active';

  factory CompanyMemberSummary.fromJson(Map<String, Object?> json) =>
      CompanyMemberSummary(
        id: _string(json['memberId']),
        fullName: _string(json['fullName']),
        role: _string(json['role']),
        status: _string(json['status']),
        identityVerificationStatus: _string(json['identityVerificationStatus']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class CompanyMemberDetails {
  const CompanyMemberDetails({
    required this.id,
    required this.fullName,
    required this.role,
    required this.status,
    required this.identityVerificationStatus,
    required this.phoneNumber,
    this.email,
    this.createdAtUtc,
    this.updatedAtUtc,
  });

  final String id;
  final String fullName;
  final String role;
  final String status;
  final String identityVerificationStatus;
  final String phoneNumber;
  final String? email;
  final DateTime? createdAtUtc;
  final DateTime? updatedAtUtc;

  factory CompanyMemberDetails.fromJson(Map<String, Object?> json) =>
      CompanyMemberDetails(
        id: _string(json['memberId']),
        fullName: _string(json['fullName']),
        role: _string(json['role']),
        status: _string(json['status']),
        identityVerificationStatus: _string(json['identityVerificationStatus']),
        phoneNumber: _string(json['phoneNumber']),
        email: _optionalString(json['email']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
        updatedAtUtc: _utcDate(json['updatedAtUtc']),
      );
}

DateTime? _utcDate(Object? value) {
  if (value is! String) return null;
  return DateTime.tryParse(value)?.toUtc();
}

String _string(Object? value) => value is String ? value : '';

String? _optionalString(Object? value) {
  if (value is! String || value.trim().isEmpty) return null;
  return value;
}

int _integer(Object? value, {int fallback = 0}) => switch (value) {
  final int number => number,
  final num number => number.toInt(),
  final String text => int.tryParse(text) ?? fallback,
  _ => fallback,
};
