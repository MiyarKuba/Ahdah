final class RoleCapabilities {
  const RoleCapabilities._({
    required this.canManageAccess,
    required this.canViewProjects,
    required this.canManageProjects,
    required this.canViewCompanyDirectory,
    required this.canViewProjectMembers,
    required this.canViewContractValue,
    required this.requiresAssignedProjects,
  });

  final bool canManageAccess;
  final bool canViewProjects;
  final bool canManageProjects;
  final bool canViewCompanyDirectory;
  final bool canViewProjectMembers;
  final bool canViewContractValue;
  final bool requiresAssignedProjects;

  factory RoleCapabilities.forRole(String? role) => switch (role) {
    'Manager' => const RoleCapabilities._(
      canManageAccess: true,
      canViewProjects: true,
      canManageProjects: true,
      canViewCompanyDirectory: true,
      canViewProjectMembers: true,
      canViewContractValue: true,
      requiresAssignedProjects: false,
    ),
    'Deputy' => const RoleCapabilities._(
      canManageAccess: false,
      canViewProjects: true,
      canManageProjects: false,
      canViewCompanyDirectory: true,
      canViewProjectMembers: true,
      canViewContractValue: false,
      requiresAssignedProjects: false,
    ),
    'Accountant' => const RoleCapabilities._(
      canManageAccess: false,
      canViewProjects: true,
      canManageProjects: false,
      canViewCompanyDirectory: false,
      canViewProjectMembers: false,
      canViewContractValue: false,
      requiresAssignedProjects: false,
    ),
    'Supervisor' => const RoleCapabilities._(
      canManageAccess: false,
      canViewProjects: true,
      canManageProjects: false,
      canViewCompanyDirectory: false,
      canViewProjectMembers: true,
      canViewContractValue: false,
      requiresAssignedProjects: true,
    ),
    _ => const RoleCapabilities._(
      canManageAccess: false,
      canViewProjects: false,
      canManageProjects: false,
      canViewCompanyDirectory: false,
      canViewProjectMembers: false,
      canViewContractValue: false,
      requiresAssignedProjects: false,
    ),
  };
}
