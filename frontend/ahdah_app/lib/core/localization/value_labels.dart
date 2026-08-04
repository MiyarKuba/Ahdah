import '../../l10n/app_localizations.dart';

abstract final class ValueLabels {
  static const assignableRoles = <String>[
    'Deputy',
    'Accountant',
    'Supervisor',
    'Worker',
  ];

  static String role(AppLocalizations l10n, String value) => switch (value) {
    'Manager' => l10n.manager,
    'Deputy' => l10n.deputy,
    'Accountant' => l10n.accountant,
    'Supervisor' => l10n.supervisor,
    'Worker' => l10n.worker,
    _ => l10n.unknownValue,
  };

  static String userStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'PendingApproval' => l10n.statusPendingApproval,
        'Active' => l10n.statusActive,
        'Suspended' => l10n.statusSuspended,
        'Inactive' => l10n.statusInactive,
        'Rejected' => l10n.statusRejected,
        _ => l10n.unknownValue,
      };

  static String companyStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'Active' => l10n.statusActive,
        'Suspended' => l10n.statusSuspended,
        'Inactive' => l10n.statusInactive,
        'PendingSetup' => l10n.statusPendingApproval,
        _ => l10n.unknownValue,
      };

  static String identityStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'NotRequired' => l10n.identityNotRequired,
        'Pending' => l10n.identityPending,
        'Verified' => l10n.identityVerified,
        'Rejected' => l10n.identityRejected,
        _ => l10n.unknownValue,
      };

  static String invitationStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'Pending' => l10n.invitationStatusPending,
        'Accepted' => l10n.invitationStatusAccepted,
        'Expired' => l10n.invitationStatusExpired,
        'Cancelled' => l10n.invitationStatusCancelled,
        _ => l10n.unknownValue,
      };

  static String joinRequestStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'Pending' => l10n.joinStatusPending,
        'Approved' => l10n.joinStatusApproved,
        'Rejected' => l10n.joinStatusRejected,
        'Cancelled' => l10n.joinStatusCancelled,
        _ => l10n.unknownValue,
      };

  static String projectStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'Active' => l10n.projectStatusActive,
        'Paused' => l10n.projectStatusPaused,
        'Completed' => l10n.projectStatusCompleted,
        'FinanciallyClosed' => l10n.projectStatusFinanciallyClosed,
        'Cancelled' => l10n.projectStatusCancelled,
        _ => l10n.unknownValue,
      };

  static String advanceStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'Draft' => l10n.advanceStatusDraft,
        'PendingConfirmation' => l10n.pendingConfirmation,
        'Open' => l10n.advanceStatusOpen,
        'InSettlement' => l10n.advanceStatusInSettlement,
        'ReadyToClose' => l10n.advanceStatusReadyToClose,
        'Closed' => l10n.advanceStatusClosed,
        'Cancelled' => l10n.advanceStatusCancelled,
        'Reversed' => l10n.advanceStatusReversed,
        _ => l10n.unknownValue,
      };

  static String transferStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'Draft' => l10n.advanceStatusDraft,
        'PendingConfirmation' => l10n.pendingConfirmation,
        'CorrectionRequired' => l10n.transferStatusCorrectionRequired,
        'Confirmed' => l10n.confirmed,
        'Rejected' => l10n.rejected,
        'Cancelled' => l10n.advanceStatusCancelled,
        'Reversed' => l10n.advanceStatusReversed,
        'Recorded' => l10n.confirmed,
        _ => l10n.unknownValue,
      };

  static String movementType(AppLocalizations l10n, String value) =>
      switch (value) {
        'FundingAllocation' => l10n.movementFundingAllocation,
        'AdvanceDelivery' => l10n.transferTypeAdvanceDelivery,
        'InternalTransfer' => l10n.transferTypeInternalTransfer,
        'BalanceReturn' => l10n.transferTypeBalanceReturn,
        _ => l10n.unknownValue,
      };

  static String fundingType(AppLocalizations l10n, String value) =>
      switch (value) {
        'ProjectOwnerPayment' => l10n.fundingSourceTypeProjectOwnerPayment,
        'ManagerContribution' => l10n.fundingSourceTypeManagerContribution,
        'CompanyCashbox' => l10n.fundingSourceTypeCompanyCashbox,
        'ReturnedAdvance' => l10n.fundingSourceTypeReturnedAdvance,
        'SupplierRefund' => l10n.fundingSourceTypeSupplierRefund,
        'Other' => l10n.other,
        _ => l10n.unknownValue,
      };

  static String fundingStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'PendingVerification' => l10n.fundingStatusPendingVerification,
        'Available' => l10n.fundingStatusAvailable,
        'PartiallyUsed' => l10n.fundingStatusPartiallyUsed,
        'FullyUsed' => l10n.fundingStatusFullyUsed,
        'Cancelled' => l10n.advanceStatusCancelled,
        'Reversed' => l10n.advanceStatusReversed,
        _ => l10n.unknownValue,
      };

  static String paymentMethod(AppLocalizations l10n, String value) =>
      switch (value) {
        'Cash' => l10n.paymentCash,
        'BankTransfer' => l10n.paymentBankTransfer,
        'Cheque' => l10n.paymentCheque,
        'Card' => l10n.paymentCard,
        'MobileWallet' => l10n.paymentMobileWallet,
        'BalanceTransfer' => l10n.paymentBalanceTransfer,
        'Other' => l10n.other,
        _ => l10n.unknownValue,
      };

  static String balanceStatus(AppLocalizations l10n, String value) =>
      switch (value) {
        'Active' => l10n.statusActive,
        'InSettlement' => l10n.balanceStatusInSettlement,
        'Settled' => l10n.balanceStatusSettled,
        'Closed' => l10n.balanceStatusClosed,
        _ => l10n.unknownValue,
      };
}
