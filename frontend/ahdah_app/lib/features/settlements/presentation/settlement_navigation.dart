import '../../../app/routing/app_routes.dart';
import '../../access/domain/role_capabilities.dart';
import '../domain/settlement_models.dart';

typedef SettlementDestination = ({String name, Map<String, String> parameters});

SettlementDestination? settlementDestination(
  SettlementBlocker blocker,
  RoleCapabilities capabilities,
) {
  if (!capabilities.canViewProjectSettlement ||
      !RegExp(r'^[A-Za-z0-9-]{1,64}$').hasMatch(blocker.recordId)) {
    return null;
  }
  final id = blocker.recordId;
  return switch (blocker.recordType) {
    'Expense' when capabilities.canViewExpenses => (
      name: AppRoutes.expenseDetails,
      parameters: {'expenseId': id},
    ),
    'PersonalClaim'
        when capabilities.canViewReimbursements &&
            !capabilities.requiresAssignedProjects =>
      (
        name: AppRoutes.reimbursementDetails,
        parameters: {'reimbursementId': id},
      ),
    'SupplierDebt' when capabilities.canViewSuppliers => (
      name: AppRoutes.supplierDebtDetails,
      parameters: {'debtId': id},
    ),
    'SupplierPayment' when capabilities.canViewSupplierFinancials => (
      name: AppRoutes.supplierPaymentDetails,
      parameters: {'paymentId': id},
    ),
    'SupplierCreditNote' when capabilities.canViewSupplierFinancials => (
      name: AppRoutes.supplierCreditDetails,
      parameters: {'creditNoteId': id},
    ),
    _ => null,
  };
}
