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
}
