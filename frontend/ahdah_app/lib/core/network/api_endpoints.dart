abstract final class ApiEndpoints {
  static const login = '/api/v1/auth/login';
  static const registerCompany = '/api/v1/auth/register-company';
  static const currentUser = '/api/v1/auth/me';
  static const acceptInvitation = '/api/v1/invitations/accept';
  static const invitations = '/api/v1/invitations';
  static const joinRequests = '/api/v1/join-requests';
  static const health = '/api/system/health';

  static String cancelInvitation(String id) => '$invitations/$id/cancel';
  static String approveJoinRequest(String id) => '$joinRequests/$id/approve';
  static String rejectJoinRequest(String id) => '$joinRequests/$id/reject';
}
