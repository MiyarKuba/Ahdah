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
}
