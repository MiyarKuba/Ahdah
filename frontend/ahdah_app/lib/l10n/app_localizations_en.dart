// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appName => 'Ahdah';

  @override
  String get arabicAppName => 'عُهدة';

  @override
  String get tagline => 'Accountability for construction operations';

  @override
  String get welcomeTitle => 'A clear start for your company';

  @override
  String get welcomeBody =>
      'Sign in, create a company, or securely join your team.';

  @override
  String get login => 'Sign in';

  @override
  String get createCompany => 'Create company';

  @override
  String get submitJoinRequest => 'Request to join';

  @override
  String get acceptInvitation => 'Accept invitation';

  @override
  String get phoneNumber => 'Phone number';

  @override
  String get phoneHint => 'Use E.164 format, for example +218912345678';

  @override
  String get password => 'Password';

  @override
  String get showPassword => 'Show password';

  @override
  String get hidePassword => 'Hide password';

  @override
  String get companyName => 'Company name';

  @override
  String get companyCode => 'Company code';

  @override
  String get companyCodeHint => '6–20 uppercase letters or numbers';

  @override
  String get fullName => 'Full name';

  @override
  String get managerFullName => 'Manager full name';

  @override
  String get managerPhone => 'Manager phone';

  @override
  String get emailOptional => 'Email (optional)';

  @override
  String get requestedRole => 'Requested role';

  @override
  String get requestMessageOptional => 'Message to the manager (optional)';

  @override
  String get requestRoleNotice =>
      'The requested role is provisional and must be approved by the company manager.';

  @override
  String get identityRoleNotice =>
      'Deputy and accountant roles may require identity verification.';

  @override
  String get invitationToken => 'Invitation token';

  @override
  String get invitationTokenHint => 'Enter the token shared with you';

  @override
  String get requiredField => 'This field is required.';

  @override
  String get invalidPhone => 'Enter a valid E.164 phone number.';

  @override
  String get invalidEmail => 'Enter a valid email address.';

  @override
  String get invalidCompanyCode => 'Use 6–20 letters or numbers.';

  @override
  String get passwordMinLogin => 'Enter your password.';

  @override
  String get passwordMinOnboarding => 'Use 12–128 characters.';

  @override
  String get invalidFullName => 'Use 1–200 characters without line breaks.';

  @override
  String get invalidCompanyName => 'Use 1–200 characters without line breaks.';

  @override
  String get invalidMessage =>
      'Use no more than 1000 characters without line breaks.';

  @override
  String get invalidInvitationToken => 'Enter a valid invitation token.';

  @override
  String get invalidRole => 'Choose an available role.';

  @override
  String get manager => 'Manager';

  @override
  String get deputy => 'Deputy';

  @override
  String get accountant => 'Accountant';

  @override
  String get supervisor => 'Supervisor';

  @override
  String get worker => 'Worker';

  @override
  String get unknownValue => 'Unknown';

  @override
  String get statusActive => 'Active';

  @override
  String get statusPendingApproval => 'Pending approval';

  @override
  String get statusSuspended => 'Suspended';

  @override
  String get statusInactive => 'Inactive';

  @override
  String get statusRejected => 'Rejected';

  @override
  String get identityNotRequired => 'Identity verification not required';

  @override
  String get identityPending => 'Identity verification pending';

  @override
  String get identityVerified => 'Identity verified';

  @override
  String get identityRejected => 'Identity verification rejected';

  @override
  String get pendingTitle => 'Next step pending';

  @override
  String get joinPendingTitle => 'Request submitted';

  @override
  String get joinPendingBody =>
      'Your request is waiting for the company manager to review it.';

  @override
  String get identityPendingTitle => 'Identity verification required';

  @override
  String get identityPendingBody =>
      'Your invitation was accepted. Identity verification is required before this account can be activated.';

  @override
  String get approvalPendingTitle => 'Manager approval required';

  @override
  String get approvalPendingBody =>
      'Your account is waiting for company approval.';

  @override
  String get genericPendingBody =>
      'Your onboarding step was received and is awaiting completion.';

  @override
  String get backToLogin => 'Return to sign in';

  @override
  String get homeFoundationTitle => 'Authenticated foundation';

  @override
  String get homeFoundationBody =>
      'Your secure Ahdah session is ready. Business and financial modules are not part of this phase.';

  @override
  String get company => 'Company';

  @override
  String get role => 'Role';

  @override
  String get accountStatus => 'Account status';

  @override
  String get companyStatus => 'Company status';

  @override
  String get logout => 'Sign out';

  @override
  String get logoutExplanation =>
      'Signing out removes the access token from this device. It does not revoke it on the server.';

  @override
  String get sessionExpired => 'Your session expired. Sign in again.';

  @override
  String get connectionUnavailableTitle => 'Connection unavailable';

  @override
  String get connectionUnavailableBody =>
      'We could not verify your session. Your saved mobile session has not been removed.';

  @override
  String get retry => 'Retry';

  @override
  String get genericError => 'Something went wrong. Please try again.';

  @override
  String get networkError => 'Check your connection and try again.';

  @override
  String get timeoutError => 'The request took too long. Please try again.';

  @override
  String get cancelledError => 'The request was cancelled.';

  @override
  String get invalidCredentials => 'The phone number or password is invalid.';

  @override
  String get validationError => 'Check the highlighted fields and try again.';

  @override
  String get conflictError =>
      'These details cannot be used. Review them or contact your company.';

  @override
  String get unauthorizedError => 'Your session is no longer valid.';

  @override
  String get permissionDenied =>
      'You do not have permission to perform this action.';

  @override
  String get serverError =>
      'The service is temporarily unavailable. Please try again later.';

  @override
  String get invitationInvalid =>
      'The invitation token is invalid or cannot be accepted.';

  @override
  String get joinUnavailable =>
      'The join request cannot be submitted with these details.';

  @override
  String get registrationConflict =>
      'The company or manager cannot be registered with these details.';

  @override
  String get changeLanguage => 'Change language';

  @override
  String get english => 'English';

  @override
  String get arabic => 'العربية';

  @override
  String get continueAction => 'Continue';

  @override
  String get configurationTitle => 'Development configuration required';

  @override
  String get configurationBody =>
      'API_BASE_URL is missing or invalid. Start Flutter with --dart-define=API_BASE_URL=<url>.';

  @override
  String get loading => 'Loading';

  @override
  String get secureSessionNote =>
      'Access is protected with a short-lived secure session.';

  @override
  String get alreadyHaveAccount => 'Already have an account?';

  @override
  String get noAccount => 'New to Ahdah?';

  @override
  String get navHome => 'Home';

  @override
  String get navInvitations => 'Invitations';

  @override
  String get navJoinRequests => 'Join requests';

  @override
  String get navAccount => 'Account';

  @override
  String welcomeUser(String name) {
    return 'Welcome, $name';
  }

  @override
  String get managerOverviewBody =>
      'Manage how people join your company without exposing sensitive account data.';

  @override
  String get memberOverviewBody =>
      'Your authenticated company workspace is ready for the next approved product modules.';

  @override
  String get identityStatus => 'Identity status';

  @override
  String get accessAdministration => 'Access administration';

  @override
  String get invitationShortcutBody =>
      'Create, review, and cancel company invitations.';

  @override
  String get joinRequestShortcutBody =>
      'Review and decide pending company join requests.';

  @override
  String get financialModulesDeferred =>
      'Financial, project, supplier, settlement, and reporting modules are not implemented in this phase.';

  @override
  String get accountTitle => 'Account and session';

  @override
  String get language => 'Application language';

  @override
  String get cancelAction => 'Cancel';

  @override
  String get finishAction => 'Finish';

  @override
  String get refresh => 'Refresh';

  @override
  String get statusFilter => 'Status filter';

  @override
  String get allStatuses => 'All statuses';

  @override
  String get assignedRole => 'Assigned role';

  @override
  String get createdAt => 'Created';

  @override
  String get expiresAt => 'Expires';

  @override
  String get acceptedAt => 'Accepted';

  @override
  String get cancelledAt => 'Cancelled';

  @override
  String get requestedAt => 'Requested';

  @override
  String get reviewedAt => 'Reviewed';

  @override
  String get email => 'Email';

  @override
  String get requestMessage => 'Request message';

  @override
  String get reviewNotes => 'Review notes';

  @override
  String get reviewNotesOptional => 'Review notes (optional)';

  @override
  String get rejectionReason => 'Rejection reason';

  @override
  String get invalidRejectionReason =>
      'Use 1–500 characters without line breaks.';

  @override
  String get invitationStatusPending => 'Pending';

  @override
  String get invitationStatusAccepted => 'Accepted';

  @override
  String get invitationStatusExpired => 'Expired';

  @override
  String get invitationStatusCancelled => 'Cancelled';

  @override
  String get joinStatusPending => 'Pending';

  @override
  String get joinStatusApproved => 'Approved';

  @override
  String get joinStatusRejected => 'Rejected';

  @override
  String get joinStatusCancelled => 'Cancelled';

  @override
  String get createInvitation => 'Create invitation';

  @override
  String get noInvitationsTitle => 'No invitations found';

  @override
  String get noInvitationsBody =>
      'Create an invitation or change the status filter.';

  @override
  String get retryLoadingMore => 'Retry loading more';

  @override
  String get cancelInvitation => 'Cancel invitation';

  @override
  String get cancelInvitationTitle => 'Cancel this invitation?';

  @override
  String get cancelInvitationBody =>
      'Cancellation prevents this invitation from being accepted in the future. The record will remain visible.';

  @override
  String get invitationCancelled => 'The invitation was cancelled.';

  @override
  String get invitationCreatedTitle => 'Invitation created';

  @override
  String get oneTimeTokenWarning =>
      'This token is shown once. Copy it now and deliver it securely; it cannot be recovered later.';

  @override
  String get invitationDeliveryNotImplemented =>
      'SMS and email delivery are not implemented. Share the token through an approved secure channel.';

  @override
  String get copyToken => 'Copy token';

  @override
  String get tokenCopied => 'Invitation token copied.';

  @override
  String get invitationCreationConflict =>
      'An active invitation or account conflicts with these details.';

  @override
  String get accessConflictError =>
      'This record changed on the server. Refresh and try again.';

  @override
  String get invitationNotFound => 'The invitation is no longer available.';

  @override
  String get joinRequestNotFound => 'The join request is no longer available.';

  @override
  String get notFoundError => 'The requested record is unavailable.';

  @override
  String get noJoinRequestsTitle => 'No join requests found';

  @override
  String get noJoinRequestsBody =>
      'There are no requests for the selected status.';

  @override
  String get approve => 'Approve';

  @override
  String get reject => 'Reject';

  @override
  String get approveJoinRequestTitle => 'Approve join request';

  @override
  String get approveJoinRequestBody =>
      'Confirm the final role. The applicant\'s requested role is informational only.';

  @override
  String get sensitiveRoleApprovalNotice =>
      'Deputy and accountant accounts may remain pending until identity verification is completed.';

  @override
  String get standardRoleApprovalNotice =>
      'Supervisor and worker accounts may be activated immediately according to server rules.';

  @override
  String get approvedAndActivated =>
      'The request was approved and the account was activated.';

  @override
  String get approvedPendingIdentity =>
      'The request was approved and is pending identity verification.';

  @override
  String get rejectJoinRequestTitle => 'Reject join request';

  @override
  String get rejectJoinRequestBody =>
      'The rejected record will remain visible and the applicant will not be able to sign in.';

  @override
  String get rejectionLoginNotice =>
      'Rejection prevents this applicant from signing in. Provide a clear reason.';

  @override
  String get joinRequestRejected => 'The join request was rejected.';
}
