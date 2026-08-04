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
  /// **'Your secure Ahdah session, company tools, advances, and authoritative balance workflows are ready.'**
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
  /// **'Expenses, receipts, suppliers, settlements, closure, debts, and reporting remain deferred.'**
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

  /// No description provided for @navProjects.
  ///
  /// In en, this message translates to:
  /// **'Projects'**
  String get navProjects;

  /// No description provided for @navCompanyMembers.
  ///
  /// In en, this message translates to:
  /// **'Company members'**
  String get navCompanyMembers;

  /// No description provided for @projectsTitle.
  ///
  /// In en, this message translates to:
  /// **'Projects and sites'**
  String get projectsTitle;

  /// No description provided for @companyMembersTitle.
  ///
  /// In en, this message translates to:
  /// **'Company member directory'**
  String get companyMembersTitle;

  /// No description provided for @searchProjects.
  ///
  /// In en, this message translates to:
  /// **'Search project name or site address'**
  String get searchProjects;

  /// No description provided for @searchMembers.
  ///
  /// In en, this message translates to:
  /// **'Search name or phone'**
  String get searchMembers;

  /// No description provided for @searchMinimum.
  ///
  /// In en, this message translates to:
  /// **'Enter at least 2 characters to search.'**
  String get searchMinimum;

  /// No description provided for @roleFilter.
  ///
  /// In en, this message translates to:
  /// **'Role filter'**
  String get roleFilter;

  /// No description provided for @allRoles.
  ///
  /// In en, this message translates to:
  /// **'All roles'**
  String get allRoles;

  /// No description provided for @projectStatusActive.
  ///
  /// In en, this message translates to:
  /// **'Active'**
  String get projectStatusActive;

  /// No description provided for @projectStatusPaused.
  ///
  /// In en, this message translates to:
  /// **'Paused'**
  String get projectStatusPaused;

  /// No description provided for @projectStatusCompleted.
  ///
  /// In en, this message translates to:
  /// **'Completed'**
  String get projectStatusCompleted;

  /// No description provided for @projectStatusFinanciallyClosed.
  ///
  /// In en, this message translates to:
  /// **'Financially closed'**
  String get projectStatusFinanciallyClosed;

  /// No description provided for @projectStatusCancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get projectStatusCancelled;

  /// No description provided for @projectName.
  ///
  /// In en, this message translates to:
  /// **'Project name'**
  String get projectName;

  /// No description provided for @siteAddress.
  ///
  /// In en, this message translates to:
  /// **'Site address'**
  String get siteAddress;

  /// No description provided for @ownerClient.
  ///
  /// In en, this message translates to:
  /// **'Project owner / client'**
  String get ownerClient;

  /// No description provided for @ownerName.
  ///
  /// In en, this message translates to:
  /// **'Owner name'**
  String get ownerName;

  /// No description provided for @contractValue.
  ///
  /// In en, this message translates to:
  /// **'Contract value'**
  String get contractValue;

  /// No description provided for @contractValueLyd.
  ///
  /// In en, this message translates to:
  /// **'Contract value (LYD)'**
  String get contractValueLyd;

  /// No description provided for @contractDate.
  ///
  /// In en, this message translates to:
  /// **'Contract date'**
  String get contractDate;

  /// No description provided for @startDate.
  ///
  /// In en, this message translates to:
  /// **'Start date'**
  String get startDate;

  /// No description provided for @expectedEndDate.
  ///
  /// In en, this message translates to:
  /// **'Expected end date'**
  String get expectedEndDate;

  /// No description provided for @actualEndDate.
  ///
  /// In en, this message translates to:
  /// **'Actual end date'**
  String get actualEndDate;

  /// No description provided for @descriptionOptional.
  ///
  /// In en, this message translates to:
  /// **'Description (optional)'**
  String get descriptionOptional;

  /// No description provided for @notesOptional.
  ///
  /// In en, this message translates to:
  /// **'Notes (optional)'**
  String get notesOptional;

  /// No description provided for @contactPhoneOptional.
  ///
  /// In en, this message translates to:
  /// **'Project contact phone (optional)'**
  String get contactPhoneOptional;

  /// No description provided for @addressOptional.
  ///
  /// In en, this message translates to:
  /// **'Address (optional)'**
  String get addressOptional;

  /// No description provided for @assignedSupervisor.
  ///
  /// In en, this message translates to:
  /// **'Assigned supervisor'**
  String get assignedSupervisor;

  /// No description provided for @assignedSupervisors.
  ///
  /// In en, this message translates to:
  /// **'Assigned supervisors'**
  String get assignedSupervisors;

  /// No description provided for @createProject.
  ///
  /// In en, this message translates to:
  /// **'Create project'**
  String get createProject;

  /// No description provided for @editProject.
  ///
  /// In en, this message translates to:
  /// **'Edit project'**
  String get editProject;

  /// No description provided for @assignSupervisor.
  ///
  /// In en, this message translates to:
  /// **'Assign supervisor'**
  String get assignSupervisor;

  /// No description provided for @replaceSupervisor.
  ///
  /// In en, this message translates to:
  /// **'Replace supervisor'**
  String get replaceSupervisor;

  /// No description provided for @projectDetails.
  ///
  /// In en, this message translates to:
  /// **'Project details'**
  String get projectDetails;

  /// No description provided for @projectMembersTitle.
  ///
  /// In en, this message translates to:
  /// **'Project supervisors'**
  String get projectMembersTitle;

  /// No description provided for @activeSupervisionAssignmentsBody.
  ///
  /// In en, this message translates to:
  /// **'This view contains active supervision assignments only. It is not a complete project staff directory.'**
  String get activeSupervisionAssignmentsBody;

  /// No description provided for @noProjectsTitle.
  ///
  /// In en, this message translates to:
  /// **'No projects found'**
  String get noProjectsTitle;

  /// No description provided for @noProjectsBody.
  ///
  /// In en, this message translates to:
  /// **'Change the filters or create the first project.'**
  String get noProjectsBody;

  /// No description provided for @noAssignedProjectsBody.
  ///
  /// In en, this message translates to:
  /// **'Only projects currently assigned to you appear here.'**
  String get noAssignedProjectsBody;

  /// No description provided for @noMembersTitle.
  ///
  /// In en, this message translates to:
  /// **'No company members found'**
  String get noMembersTitle;

  /// No description provided for @noMembersBody.
  ///
  /// In en, this message translates to:
  /// **'Change the role, status, or search filter.'**
  String get noMembersBody;

  /// No description provided for @memberDetails.
  ///
  /// In en, this message translates to:
  /// **'Member details'**
  String get memberDetails;

  /// No description provided for @directoryReadOnly.
  ///
  /// In en, this message translates to:
  /// **'This company directory is read-only.'**
  String get directoryReadOnly;

  /// No description provided for @projectCreated.
  ///
  /// In en, this message translates to:
  /// **'The project was created.'**
  String get projectCreated;

  /// No description provided for @projectUpdated.
  ///
  /// In en, this message translates to:
  /// **'The project was updated.'**
  String get projectUpdated;

  /// No description provided for @supervisorAssigned.
  ///
  /// In en, this message translates to:
  /// **'The supervisor assignment was updated.'**
  String get supervisorAssigned;

  /// No description provided for @projectChangedConflict.
  ///
  /// In en, this message translates to:
  /// **'This project changed on the server. Reload it before trying again.'**
  String get projectChangedConflict;

  /// No description provided for @reloadProject.
  ///
  /// In en, this message translates to:
  /// **'Reload project'**
  String get reloadProject;

  /// No description provided for @newOwnerTitle.
  ///
  /// In en, this message translates to:
  /// **'New client / owner'**
  String get newOwnerTitle;

  /// No description provided for @existingOwnerUnavailable.
  ///
  /// In en, this message translates to:
  /// **'A safe owner directory is not available yet. This form creates the owner and project atomically.'**
  String get existingOwnerUnavailable;

  /// No description provided for @chooseSupervisor.
  ///
  /// In en, this message translates to:
  /// **'Choose supervisor'**
  String get chooseSupervisor;

  /// No description provided for @optionalSupervisor.
  ///
  /// In en, this message translates to:
  /// **'Initial supervisor (optional)'**
  String get optionalSupervisor;

  /// No description provided for @noSupervisor.
  ///
  /// In en, this message translates to:
  /// **'No active supervisor'**
  String get noSupervisor;

  /// No description provided for @replaceSupervisorTitle.
  ///
  /// In en, this message translates to:
  /// **'Replace the active supervisor?'**
  String get replaceSupervisorTitle;

  /// No description provided for @replaceSupervisorBody.
  ///
  /// In en, this message translates to:
  /// **'The previous assignment will be ended, not deleted. The server preserves assignment history.'**
  String get replaceSupervisorBody;

  /// No description provided for @supervisorHistoryNotice.
  ///
  /// In en, this message translates to:
  /// **'Replacing a supervisor preserves earlier assignment history on the server.'**
  String get supervisorHistoryNotice;

  /// No description provided for @saveAction.
  ///
  /// In en, this message translates to:
  /// **'Save'**
  String get saveAction;

  /// No description provided for @selectDate.
  ///
  /// In en, this message translates to:
  /// **'Select date'**
  String get selectDate;

  /// No description provided for @clearAction.
  ///
  /// In en, this message translates to:
  /// **'Clear'**
  String get clearAction;

  /// No description provided for @viewDetails.
  ///
  /// In en, this message translates to:
  /// **'View details'**
  String get viewDetails;

  /// No description provided for @invalidRequiredText.
  ///
  /// In en, this message translates to:
  /// **'Enter a non-blank value within the allowed length.'**
  String get invalidRequiredText;

  /// No description provided for @invalidContractValue.
  ///
  /// In en, this message translates to:
  /// **'Enter a positive value with at most 2 decimal places and no more than 16 whole-number digits.'**
  String get invalidContractValue;

  /// No description provided for @invalidDateOrder.
  ///
  /// In en, this message translates to:
  /// **'The expected end date cannot be before the start date.'**
  String get invalidDateOrder;

  /// No description provided for @supervisorPickerEmpty.
  ///
  /// In en, this message translates to:
  /// **'No active supervisors match this search.'**
  String get supervisorPickerEmpty;

  /// No description provided for @updatedAt.
  ///
  /// In en, this message translates to:
  /// **'Updated'**
  String get updatedAt;

  /// No description provided for @assignedAt.
  ///
  /// In en, this message translates to:
  /// **'Assigned'**
  String get assignedAt;

  /// No description provided for @readOnly.
  ///
  /// In en, this message translates to:
  /// **'Read-only'**
  String get readOnly;

  /// No description provided for @navAdvances.
  ///
  /// In en, this message translates to:
  /// **'Advances'**
  String get navAdvances;

  /// No description provided for @advancesTitle.
  ///
  /// In en, this message translates to:
  /// **'Advances'**
  String get advancesTitle;

  /// No description provided for @advanceDetails.
  ///
  /// In en, this message translates to:
  /// **'Advance details'**
  String get advanceDetails;

  /// No description provided for @movementHistory.
  ///
  /// In en, this message translates to:
  /// **'Movement history'**
  String get movementHistory;

  /// No description provided for @personalBalance.
  ///
  /// In en, this message translates to:
  /// **'My advance balances'**
  String get personalBalance;

  /// No description provided for @userBalances.
  ///
  /// In en, this message translates to:
  /// **'User balances'**
  String get userBalances;

  /// No description provided for @fundingSources.
  ///
  /// In en, this message translates to:
  /// **'Funding sources'**
  String get fundingSources;

  /// No description provided for @createAdvance.
  ///
  /// In en, this message translates to:
  /// **'Create advance'**
  String get createAdvance;

  /// No description provided for @distributeMoney.
  ///
  /// In en, this message translates to:
  /// **'Distribute money'**
  String get distributeMoney;

  /// No description provided for @confirmReceipt.
  ///
  /// In en, this message translates to:
  /// **'Confirm receipt'**
  String get confirmReceipt;

  /// No description provided for @rejectTransfer.
  ///
  /// In en, this message translates to:
  /// **'Reject transfer'**
  String get rejectTransfer;

  /// No description provided for @returnUnusedMoney.
  ///
  /// In en, this message translates to:
  /// **'Return unused money'**
  String get returnUnusedMoney;

  /// No description provided for @originalAmount.
  ///
  /// In en, this message translates to:
  /// **'Original amount'**
  String get originalAmount;

  /// No description provided for @availableAmount.
  ///
  /// In en, this message translates to:
  /// **'Available amount'**
  String get availableAmount;

  /// No description provided for @reservedAmount.
  ///
  /// In en, this message translates to:
  /// **'Reserved amount'**
  String get reservedAmount;

  /// No description provided for @currency.
  ///
  /// In en, this message translates to:
  /// **'Currency'**
  String get currency;

  /// No description provided for @fundingSource.
  ///
  /// In en, this message translates to:
  /// **'Funding source'**
  String get fundingSource;

  /// No description provided for @fundingAllocation.
  ///
  /// In en, this message translates to:
  /// **'Funding allocation'**
  String get fundingAllocation;

  /// No description provided for @recipient.
  ///
  /// In en, this message translates to:
  /// **'Recipient'**
  String get recipient;

  /// No description provided for @sender.
  ///
  /// In en, this message translates to:
  /// **'Sender'**
  String get sender;

  /// No description provided for @status.
  ///
  /// In en, this message translates to:
  /// **'Status'**
  String get status;

  /// No description provided for @transferType.
  ///
  /// In en, this message translates to:
  /// **'Transfer type'**
  String get transferType;

  /// No description provided for @paymentMethod.
  ///
  /// In en, this message translates to:
  /// **'Payment method'**
  String get paymentMethod;

  /// No description provided for @pendingConfirmation.
  ///
  /// In en, this message translates to:
  /// **'Pending confirmation'**
  String get pendingConfirmation;

  /// No description provided for @confirmed.
  ///
  /// In en, this message translates to:
  /// **'Confirmed'**
  String get confirmed;

  /// No description provided for @rejected.
  ///
  /// In en, this message translates to:
  /// **'Rejected'**
  String get rejected;

  /// No description provided for @retrySameOperation.
  ///
  /// In en, this message translates to:
  /// **'Retry same operation'**
  String get retrySameOperation;

  /// No description provided for @operationStatusUncertain.
  ///
  /// In en, this message translates to:
  /// **'Operation status uncertain'**
  String get operationStatusUncertain;

  /// No description provided for @idempotentRetryExplanation.
  ///
  /// In en, this message translates to:
  /// **'The request may have reached the server. Retry only the exact same operation; Ahdah will reuse its protected operation identity.'**
  String get idempotentRetryExplanation;

  /// No description provided for @insufficientAvailableBalance.
  ///
  /// In en, this message translates to:
  /// **'The amount exceeds the currently displayed available balance.'**
  String get insufficientAvailableBalance;

  /// No description provided for @staleFinancialState.
  ///
  /// In en, this message translates to:
  /// **'The financial state changed. Reload before trying again.'**
  String get staleFinancialState;

  /// No description provided for @balanceChangedReload.
  ///
  /// In en, this message translates to:
  /// **'Balance changed; reload'**
  String get balanceChangedReload;

  /// No description provided for @returnDestinationDerived.
  ///
  /// In en, this message translates to:
  /// **'The return destination is derived automatically from confirmed upstream transfer history.'**
  String get returnDestinationDerived;

  /// No description provided for @expensesNotImplemented.
  ///
  /// In en, this message translates to:
  /// **'Expenses are not implemented in this phase.'**
  String get expensesNotImplemented;

  /// No description provided for @settlementNotImplemented.
  ///
  /// In en, this message translates to:
  /// **'Settlement and closure are not implemented in this phase.'**
  String get settlementNotImplemented;

  /// No description provided for @noAdvancesTitle.
  ///
  /// In en, this message translates to:
  /// **'No advances found'**
  String get noAdvancesTitle;

  /// No description provided for @noAdvancesBody.
  ///
  /// In en, this message translates to:
  /// **'Change the verified filters or create the first advance.'**
  String get noAdvancesBody;

  /// No description provided for @participantAdvancesBody.
  ///
  /// In en, this message translates to:
  /// **'Only advances involving you appear here.'**
  String get participantAdvancesBody;

  /// No description provided for @searchAdvanceReference.
  ///
  /// In en, this message translates to:
  /// **'Search advance reference'**
  String get searchAdvanceReference;

  /// No description provided for @advanceReference.
  ///
  /// In en, this message translates to:
  /// **'Advance reference'**
  String get advanceReference;

  /// No description provided for @purpose.
  ///
  /// In en, this message translates to:
  /// **'Purpose'**
  String get purpose;

  /// No description provided for @issueDate.
  ///
  /// In en, this message translates to:
  /// **'Issue date'**
  String get issueDate;

  /// No description provided for @settlementDueDateOptional.
  ///
  /// In en, this message translates to:
  /// **'Settlement due date (optional)'**
  String get settlementDueDateOptional;

  /// No description provided for @transferDate.
  ///
  /// In en, this message translates to:
  /// **'Transfer date'**
  String get transferDate;

  /// No description provided for @bankName.
  ///
  /// In en, this message translates to:
  /// **'Bank name'**
  String get bankName;

  /// No description provided for @referenceNumber.
  ///
  /// In en, this message translates to:
  /// **'Reference number'**
  String get referenceNumber;

  /// No description provided for @description.
  ///
  /// In en, this message translates to:
  /// **'Description'**
  String get description;

  /// No description provided for @amount.
  ///
  /// In en, this message translates to:
  /// **'Amount'**
  String get amount;

  /// No description provided for @invalidMoney.
  ///
  /// In en, this message translates to:
  /// **'Enter a positive amount with up to 16 whole digits and 2 decimal places.'**
  String get invalidMoney;

  /// No description provided for @fundingTotalMismatch.
  ///
  /// In en, this message translates to:
  /// **'Funding allocations must exactly equal the advance amount.'**
  String get fundingTotalMismatch;

  /// No description provided for @selectRecipient.
  ///
  /// In en, this message translates to:
  /// **'Select recipient'**
  String get selectRecipient;

  /// No description provided for @noEligibleRecipients.
  ///
  /// In en, this message translates to:
  /// **'No eligible active recipients were found.'**
  String get noEligibleRecipients;

  /// No description provided for @noFundingSources.
  ///
  /// In en, this message translates to:
  /// **'No usable funding sources were returned by the server.'**
  String get noFundingSources;

  /// No description provided for @advanceCreatedPending.
  ///
  /// In en, this message translates to:
  /// **'The advance was created and is pending recipient confirmation.'**
  String get advanceCreatedPending;

  /// No description provided for @distributionPending.
  ///
  /// In en, this message translates to:
  /// **'The distribution is reserved and pending recipient confirmation.'**
  String get distributionPending;

  /// No description provided for @returnPending.
  ///
  /// In en, this message translates to:
  /// **'The return is reserved and pending receiver confirmation.'**
  String get returnPending;

  /// No description provided for @confirmFinancialOperationTitle.
  ///
  /// In en, this message translates to:
  /// **'Confirm this financial operation?'**
  String get confirmFinancialOperationTitle;

  /// No description provided for @confirmReceiptBody.
  ///
  /// In en, this message translates to:
  /// **'Confirm only if you received this exact amount. The server will apply the authoritative balance effect.'**
  String get confirmReceiptBody;

  /// No description provided for @rejectTransferBody.
  ///
  /// In en, this message translates to:
  /// **'Rejecting keeps the transfer history and releases the sender reservation according to server rules.'**
  String get rejectTransferBody;

  /// No description provided for @distributionEffectBody.
  ///
  /// In en, this message translates to:
  /// **'The amount will be reserved until the recipient confirms or rejects it.'**
  String get distributionEffectBody;

  /// No description provided for @cancelUncertainOperation.
  ///
  /// In en, this message translates to:
  /// **'Cancel local retry'**
  String get cancelUncertainOperation;

  /// No description provided for @noMovementsTitle.
  ///
  /// In en, this message translates to:
  /// **'No movements recorded'**
  String get noMovementsTitle;

  /// No description provided for @noMovementsBody.
  ///
  /// In en, this message translates to:
  /// **'The server has not returned any movements for this advance.'**
  String get noMovementsBody;

  /// No description provided for @noBalancesTitle.
  ///
  /// In en, this message translates to:
  /// **'No advance balances'**
  String get noBalancesTitle;

  /// No description provided for @noBalancesBody.
  ///
  /// In en, this message translates to:
  /// **'No authoritative held balances were returned.'**
  String get noBalancesBody;

  /// No description provided for @pendingReservationsNotice.
  ///
  /// In en, this message translates to:
  /// **'Reserved amounts remain unavailable while a supported transfer is pending.'**
  String get pendingReservationsNotice;

  /// No description provided for @authorizedBalanceLookup.
  ///
  /// In en, this message translates to:
  /// **'View a user balance'**
  String get authorizedBalanceLookup;

  /// No description provided for @accountantBalanceLookupLimitation.
  ///
  /// In en, this message translates to:
  /// **'The API requires a user ID, but accountants do not have a safe company-directory selector. Personal balances remain available.'**
  String get accountantBalanceLookupLimitation;

  /// No description provided for @balanceHolder.
  ///
  /// In en, this message translates to:
  /// **'Balance holder'**
  String get balanceHolder;

  /// No description provided for @receivedAmount.
  ///
  /// In en, this message translates to:
  /// **'Received'**
  String get receivedAmount;

  /// No description provided for @restoredAmount.
  ///
  /// In en, this message translates to:
  /// **'Restored'**
  String get restoredAmount;

  /// No description provided for @transferredAmount.
  ///
  /// In en, this message translates to:
  /// **'Transferred out'**
  String get transferredAmount;

  /// No description provided for @returnedAmount.
  ///
  /// In en, this message translates to:
  /// **'Returned'**
  String get returnedAmount;

  /// No description provided for @fundingSourceTypeProjectOwnerPayment.
  ///
  /// In en, this message translates to:
  /// **'Project owner payment'**
  String get fundingSourceTypeProjectOwnerPayment;

  /// No description provided for @fundingSourceTypeManagerContribution.
  ///
  /// In en, this message translates to:
  /// **'Manager contribution'**
  String get fundingSourceTypeManagerContribution;

  /// No description provided for @fundingSourceTypeCompanyCashbox.
  ///
  /// In en, this message translates to:
  /// **'Company cashbox'**
  String get fundingSourceTypeCompanyCashbox;

  /// No description provided for @fundingSourceTypeReturnedAdvance.
  ///
  /// In en, this message translates to:
  /// **'Returned advance'**
  String get fundingSourceTypeReturnedAdvance;

  /// No description provided for @fundingSourceTypeSupplierRefund.
  ///
  /// In en, this message translates to:
  /// **'Supplier refund'**
  String get fundingSourceTypeSupplierRefund;

  /// No description provided for @other.
  ///
  /// In en, this message translates to:
  /// **'Other'**
  String get other;

  /// No description provided for @paymentCash.
  ///
  /// In en, this message translates to:
  /// **'Cash'**
  String get paymentCash;

  /// No description provided for @paymentBankTransfer.
  ///
  /// In en, this message translates to:
  /// **'Bank transfer'**
  String get paymentBankTransfer;

  /// No description provided for @paymentCheque.
  ///
  /// In en, this message translates to:
  /// **'Cheque'**
  String get paymentCheque;

  /// No description provided for @paymentCard.
  ///
  /// In en, this message translates to:
  /// **'Card'**
  String get paymentCard;

  /// No description provided for @paymentMobileWallet.
  ///
  /// In en, this message translates to:
  /// **'Mobile wallet'**
  String get paymentMobileWallet;

  /// No description provided for @paymentBalanceTransfer.
  ///
  /// In en, this message translates to:
  /// **'Balance transfer'**
  String get paymentBalanceTransfer;

  /// No description provided for @advanceStatusDraft.
  ///
  /// In en, this message translates to:
  /// **'Draft'**
  String get advanceStatusDraft;

  /// No description provided for @advanceStatusOpen.
  ///
  /// In en, this message translates to:
  /// **'Open'**
  String get advanceStatusOpen;

  /// No description provided for @advanceStatusInSettlement.
  ///
  /// In en, this message translates to:
  /// **'In settlement'**
  String get advanceStatusInSettlement;

  /// No description provided for @advanceStatusReadyToClose.
  ///
  /// In en, this message translates to:
  /// **'Ready to close'**
  String get advanceStatusReadyToClose;

  /// No description provided for @advanceStatusClosed.
  ///
  /// In en, this message translates to:
  /// **'Closed'**
  String get advanceStatusClosed;

  /// No description provided for @advanceStatusCancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get advanceStatusCancelled;

  /// No description provided for @advanceStatusReversed.
  ///
  /// In en, this message translates to:
  /// **'Reversed'**
  String get advanceStatusReversed;

  /// No description provided for @transferStatusCorrectionRequired.
  ///
  /// In en, this message translates to:
  /// **'Correction required'**
  String get transferStatusCorrectionRequired;

  /// No description provided for @transferTypeAdvanceDelivery.
  ///
  /// In en, this message translates to:
  /// **'Initial advance delivery'**
  String get transferTypeAdvanceDelivery;

  /// No description provided for @transferTypeInternalTransfer.
  ///
  /// In en, this message translates to:
  /// **'Internal distribution'**
  String get transferTypeInternalTransfer;

  /// No description provided for @transferTypeBalanceReturn.
  ///
  /// In en, this message translates to:
  /// **'Balance return'**
  String get transferTypeBalanceReturn;

  /// No description provided for @movementFundingAllocation.
  ///
  /// In en, this message translates to:
  /// **'Funding allocation'**
  String get movementFundingAllocation;

  /// No description provided for @fundingStatusAvailable.
  ///
  /// In en, this message translates to:
  /// **'Available'**
  String get fundingStatusAvailable;

  /// No description provided for @fundingStatusPartiallyUsed.
  ///
  /// In en, this message translates to:
  /// **'Partially used'**
  String get fundingStatusPartiallyUsed;

  /// No description provided for @fundingStatusFullyUsed.
  ///
  /// In en, this message translates to:
  /// **'Fully used'**
  String get fundingStatusFullyUsed;

  /// No description provided for @fundingStatusPendingVerification.
  ///
  /// In en, this message translates to:
  /// **'Pending verification'**
  String get fundingStatusPendingVerification;

  /// No description provided for @balanceStatusInSettlement.
  ///
  /// In en, this message translates to:
  /// **'In settlement'**
  String get balanceStatusInSettlement;

  /// No description provided for @balanceStatusSettled.
  ///
  /// In en, this message translates to:
  /// **'Settled'**
  String get balanceStatusSettled;

  /// No description provided for @balanceStatusClosed.
  ///
  /// In en, this message translates to:
  /// **'Closed'**
  String get balanceStatusClosed;

  /// No description provided for @selectUser.
  ///
  /// In en, this message translates to:
  /// **'Select user'**
  String get selectUser;

  /// No description provided for @viewMovements.
  ///
  /// In en, this message translates to:
  /// **'View movements'**
  String get viewMovements;

  /// No description provided for @financialOperationSucceeded.
  ///
  /// In en, this message translates to:
  /// **'The financial operation completed successfully.'**
  String get financialOperationSucceeded;

  /// No description provided for @navExpenses.
  ///
  /// In en, this message translates to:
  /// **'Expenses'**
  String get navExpenses;

  /// No description provided for @expensesTitle.
  ///
  /// In en, this message translates to:
  /// **'Expenses'**
  String get expensesTitle;

  /// No description provided for @expenseDetails.
  ///
  /// In en, this message translates to:
  /// **'Expense details'**
  String get expenseDetails;

  /// No description provided for @createExpense.
  ///
  /// In en, this message translates to:
  /// **'Create expense'**
  String get createExpense;

  /// No description provided for @expenseCategories.
  ///
  /// In en, this message translates to:
  /// **'Expense categories'**
  String get expenseCategories;

  /// No description provided for @createExpenseCategory.
  ///
  /// In en, this message translates to:
  /// **'Create expense category'**
  String get createExpenseCategory;

  /// No description provided for @reimbursements.
  ///
  /// In en, this message translates to:
  /// **'Reimbursements'**
  String get reimbursements;

  /// No description provided for @reimbursementDetails.
  ///
  /// In en, this message translates to:
  /// **'Reimbursement details'**
  String get reimbursementDetails;

  /// No description provided for @expenseHistory.
  ///
  /// In en, this message translates to:
  /// **'Expense history'**
  String get expenseHistory;

  /// No description provided for @documentMetadata.
  ///
  /// In en, this message translates to:
  /// **'Document metadata'**
  String get documentMetadata;

  /// No description provided for @binaryUploadNotImplemented.
  ///
  /// In en, this message translates to:
  /// **'File upload is not implemented in this phase. These are document metadata records only.'**
  String get binaryUploadNotImplemented;

  /// No description provided for @paymentMode.
  ///
  /// In en, this message translates to:
  /// **'Payment mode'**
  String get paymentMode;

  /// No description provided for @advanceBalance.
  ///
  /// In en, this message translates to:
  /// **'Advance balance'**
  String get advanceBalance;

  /// No description provided for @personalFunds.
  ///
  /// In en, this message translates to:
  /// **'Personal funds'**
  String get personalFunds;

  /// No description provided for @personalFundsClaimNotice.
  ///
  /// In en, this message translates to:
  /// **'A reimbursement claim will be created as an unpaid liability.'**
  String get personalFundsClaimNotice;

  /// No description provided for @advanceReservationNotice.
  ///
  /// In en, this message translates to:
  /// **'The allocation is reserved until approval. Approval confirms it; rejection releases it.'**
  String get advanceReservationNotice;

  /// No description provided for @expenseReference.
  ///
  /// In en, this message translates to:
  /// **'Expense reference'**
  String get expenseReference;

  /// No description provided for @searchExpenseReference.
  ///
  /// In en, this message translates to:
  /// **'Search expense reference'**
  String get searchExpenseReference;

  /// No description provided for @expenseDate.
  ///
  /// In en, this message translates to:
  /// **'Expense date'**
  String get expenseDate;

  /// No description provided for @category.
  ///
  /// In en, this message translates to:
  /// **'Category'**
  String get category;

  /// No description provided for @categoryName.
  ///
  /// In en, this message translates to:
  /// **'Category name'**
  String get categoryName;

  /// No description provided for @categoryCodeOptional.
  ///
  /// In en, this message translates to:
  /// **'Category code (optional)'**
  String get categoryCodeOptional;

  /// No description provided for @categoryGroup.
  ///
  /// In en, this message translates to:
  /// **'Category group'**
  String get categoryGroup;

  /// No description provided for @categoryScope.
  ///
  /// In en, this message translates to:
  /// **'Category scope'**
  String get categoryScope;

  /// No description provided for @requiresReceipt.
  ///
  /// In en, this message translates to:
  /// **'Requires supporting receipt'**
  String get requiresReceipt;

  /// No description provided for @requiresSupplier.
  ///
  /// In en, this message translates to:
  /// **'Requires supplier'**
  String get requiresSupplier;

  /// No description provided for @supportsQuantityDetails.
  ///
  /// In en, this message translates to:
  /// **'Supports quantity details'**
  String get supportsQuantityDetails;

  /// No description provided for @displayOrder.
  ///
  /// In en, this message translates to:
  /// **'Display order'**
  String get displayOrder;

  /// No description provided for @receiptNumberOptional.
  ///
  /// In en, this message translates to:
  /// **'Receipt number (optional)'**
  String get receiptNumberOptional;

  /// No description provided for @invoiceNumberOptional.
  ///
  /// In en, this message translates to:
  /// **'Invoice number (optional)'**
  String get invoiceNumberOptional;

  /// No description provided for @merchantNameOptional.
  ///
  /// In en, this message translates to:
  /// **'Merchant name (optional)'**
  String get merchantNameOptional;

  /// No description provided for @expenseLocationOptional.
  ///
  /// In en, this message translates to:
  /// **'Expense location (optional)'**
  String get expenseLocationOptional;

  /// No description provided for @projectOptional.
  ///
  /// In en, this message translates to:
  /// **'Project (optional)'**
  String get projectOptional;

  /// No description provided for @companyExpense.
  ///
  /// In en, this message translates to:
  /// **'Company expense'**
  String get companyExpense;

  /// No description provided for @projectExpense.
  ///
  /// In en, this message translates to:
  /// **'Project expense'**
  String get projectExpense;

  /// No description provided for @incurredBy.
  ///
  /// In en, this message translates to:
  /// **'Payer'**
  String get incurredBy;

  /// No description provided for @submittedBy.
  ///
  /// In en, this message translates to:
  /// **'Submitted by'**
  String get submittedBy;

  /// No description provided for @reviewedBy.
  ///
  /// In en, this message translates to:
  /// **'Reviewed by'**
  String get reviewedBy;

  /// No description provided for @reviewExpense.
  ///
  /// In en, this message translates to:
  /// **'Review expense'**
  String get reviewExpense;

  /// No description provided for @approveExpense.
  ///
  /// In en, this message translates to:
  /// **'Approve expense'**
  String get approveExpense;

  /// No description provided for @rejectExpense.
  ///
  /// In en, this message translates to:
  /// **'Reject expense'**
  String get rejectExpense;

  /// No description provided for @approveExpenseNotice.
  ///
  /// In en, this message translates to:
  /// **'Approval confirms reserved advance allocations. Personal claims remain unpaid liabilities.'**
  String get approveExpenseNotice;

  /// No description provided for @rejectExpenseNotice.
  ///
  /// In en, this message translates to:
  /// **'Rejection releases advance reservations and may cancel an untouched claim. The expense remains in history.'**
  String get rejectExpenseNotice;

  /// No description provided for @claimUnpaid.
  ///
  /// In en, this message translates to:
  /// **'Claim unpaid'**
  String get claimUnpaid;

  /// No description provided for @outstandingAmount.
  ///
  /// In en, this message translates to:
  /// **'Outstanding amount'**
  String get outstandingAmount;

  /// No description provided for @claimant.
  ///
  /// In en, this message translates to:
  /// **'Claimant'**
  String get claimant;

  /// No description provided for @noExpensesTitle.
  ///
  /// In en, this message translates to:
  /// **'No expenses found'**
  String get noExpensesTitle;

  /// No description provided for @noExpensesBody.
  ///
  /// In en, this message translates to:
  /// **'Change the verified filters or create the first expense.'**
  String get noExpensesBody;

  /// No description provided for @supervisorExpensesBody.
  ///
  /// In en, this message translates to:
  /// **'Only your expenses and expenses for actively assigned projects appear here.'**
  String get supervisorExpensesBody;

  /// No description provided for @workerExpensesBody.
  ///
  /// In en, this message translates to:
  /// **'Only your personal expenses appear here.'**
  String get workerExpensesBody;

  /// No description provided for @noReimbursements.
  ///
  /// In en, this message translates to:
  /// **'No reimbursements were returned.'**
  String get noReimbursements;

  /// No description provided for @noCategories.
  ///
  /// In en, this message translates to:
  /// **'No active expense categories were returned.'**
  String get noCategories;

  /// No description provided for @noDocuments.
  ///
  /// In en, this message translates to:
  /// **'No document metadata records were returned.'**
  String get noDocuments;

  /// No description provided for @noExpenseHistory.
  ///
  /// In en, this message translates to:
  /// **'No expense history events were returned.'**
  String get noExpenseHistory;

  /// No description provided for @allocationTotalMismatch.
  ///
  /// In en, this message translates to:
  /// **'Advance allocations must exactly equal the expense amount.'**
  String get allocationTotalMismatch;

  /// No description provided for @selectAdvanceBalance.
  ///
  /// In en, this message translates to:
  /// **'Select an advance balance'**
  String get selectAdvanceBalance;

  /// No description provided for @supportingDocumentRequired.
  ///
  /// In en, this message translates to:
  /// **'A supporting Receipt or Invoice metadata record may be required before approval.'**
  String get supportingDocumentRequired;

  /// No description provided for @expenseCreatedPending.
  ///
  /// In en, this message translates to:
  /// **'The expense was created and is pending review.'**
  String get expenseCreatedPending;

  /// No description provided for @expenseStatusPendingReview.
  ///
  /// In en, this message translates to:
  /// **'Pending review'**
  String get expenseStatusPendingReview;

  /// No description provided for @expenseStatusCorrectionRequired.
  ///
  /// In en, this message translates to:
  /// **'Correction required'**
  String get expenseStatusCorrectionRequired;

  /// No description provided for @expenseStatusApproved.
  ///
  /// In en, this message translates to:
  /// **'Approved'**
  String get expenseStatusApproved;

  /// No description provided for @expenseStatusRejected.
  ///
  /// In en, this message translates to:
  /// **'Rejected'**
  String get expenseStatusRejected;

  /// No description provided for @expenseStatusCancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get expenseStatusCancelled;

  /// No description provided for @expenseStatusReversed.
  ///
  /// In en, this message translates to:
  /// **'Reversed'**
  String get expenseStatusReversed;

  /// No description provided for @categoryGroupMaterials.
  ///
  /// In en, this message translates to:
  /// **'Materials'**
  String get categoryGroupMaterials;

  /// No description provided for @categoryGroupLabor.
  ///
  /// In en, this message translates to:
  /// **'Labor'**
  String get categoryGroupLabor;

  /// No description provided for @categoryGroupSubcontracting.
  ///
  /// In en, this message translates to:
  /// **'Subcontracting'**
  String get categoryGroupSubcontracting;

  /// No description provided for @categoryGroupTransportation.
  ///
  /// In en, this message translates to:
  /// **'Transportation'**
  String get categoryGroupTransportation;

  /// No description provided for @categoryGroupEquipment.
  ///
  /// In en, this message translates to:
  /// **'Equipment'**
  String get categoryGroupEquipment;

  /// No description provided for @categoryGroupFuel.
  ///
  /// In en, this message translates to:
  /// **'Fuel'**
  String get categoryGroupFuel;

  /// No description provided for @categoryGroupServices.
  ///
  /// In en, this message translates to:
  /// **'Services'**
  String get categoryGroupServices;

  /// No description provided for @categoryGroupAdministrative.
  ///
  /// In en, this message translates to:
  /// **'Administrative'**
  String get categoryGroupAdministrative;

  /// No description provided for @categoryGroupUtilities.
  ///
  /// In en, this message translates to:
  /// **'Utilities'**
  String get categoryGroupUtilities;

  /// No description provided for @categoryGroupPermits.
  ///
  /// In en, this message translates to:
  /// **'Permits'**
  String get categoryGroupPermits;

  /// No description provided for @scopeProjectOnly.
  ///
  /// In en, this message translates to:
  /// **'Project only'**
  String get scopeProjectOnly;

  /// No description provided for @scopeCompanyOnly.
  ///
  /// In en, this message translates to:
  /// **'Company only'**
  String get scopeCompanyOnly;

  /// No description provided for @scopeBoth.
  ///
  /// In en, this message translates to:
  /// **'Project or company'**
  String get scopeBoth;

  /// No description provided for @documentTypeReceipt.
  ///
  /// In en, this message translates to:
  /// **'Receipt'**
  String get documentTypeReceipt;

  /// No description provided for @documentTypeInvoice.
  ///
  /// In en, this message translates to:
  /// **'Invoice'**
  String get documentTypeInvoice;

  /// No description provided for @documentTypeQuotation.
  ///
  /// In en, this message translates to:
  /// **'Quotation'**
  String get documentTypeQuotation;

  /// No description provided for @documentTypeDeliveryNote.
  ///
  /// In en, this message translates to:
  /// **'Delivery note'**
  String get documentTypeDeliveryNote;

  /// No description provided for @documentTypePaymentProof.
  ///
  /// In en, this message translates to:
  /// **'Payment proof'**
  String get documentTypePaymentProof;

  /// No description provided for @documentTypeContract.
  ///
  /// In en, this message translates to:
  /// **'Contract'**
  String get documentTypeContract;

  /// No description provided for @documentTypePurchaseOrder.
  ///
  /// In en, this message translates to:
  /// **'Purchase order'**
  String get documentTypePurchaseOrder;

  /// No description provided for @documentStatusPendingVerification.
  ///
  /// In en, this message translates to:
  /// **'Pending verification'**
  String get documentStatusPendingVerification;

  /// No description provided for @documentStatusVerified.
  ///
  /// In en, this message translates to:
  /// **'Verified'**
  String get documentStatusVerified;

  /// No description provided for @claimStatusOpen.
  ///
  /// In en, this message translates to:
  /// **'Open'**
  String get claimStatusOpen;

  /// No description provided for @claimStatusPartiallySettled.
  ///
  /// In en, this message translates to:
  /// **'Partially settled'**
  String get claimStatusPartiallySettled;

  /// No description provided for @claimStatusSettled.
  ///
  /// In en, this message translates to:
  /// **'Settled'**
  String get claimStatusSettled;

  /// No description provided for @claimStatusCancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get claimStatusCancelled;

  /// No description provided for @claimStatusReversed.
  ///
  /// In en, this message translates to:
  /// **'Reversed'**
  String get claimStatusReversed;

  /// No description provided for @receiptNumber.
  ///
  /// In en, this message translates to:
  /// **'Receipt number'**
  String get receiptNumber;

  /// No description provided for @invoiceNumber.
  ///
  /// In en, this message translates to:
  /// **'Invoice number'**
  String get invoiceNumber;

  /// No description provided for @advanceAllocations.
  ///
  /// In en, this message translates to:
  /// **'Advance allocations'**
  String get advanceAllocations;

  /// No description provided for @expenseItems.
  ///
  /// In en, this message translates to:
  /// **'Expense items'**
  String get expenseItems;

  /// No description provided for @documents.
  ///
  /// In en, this message translates to:
  /// **'Documents'**
  String get documents;

  /// No description provided for @history.
  ///
  /// In en, this message translates to:
  /// **'History'**
  String get history;

  /// No description provided for @fileName.
  ///
  /// In en, this message translates to:
  /// **'File name'**
  String get fileName;

  /// No description provided for @fileSize.
  ///
  /// In en, this message translates to:
  /// **'File size'**
  String get fileSize;

  /// No description provided for @verificationStatus.
  ///
  /// In en, this message translates to:
  /// **'Verification status'**
  String get verificationStatus;

  /// No description provided for @claimDate.
  ///
  /// In en, this message translates to:
  /// **'Claim date'**
  String get claimDate;

  /// No description provided for @dueDate.
  ///
  /// In en, this message translates to:
  /// **'Due date'**
  String get dueDate;

  /// No description provided for @categoryCreated.
  ///
  /// In en, this message translates to:
  /// **'The expense category was created.'**
  String get categoryCreated;

  /// No description provided for @expenseApproved.
  ///
  /// In en, this message translates to:
  /// **'The expense was approved.'**
  String get expenseApproved;

  /// No description provided for @expenseRejected.
  ///
  /// In en, this message translates to:
  /// **'The expense was rejected.'**
  String get expenseRejected;

  /// No description provided for @historyExpenseCreated.
  ///
  /// In en, this message translates to:
  /// **'Expense submitted for review'**
  String get historyExpenseCreated;

  /// No description provided for @historyDocumentAdded.
  ///
  /// In en, this message translates to:
  /// **'Supporting document metadata added'**
  String get historyDocumentAdded;

  /// No description provided for @historyExpenseApproved.
  ///
  /// In en, this message translates to:
  /// **'Expense approved'**
  String get historyExpenseApproved;

  /// No description provided for @historyExpenseRejected.
  ///
  /// In en, this message translates to:
  /// **'Expense rejected'**
  String get historyExpenseRejected;

  /// No description provided for @historyOutcomeSuccess.
  ///
  /// In en, this message translates to:
  /// **'Succeeded'**
  String get historyOutcomeSuccess;
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
