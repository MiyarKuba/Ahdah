final class CompanySummary {
  const CompanySummary({
    required this.companyId,
    required this.companyName,
    required this.companyCode,
    required this.status,
  });

  final String companyId;
  final String companyName;
  final String companyCode;
  final String status;

  factory CompanySummary.fromJson(Map<String, Object?> json) => CompanySummary(
    companyId: json['companyId']! as String,
    companyName: json['companyName']! as String,
    companyCode: json['companyCode']! as String,
    status: json['status']! as String,
  );
}

final class AuthenticatedUser {
  const AuthenticatedUser({
    required this.userId,
    required this.fullName,
    required this.role,
    required this.status,
    required this.identityVerificationStatus,
  });

  final String userId;
  final String fullName;
  final String role;
  final String status;
  final String identityVerificationStatus;

  factory AuthenticatedUser.fromJson(Map<String, Object?> json) =>
      AuthenticatedUser(
        userId: json['userId']! as String,
        fullName: json['fullName']! as String,
        role: json['role']! as String,
        status: json['status']! as String,
        identityVerificationStatus:
            json['identityVerificationStatus'] as String? ?? 'Unknown',
      );
}

final class AuthenticationResult {
  const AuthenticationResult({
    required this.accessToken,
    required this.tokenType,
    required this.accessTokenExpiresAtUtc,
    required this.user,
    required this.company,
    required this.role,
  });

  final String accessToken;
  final String tokenType;
  final DateTime accessTokenExpiresAtUtc;
  final AuthenticatedUser user;
  final CompanySummary company;
  final String role;

  factory AuthenticationResult.fromJson(Map<String, Object?> json) =>
      AuthenticationResult(
        accessToken: json['accessToken']! as String,
        tokenType: json['tokenType']! as String,
        accessTokenExpiresAtUtc: DateTime.parse(
          json['accessTokenExpiresAtUtc']! as String,
        ).toUtc(),
        user: AuthenticatedUser.fromJson(
          Map<String, Object?>.from(json['user']! as Map),
        ),
        company: CompanySummary.fromJson(
          Map<String, Object?>.from(json['company']! as Map),
        ),
        role: json['role']! as String,
      );

  @override
  String toString() =>
      'AuthenticationResult(user: ${user.fullName}, company: ${company.companyName}, role: $role, expiresAtUtc: $accessTokenExpiresAtUtc)';
}

final class CurrentSessionResult {
  const CurrentSessionResult({
    required this.user,
    required this.company,
    required this.role,
  });

  final AuthenticatedUser user;
  final CompanySummary company;
  final String role;

  factory CurrentSessionResult.fromJson(Map<String, Object?> json) =>
      CurrentSessionResult(
        user: AuthenticatedUser.fromJson(
          Map<String, Object?>.from(json['user']! as Map),
        ),
        company: CompanySummary.fromJson(
          Map<String, Object?>.from(json['company']! as Map),
        ),
        role: json['role']! as String,
      );
}

final class RegisterCompanyResult {
  const RegisterCompanyResult({
    required this.company,
    required this.manager,
    required this.authentication,
  });

  final CompanySummary company;
  final AuthenticatedUser manager;
  final AuthenticationResult authentication;

  factory RegisterCompanyResult.fromJson(Map<String, Object?> json) =>
      RegisterCompanyResult(
        company: CompanySummary.fromJson(
          Map<String, Object?>.from(json['company']! as Map),
        ),
        manager: AuthenticatedUser.fromJson(
          Map<String, Object?>.from(json['manager']! as Map),
        ),
        authentication: AuthenticationResult.fromJson(
          Map<String, Object?>.from(json['authentication']! as Map),
        ),
      );
}

final class InvitationAcceptanceResult {
  const InvitationAcceptanceResult({
    required this.outcome,
    required this.userStatus,
    required this.assignedRole,
    this.authentication,
  });

  final String outcome;
  final String userStatus;
  final String assignedRole;
  final AuthenticationResult? authentication;

  bool get requiresIdentityVerification =>
      outcome == 'AcceptedPendingIdentityVerification';

  factory InvitationAcceptanceResult.fromJson(Map<String, Object?> json) =>
      InvitationAcceptanceResult(
        outcome: json['outcome']! as String,
        userStatus: json['userStatus']! as String,
        assignedRole: json['assignedRole']! as String,
        authentication: json['authentication'] == null
            ? null
            : AuthenticationResult.fromJson(
                Map<String, Object?>.from(json['authentication']! as Map),
              ),
      );
}

final class JoinRequestAcknowledgement {
  const JoinRequestAcknowledgement({required this.status});

  final String status;

  factory JoinRequestAcknowledgement.fromJson(Map<String, Object?> json) =>
      JoinRequestAcknowledgement(status: json['status']! as String);
}
