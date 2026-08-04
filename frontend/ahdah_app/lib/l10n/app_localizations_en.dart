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
      'Your secure Ahdah session, company tools, advances, and authoritative balance workflows are ready.';

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
      'Expenses, receipts, suppliers, settlements, closure, debts, and reporting remain deferred.';

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

  @override
  String get navProjects => 'Projects';

  @override
  String get navCompanyMembers => 'Company members';

  @override
  String get projectsTitle => 'Projects and sites';

  @override
  String get companyMembersTitle => 'Company member directory';

  @override
  String get searchProjects => 'Search project name or site address';

  @override
  String get searchMembers => 'Search name or phone';

  @override
  String get searchMinimum => 'Enter at least 2 characters to search.';

  @override
  String get roleFilter => 'Role filter';

  @override
  String get allRoles => 'All roles';

  @override
  String get projectStatusActive => 'Active';

  @override
  String get projectStatusPaused => 'Paused';

  @override
  String get projectStatusCompleted => 'Completed';

  @override
  String get projectStatusFinanciallyClosed => 'Financially closed';

  @override
  String get projectStatusCancelled => 'Cancelled';

  @override
  String get projectName => 'Project name';

  @override
  String get siteAddress => 'Site address';

  @override
  String get ownerClient => 'Project owner / client';

  @override
  String get ownerName => 'Owner name';

  @override
  String get contractValue => 'Contract value';

  @override
  String get contractValueLyd => 'Contract value (LYD)';

  @override
  String get contractDate => 'Contract date';

  @override
  String get startDate => 'Start date';

  @override
  String get expectedEndDate => 'Expected end date';

  @override
  String get actualEndDate => 'Actual end date';

  @override
  String get descriptionOptional => 'Description (optional)';

  @override
  String get notesOptional => 'Notes (optional)';

  @override
  String get contactPhoneOptional => 'Project contact phone (optional)';

  @override
  String get addressOptional => 'Address (optional)';

  @override
  String get assignedSupervisor => 'Assigned supervisor';

  @override
  String get assignedSupervisors => 'Assigned supervisors';

  @override
  String get createProject => 'Create project';

  @override
  String get editProject => 'Edit project';

  @override
  String get assignSupervisor => 'Assign supervisor';

  @override
  String get replaceSupervisor => 'Replace supervisor';

  @override
  String get projectDetails => 'Project details';

  @override
  String get projectMembersTitle => 'Project supervisors';

  @override
  String get activeSupervisionAssignmentsBody =>
      'This view contains active supervision assignments only. It is not a complete project staff directory.';

  @override
  String get noProjectsTitle => 'No projects found';

  @override
  String get noProjectsBody =>
      'Change the filters or create the first project.';

  @override
  String get noAssignedProjectsBody =>
      'Only projects currently assigned to you appear here.';

  @override
  String get noMembersTitle => 'No company members found';

  @override
  String get noMembersBody => 'Change the role, status, or search filter.';

  @override
  String get memberDetails => 'Member details';

  @override
  String get directoryReadOnly => 'This company directory is read-only.';

  @override
  String get projectCreated => 'The project was created.';

  @override
  String get projectUpdated => 'The project was updated.';

  @override
  String get supervisorAssigned => 'The supervisor assignment was updated.';

  @override
  String get projectChangedConflict =>
      'This project changed on the server. Reload it before trying again.';

  @override
  String get reloadProject => 'Reload project';

  @override
  String get newOwnerTitle => 'New client / owner';

  @override
  String get existingOwnerUnavailable =>
      'A safe owner directory is not available yet. This form creates the owner and project atomically.';

  @override
  String get chooseSupervisor => 'Choose supervisor';

  @override
  String get optionalSupervisor => 'Initial supervisor (optional)';

  @override
  String get noSupervisor => 'No active supervisor';

  @override
  String get replaceSupervisorTitle => 'Replace the active supervisor?';

  @override
  String get replaceSupervisorBody =>
      'The previous assignment will be ended, not deleted. The server preserves assignment history.';

  @override
  String get supervisorHistoryNotice =>
      'Replacing a supervisor preserves earlier assignment history on the server.';

  @override
  String get saveAction => 'Save';

  @override
  String get selectDate => 'Select date';

  @override
  String get clearAction => 'Clear';

  @override
  String get viewDetails => 'View details';

  @override
  String get invalidRequiredText =>
      'Enter a non-blank value within the allowed length.';

  @override
  String get invalidContractValue =>
      'Enter a positive value with at most 2 decimal places and no more than 16 whole-number digits.';

  @override
  String get invalidDateOrder =>
      'The expected end date cannot be before the start date.';

  @override
  String get supervisorPickerEmpty =>
      'No active supervisors match this search.';

  @override
  String get updatedAt => 'Updated';

  @override
  String get assignedAt => 'Assigned';

  @override
  String get readOnly => 'Read-only';

  @override
  String get navAdvances => 'Advances';

  @override
  String get advancesTitle => 'Advances';

  @override
  String get advanceDetails => 'Advance details';

  @override
  String get movementHistory => 'Movement history';

  @override
  String get personalBalance => 'My advance balances';

  @override
  String get userBalances => 'User balances';

  @override
  String get fundingSources => 'Funding sources';

  @override
  String get createAdvance => 'Create advance';

  @override
  String get distributeMoney => 'Distribute money';

  @override
  String get confirmReceipt => 'Confirm receipt';

  @override
  String get rejectTransfer => 'Reject transfer';

  @override
  String get returnUnusedMoney => 'Return unused money';

  @override
  String get originalAmount => 'Original amount';

  @override
  String get availableAmount => 'Available amount';

  @override
  String get reservedAmount => 'Reserved amount';

  @override
  String get currency => 'Currency';

  @override
  String get fundingSource => 'Funding source';

  @override
  String get fundingAllocation => 'Funding allocation';

  @override
  String get recipient => 'Recipient';

  @override
  String get sender => 'Sender';

  @override
  String get status => 'Status';

  @override
  String get transferType => 'Transfer type';

  @override
  String get paymentMethod => 'Payment method';

  @override
  String get pendingConfirmation => 'Pending confirmation';

  @override
  String get confirmed => 'Confirmed';

  @override
  String get rejected => 'Rejected';

  @override
  String get retrySameOperation => 'Retry same operation';

  @override
  String get operationStatusUncertain => 'Operation status uncertain';

  @override
  String get idempotentRetryExplanation =>
      'The request may have reached the server. Retry only the exact same operation; Ahdah will reuse its protected operation identity.';

  @override
  String get insufficientAvailableBalance =>
      'The amount exceeds the currently displayed available balance.';

  @override
  String get staleFinancialState =>
      'The financial state changed. Reload before trying again.';

  @override
  String get balanceChangedReload => 'Balance changed; reload';

  @override
  String get returnDestinationDerived =>
      'The return destination is derived automatically from confirmed upstream transfer history.';

  @override
  String get expensesNotImplemented =>
      'Expenses are not implemented in this phase.';

  @override
  String get settlementNotImplemented =>
      'Settlement and closure are not implemented in this phase.';

  @override
  String get noAdvancesTitle => 'No advances found';

  @override
  String get noAdvancesBody =>
      'Change the verified filters or create the first advance.';

  @override
  String get participantAdvancesBody =>
      'Only advances involving you appear here.';

  @override
  String get searchAdvanceReference => 'Search advance reference';

  @override
  String get advanceReference => 'Advance reference';

  @override
  String get purpose => 'Purpose';

  @override
  String get issueDate => 'Issue date';

  @override
  String get settlementDueDateOptional => 'Settlement due date (optional)';

  @override
  String get transferDate => 'Transfer date';

  @override
  String get bankName => 'Bank name';

  @override
  String get referenceNumber => 'Reference number';

  @override
  String get description => 'Description';

  @override
  String get amount => 'Amount';

  @override
  String get invalidMoney =>
      'Enter a positive amount with up to 16 whole digits and 2 decimal places.';

  @override
  String get fundingTotalMismatch =>
      'Funding allocations must exactly equal the advance amount.';

  @override
  String get selectRecipient => 'Select recipient';

  @override
  String get noEligibleRecipients =>
      'No eligible active recipients were found.';

  @override
  String get noFundingSources =>
      'No usable funding sources were returned by the server.';

  @override
  String get advanceCreatedPending =>
      'The advance was created and is pending recipient confirmation.';

  @override
  String get distributionPending =>
      'The distribution is reserved and pending recipient confirmation.';

  @override
  String get returnPending =>
      'The return is reserved and pending receiver confirmation.';

  @override
  String get confirmFinancialOperationTitle =>
      'Confirm this financial operation?';

  @override
  String get confirmReceiptBody =>
      'Confirm only if you received this exact amount. The server will apply the authoritative balance effect.';

  @override
  String get rejectTransferBody =>
      'Rejecting keeps the transfer history and releases the sender reservation according to server rules.';

  @override
  String get distributionEffectBody =>
      'The amount will be reserved until the recipient confirms or rejects it.';

  @override
  String get cancelUncertainOperation => 'Cancel local retry';

  @override
  String get noMovementsTitle => 'No movements recorded';

  @override
  String get noMovementsBody =>
      'The server has not returned any movements for this advance.';

  @override
  String get noBalancesTitle => 'No advance balances';

  @override
  String get noBalancesBody => 'No authoritative held balances were returned.';

  @override
  String get pendingReservationsNotice =>
      'Reserved amounts remain unavailable while a supported transfer is pending.';

  @override
  String get authorizedBalanceLookup => 'View a user balance';

  @override
  String get accountantBalanceLookupLimitation =>
      'The API requires a user ID, but accountants do not have a safe company-directory selector. Personal balances remain available.';

  @override
  String get balanceHolder => 'Balance holder';

  @override
  String get receivedAmount => 'Received';

  @override
  String get restoredAmount => 'Restored';

  @override
  String get transferredAmount => 'Transferred out';

  @override
  String get returnedAmount => 'Returned';

  @override
  String get fundingSourceTypeProjectOwnerPayment => 'Project owner payment';

  @override
  String get fundingSourceTypeManagerContribution => 'Manager contribution';

  @override
  String get fundingSourceTypeCompanyCashbox => 'Company cashbox';

  @override
  String get fundingSourceTypeReturnedAdvance => 'Returned advance';

  @override
  String get fundingSourceTypeSupplierRefund => 'Supplier refund';

  @override
  String get other => 'Other';

  @override
  String get paymentCash => 'Cash';

  @override
  String get paymentBankTransfer => 'Bank transfer';

  @override
  String get paymentCheque => 'Cheque';

  @override
  String get paymentCard => 'Card';

  @override
  String get paymentMobileWallet => 'Mobile wallet';

  @override
  String get paymentBalanceTransfer => 'Balance transfer';

  @override
  String get advanceStatusDraft => 'Draft';

  @override
  String get advanceStatusOpen => 'Open';

  @override
  String get advanceStatusInSettlement => 'In settlement';

  @override
  String get advanceStatusReadyToClose => 'Ready to close';

  @override
  String get advanceStatusClosed => 'Closed';

  @override
  String get advanceStatusCancelled => 'Cancelled';

  @override
  String get advanceStatusReversed => 'Reversed';

  @override
  String get transferStatusCorrectionRequired => 'Correction required';

  @override
  String get transferTypeAdvanceDelivery => 'Initial advance delivery';

  @override
  String get transferTypeInternalTransfer => 'Internal distribution';

  @override
  String get transferTypeBalanceReturn => 'Balance return';

  @override
  String get movementFundingAllocation => 'Funding allocation';

  @override
  String get fundingStatusAvailable => 'Available';

  @override
  String get fundingStatusPartiallyUsed => 'Partially used';

  @override
  String get fundingStatusFullyUsed => 'Fully used';

  @override
  String get fundingStatusPendingVerification => 'Pending verification';

  @override
  String get balanceStatusInSettlement => 'In settlement';

  @override
  String get balanceStatusSettled => 'Settled';

  @override
  String get balanceStatusClosed => 'Closed';

  @override
  String get selectUser => 'Select user';

  @override
  String get viewMovements => 'View movements';

  @override
  String get financialOperationSucceeded =>
      'The financial operation completed successfully.';
}
