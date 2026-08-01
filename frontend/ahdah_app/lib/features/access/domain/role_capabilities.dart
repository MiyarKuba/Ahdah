final class RoleCapabilities {
  const RoleCapabilities._({required this.canManageAccess});

  final bool canManageAccess;

  factory RoleCapabilities.forRole(String? role) =>
      RoleCapabilities._(canManageAccess: role == 'Manager');
}
