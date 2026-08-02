import 'dart:convert';

final class NewProjectOwnerInput {
  const NewProjectOwnerInput({
    required this.ownerName,
    required this.phoneNumber,
    this.email,
    this.address,
    this.notes,
  });

  final String ownerName;
  final String phoneNumber;
  final String? email;
  final String? address;
  final String? notes;

  Map<String, Object?> toJson() => {
    'ownerName': ownerName.trim(),
    'phoneNumber': phoneNumber.trim(),
    'email': _normalizedOptional(email),
    'address': _normalizedOptional(address),
    'notes': _normalizedOptional(notes),
  };
}

final class ProjectCreateInput {
  const ProjectCreateInput({
    required this.projectName,
    required this.siteAddress,
    required this.contractValue,
    required this.contractDate,
    required this.startDate,
    required this.newOwner,
    this.contactPhoneNumber,
    this.expectedEndDate,
    this.description,
    this.notes,
    this.supervisorUserId,
  });

  final String projectName;
  final String siteAddress;
  final String? contactPhoneNumber;
  final String contractValue;
  final DateTime contractDate;
  final DateTime startDate;
  final DateTime? expectedEndDate;
  final String? description;
  final String? notes;
  final NewProjectOwnerInput newOwner;
  final String? supervisorUserId;

  Map<String, Object?> toJson() => {
    'projectName': projectName.trim(),
    'siteAddress': siteAddress.trim(),
    'contactPhoneNumber': _normalizedOptional(contactPhoneNumber),
    'contractValue': contractValue.trim(),
    'contractDate': _date(contractDate),
    'startDate': _date(startDate),
    'expectedEndDate': expectedEndDate == null ? null : _date(expectedEndDate!),
    'description': _normalizedOptional(description),
    'notes': _normalizedOptional(notes),
    'newOwner': newOwner.toJson(),
    'supervisorUserId': _normalizedOptional(supervisorUserId),
  };

  String toJsonBody() {
    final marker = '__AHDAH_VALIDATED_DECIMAL__';
    final body = toJson()..['contractValue'] = marker;
    return jsonEncode(body).replaceFirst('"$marker"', contractValue.trim());
  }
}

final class ProjectUpdateInput {
  const ProjectUpdateInput({
    required this.expectedVersion,
    this.projectName,
    this.siteAddress,
    this.contactPhoneNumber,
    this.contractDate,
    this.startDate,
    this.expectedEndDate,
    this.status,
    this.description,
    this.notes,
  });

  final int expectedVersion;
  final String? projectName;
  final String? siteAddress;
  final String? contactPhoneNumber;
  final DateTime? contractDate;
  final DateTime? startDate;
  final DateTime? expectedEndDate;
  final String? status;
  final String? description;
  final String? notes;

  Map<String, Object?> toJson() => {
    'expectedVersion': expectedVersion,
    if (projectName != null) 'projectName': projectName!.trim(),
    if (siteAddress != null) 'siteAddress': siteAddress!.trim(),
    if (contactPhoneNumber != null)
      'contactPhoneNumber': _normalizedOptional(contactPhoneNumber),
    if (contractDate != null) 'contractDate': _date(contractDate!),
    if (startDate != null) 'startDate': _date(startDate!),
    if (expectedEndDate != null) 'expectedEndDate': _date(expectedEndDate!),
    if (status != null) 'status': status,
    if (description != null) 'description': _normalizedOptional(description),
    if (notes != null) 'notes': _normalizedOptional(notes),
  };
}

final class SupervisorAssignmentInput {
  const SupervisorAssignmentInput({
    required this.supervisorUserId,
    required this.expectedVersion,
  });

  final String supervisorUserId;
  final int expectedVersion;

  Map<String, Object?> toJson() => {
    'supervisorUserId': supervisorUserId,
    'expectedVersion': expectedVersion,
  };
}

String _date(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';

String? _normalizedOptional(String? value) {
  final normalized = value?.trim();
  return normalized == null || normalized.isEmpty ? null : normalized;
}
