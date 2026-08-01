final class LoginRequest {
  const LoginRequest({required this.phoneNumber, required this.password});
  final String phoneNumber;
  final String password;
  Map<String, Object?> toJson() => {
    'phoneNumber': phoneNumber.trim(),
    'password': password,
  };
}

final class RegisterCompanyRequest {
  const RegisterCompanyRequest({
    required this.companyName,
    required this.companyCode,
    required this.managerFullName,
    required this.managerPhone,
    required this.managerEmail,
    required this.password,
  });
  final String companyName;
  final String companyCode;
  final String managerFullName;
  final String managerPhone;
  final String? managerEmail;
  final String password;
  Map<String, Object?> toJson() => {
    'companyName': companyName.trim(),
    'companyCode': companyCode.trim().toUpperCase(),
    'managerFullName': managerFullName.trim(),
    'managerPhone': managerPhone.trim(),
    'managerEmail': _optional(managerEmail),
    'password': password,
  };
}

final class SubmitJoinRequest {
  const SubmitJoinRequest({
    required this.companyCode,
    required this.fullName,
    required this.phoneNumber,
    required this.email,
    required this.password,
    required this.requestedRole,
    required this.requestMessage,
  });
  final String companyCode;
  final String fullName;
  final String phoneNumber;
  final String? email;
  final String password;
  final String requestedRole;
  final String? requestMessage;
  Map<String, Object?> toJson() => {
    'companyCode': companyCode.trim().toUpperCase(),
    'fullName': fullName.trim(),
    'phoneNumber': phoneNumber.trim(),
    'email': _optional(email),
    'password': password,
    'requestedRole': requestedRole,
    'requestMessage': _optional(requestMessage),
  };
}

final class AcceptInvitationRequest {
  const AcceptInvitationRequest({
    required this.token,
    required this.fullName,
    required this.password,
    required this.email,
  });
  final String token;
  final String fullName;
  final String password;
  final String? email;
  Map<String, Object?> toJson() => {
    'token': token.trim(),
    'fullName': fullName.trim(),
    'password': password,
    'email': _optional(email),
  };
}

String? _optional(String? value) {
  final normalized = value?.trim();
  return normalized == null || normalized.isEmpty ? null : normalized;
}
