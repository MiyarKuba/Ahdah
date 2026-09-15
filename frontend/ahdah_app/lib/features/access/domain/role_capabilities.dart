final class RoleCapabilities {
  const RoleCapabilities._({
    required this.role,
    required this.canManageAccess,
    required this.canViewProjects,
    required this.canManageProjects,
    required this.canViewCompanyDirectory,
    required this.canViewProjectMembers,
    required this.canViewContractValue,
    required this.requiresAssignedProjects,
    required this.canViewAdvances,
    required this.canCreateTopLevelAdvance,
    required this.canDistributeAdvances,
    required this.canViewAuthorizedBalances,
    required this.canSelectAuthorizedBalanceUser,
    required this.canConfirmReceivedAdvance,
    required this.canRejectReceivedTransfer,
    required this.canReturnHeldBalance,
    required this.canViewExpenses,
    required this.canCreateExpenses,
    required this.canReviewExpenses,
    required this.canManageExpenseCategories,
    required this.canViewReimbursements,
    required this.canCreateProjectLinkedExpense,
    required this.canUseAdvanceBalanceForExpense,
    required this.canCreatePersonalFundsExpense,
    required this.canContributeExpenseDocuments,
  });

  final String? role;
  final bool canManageAccess;
  final bool canViewProjects;
  final bool canManageProjects;
  final bool canViewCompanyDirectory;
  final bool canViewProjectMembers;
  final bool canViewContractValue;
  final bool requiresAssignedProjects;
  final bool canViewAdvances;
  final bool canCreateTopLevelAdvance;
  final bool canDistributeAdvances;
  final bool canViewAuthorizedBalances;
  final bool canSelectAuthorizedBalanceUser;
  final bool canConfirmReceivedAdvance;
  final bool canRejectReceivedTransfer;
  final bool canReturnHeldBalance;
  final bool canViewExpenses;
  final bool canCreateExpenses;
  final bool canReviewExpenses;
  final bool canManageExpenseCategories;
  final bool canViewReimbursements;
  final bool canCreateProjectLinkedExpense;
  final bool canUseAdvanceBalanceForExpense;
  final bool canCreatePersonalFundsExpense;
  final bool canContributeExpenseDocuments;

  bool get canViewSuppliers => switch (role) {
    'Manager' || 'Deputy' || 'Accountant' || 'Supervisor' => true,
    _ => false,
  };
  bool get canManageSuppliers => role == 'Manager';
  bool get canCreateSupplierInvoice =>
      role == 'Manager' || role == 'Deputy' || role == 'Accountant';
  bool get canViewSupplierFinancials =>
      role == 'Manager' || role == 'Deputy' || role == 'Accountant';
  bool get canRecordSupplierPayment =>
      role == 'Manager' || role == 'Accountant';
  bool get canConfirmSupplierPayment => canRecordSupplierPayment;
  bool get canManageSupplierCredits => canRecordSupplierPayment;
  bool get canCreateSupplierPaymentAccount => canRecordSupplierPayment;
  bool get canViewSupplierRefunds => canViewSupplierFinancials;
  bool get canViewSupplierStatement => canViewSupplierFinancials;
  bool get requiresAssignedSupplierProjects => role == 'Supervisor';

  factory RoleCapabilities.forRole(String? role) => switch (role) {
    'Manager' => const RoleCapabilities._(
      role: 'Manager',
      canManageAccess: true,
      canViewProjects: true,
      canManageProjects: true,
      canViewCompanyDirectory: true,
      canViewProjectMembers: true,
      canViewContractValue: true,
      requiresAssignedProjects: false,
      canViewAdvances: true,
      canCreateTopLevelAdvance: true,
      canDistributeAdvances: true,
      canViewAuthorizedBalances: true,
      canSelectAuthorizedBalanceUser: true,
      canConfirmReceivedAdvance: true,
      canRejectReceivedTransfer: true,
      canReturnHeldBalance: true,
      canViewExpenses: true,
      canCreateExpenses: true,
      canReviewExpenses: true,
      canManageExpenseCategories: true,
      canViewReimbursements: true,
      canCreateProjectLinkedExpense: true,
      canUseAdvanceBalanceForExpense: true,
      canCreatePersonalFundsExpense: true,
      canContributeExpenseDocuments: true,
    ),
    'Deputy' => const RoleCapabilities._(
      role: 'Deputy',
      canManageAccess: false,
      canViewProjects: true,
      canManageProjects: false,
      canViewCompanyDirectory: true,
      canViewProjectMembers: true,
      canViewContractValue: false,
      requiresAssignedProjects: false,
      canViewAdvances: true,
      canCreateTopLevelAdvance: false,
      canDistributeAdvances: true,
      canViewAuthorizedBalances: true,
      canSelectAuthorizedBalanceUser: true,
      canConfirmReceivedAdvance: true,
      canRejectReceivedTransfer: true,
      canReturnHeldBalance: true,
      canViewExpenses: true,
      canCreateExpenses: true,
      canReviewExpenses: true,
      canManageExpenseCategories: false,
      canViewReimbursements: true,
      canCreateProjectLinkedExpense: true,
      canUseAdvanceBalanceForExpense: true,
      canCreatePersonalFundsExpense: true,
      canContributeExpenseDocuments: true,
    ),
    'Accountant' => const RoleCapabilities._(
      role: 'Accountant',
      canManageAccess: false,
      canViewProjects: true,
      canManageProjects: false,
      canViewCompanyDirectory: false,
      canViewProjectMembers: false,
      canViewContractValue: false,
      requiresAssignedProjects: false,
      canViewAdvances: true,
      canCreateTopLevelAdvance: false,
      canDistributeAdvances: false,
      canViewAuthorizedBalances: true,
      canSelectAuthorizedBalanceUser: false,
      canConfirmReceivedAdvance: false,
      canRejectReceivedTransfer: false,
      canReturnHeldBalance: false,
      canViewExpenses: true,
      canCreateExpenses: false,
      canReviewExpenses: true,
      canManageExpenseCategories: false,
      canViewReimbursements: true,
      canCreateProjectLinkedExpense: false,
      canUseAdvanceBalanceForExpense: false,
      canCreatePersonalFundsExpense: false,
      canContributeExpenseDocuments: true,
    ),
    'Supervisor' => const RoleCapabilities._(
      role: 'Supervisor',
      canManageAccess: false,
      canViewProjects: true,
      canManageProjects: false,
      canViewCompanyDirectory: false,
      canViewProjectMembers: true,
      canViewContractValue: false,
      requiresAssignedProjects: true,
      canViewAdvances: true,
      canCreateTopLevelAdvance: false,
      canDistributeAdvances: false,
      canViewAuthorizedBalances: false,
      canSelectAuthorizedBalanceUser: false,
      canConfirmReceivedAdvance: true,
      canRejectReceivedTransfer: true,
      canReturnHeldBalance: true,
      canViewExpenses: true,
      canCreateExpenses: true,
      canReviewExpenses: false,
      canManageExpenseCategories: false,
      canViewReimbursements: true,
      canCreateProjectLinkedExpense: true,
      canUseAdvanceBalanceForExpense: true,
      canCreatePersonalFundsExpense: true,
      canContributeExpenseDocuments: true,
    ),
    'Worker' => const RoleCapabilities._(
      role: 'Worker',
      canManageAccess: false,
      canViewProjects: false,
      canManageProjects: false,
      canViewCompanyDirectory: false,
      canViewProjectMembers: false,
      canViewContractValue: false,
      requiresAssignedProjects: false,
      canViewAdvances: true,
      canCreateTopLevelAdvance: false,
      canDistributeAdvances: false,
      canViewAuthorizedBalances: false,
      canSelectAuthorizedBalanceUser: false,
      canConfirmReceivedAdvance: true,
      canRejectReceivedTransfer: true,
      canReturnHeldBalance: true,
      canViewExpenses: true,
      canCreateExpenses: true,
      canReviewExpenses: false,
      canManageExpenseCategories: false,
      canViewReimbursements: true,
      canCreateProjectLinkedExpense: false,
      canUseAdvanceBalanceForExpense: true,
      canCreatePersonalFundsExpense: true,
      canContributeExpenseDocuments: true,
    ),
    _ => const RoleCapabilities._(
      role: null,
      canManageAccess: false,
      canViewProjects: false,
      canManageProjects: false,
      canViewCompanyDirectory: false,
      canViewProjectMembers: false,
      canViewContractValue: false,
      requiresAssignedProjects: false,
      canViewAdvances: false,
      canCreateTopLevelAdvance: false,
      canDistributeAdvances: false,
      canViewAuthorizedBalances: false,
      canSelectAuthorizedBalanceUser: false,
      canConfirmReceivedAdvance: false,
      canRejectReceivedTransfer: false,
      canReturnHeldBalance: false,
      canViewExpenses: false,
      canCreateExpenses: false,
      canReviewExpenses: false,
      canManageExpenseCategories: false,
      canViewReimbursements: false,
      canCreateProjectLinkedExpense: false,
      canUseAdvanceBalanceForExpense: false,
      canCreatePersonalFundsExpense: false,
      canContributeExpenseDocuments: false,
    ),
  };
}
