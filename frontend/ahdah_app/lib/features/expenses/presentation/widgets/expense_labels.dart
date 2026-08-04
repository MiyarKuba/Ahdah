import '../../../../l10n/app_localizations.dart';

String expenseStatusLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'Draft' => l10n.advanceStatusDraft,
      'PendingReview' => l10n.expenseStatusPendingReview,
      'CorrectionRequired' => l10n.expenseStatusCorrectionRequired,
      'Approved' => l10n.expenseStatusApproved,
      'Rejected' => l10n.expenseStatusRejected,
      'Cancelled' => l10n.expenseStatusCancelled,
      'Reversed' => l10n.expenseStatusReversed,
      _ => l10n.unknownValue,
    };

String expensePaymentModeLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'AdvanceBalance' => l10n.advanceBalance,
      'PersonalFunds' => l10n.personalFunds,
      _ => l10n.unknownValue,
    };

String expenseCategoryGroupLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'Materials' => l10n.categoryGroupMaterials,
      'Labor' => l10n.categoryGroupLabor,
      'Subcontracting' => l10n.categoryGroupSubcontracting,
      'Transportation' => l10n.categoryGroupTransportation,
      'Equipment' => l10n.categoryGroupEquipment,
      'Fuel' => l10n.categoryGroupFuel,
      'Services' => l10n.categoryGroupServices,
      'Administrative' => l10n.categoryGroupAdministrative,
      'Utilities' => l10n.categoryGroupUtilities,
      'Permits' => l10n.categoryGroupPermits,
      'Other' => l10n.other,
      _ => l10n.unknownValue,
    };

String expenseScopeLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'ProjectOnly' => l10n.scopeProjectOnly,
      'CompanyOnly' => l10n.scopeCompanyOnly,
      'Both' => l10n.scopeBoth,
      _ => l10n.unknownValue,
    };

String expenseDocumentTypeLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'Receipt' => l10n.documentTypeReceipt,
      'Invoice' => l10n.documentTypeInvoice,
      'Quotation' => l10n.documentTypeQuotation,
      'DeliveryNote' => l10n.documentTypeDeliveryNote,
      'PaymentProof' => l10n.documentTypePaymentProof,
      'Contract' => l10n.documentTypeContract,
      'PurchaseOrder' => l10n.documentTypePurchaseOrder,
      'Other' => l10n.other,
      _ => l10n.unknownValue,
    };

String documentStatusLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'PendingVerification' => l10n.documentStatusPendingVerification,
      'Verified' => l10n.documentStatusVerified,
      'Rejected' => l10n.expenseStatusRejected,
      _ => l10n.unknownValue,
    };

String reimbursementStatusLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'Open' => l10n.claimStatusOpen,
      'PartiallySettled' => l10n.claimStatusPartiallySettled,
      'Settled' => l10n.claimStatusSettled,
      'Cancelled' => l10n.claimStatusCancelled,
      'Reversed' => l10n.claimStatusReversed,
      _ => l10n.unknownValue,
    };

String expenseHistoryEventLabel(AppLocalizations l10n, String value) =>
    switch (value) {
      'ExpenseCreated' => l10n.historyExpenseCreated,
      'ExpenseDocumentMetadataAdded' => l10n.historyDocumentAdded,
      'ExpenseApproved' => l10n.historyExpenseApproved,
      'ExpenseRejected' => l10n.historyExpenseRejected,
      _ => l10n.unknownValue,
    };
