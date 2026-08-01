import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../app/routing/app_routes.dart';
import '../../../core/localization/value_labels.dart';
import '../../../l10n/app_localizations.dart';
import '../../access/domain/role_capabilities.dart';
import '../../session/presentation/session_controller.dart';

final class HomePage extends ConsumerWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final current = ref.watch(sessionControllerProvider).current!;
    final canManage = RoleCapabilities.forRole(current.role).canManageAccess;
    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            l10n.welcomeUser(current.user.fullName),
            style: Theme.of(
              context,
            ).textTheme.headlineMedium?.copyWith(fontWeight: FontWeight.w800),
          ),
          const SizedBox(height: 8),
          Text(canManage ? l10n.managerOverviewBody : l10n.memberOverviewBody),
          const SizedBox(height: 24),
          Card(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Wrap(
                spacing: 32,
                runSpacing: 20,
                children: [
                  _OverviewValue(
                    label: l10n.company,
                    value: current.company.companyName,
                  ),
                  _OverviewValue(
                    label: l10n.role,
                    value: ValueLabels.role(l10n, current.role),
                  ),
                  _OverviewValue(
                    label: l10n.accountStatus,
                    value: ValueLabels.userStatus(l10n, current.user.status),
                  ),
                  _OverviewValue(
                    label: l10n.identityStatus,
                    value: ValueLabels.identityStatus(
                      l10n,
                      current.user.identityVerificationStatus,
                    ),
                  ),
                ],
              ),
            ),
          ),
          if (canManage) ...[
            const SizedBox(height: 24),
            Text(
              l10n.accessAdministration,
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 16,
              runSpacing: 16,
              children: [
                _ShortcutCard(
                  icon: Icons.mark_email_unread_outlined,
                  title: l10n.navInvitations,
                  body: l10n.invitationShortcutBody,
                  onTap: () => context.go(AppRoutes.invitationsPath),
                ),
                _ShortcutCard(
                  icon: Icons.group_add_outlined,
                  title: l10n.navJoinRequests,
                  body: l10n.joinRequestShortcutBody,
                  onTap: () => context.go(AppRoutes.joinRequestsPath),
                ),
              ],
            ),
          ],
          const SizedBox(height: 24),
          Card(
            color: Theme.of(context).colorScheme.surfaceContainerLow,
            child: Padding(
              padding: const EdgeInsets.all(20),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Icon(Icons.info_outline),
                  const SizedBox(width: 12),
                  Expanded(child: Text(l10n.financialModulesDeferred)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

final class _OverviewValue extends StatelessWidget {
  const _OverviewValue({required this.label, required this.value});
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) => SizedBox(
    width: 230,
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: Theme.of(context).textTheme.labelLarge),
        const SizedBox(height: 4),
        Text(value, style: Theme.of(context).textTheme.titleMedium),
      ],
    ),
  );
}

final class _ShortcutCard extends StatelessWidget {
  const _ShortcutCard({
    required this.icon,
    required this.title,
    required this.body,
    required this.onTap,
  });
  final IconData icon;
  final String title;
  final String body;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) => SizedBox(
    width: 330,
    child: Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: Row(
            children: [
              Icon(icon, size: 34),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(title, style: Theme.of(context).textTheme.titleMedium),
                    const SizedBox(height: 4),
                    Text(body),
                  ],
                ),
              ),
              const Icon(Icons.chevron_right),
            ],
          ),
        ),
      ),
    ),
  );
}
