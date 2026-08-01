import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_ar.dart';
import 'app_localizations_en.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'l10n/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
    : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations)!;
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
        delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
      ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('ar'),
    Locale('en'),
  ];

  /// No description provided for @appName.
  ///
  /// In en, this message translates to:
  /// **'Ahdah'**
  String get appName;

  /// No description provided for @arabicAppName.
  ///
  /// In en, this message translates to:
  /// **'عُهدة'**
  String get arabicAppName;

  /// No description provided for @tagline.
  ///
  /// In en, this message translates to:
  /// **'Accountability for construction operations'**
  String get tagline;

  /// No description provided for @welcomeTitle.
  ///
  /// In en, this message translates to:
  /// **'A clear start for your company'**
  String get welcomeTitle;

  /// No description provided for @welcomeBody.
  ///
  /// In en, this message translates to:
  /// **'Sign in, create a company, or securely join your team.'**
  String get welcomeBody;

  /// No description provided for @login.
  ///
  /// In en, this message translates to:
  /// **'Sign in'**
  String get login;

  /// No description provided for @createCompany.
  ///
  /// In en, this message translates to:
  /// **'Create company'**
  String get createCompany;

  /// No description provided for @submitJoinRequest.
  ///
  /// In en, this message translates to:
  /// **'Request to join'**
  String get submitJoinRequest;

  /// No description provided for @acceptInvitation.
  ///
  /// In en, this message translates to:
  /// **'Accept invitation'**
  String get acceptInvitation;

  /// No description provided for @phoneNumber.
  ///
  /// In en, this message translates to:
  /// **'Phone number'**
  String get phoneNumber;

  /// No description provided for @phoneHint.
  ///
  /// In en, this message translates to:
  /// **'Use E.164 format, for example +218912345678'**
  String get phoneHint;

  /// No description provided for @password.
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get password;

  /// No description provided for @showPassword.
  ///
  /// In en, this message translates to:
  /// **'Show password'**
  String get showPassword;

  /// No description provided for @hidePassword.
  ///
  /// In en, this message translates to:
  /// **'Hide password'**
  String get hidePassword;

  /// No description provided for @companyName.
  ///
  /// In en, this message translates to:
  /// **'Company name'**
  String get companyName;

  /// No description provided for @companyCode.
  ///
  /// In en, this message translates to:
  /// **'Company code'**
  String get companyCode;

  /// No description provided for @companyCodeHint.
  ///
  /// In en, this message translates to:
  /// **'6–20 uppercase letters or numbers'**
  String get companyCodeHint;

  /// No description provided for @fullName.
  ///
  /// In en, this message translates to:
  /// **'Full name'**
  String get fullName;

  /// No description provided for @managerFullName.
  ///
  /// In en, this message translates to:
  /// **'Manager full name'**
  String get managerFullName;

  /// No description provided for @managerPhone.
  ///
  /// In en, this message translates to:
  /// **'Manager phone'**
  String get managerPhone;

  /// No description provided for @emailOptional.
  ///
  /// In en, this message translates to:
  /// **'Email (optional)'**
  String get emailOptional;

  /// No description provided for @requestedRole.
  ///
  /// In en, this message translates to:
  /// **'Requested role'**
  String get requestedRole;

  /// No description provided for @requestMessageOptional.
  ///
  /// In en, this message translates to:
  /// **'Message to the manager (optional)'**
  String get requestMessageOptional;

  /// No description provided for @requestRoleNotice.
  ///
  /// In en, this message translates to:
  /// **'The requested role is provisional and must be approved by the company manager.'**
  String get requestRoleNotice;

  /// No description provided for @identityRoleNotice.
  ///
  /// In en, this message translates to:
  /// **'Deputy and accountant roles may require identity verification.'**
  String get identityRoleNotice;

  /// No description provided for @invitationToken.
  ///
  /// In en, this message translates to:
  /// **'Invitation token'**
  String get invitationToken;

  /// No description provided for @invitationTokenHint.
  ///
  /// In en, this message translates to:
  /// **'Enter the token shared with you'**
  String get invitationTokenHint;

  /// No description provided for @requiredField.
  ///
  /// In en, this message translates to:
  /// **'This field is required.'**
  String get requiredField;

  /// No description provided for @invalidPhone.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid E.164 phone number.'**
  String get invalidPhone;

  /// No description provided for @invalidEmail.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid email address.'**
  String get invalidEmail;

  /// No description provided for @invalidCompanyCode.
  ///
  /// In en, this message translates to:
  /// **'Use 6–20 letters or numbers.'**
  String get invalidCompanyCode;

  /// No description provided for @passwordMinLogin.
  ///
  /// In en, this message translates to:
  /// **'Enter your password.'**
  String get passwordMinLogin;

  /// No description provided for @passwordMinOnboarding.
  ///
  /// In en, this message translates to:
  /// **'Use 12–128 characters.'**
  String get passwordMinOnboarding;

  /// No description provided for @invalidFullName.
  ///
  /// In en, this message translates to:
  /// **'Use 1–200 characters without line breaks.'**
  String get invalidFullName;

  /// No description provided for @invalidCompanyName.
  ///
  /// In en, this message translates to:
  /// **'Use 1–200 characters without line breaks.'**
  String get invalidCompanyName;

  /// No description provided for @invalidMessage.
  ///
  /// In en, this message translates to:
  /// **'Use no more than 1000 characters without line breaks.'**
  String get invalidMessage;

  /// No description provided for @invalidInvitationToken.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid invitation token.'**
  String get invalidInvitationToken;

  /// No description provided for @invalidRole.
  ///
  /// In en, this message translates to:
  /// **'Choose an available role.'**
  String get invalidRole;

  /// No description provided for @manager.
  ///
  /// In en, this message translates to:
  /// **'Manager'**
  String get manager;

  /// No description provided for @deputy.
  ///
  /// In en, this message translates to:
  /// **'Deputy'**
  String get deputy;

  /// No description provided for @accountant.
  ///
  /// In en, this message translates to:
  /// **'Accountant'**
  String get accountant;

  /// No description provided for @supervisor.
  ///
  /// In en, this message translates to:
  /// **'Supervisor'**
  String get supervisor;

  /// No description provided for @worker.
  ///
  /// In en, this message translates to:
  /// **'Worker'**
  String get worker;

  /// No description provided for @unknownValue.
  ///
  /// In en, this message translates to:
  /// **'Unknown'**
  String get unknownValue;

  /// No description provided for @statusActive.
  ///
  /// In en, this message translates to:
  /// **'Active'**
  String get statusActive;

  /// No description provided for @statusPendingApproval.
  ///
  /// In en, this message translates to:
  /// **'Pending approval'**
  String get statusPendingApproval;

  /// No description provided for @statusSuspended.
  ///
  /// In en, this message translates to:
  /// **'Suspended'**
  String get statusSuspended;

  /// No description provided for @statusInactive.
  ///
  /// In en, this message translates to:
  /// **'Inactive'**
  String get statusInactive;

  /// No description provided for @statusRejected.
  ///
  /// In en, this message translates to:
  /// **'Rejected'**
  String get statusRejected;

  /// No description provided for @identityNotRequired.
  ///
  /// In en, this message translates to:
  /// **'Identity verification not required'**
  String get identityNotRequired;

  /// No description provided for @identityPending.
  ///
  /// In en, this message translates to:
  /// **'Identity verification pending'**
  String get identityPending;

  /// No description provided for @identityVerified.
  ///
  /// In en, this message translates to:
  /// **'Identity verified'**
  String get identityVerified;

  /// No description provided for @identityRejected.
  ///
  /// In en, this message translates to:
  /// **'Identity verification rejected'**
  String get identityRejected;

  /// No description provided for @pendingTitle.
  ///
  /// In en, this message translates to:
  /// **'Next step pending'**
  String get pendingTitle;

  /// No description provided for @joinPendingTitle.
  ///
  /// In en, this message translates to:
  /// **'Request submitted'**
  String get joinPendingTitle;

  /// No description provided for @joinPendingBody.
  ///
  /// In en, this message translates to:
  /// **'Your request is waiting for the company manager to review it.'**
  String get joinPendingBody;

  /// No description provided for @identityPendingTitle.
  ///
  /// In en, this message translates to:
  /// **'Identity verification required'**
  String get identityPendingTitle;

  /// No description provided for @identityPendingBody.
  ///
  /// In en, this message translates to:
  /// **'Your invitation was accepted. Identity verification is required before this account can be activated.'**
  String get identityPendingBody;

  /// No description provided for @approvalPendingTitle.
  ///
  /// In en, this message translates to:
  /// **'Manager approval required'**
  String get approvalPendingTitle;

  /// No description provided for @approvalPendingBody.
  ///
  /// In en, this message translates to:
  /// **'Your account is waiting for company approval.'**
  String get approvalPendingBody;

  /// No description provided for @genericPendingBody.
  ///
  /// In en, this message translates to:
  /// **'Your onboarding step was received and is awaiting completion.'**
  String get genericPendingBody;

  /// No description provided for @backToLogin.
  ///
  /// In en, this message translates to:
  /// **'Return to sign in'**
  String get backToLogin;

  /// No description provided for @homeFoundationTitle.
  ///
  /// In en, this message translates to:
  /// **'Authenticated foundation'**
  String get homeFoundationTitle;

  /// No description provided for @homeFoundationBody.
  ///
  /// In en, this message translates to:
  /// **'Your secure Ahdah session is ready. Business and financial modules are not part of this phase.'**
  String get homeFoundationBody;

  /// No description provided for @company.
  ///
  /// In en, this message translates to:
  /// **'Company'**
  String get company;

  /// No description provided for @role.
  ///
  /// In en, this message translates to:
  /// **'Role'**
  String get role;

  /// No description provided for @accountStatus.
  ///
  /// In en, this message translates to:
  /// **'Account status'**
  String get accountStatus;

  /// No description provided for @companyStatus.
  ///
  /// In en, this message translates to:
  /// **'Company status'**
  String get companyStatus;

  /// No description provided for @logout.
  ///
  /// In en, this message translates to:
  /// **'Sign out'**
  String get logout;

  /// No description provided for @logoutExplanation.
  ///
  /// In en, this message translates to:
  /// **'Signing out removes the access token from this device. It does not revoke it on the server.'**
  String get logoutExplanation;

  /// No description provided for @sessionExpired.
  ///
  /// In en, this message translates to:
  /// **'Your session expired. Sign in again.'**
  String get sessionExpired;

  /// No description provided for @connectionUnavailableTitle.
  ///
  /// In en, this message translates to:
  /// **'Connection unavailable'**
  String get connectionUnavailableTitle;

  /// No description provided for @connectionUnavailableBody.
  ///
  /// In en, this message translates to:
  /// **'We could not verify your session. Your saved mobile session has not been removed.'**
  String get connectionUnavailableBody;

  /// No description provided for @retry.
  ///
  /// In en, this message translates to:
  /// **'Retry'**
  String get retry;

  /// No description provided for @genericError.
  ///
  /// In en, this message translates to:
  /// **'Something went wrong. Please try again.'**
  String get genericError;

  /// No description provided for @networkError.
  ///
  /// In en, this message translates to:
  /// **'Check your connection and try again.'**
  String get networkError;

  /// No description provided for @timeoutError.
  ///
  /// In en, this message translates to:
  /// **'The request took too long. Please try again.'**
  String get timeoutError;

  /// No description provided for @cancelledError.
  ///
  /// In en, this message translates to:
  /// **'The request was cancelled.'**
  String get cancelledError;

  /// No description provided for @invalidCredentials.
  ///
  /// In en, this message translates to:
  /// **'The phone number or password is invalid.'**
  String get invalidCredentials;

  /// No description provided for @validationError.
  ///
  /// In en, this message translates to:
  /// **'Check the highlighted fields and try again.'**
  String get validationError;

  /// No description provided for @conflictError.
  ///
  /// In en, this message translates to:
  /// **'These details cannot be used. Review them or contact your company.'**
  String get conflictError;

  /// No description provided for @unauthorizedError.
  ///
  /// In en, this message translates to:
  /// **'Your session is no longer valid.'**
  String get unauthorizedError;

  /// No description provided for @permissionDenied.
  ///
  /// In en, this message translates to:
  /// **'You do not have permission to perform this action.'**
  String get permissionDenied;

  /// No description provided for @serverError.
  ///
  /// In en, this message translates to:
  /// **'The service is temporarily unavailable. Please try again later.'**
  String get serverError;

  /// No description provided for @invitationInvalid.
  ///
  /// In en, this message translates to:
  /// **'The invitation token is invalid or cannot be accepted.'**
  String get invitationInvalid;

  /// No description provided for @joinUnavailable.
  ///
  /// In en, this message translates to:
  /// **'The join request cannot be submitted with these details.'**
  String get joinUnavailable;

  /// No description provided for @registrationConflict.
  ///
  /// In en, this message translates to:
  /// **'The company or manager cannot be registered with these details.'**
  String get registrationConflict;

  /// No description provided for @changeLanguage.
  ///
  /// In en, this message translates to:
  /// **'Change language'**
  String get changeLanguage;

  /// No description provided for @english.
  ///
  /// In en, this message translates to:
  /// **'English'**
  String get english;

  /// No description provided for @arabic.
  ///
  /// In en, this message translates to:
  /// **'العربية'**
  String get arabic;

  /// No description provided for @continueAction.
  ///
  /// In en, this message translates to:
  /// **'Continue'**
  String get continueAction;

  /// No description provided for @configurationTitle.
  ///
  /// In en, this message translates to:
  /// **'Development configuration required'**
  String get configurationTitle;

  /// No description provided for @configurationBody.
  ///
  /// In en, this message translates to:
  /// **'API_BASE_URL is missing or invalid. Start Flutter with --dart-define=API_BASE_URL=<url>.'**
  String get configurationBody;

  /// No description provided for @loading.
  ///
  /// In en, this message translates to:
  /// **'Loading'**
  String get loading;

  /// No description provided for @secureSessionNote.
  ///
  /// In en, this message translates to:
  /// **'Access is protected with a short-lived secure session.'**
  String get secureSessionNote;

  /// No description provided for @alreadyHaveAccount.
  ///
  /// In en, this message translates to:
  /// **'Already have an account?'**
  String get alreadyHaveAccount;

  /// No description provided for @noAccount.
  ///
  /// In en, this message translates to:
  /// **'New to Ahdah?'**
  String get noAccount;

  /// No description provided for @navHome.
  ///
  /// In en, this message translates to:
  /// **'Home'**
  String get navHome;

  /// No description provided for @navInvitations.
  ///
  /// In en, this message translates to:
  /// **'Invitations'**
  String get navInvitations;

  /// No description provided for @navJoinRequests.
  ///
  /// In en, this message translates to:
  /// **'Join requests'**
  String get navJoinRequests;

  /// No description provided for @navAccount.
  ///
  /// In en, this message translates to:
  /// **'Account'**
  String get navAccount;

  /// No description provided for @welcomeUser.
  ///
  /// In en, this message translates to:
  /// **'Welcome, {name}'**
  String welcomeUser(String name);

  /// No description provided for @managerOverviewBody.
  ///
  /// In en, this message translates to:
  /// **'Manage how people join your company without exposing sensitive account data.'**
  String get managerOverviewBody;

  /// No description provided for @memberOverviewBody.
  ///
  /// In en, this message translates to:
  /// **'Your authenticated company workspace is ready for the next approved product modules.'**
  String get memberOverviewBody;

  /// No description provided for @identityStatus.
  ///
  /// In en, this message translates to:
  /// **'Identity status'**
  String get identityStatus;

  /// No description provided for @accessAdministration.
  ///
  /// In en, this message translates to:
  /// **'Access administration'**
  String get accessAdministration;

  /// No description provided for @invitationShortcutBody.
  ///
  /// In en, this message translates to:
  /// **'Create, review, and cancel company invitations.'**
  String get invitationShortcutBody;

  /// No description provided for @joinRequestShortcutBody.
  ///
  /// In en, this message translates to:
  /// **'Review and decide pending company join requests.'**
  String get joinRequestShortcutBody;

  /// No description provided for @financialModulesDeferred.
  ///
  /// In en, this message translates to:
  /// **'Financial, project, supplier, settlement, and reporting modules are not implemented in this phase.'**
  String get financialModulesDeferred;

  /// No description provided for @accountTitle.
  ///
  /// In en, this message translates to:
  /// **'Account and session'**
  String get accountTitle;

  /// No description provided for @language.
  ///
  /// In en, this message translates to:
  /// **'Application language'**
  String get language;

  /// No description provided for @cancelAction.
  ///
  /// In en, this message translates to:
  /// **'Cancel'**
  String get cancelAction;

  /// No description provided for @finishAction.
  ///
  /// In en, this message translates to:
  /// **'Finish'**
  String get finishAction;

  /// No description provided for @refresh.
  ///
  /// In en, this message translates to:
  /// **'Refresh'**
  String get refresh;

  /// No description provided for @statusFilter.
  ///
  /// In en, this message translates to:
  /// **'Status filter'**
  String get statusFilter;

  /// No description provided for @allStatuses.
  ///
  /// In en, this message translates to:
  /// **'All statuses'**
  String get allStatuses;

  /// No description provided for @assignedRole.
  ///
  /// In en, this message translates to:
  /// **'Assigned role'**
  String get assignedRole;

  /// No description provided for @createdAt.
  ///
  /// In en, this message translates to:
  /// **'Created'**
  String get createdAt;

  /// No description provided for @expiresAt.
  ///
  /// In en, this message translates to:
  /// **'Expires'**
  String get expiresAt;

  /// No description provided for @acceptedAt.
  ///
  /// In en, this message translates to:
  /// **'Accepted'**
  String get acceptedAt;

  /// No description provided for @cancelledAt.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get cancelledAt;

  /// No description provided for @requestedAt.
  ///
  /// In en, this message translates to:
  /// **'Requested'**
  String get requestedAt;

  /// No description provided for @reviewedAt.
  ///
  /// In en, this message translates to:
  /// **'Reviewed'**
  String get reviewedAt;

  /// No description provided for @email.
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get email;

  /// No description provided for @requestMessage.
  ///
  /// In en, this message translates to:
  /// **'Request message'**
  String get requestMessage;

  /// No description provided for @reviewNotes.
  ///
  /// In en, this message translates to:
  /// **'Review notes'**
  String get reviewNotes;

  /// No description provided for @reviewNotesOptional.
  ///
  /// In en, this message translates to:
  /// **'Review notes (optional)'**
  String get reviewNotesOptional;

  /// No description provided for @rejectionReason.
  ///
  /// In en, this message translates to:
  /// **'Rejection reason'**
  String get rejectionReason;

  /// No description provided for @invalidRejectionReason.
  ///
  /// In en, this message translates to:
  /// **'Use 1–500 characters without line breaks.'**
  String get invalidRejectionReason;

  /// No description provided for @invitationStatusPending.
  ///
  /// In en, this message translates to:
  /// **'Pending'**
  String get invitationStatusPending;

  /// No description provided for @invitationStatusAccepted.
  ///
  /// In en, this message translates to:
  /// **'Accepted'**
  String get invitationStatusAccepted;

  /// No description provided for @invitationStatusExpired.
  ///
  /// In en, this message translates to:
  /// **'Expired'**
  String get invitationStatusExpired;

  /// No description provided for @invitationStatusCancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get invitationStatusCancelled;

  /// No description provided for @joinStatusPending.
  ///
  /// In en, this message translates to:
  /// **'Pending'**
  String get joinStatusPending;

  /// No description provided for @joinStatusApproved.
  ///
  /// In en, this message translates to:
  /// **'Approved'**
  String get joinStatusApproved;

  /// No description provided for @joinStatusRejected.
  ///
  /// In en, this message translates to:
  /// **'Rejected'**
  String get joinStatusRejected;

  /// No description provided for @joinStatusCancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get joinStatusCancelled;

  /// No description provided for @createInvitation.
  ///
  /// In en, this message translates to:
  /// **'Create invitation'**
  String get createInvitation;

  /// No description provided for @noInvitationsTitle.
  ///
  /// In en, this message translates to:
  /// **'No invitations found'**
  String get noInvitationsTitle;

  /// No description provided for @noInvitationsBody.
  ///
  /// In en, this message translates to:
  /// **'Create an invitation or change the status filter.'**
  String get noInvitationsBody;

  /// No description provided for @retryLoadingMore.
  ///
  /// In en, this message translates to:
  /// **'Retry loading more'**
  String get retryLoadingMore;

  /// No description provided for @cancelInvitation.
  ///
  /// In en, this message translates to:
  /// **'Cancel invitation'**
  String get cancelInvitation;

  /// No description provided for @cancelInvitationTitle.
  ///
  /// In en, this message translates to:
  /// **'Cancel this invitation?'**
  String get cancelInvitationTitle;

  /// No description provided for @cancelInvitationBody.
  ///
  /// In en, this message translates to:
  /// **'Cancellation prevents this invitation from being accepted in the future. The record will remain visible.'**
  String get cancelInvitationBody;

  /// No description provided for @invitationCancelled.
  ///
  /// In en, this message translates to:
  /// **'The invitation was cancelled.'**
  String get invitationCancelled;

  /// No description provided for @invitationCreatedTitle.
  ///
  /// In en, this message translates to:
  /// **'Invitation created'**
  String get invitationCreatedTitle;

  /// No description provided for @oneTimeTokenWarning.
  ///
  /// In en, this message translates to:
  /// **'This token is shown once. Copy it now and deliver it securely; it cannot be recovered later.'**
  String get oneTimeTokenWarning;

  /// No description provided for @invitationDeliveryNotImplemented.
  ///
  /// In en, this message translates to:
  /// **'SMS and email delivery are not implemented. Share the token through an approved secure channel.'**
  String get invitationDeliveryNotImplemented;

  /// No description provided for @copyToken.
  ///
  /// In en, this message translates to:
  /// **'Copy token'**
  String get copyToken;

  /// No description provided for @tokenCopied.
  ///
  /// In en, this message translates to:
  /// **'Invitation token copied.'**
  String get tokenCopied;

  /// No description provided for @invitationCreationConflict.
  ///
  /// In en, this message translates to:
  /// **'An active invitation or account conflicts with these details.'**
  String get invitationCreationConflict;

  /// No description provided for @accessConflictError.
  ///
  /// In en, this message translates to:
  /// **'This record changed on the server. Refresh and try again.'**
  String get accessConflictError;

  /// No description provided for @invitationNotFound.
  ///
  /// In en, this message translates to:
  /// **'The invitation is no longer available.'**
  String get invitationNotFound;

  /// No description provided for @joinRequestNotFound.
  ///
  /// In en, this message translates to:
  /// **'The join request is no longer available.'**
  String get joinRequestNotFound;

  /// No description provided for @notFoundError.
  ///
  /// In en, this message translates to:
  /// **'The requested record is unavailable.'**
  String get notFoundError;

  /// No description provided for @noJoinRequestsTitle.
  ///
  /// In en, this message translates to:
  /// **'No join requests found'**
  String get noJoinRequestsTitle;

  /// No description provided for @noJoinRequestsBody.
  ///
  /// In en, this message translates to:
  /// **'There are no requests for the selected status.'**
  String get noJoinRequestsBody;

  /// No description provided for @approve.
  ///
  /// In en, this message translates to:
  /// **'Approve'**
  String get approve;

  /// No description provided for @reject.
  ///
  /// In en, this message translates to:
  /// **'Reject'**
  String get reject;

  /// No description provided for @approveJoinRequestTitle.
  ///
  /// In en, this message translates to:
  /// **'Approve join request'**
  String get approveJoinRequestTitle;

  /// No description provided for @approveJoinRequestBody.
  ///
  /// In en, this message translates to:
  /// **'Confirm the final role. The applicant\'s requested role is informational only.'**
  String get approveJoinRequestBody;

  /// No description provided for @sensitiveRoleApprovalNotice.
  ///
  /// In en, this message translates to:
  /// **'Deputy and accountant accounts may remain pending until identity verification is completed.'**
  String get sensitiveRoleApprovalNotice;

  /// No description provided for @standardRoleApprovalNotice.
  ///
  /// In en, this message translates to:
  /// **'Supervisor and worker accounts may be activated immediately according to server rules.'**
  String get standardRoleApprovalNotice;

  /// No description provided for @approvedAndActivated.
  ///
  /// In en, this message translates to:
  /// **'The request was approved and the account was activated.'**
  String get approvedAndActivated;

  /// No description provided for @approvedPendingIdentity.
  ///
  /// In en, this message translates to:
  /// **'The request was approved and is pending identity verification.'**
  String get approvedPendingIdentity;

  /// No description provided for @rejectJoinRequestTitle.
  ///
  /// In en, this message translates to:
  /// **'Reject join request'**
  String get rejectJoinRequestTitle;

  /// No description provided for @rejectJoinRequestBody.
  ///
  /// In en, this message translates to:
  /// **'The rejected record will remain visible and the applicant will not be able to sign in.'**
  String get rejectJoinRequestBody;

  /// No description provided for @rejectionLoginNotice.
  ///
  /// In en, this message translates to:
  /// **'Rejection prevents this applicant from signing in. Provide a clear reason.'**
  String get rejectionLoginNotice;

  /// No description provided for @joinRequestRejected.
  ///
  /// In en, this message translates to:
  /// **'The join request was rejected.'**
  String get joinRequestRejected;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['ar', 'en'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'ar':
      return AppLocalizationsAr();
    case 'en':
      return AppLocalizationsEn();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
