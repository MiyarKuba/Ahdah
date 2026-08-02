final class ProjectPage {
  const ProjectPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<ProjectDetails> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasMore => page < totalPages;

  factory ProjectPage.fromJson(Map<String, Object?> json) {
    final rawItems = json['items'];
    return ProjectPage(
      items: rawItems is List
          ? rawItems
                .whereType<Map>()
                .map(
                  (item) =>
                      ProjectDetails.fromJson(Map<String, Object?>.from(item)),
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

final class ProjectOwnerSummary {
  const ProjectOwnerSummary({
    required this.id,
    required this.ownerName,
    required this.phoneNumber,
    this.email,
    this.address,
  });

  final String id;
  final String ownerName;
  final String phoneNumber;
  final String? email;
  final String? address;

  factory ProjectOwnerSummary.fromJson(Map<String, Object?> json) =>
      ProjectOwnerSummary(
        id: _string(json['projectOwnerId']),
        ownerName: _string(json['ownerName']),
        phoneNumber: _string(json['phoneNumber']),
        email: _optionalString(json['email']),
        address: _optionalString(json['address']),
      );
}

final class ProjectSupervisorSummary {
  const ProjectSupervisorSummary({
    required this.userId,
    required this.fullName,
    required this.status,
    this.assignedAtUtc,
  });

  final String userId;
  final String fullName;
  final String status;
  final DateTime? assignedAtUtc;

  factory ProjectSupervisorSummary.fromJson(Map<String, Object?> json) =>
      ProjectSupervisorSummary(
        userId: _string(json['userId']),
        fullName: _string(json['fullName']),
        status: _string(json['status']),
        assignedAtUtc: _utcDate(json['assignedAtUtc']),
      );
}

final class ProjectDetails {
  const ProjectDetails({
    required this.id,
    required this.projectName,
    required this.owner,
    required this.siteAddress,
    required this.contractDate,
    required this.startDate,
    required this.status,
    required this.assignedSupervisors,
    required this.versionNumber,
    this.latitude,
    this.longitude,
    this.contactPhoneNumber,
    this.contractValue,
    this.expectedEndDate,
    this.actualEndDate,
    this.description,
    this.notes,
    this.createdAtUtc,
    this.updatedAtUtc,
  });

  final String id;
  final String projectName;
  final ProjectOwnerSummary owner;
  final String siteAddress;
  final String? latitude;
  final String? longitude;
  final String? contactPhoneNumber;
  final String? contractValue;
  final DateTime? contractDate;
  final DateTime? startDate;
  final DateTime? expectedEndDate;
  final DateTime? actualEndDate;
  final String status;
  final String? description;
  final String? notes;
  final List<ProjectSupervisorSummary> assignedSupervisors;
  final int versionNumber;
  final DateTime? createdAtUtc;
  final DateTime? updatedAtUtc;

  factory ProjectDetails.fromJson(Map<String, Object?> json) {
    final rawOwner = json['owner'];
    final rawSupervisors = json['assignedSupervisors'];
    return ProjectDetails(
      id: _string(json['projectId']),
      projectName: _string(json['projectName']),
      owner: ProjectOwnerSummary.fromJson(
        rawOwner is Map ? Map<String, Object?>.from(rawOwner) : const {},
      ),
      siteAddress: _string(json['siteAddress']),
      latitude: _decimalText(json['latitude']),
      longitude: _decimalText(json['longitude']),
      contactPhoneNumber: _optionalString(json['contactPhoneNumber']),
      contractValue: _decimalText(json['contractValue']),
      contractDate: _dateOnly(json['contractDate']),
      startDate: _dateOnly(json['startDate']),
      expectedEndDate: _dateOnly(json['expectedEndDate']),
      actualEndDate: _dateOnly(json['actualEndDate']),
      status: _string(json['status']),
      description: _optionalString(json['description']),
      notes: _optionalString(json['notes']),
      assignedSupervisors: rawSupervisors is List
          ? rawSupervisors
                .whereType<Map>()
                .map(
                  (item) => ProjectSupervisorSummary.fromJson(
                    Map<String, Object?>.from(item),
                  ),
                )
                .toList(growable: false)
          : const [],
      versionNumber: _integer(json['versionNumber']),
      createdAtUtc: _utcDate(json['createdAtUtc']),
      updatedAtUtc: _utcDate(json['updatedAtUtc']),
    );
  }
}

final class ProjectMemberPage {
  const ProjectMemberPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<ProjectMemberSummary> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasMore => page < totalPages;

  factory ProjectMemberPage.fromJson(Map<String, Object?> json) {
    final rawItems = json['items'];
    return ProjectMemberPage(
      items: rawItems is List
          ? rawItems
                .whereType<Map>()
                .map(
                  (item) => ProjectMemberSummary.fromJson(
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

final class ProjectMemberSummary {
  const ProjectMemberSummary({
    required this.id,
    required this.fullName,
    required this.role,
    required this.status,
    required this.identityVerificationStatus,
    required this.phoneNumber,
    this.email,
    this.assignedAtUtc,
  });

  final String id;
  final String fullName;
  final String role;
  final String status;
  final String identityVerificationStatus;
  final String phoneNumber;
  final String? email;
  final DateTime? assignedAtUtc;

  factory ProjectMemberSummary.fromJson(Map<String, Object?> json) =>
      ProjectMemberSummary(
        id: _string(json['memberId']),
        fullName: _string(json['fullName']),
        role: _string(json['role']),
        status: _string(json['status']),
        identityVerificationStatus: _string(json['identityVerificationStatus']),
        phoneNumber: _string(json['phoneNumber']),
        email: _optionalString(json['email']),
        assignedAtUtc: _utcDate(json['assignedAtUtc']),
      );
}

DateTime? _utcDate(Object? value) {
  if (value is! String) return null;
  return DateTime.tryParse(value)?.toUtc();
}

DateTime? _dateOnly(Object? value) {
  if (value is! String) return null;
  final parsed = DateTime.tryParse(value);
  return parsed == null
      ? null
      : DateTime(parsed.year, parsed.month, parsed.day);
}

String _string(Object? value) => value is String ? value : '';

String? _optionalString(Object? value) {
  if (value is! String || value.trim().isEmpty) return null;
  return value;
}

String? _decimalText(Object? value) => switch (value) {
  final String text when text.trim().isNotEmpty => text.trim(),
  final int number => number.toString(),
  final num number => number.toString(),
  _ => null,
};

int _integer(Object? value, {int fallback = 0}) => switch (value) {
  final int number => number,
  final num number => number.toInt(),
  final String text => int.tryParse(text) ?? fallback,
  _ => fallback,
};
