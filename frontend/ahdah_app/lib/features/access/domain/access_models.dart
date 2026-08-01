final class AccessPage<T> {
  const AccessPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<T> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  factory AccessPage.fromJson(
    Map<String, Object?> json,
    T Function(Map<String, Object?>) parseItem,
  ) {
    final rawItems = json['items'];
    return AccessPage(
      items: rawItems is List
          ? rawItems
                .whereType<Map>()
                .map((item) => parseItem(Map<String, Object?>.from(item)))
                .toList(growable: false)
          : const [],
      page: _integer(json['page'], fallback: 1),
      pageSize: _integer(json['pageSize'], fallback: 20),
      totalCount: _integer(json['totalCount']),
      totalPages: _integer(json['totalPages']),
    );
  }

  bool get hasMore => page < totalPages;
}

final class Invitation {
  const Invitation({
    required this.id,
    required this.invitedPhoneNumber,
    required this.assignedRole,
    required this.status,
    this.expiresAtUtc,
    this.acceptedAtUtc,
    this.cancelledAtUtc,
    this.createdAtUtc,
  });

  final String id;
  final String invitedPhoneNumber;
  final String assignedRole;
  final String status;
  final DateTime? expiresAtUtc;
  final DateTime? acceptedAtUtc;
  final DateTime? cancelledAtUtc;
  final DateTime? createdAtUtc;

  bool get canCancel => status == 'Pending';

  factory Invitation.fromJson(Map<String, Object?> json) => Invitation(
    id: _string(json['invitationId']),
    invitedPhoneNumber: _string(json['invitedPhoneNumber']),
    assignedRole: _string(json['assignedRole']),
    status: _string(json['status']),
    expiresAtUtc: _utcDate(json['expiresAtUtc']),
    acceptedAtUtc: _utcDate(json['acceptedAtUtc']),
    cancelledAtUtc: _utcDate(json['cancelledAtUtc']),
    createdAtUtc: _utcDate(json['createdAtUtc']),
  );

  @override
  String toString() =>
      'Invitation(phone: $invitedPhoneNumber, role: $assignedRole, status: $status)';
}

final class CreateInvitationInput {
  const CreateInvitationInput({
    required this.phoneNumber,
    required this.assignedRole,
  });

  final String phoneNumber;
  final String assignedRole;

  Map<String, Object?> toJson() => {
    'phoneNumber': phoneNumber.trim(),
    'assignedRole': assignedRole,
  };
}

final class CreatedInvitation {
  const CreatedInvitation({required this.invitation, required this.token});

  final Invitation invitation;
  final String token;

  factory CreatedInvitation.fromJson(Map<String, Object?> json) =>
      CreatedInvitation(
        invitation: Invitation.fromJson(
          Map<String, Object?>.from(json['invitation']! as Map),
        ),
        token: _string(json['token']),
      );

  @override
  String toString() =>
      'CreatedInvitation(invitation: $invitation, token: redacted)';
}

final class JoinRequest {
  const JoinRequest({
    required this.id,
    required this.fullName,
    required this.phoneNumber,
    required this.requestedRole,
    required this.status,
    this.email,
    this.assignedRole,
    this.requestMessage,
    this.reviewNotes,
    this.rejectionReason,
    this.requestedAtUtc,
    this.reviewedAtUtc,
    this.cancelledAtUtc,
  });

  final String id;
  final String fullName;
  final String phoneNumber;
  final String? email;
  final String requestedRole;
  final String? assignedRole;
  final String status;
  final String? requestMessage;
  final String? reviewNotes;
  final String? rejectionReason;
  final DateTime? requestedAtUtc;
  final DateTime? reviewedAtUtc;
  final DateTime? cancelledAtUtc;

  bool get isPending => status == 'Pending';

  factory JoinRequest.fromJson(Map<String, Object?> json) => JoinRequest(
    id: _string(json['joinRequestId']),
    fullName: _string(json['fullName']),
    phoneNumber: _string(json['phoneNumber']),
    email: _optionalString(json['email']),
    requestedRole: _string(json['requestedRole']),
    assignedRole: _optionalString(json['assignedRole']),
    status: _string(json['status']),
    requestMessage: _optionalString(json['requestMessage']),
    reviewNotes: _optionalString(json['reviewNotes']),
    rejectionReason: _optionalString(json['rejectionReason']),
    requestedAtUtc: _utcDate(json['requestedAtUtc']),
    reviewedAtUtc: _utcDate(json['reviewedAtUtc']),
    cancelledAtUtc: _utcDate(json['cancelledAtUtc']),
  );
}

final class ApproveJoinRequestInput {
  const ApproveJoinRequestInput({required this.assignedRole, this.reviewNotes});

  final String assignedRole;
  final String? reviewNotes;

  Map<String, Object?> toJson() => {
    'assignedRole': assignedRole,
    'reviewNotes': _normalizedOptional(reviewNotes),
  };
}

final class RejectJoinRequestInput {
  const RejectJoinRequestInput({required this.reason, this.reviewNotes});

  final String reason;
  final String? reviewNotes;

  Map<String, Object?> toJson() => {
    'reason': reason.trim(),
    'reviewNotes': _normalizedOptional(reviewNotes),
  };
}

final class JoinRequestDecision {
  const JoinRequestDecision({
    required this.outcome,
    required this.userStatus,
    required this.request,
  });

  final String outcome;
  final String userStatus;
  final JoinRequest request;

  factory JoinRequestDecision.fromJson(Map<String, Object?> json) =>
      JoinRequestDecision(
        outcome: _string(json['outcome']),
        userStatus: _string(json['userStatus']),
        request: JoinRequest.fromJson(
          Map<String, Object?>.from(json['request']! as Map),
        ),
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

String? _normalizedOptional(String? value) {
  final normalized = value?.trim();
  return normalized == null || normalized.isEmpty ? null : normalized;
}

int _integer(Object? value, {int fallback = 0}) => switch (value) {
  final int number => number,
  final num number => number.toInt(),
  final String text => int.tryParse(text) ?? fallback,
  _ => fallback,
};
