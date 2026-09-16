import '../../../l10n/app_localizations.dart';
import '../../suppliers/presentation/widgets/supplier_labels.dart';

abstract final class SettlementLabels {
  static String status(AppLocalizations l10n, String value) => switch (value) {
    'Approved' => l10n.expenseStatusApproved,
    'PendingVerification' => l10n.pendingVerification,
    'Verified' => l10n.documentStatusVerified,
    _ => SupplierLabels.status(l10n, value),
  };

  static String code(AppLocalizations l10n, String value) => switch (value) {
    'Advances' => l10n.settlementCodeAdvances,
    'Expenses' => l10n.settlementCodeExpenses,
    'ExpenseDocuments' => l10n.settlementCodeExpenseDocuments,
    'Reimbursements' => l10n.settlementCodeReimbursements,
    'ManagerContributions' => l10n.settlementCodeManagerContributions,
    'SupplierDebt' => l10n.settlementCodeSupplierDebt,
    'SupplierPayments' => l10n.settlementCodeSupplierPayments,
    'SupplierCredits' => l10n.settlementCodeSupplierCredits,
    'ReimbursementPayments' => l10n.settlementCodeReimbursementPayments,
    'ExpenseReturns' => l10n.settlementCodeExpenseReturns,
    'SupplierRefunds' => l10n.settlementCodeSupplierRefunds,
    'OwnerOperations' => l10n.settlementCodeOwnerOperations,
    'ProjectLifecycle' => l10n.settlementCodeProjectLifecycle,
    'Evaluated' => l10n.settlementCodeEvaluated,
    'NotVisible' => l10n.settlementCodeNotVisible,
    'NotAttributable' => l10n.settlementCodeNotAttributable,
    'Blocked' => l10n.settlementCodeBlocked,
    'Indeterminate' => l10n.settlementCodeIndeterminate,
    'CompanyFinancial' => l10n.settlementCodeCompanyFinancial,
    'AssignedProjectLimited' => l10n.settlementCodeAssignedProjectLimited,
    'KnownFinancialBlockers' => l10n.settlementCodeKnownFinancialBlockers,
    'ProjectCancelled' => l10n.settlementCodeProjectCancelled,
    'ProjectAlreadyFinanciallyClosed' =>
      l10n.settlementCodeProjectAlreadyFinanciallyClosed,
    'ProjectNotCompleted' => l10n.settlementCodeProjectNotCompleted,
    'UnknownProjectStatus' => l10n.settlementCodeUnknownProjectStatus,
    'ProjectCompletionEvidenceUnavailable' =>
      l10n.settlementCodeProjectCompletionEvidenceUnavailable,
    'ExpenseDraft' => l10n.settlementCodeExpenseDraft,
    'ExpensePendingReview' => l10n.settlementCodeExpensePendingReview,
    'ExpenseCorrectionRequired' => l10n.settlementCodeExpenseCorrectionRequired,
    'MissingExpenseDocument' => l10n.settlementCodeMissingExpenseDocument,
    'UnresolvedReimbursement' => l10n.settlementCodeUnresolvedReimbursement,
    'UnsettledManagerContributionClaim' =>
      l10n.settlementCodeUnsettledManagerContributionClaim,
    'OutstandingSupplierDebt' => l10n.settlementCodeOutstandingSupplierDebt,
    'PendingSupplierPayment' => l10n.settlementCodePendingSupplierPayment,
    'PendingSupplierCredit' => l10n.settlementCodePendingSupplierCredit,
    'PendingReimbursementPayment' =>
      l10n.settlementCodePendingReimbursementPayment,
    'PendingExpenseReturn' => l10n.settlementCodePendingExpenseReturn,
    'PendingSupplierRefund' => l10n.settlementCodePendingSupplierRefund,
    'PendingOwnerRefund' => l10n.settlementCodePendingOwnerRefund,
    'PendingProjectContractChange' =>
      l10n.settlementCodePendingProjectContractChange,
    'ProjectAdvanceAttributionUnavailable' =>
      l10n.settlementCodeProjectAdvanceAttributionUnavailable,
    'ApprovedReturnEffectsRequireReconciliation' =>
      l10n.settlementCodeApprovedReturnEffectsRequireReconciliation,
    'UnallocatedCreditProjectIntentUnavailable' =>
      l10n.settlementCodeUnallocatedCreditProjectIntentUnavailable,
    'FinancialRecordRequiresReview' =>
      l10n.settlementCodeFinancialRecordRequiresReview,
    'OutstandingAmountUnavailable' =>
      l10n.settlementCodeOutstandingAmountUnavailable,
    'ProjectSettlementPolicyUndefined' =>
      l10n.settlementCodeProjectSettlementPolicyUndefined,
    'ProjectClosurePolicyUndefined' =>
      l10n.settlementCodeProjectClosurePolicyUndefined,
    'ActiveDocumentSettingsUnavailable' =>
      l10n.settlementCodeActiveDocumentSettingsUnavailable,
    'DocumentPolicyUnavailable' => l10n.settlementCodeDocumentPolicyUnavailable,
    'FinancialCategoryNotVisible' =>
      l10n.settlementCodeFinancialCategoryNotVisible,
    'UnknownExpenseStatus' => l10n.settlementCodeUnknownExpenseStatus,
    'UnknownSupplierDebtStatus' => l10n.settlementCodeUnknownSupplierDebtStatus,
    'UnknownClaimStatus' => l10n.settlementCodeUnknownClaimStatus,
    'UnknownSupplierPaymentStatus' =>
      l10n.settlementCodeUnknownSupplierPaymentStatus,
    'UnknownSupplierCreditStatus' =>
      l10n.settlementCodeUnknownSupplierCreditStatus,
    'UnknownClaimPaymentStatus' => l10n.settlementCodeUnknownClaimPaymentStatus,
    'UnknownExpenseReturnStatus' =>
      l10n.settlementCodeUnknownExpenseReturnStatus,
    'UnknownSupplierRefundStatus' =>
      l10n.settlementCodeUnknownSupplierRefundStatus,
    'PendingExpenseAmount' => l10n.settlementCodePendingExpenseAmount,
    'OutstandingLiability' => l10n.settlementCodeOutstandingLiability,
    'PendingProjectAllocation' => l10n.settlementCodePendingProjectAllocation,
    'PendingReturnAmount' => l10n.settlementCodePendingReturnAmount,
    'PendingRefundAmount' => l10n.settlementCodePendingRefundAmount,
    'None' => l10n.settlementCodeNone,
    'Expense' => l10n.settlementCodeExpense,
    'PersonalClaim' => l10n.settlementCodePersonalClaim,
    'SupplierPayment' => l10n.settlementCodeSupplierPayment,
    'SupplierCreditNote' => l10n.settlementCodeSupplierCreditNote,
    'PersonalClaimPayment' => l10n.settlementCodePersonalClaimPayment,
    'ExpenseReturn' => l10n.settlementCodeExpenseReturn,
    'SupplierRefund' => l10n.settlementCodeSupplierRefund,
    'OwnerPaymentRefund' => l10n.settlementCodeOwnerPaymentRefund,
    'ProjectContractChange' => l10n.settlementCodeProjectContractChange,
    _ => l10n.settlementUnknown,
  };
}
