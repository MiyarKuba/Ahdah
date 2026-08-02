import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../app/routing/app_routes.dart';
import '../../../l10n/app_localizations.dart';
import '../../access/domain/role_capabilities.dart';
import '../../session/presentation/session_controller.dart';

final class AuthenticatedShell extends ConsumerWidget {
  const AuthenticatedShell({
    required this.location,
    required this.child,
    super.key,
  });

  final String location;
  final Widget child;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final current = ref.watch(sessionControllerProvider).current!;
    final capabilities = RoleCapabilities.forRole(current.role);
    final destinations = <_ShellDestination>[
      _ShellDestination(
        path: AppRoutes.homePath,
        label: l10n.navHome,
        icon: Icons.home_outlined,
        selectedIcon: Icons.home,
      ),
      if (capabilities.canViewProjects)
        _ShellDestination(
          path: AppRoutes.projectsPath,
          label: l10n.navProjects,
          icon: Icons.business_outlined,
          selectedIcon: Icons.business,
        ),
      if (capabilities.canViewCompanyDirectory)
        _ShellDestination(
          path: AppRoutes.companyMembersPath,
          label: l10n.navCompanyMembers,
          icon: Icons.groups_outlined,
          selectedIcon: Icons.groups,
        ),
      if (capabilities.canManageAccess) ...[
        _ShellDestination(
          path: AppRoutes.invitationsPath,
          label: l10n.navInvitations,
          icon: Icons.mark_email_unread_outlined,
          selectedIcon: Icons.mark_email_unread,
        ),
        _ShellDestination(
          path: AppRoutes.joinRequestsPath,
          label: l10n.navJoinRequests,
          icon: Icons.group_add_outlined,
          selectedIcon: Icons.group_add,
        ),
      ],
      _ShellDestination(
        path: AppRoutes.accountPath,
        label: l10n.navAccount,
        icon: Icons.account_circle_outlined,
        selectedIcon: Icons.account_circle,
      ),
    ];
    final selectedIndex = destinations.indexWhere(
      (destination) => location.startsWith(destination.path),
    );
    final safeIndex = selectedIndex < 0 ? 0 : selectedIndex;
    final wide = MediaQuery.sizeOf(context).width >= 840;

    return Scaffold(
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(destinations[safeIndex].label),
            Text(
              '${current.company.companyName} · ${current.user.fullName}',
              overflow: TextOverflow.ellipsis,
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ],
        ),
      ),
      body: SafeArea(
        child: Row(
          children: [
            if (wide)
              NavigationRail(
                selectedIndex: safeIndex,
                labelType: NavigationRailLabelType.all,
                destinations: destinations
                    .map(
                      (destination) => NavigationRailDestination(
                        icon: Icon(destination.icon),
                        selectedIcon: Icon(destination.selectedIcon),
                        label: Text(destination.label),
                      ),
                    )
                    .toList(growable: false),
                onDestinationSelected: (index) =>
                    context.go(destinations[index].path),
              ),
            if (wide) const VerticalDivider(width: 1),
            Expanded(
              child: Center(
                child: ConstrainedBox(
                  constraints: const BoxConstraints(maxWidth: 1240),
                  child: child,
                ),
              ),
            ),
          ],
        ),
      ),
      bottomNavigationBar: wide
          ? null
          : NavigationBar(
              selectedIndex: safeIndex,
              destinations: destinations
                  .map(
                    (destination) => NavigationDestination(
                      icon: Icon(destination.icon),
                      selectedIcon: Icon(destination.selectedIcon),
                      label: destination.label,
                    ),
                  )
                  .toList(growable: false),
              onDestinationSelected: (index) =>
                  context.go(destinations[index].path),
            ),
    );
  }
}

final class _ShellDestination {
  const _ShellDestination({
    required this.path,
    required this.label,
    required this.icon,
    required this.selectedIcon,
  });

  final String path;
  final String label;
  final IconData icon;
  final IconData selectedIcon;
}
