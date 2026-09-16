abstract final class ApiEndpoints {
  static const login = '/api/v1/auth/login';
  static const registerCompany = '/api/v1/auth/register-company';
  static const currentUser = '/api/v1/auth/me';
  static const acceptInvitation = '/api/v1/invitations/accept';
  static const invitations = '/api/v1/invitations';
  static const joinRequests = '/api/v1/join-requests';
  static const projects = '/api/v1/projects';
  static const companyMembers = '/api/v1/company/members';
  static const advances = '/api/v1/advances';
  static const advanceFundingSources = '/api/v1/advance-funding-sources';
  static const myAdvanceBalances = '/api/v1/advance-balances/me';
  static const expenseCategories = '/api/v1/expense-categories';
  static const expenses = '/api/v1/expenses';
  static const reimbursements = '/api/v1/reimbursements';
  static const suppliers = '/api/v1/suppliers';
  static const supplierInvoices = '/api/v1/supplier-invoices';
  static const supplierDebts = '/api/v1/supplier-debts';
  static const supplierPayments = '/api/v1/supplier-payments';
  static const supplierCreditNotes = '/api/v1/supplier-credit-notes';
  static const supplierRefunds = '/api/v1/supplier-refunds';
  static const health = '/api/system/health';

  static String cancelInvitation(String id) => '$invitations/$id/cancel';
  static String approveJoinRequest(String id) => '$joinRequests/$id/approve';
  static String rejectJoinRequest(String id) => '$joinRequests/$id/reject';
  static String project(String id) => '$projects/$id';
  static String projectSettlement(String id) =>
      '$projects/${Uri.encodeComponent(id)}/settlement';
  static String projectSupervisor(String id) => '$projects/$id/supervisor';
  static String projectMembers(String id) => '$projects/$id/members';
  static String companyMember(String id) => '$companyMembers/$id';
  static String advance(String id) => '$advances/$id';
  static String advanceMovements(String id) => '$advances/$id/movements';
  static String advanceDistributions(String id) =>
      '$advances/$id/distributions';
  static String advanceReturns(String id) => '$advances/$id/returns';
  static String advanceTransferConfirm(String id) =>
      '/api/v1/advance-transfers/$id/confirm';
  static String advanceTransferReject(String id) =>
      '/api/v1/advance-transfers/$id/reject';
  static String userAdvanceBalances(String id) =>
      '/api/v1/advance-balances/users/$id';
  static String expense(String id) => '$expenses/$id';
  static String expenseAllocations(String id) => '$expenses/$id/allocations';
  static String expenseAttachments(String id) => '$expenses/$id/attachments';
  static String expenseHistory(String id) => '$expenses/$id/history';
  static String approveExpense(String id) => '$expenses/$id/approve';
  static String rejectExpense(String id) => '$expenses/$id/reject';
  static String reimbursement(String id) => '$reimbursements/$id';
  static String supplier(String id) => '$suppliers/$id';
  static String supplierPaymentAccounts(String id) =>
      '$suppliers/$id/payment-accounts';
  static String supplierStatement(String id) => '$suppliers/$id/statement';
  static String supplierInvoice(String id) => '$supplierInvoices/$id';
  static String supplierDebt(String id) => '$supplierDebts/$id';
  static String supplierPayment(String id) => '$supplierPayments/$id';
  static String supplierPaymentConfirm(String id) =>
      '$supplierPayments/$id/confirm';
  static String supplierPaymentReject(String id) =>
      '$supplierPayments/$id/reject';
  static String get supplierFundingSources =>
      '$supplierPayments/funding-sources';
  static String supplierCreditNote(String id) => '$supplierCreditNotes/$id';
  static String supplierCreditApprove(String id) =>
      '$supplierCreditNotes/$id/approve';
  static String supplierCreditAllocations(String id) =>
      '$supplierCreditNotes/$id/allocations';
}
