final class RoleCapabilities {
  const RoleCapabilities._({
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
  });

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

  factory RoleCapabilities.forRole(String? role) => switch (role) {
    'Manager' => const RoleCapabilities._(
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
    ),
    'Deputy' => const RoleCapabilities._(
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
    ),
    'Accountant' => const RoleCapabilities._(
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
    ),
    'Supervisor' => const RoleCapabilities._(
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
    ),
    'Worker' => const RoleCapabilities._(
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
    ),
    _ => const RoleCapabilities._(
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
    ),
  };
}
