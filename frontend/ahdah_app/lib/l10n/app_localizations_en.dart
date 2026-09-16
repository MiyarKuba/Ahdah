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
      'Enter a positive amount with no more than two decimal places.';

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

  @override
  String get navExpenses => 'Expenses';

  @override
  String get expensesTitle => 'Expenses';

  @override
  String get expenseDetails => 'Expense details';

  @override
  String get createExpense => 'Create expense';

  @override
  String get expenseCategories => 'Expense categories';

  @override
  String get createExpenseCategory => 'Create expense category';

  @override
  String get reimbursements => 'Reimbursements';

  @override
  String get reimbursementDetails => 'Reimbursement details';

  @override
  String get expenseHistory => 'Expense history';

  @override
  String get documentMetadata => 'Document metadata';

  @override
  String get binaryUploadNotImplemented =>
      'File upload is not implemented in this phase. These are document metadata records only.';

  @override
  String get paymentMode => 'Payment mode';

  @override
  String get advanceBalance => 'Advance balance';

  @override
  String get personalFunds => 'Personal funds';

  @override
  String get personalFundsClaimNotice =>
      'A reimbursement claim will be created as an unpaid liability.';

  @override
  String get advanceReservationNotice =>
      'The allocation is reserved until approval. Approval confirms it; rejection releases it.';

  @override
  String get expenseReference => 'Expense reference';

  @override
  String get searchExpenseReference => 'Search expense reference';

  @override
  String get expenseDate => 'Expense date';

  @override
  String get category => 'Category';

  @override
  String get categoryName => 'Category name';

  @override
  String get categoryCodeOptional => 'Category code (optional)';

  @override
  String get categoryGroup => 'Category group';

  @override
  String get categoryScope => 'Category scope';

  @override
  String get requiresReceipt => 'Requires supporting receipt';

  @override
  String get requiresSupplier => 'Requires supplier';

  @override
  String get supportsQuantityDetails => 'Supports quantity details';

  @override
  String get displayOrder => 'Display order';

  @override
  String get receiptNumberOptional => 'Receipt number (optional)';

  @override
  String get invoiceNumberOptional => 'Invoice number (optional)';

  @override
  String get merchantNameOptional => 'Merchant name (optional)';

  @override
  String get expenseLocationOptional => 'Expense location (optional)';

  @override
  String get projectOptional => 'Project (optional)';

  @override
  String get companyExpense => 'Company expense';

  @override
  String get projectExpense => 'Project expense';

  @override
  String get incurredBy => 'Payer';

  @override
  String get submittedBy => 'Submitted by';

  @override
  String get reviewedBy => 'Reviewed by';

  @override
  String get reviewExpense => 'Review expense';

  @override
  String get approveExpense => 'Approve expense';

  @override
  String get rejectExpense => 'Reject expense';

  @override
  String get approveExpenseNotice =>
      'Approval confirms reserved advance allocations. Personal claims remain unpaid liabilities.';

  @override
  String get rejectExpenseNotice =>
      'Rejection releases advance reservations and may cancel an untouched claim. The expense remains in history.';

  @override
  String get claimUnpaid => 'Claim unpaid';

  @override
  String get outstandingAmount => 'Outstanding amount';

  @override
  String get claimant => 'Claimant';

  @override
  String get noExpensesTitle => 'No expenses found';

  @override
  String get noExpensesBody =>
      'Change the verified filters or create the first expense.';

  @override
  String get supervisorExpensesBody =>
      'Only your expenses and expenses for actively assigned projects appear here.';

  @override
  String get workerExpensesBody => 'Only your personal expenses appear here.';

  @override
  String get noReimbursements => 'No reimbursements were returned.';

  @override
  String get noCategories => 'No active expense categories were returned.';

  @override
  String get noDocuments => 'No document metadata records were returned.';

  @override
  String get noExpenseHistory => 'No expense history events were returned.';

  @override
  String get allocationTotalMismatch =>
      'Advance allocations must exactly equal the expense amount.';

  @override
  String get selectAdvanceBalance => 'Select an advance balance';

  @override
  String get supportingDocumentRequired =>
      'A supporting Receipt or Invoice metadata record may be required before approval.';

  @override
  String get expenseCreatedPending =>
      'The expense was created and is pending review.';

  @override
  String get expenseStatusPendingReview => 'Pending review';

  @override
  String get expenseStatusCorrectionRequired => 'Correction required';

  @override
  String get expenseStatusApproved => 'Approved';

  @override
  String get expenseStatusRejected => 'Rejected';

  @override
  String get expenseStatusCancelled => 'Cancelled';

  @override
  String get expenseStatusReversed => 'Reversed';

  @override
  String get categoryGroupMaterials => 'Materials';

  @override
  String get categoryGroupLabor => 'Labor';

  @override
  String get categoryGroupSubcontracting => 'Subcontracting';

  @override
  String get categoryGroupTransportation => 'Transportation';

  @override
  String get categoryGroupEquipment => 'Equipment';

  @override
  String get categoryGroupFuel => 'Fuel';

  @override
  String get categoryGroupServices => 'Services';

  @override
  String get categoryGroupAdministrative => 'Administrative';

  @override
  String get categoryGroupUtilities => 'Utilities';

  @override
  String get categoryGroupPermits => 'Permits';

  @override
  String get scopeProjectOnly => 'Project only';

  @override
  String get scopeCompanyOnly => 'Company only';

  @override
  String get scopeBoth => 'Project or company';

  @override
  String get documentTypeReceipt => 'Receipt';

  @override
  String get documentTypeInvoice => 'Invoice';

  @override
  String get documentTypeQuotation => 'Quotation';

  @override
  String get documentTypeDeliveryNote => 'Delivery note';

  @override
  String get documentTypePaymentProof => 'Payment proof';

  @override
  String get documentTypeContract => 'Contract';

  @override
  String get documentTypePurchaseOrder => 'Purchase order';

  @override
  String get documentStatusPendingVerification => 'Pending verification';

  @override
  String get documentStatusVerified => 'Verified';

  @override
  String get claimStatusOpen => 'Open';

  @override
  String get claimStatusPartiallySettled => 'Partially settled';

  @override
  String get claimStatusSettled => 'Settled';

  @override
  String get claimStatusCancelled => 'Cancelled';

  @override
  String get claimStatusReversed => 'Reversed';

  @override
  String get receiptNumber => 'Receipt number';

  @override
  String get invoiceNumber => 'Invoice number';

  @override
  String get advanceAllocations => 'Advance allocations';

  @override
  String get expenseItems => 'Expense items';

  @override
  String get documents => 'Documents';

  @override
  String get history => 'History';

  @override
  String get fileName => 'File name';

  @override
  String get fileSize => 'File size';

  @override
  String get verificationStatus => 'Verification status';

  @override
  String get claimDate => 'Claim date';

  @override
  String get dueDate => 'Due date';

  @override
  String get categoryCreated => 'The expense category was created.';

  @override
  String get expenseApproved => 'The expense was approved.';

  @override
  String get expenseRejected => 'The expense was rejected.';

  @override
  String get historyExpenseCreated => 'Expense submitted for review';

  @override
  String get historyDocumentAdded => 'Supporting document metadata added';

  @override
  String get historyExpenseApproved => 'Expense approved';

  @override
  String get historyExpenseRejected => 'Expense rejected';

  @override
  String get historyOutcomeSuccess => 'Succeeded';

  @override
  String get navSuppliers => 'Suppliers';

  @override
  String get suppliers => 'Suppliers';

  @override
  String get supplierDetails => 'Supplier details';

  @override
  String get addSupplier => 'Add supplier';

  @override
  String get editSupplier => 'Edit supplier';

  @override
  String get deactivateSupplier => 'Deactivate supplier';

  @override
  String get deactivateSupplierTitle => 'Deactivate this supplier?';

  @override
  String get deactivateSupplierBody =>
      'New financial activity will be blocked, while all supplier history is preserved.';

  @override
  String get supplierHistoryPreserved =>
      'Supplier history is preserved after deactivation.';

  @override
  String get supplierName => 'Supplier name';

  @override
  String get supplierCode => 'Supplier code';

  @override
  String get supplierType => 'Supplier type';

  @override
  String get contactPerson => 'Contact person';

  @override
  String get secondaryPhoneNumber => 'Secondary phone number';

  @override
  String get emailAddress => 'Email address';

  @override
  String get city => 'City';

  @override
  String get defaultCurrency => 'Default currency';

  @override
  String get transactionMode => 'Transaction mode';

  @override
  String get paymentTermsDays => 'Payment terms (days)';

  @override
  String get creditLimit => 'Credit limit';

  @override
  String get preferredPaymentMethod => 'Preferred payment method';

  @override
  String get commercialRegistration => 'Commercial registration number';

  @override
  String get taxRegistration => 'Tax registration number';

  @override
  String get searchSuppliers => 'Search supplier name, code, contact, or phone';

  @override
  String get noSuppliers => 'No suppliers match these filters.';

  @override
  String get supplierCreated => 'The supplier was created.';

  @override
  String get supplierUpdated => 'The supplier was updated.';

  @override
  String get supplierDeactivated =>
      'The supplier was deactivated and its history was preserved.';

  @override
  String get paymentAccounts => 'Payment accounts';

  @override
  String get addPaymentAccount => 'Add payment account';

  @override
  String get accountType => 'Account type';

  @override
  String get accountLabel => 'Account label';

  @override
  String get accountHolder => 'Account holder';

  @override
  String get bankBranch => 'Bank branch';

  @override
  String get accountNumber => 'Account number';

  @override
  String get iban => 'IBAN';

  @override
  String get walletProvider => 'Wallet provider';

  @override
  String get walletNumber => 'Wallet number';

  @override
  String get maskedAccount => 'Masked account';

  @override
  String get pendingVerification => 'Pending verification';

  @override
  String get noPaymentAccounts => 'No payment-account metadata is available.';

  @override
  String get supplierInvoices => 'Supplier invoices';

  @override
  String get supplierDebts => 'Supplier debts';

  @override
  String get createSupplierInvoice => 'Create supplier invoice';

  @override
  String get invoiceDate => 'Invoice date';

  @override
  String get invoiceDescription => 'Invoice description';

  @override
  String get expenseCategory => 'Expense category';

  @override
  String get invoiceItemsOptional => 'Invoice items (optional)';

  @override
  String get addInvoiceItem => 'Add invoice item';

  @override
  String get itemName => 'Item name';

  @override
  String get itemQuantity => 'Quantity';

  @override
  String get unitCode => 'Unit';

  @override
  String get unitPrice => 'Unit price';

  @override
  String get discountAmount => 'Discount';

  @override
  String get taxAmount => 'Tax';

  @override
  String get duplicateInvoiceWarning =>
      'Invoice references are not uniquely enforced. Verify possible duplicates before submitting.';

  @override
  String get noSupplierInvoices =>
      'No supplier invoices or debts are available.';

  @override
  String get amountPaid => 'Amount paid';

  @override
  String get debtNumber => 'Debt number';

  @override
  String get expenseStatus => 'Expense status';

  @override
  String get debtStatus => 'Debt status';

  @override
  String get partialPayment => 'Partial payment';

  @override
  String get fullPayment => 'Full payment';

  @override
  String get supplierPayments => 'Supplier payments';

  @override
  String get recordPayment => 'Record payment';

  @override
  String get paymentDate => 'Payment date';

  @override
  String get paymentAmount => 'Payment amount';

  @override
  String get paymentReference => 'Payment reference';

  @override
  String get payerBankName => 'Payer bank name';

  @override
  String get proofPathOptional => 'Existing proof path (optional)';

  @override
  String get paymentFeesUnsupported =>
      'Payment fees are not supported in this phase.';

  @override
  String get advanceFundingUnavailable =>
      'Advance-balance supplier funding is unavailable.';

  @override
  String get finalSettlementUnavailable =>
      'Final settlement is unavailable in this phase.';

  @override
  String get debtAllocations => 'Debt allocations';

  @override
  String get fundingAllocations => 'Funding allocations';

  @override
  String get allocationAmount => 'Allocation amount';

  @override
  String get managerPersonalContribution => 'Manager personal contribution';

  @override
  String get companyCashbox => 'Company cashbox';

  @override
  String get fundingReserved => 'Funding reserved';

  @override
  String get fundingConsumed => 'Funding consumed';

  @override
  String get fundingReleased => 'Funding released';

  @override
  String get allocationTotalsMismatch =>
      'Debt and funding allocations must each total the payment exactly.';

  @override
  String get noSupplierPayments => 'No supplier payments are available.';

  @override
  String get paymentPendingApproval => 'Payment pending approval';

  @override
  String get confirmPayment => 'Confirm payment';

  @override
  String get rejectPayment => 'Reject payment';

  @override
  String get confirmPaymentBody =>
      'Confirm only after verifying the payment, debt allocations, and funding allocations.';

  @override
  String get rejectPaymentBody =>
      'Rejection preserves the record and releases reserved funding.';

  @override
  String get supplierCreditNotes => 'Supplier credit notes';

  @override
  String get createCreditNote => 'Create credit note';

  @override
  String get creditNoteDate => 'Credit-note date';

  @override
  String get creditReason => 'Credit reason';

  @override
  String get availableCredit => 'Available credit';

  @override
  String get appliedCredit => 'Applied credit';

  @override
  String get approveCreditNote => 'Approve credit note';

  @override
  String get applyCredit => 'Apply credit';

  @override
  String get creditAllocationExceeded =>
      'Allocations must be unique and cannot exceed available credit or eligible debt.';

  @override
  String get noCreditNotes => 'No supplier credit notes are available.';

  @override
  String get supplierRefunds => 'Supplier refunds';

  @override
  String get refundHistoryOnly =>
      'Refund history is read-only. Refund creation and verification are not available.';

  @override
  String get noSupplierRefunds => 'No supplier refund history is available.';

  @override
  String get supplierStatement => 'Supplier statement';

  @override
  String get differentCurrenciesSeparate =>
      'Different currencies are shown separately and are never totaled together.';

  @override
  String get noStatementEntries => 'No statement entries are available.';

  @override
  String get operationOutcomeUncertain =>
      'The operation outcome is uncertain. Do not submit a different operation; retry the same operation with the preserved key.';

  @override
  String get selectSupplier => 'Select a supplier';

  @override
  String get selectDebt => 'Select a debt';

  @override
  String get selectFundingSource => 'Select a funding source';

  @override
  String get selectCategory => 'Select a supplier-enabled expense category';

  @override
  String get invalidQuantity =>
      'Enter a positive quantity with no more than three decimal places.';

  @override
  String get createAction => 'Create';

  @override
  String get applyAction => 'Apply';

  @override
  String get viewStatement => 'View statement';

  @override
  String get viewRefunds => 'View refund history';

  @override
  String get viewInvoices => 'View invoices';

  @override
  String get viewPayments => 'View payments';

  @override
  String get viewCreditNotes => 'View credit notes';

  @override
  String get supplierTypeGeneral => 'General supplier';

  @override
  String get supplierTypeMaterials => 'Materials supplier';

  @override
  String get supplierTypeEquipment => 'Equipment supplier';

  @override
  String get supplierTypeEquipmentRental => 'Equipment rental';

  @override
  String get supplierTypeFuel => 'Fuel supplier';

  @override
  String get supplierTypeSubcontractor => 'Subcontractor';

  @override
  String get supplierTypeTransport => 'Transport provider';

  @override
  String get supplierTypeMaintenance => 'Maintenance provider';

  @override
  String get supplierTypeService => 'Service provider';

  @override
  String get transactionCashOnly => 'Cash only';

  @override
  String get transactionCreditOnly => 'Credit only';

  @override
  String get transactionCashAndCredit => 'Cash and credit';

  @override
  String get accountTypeBank => 'Bank account';

  @override
  String get accountTypeWallet => 'Mobile wallet';

  @override
  String get accountTypeCashCollection => 'Cash collection';

  @override
  String get creditReasonReturnedGoods => 'Returned goods';

  @override
  String get creditReasonDamagedGoods => 'Damaged goods';

  @override
  String get creditReasonPricingCorrection => 'Pricing correction';

  @override
  String get creditReasonOverbilling => 'Overbilling';

  @override
  String get creditReasonAdditionalDiscount => 'Additional discount';

  @override
  String get creditReasonServiceCompensation => 'Service compensation';

  @override
  String get unitPiece => 'Piece';

  @override
  String get unitPackage => 'Package';

  @override
  String get unitBox => 'Box';

  @override
  String get unitBag => 'Bag';

  @override
  String get unitKilogram => 'Kilogram';

  @override
  String get unitTon => 'Ton';

  @override
  String get unitMeter => 'Meter';

  @override
  String get unitSquareMeter => 'Square meter';

  @override
  String get unitCubicMeter => 'Cubic meter';

  @override
  String get unitLiter => 'Liter';

  @override
  String get unitHour => 'Hour';

  @override
  String get unitDay => 'Day';

  @override
  String get unitTrip => 'Trip';

  @override
  String get unitService => 'Service';

  @override
  String get unitLumpSum => 'Lump sum';

  @override
  String get loadMore => 'Load more';

  @override
  String get settlementCodeAdvances => 'Advances';

  @override
  String get settlementCodeExpenses => 'Expenses';

  @override
  String get settlementCodeExpenseDocuments => 'Expense documents';

  @override
  String get settlementCodeReimbursements => 'Reimbursements';

  @override
  String get settlementCodeManagerContributions =>
      'Manager contribution claims';

  @override
  String get settlementCodeSupplierDebt => 'Supplier debt';

  @override
  String get settlementCodeSupplierPayments => 'Supplier payments';

  @override
  String get settlementCodeSupplierCredits => 'Supplier credit notes';

  @override
  String get settlementCodeReimbursementPayments => 'Reimbursement payments';

  @override
  String get settlementCodeExpenseReturns => 'Expense returns';

  @override
  String get settlementCodeSupplierRefunds => 'Supplier refunds';

  @override
  String get settlementCodeOwnerOperations => 'Owner operations';

  @override
  String get settlementCodeProjectLifecycle => 'Project lifecycle';

  @override
  String get settlementCodeEvaluated => 'Evaluated';

  @override
  String get settlementCodeNotVisible => 'Not visible';

  @override
  String get settlementCodeNotAttributable => 'Cannot be attributed';

  @override
  String get settlementCodeBlocked => 'Blocked';

  @override
  String get settlementCodeIndeterminate => 'Indeterminate';

  @override
  String get settlementCodeCompanyFinancial => 'Company financial view';

  @override
  String get settlementCodeAssignedProjectLimited =>
      'Limited assigned-project view';

  @override
  String get settlementCodeKnownFinancialBlockers =>
      'Known financial blockers remain.';

  @override
  String get settlementCodeProjectCancelled => 'The project is cancelled.';

  @override
  String get settlementCodeProjectAlreadyFinanciallyClosed =>
      'The project is already financially closed.';

  @override
  String get settlementCodeProjectNotCompleted =>
      'The project is not operationally completed.';

  @override
  String get settlementCodeUnknownProjectStatus =>
      'The project status needs review.';

  @override
  String get settlementCodeProjectCompletionEvidenceUnavailable =>
      'Project completion evidence is unavailable.';

  @override
  String get settlementCodeExpenseDraft => 'An expense is still a draft.';

  @override
  String get settlementCodeExpensePendingReview =>
      'An expense is awaiting review.';

  @override
  String get settlementCodeExpenseCorrectionRequired =>
      'An expense needs correction.';

  @override
  String get settlementCodeMissingExpenseDocument =>
      'Required expense documentation is missing.';

  @override
  String get settlementCodeUnresolvedReimbursement =>
      'A reimbursement claim remains unresolved.';

  @override
  String get settlementCodeUnsettledManagerContributionClaim =>
      'A manager contribution claim remains unsettled.';

  @override
  String get settlementCodeOutstandingSupplierDebt =>
      'Supplier debt remains outstanding.';

  @override
  String get settlementCodePendingSupplierPayment =>
      'A supplier payment is pending.';

  @override
  String get settlementCodePendingSupplierCredit =>
      'A supplier credit note is pending.';

  @override
  String get settlementCodePendingReimbursementPayment =>
      'A reimbursement payment is pending.';

  @override
  String get settlementCodePendingExpenseReturn =>
      'An expense return is pending.';

  @override
  String get settlementCodePendingSupplierRefund =>
      'A supplier refund awaits verification.';

  @override
  String get settlementCodePendingOwnerRefund =>
      'An owner refund awaits approval.';

  @override
  String get settlementCodePendingProjectContractChange =>
      'A project contract change awaits approval.';

  @override
  String get settlementCodeProjectAdvanceAttributionUnavailable =>
      'Advance balances cannot currently be safely attributed to this project.';

  @override
  String get settlementCodeApprovedReturnEffectsRequireReconciliation =>
      'The financial effects of approved returns still need reconciliation.';

  @override
  String get settlementCodeUnallocatedCreditProjectIntentUnavailable =>
      'Unallocated supplier credit cannot be assigned to a project from the available records.';

  @override
  String get settlementCodeFinancialRecordRequiresReview =>
      'A financial record needs review before evaluation can be completed.';

  @override
  String get settlementCodeOutstandingAmountUnavailable =>
      'The authoritative outstanding amount is unavailable.';

  @override
  String get settlementCodeProjectSettlementPolicyUndefined =>
      'Project settlement rules have not yet been defined.';

  @override
  String get settlementCodeProjectClosurePolicyUndefined =>
      'Project financial closure rules have not yet been defined.';

  @override
  String get settlementCodeActiveDocumentSettingsUnavailable =>
      'Active document requirements are unavailable.';

  @override
  String get settlementCodeDocumentPolicyUnavailable =>
      'Document requirements cannot currently be determined.';

  @override
  String get settlementCodeFinancialCategoryNotVisible =>
      'Your access does not include this financial category.';

  @override
  String get settlementCodeUnknownExpenseStatus =>
      'An expense has an unrecognized status.';

  @override
  String get settlementCodeUnknownSupplierDebtStatus =>
      'A supplier debt has an unrecognized status.';

  @override
  String get settlementCodeUnknownClaimStatus =>
      'A claim has an unrecognized status.';

  @override
  String get settlementCodeUnknownSupplierPaymentStatus =>
      'A supplier payment has an unrecognized status.';

  @override
  String get settlementCodeUnknownSupplierCreditStatus =>
      'A supplier credit has an unrecognized status.';

  @override
  String get settlementCodeUnknownClaimPaymentStatus =>
      'A reimbursement payment has an unrecognized status.';

  @override
  String get settlementCodeUnknownExpenseReturnStatus =>
      'An expense return has an unrecognized status.';

  @override
  String get settlementCodeUnknownSupplierRefundStatus =>
      'A supplier refund has an unrecognized status.';

  @override
  String get settlementCodePendingExpenseAmount =>
      'Expense amounts awaiting review';

  @override
  String get settlementCodeOutstandingLiability => 'Outstanding liabilities';

  @override
  String get settlementCodePendingProjectAllocation =>
      'Pending allocations to this project';

  @override
  String get settlementCodePendingReturnAmount => 'Pending return amounts';

  @override
  String get settlementCodePendingRefundAmount => 'Pending refund amounts';

  @override
  String get settlementCodeNone => 'No monetary total';

  @override
  String get settlementCodeExpense => 'Expense';

  @override
  String get settlementCodePersonalClaim => 'Personal claim';

  @override
  String get settlementCodeSupplierPayment => 'Supplier payment';

  @override
  String get settlementCodeSupplierCreditNote => 'Supplier credit note';

  @override
  String get settlementCodePersonalClaimPayment => 'Personal claim payment';

  @override
  String get settlementCodeExpenseReturn => 'Expense return';

  @override
  String get settlementCodeSupplierRefund => 'Supplier refund';

  @override
  String get settlementCodeOwnerPaymentRefund => 'Owner refund';

  @override
  String get settlementCodeProjectContractChange => 'Project contract change';

  @override
  String get settlementTitle => 'Project settlement';

  @override
  String get settlementSettlement => 'Settlement readiness';

  @override
  String get settlementClosure => 'Financial closure readiness';

  @override
  String get settlementSettlementUnknown =>
      'Settlement readiness cannot yet be fully determined.';

  @override
  String get settlementClosureUnknown =>
      'Financial closure readiness cannot yet be fully determined.';

  @override
  String get settlementSettlementBlocked =>
      'Known impediments prevent settlement.';

  @override
  String get settlementClosureBlocked =>
      'Known impediments prevent financial closure.';

  @override
  String get settlementUnsupported =>
      'This state is not supported by this app version. Readiness is not certified.';

  @override
  String get settlementKnownBlockers => 'Known / visible blockers';

  @override
  String get settlementNoKnown =>
      'No known blockers in the evaluated categories. This does not certify financial readiness.';

  @override
  String get settlementCountScope =>
      'This count covers visible findings only, not every possible obligation.';

  @override
  String get settlementGaps => 'Evaluation gaps';

  @override
  String get settlementGapsHelp =>
      'These limitations explain why readiness cannot be fully certified.';

  @override
  String get settlementNoGaps =>
      'No evaluation gaps were reported. Readiness remains as reported above.';

  @override
  String get settlementCategories => 'Financial categories';

  @override
  String get settlementHidden =>
      'This category is outside your access. Its records and amounts are not shown.';

  @override
  String get settlementUnattributable =>
      'The system cannot safely assign this category to this project. This is not a zero balance.';

  @override
  String get settlementNoCategoryBlockers =>
      'No known blockers in this evaluated category.';

  @override
  String get settlementCurrencies =>
      'Amounts remain separate by category and currency. Do not add them together.';

  @override
  String get settlementEvaluatedAt => 'Evaluated at';

  @override
  String get settlementVisibility => 'Visibility scope';

  @override
  String get settlementProjectReference => 'Project reference';

  @override
  String get settlementSettlementImpediments => 'Settlement impediments';

  @override
  String get settlementClosureImpediments => 'Closure impediments';

  @override
  String get settlementNoImpediments =>
      'No impediments reported in this section.';

  @override
  String get settlementOpenRecord => 'View record';

  @override
  String get settlementRecordReference => 'Record reference';

  @override
  String get settlementUnknown => 'Unrecognized value — review required';

  @override
  String get settlementRefresh => 'Refresh evaluation';

  @override
  String get settlementBack => 'Project details';

  @override
  String get settlementNoCategories =>
      'No financial categories were returned. Readiness is not certified.';
}
