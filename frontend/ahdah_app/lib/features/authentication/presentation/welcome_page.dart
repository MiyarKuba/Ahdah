import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../app/routing/app_routes.dart';
import '../../../core/widgets/auth_shell.dart';
import '../../../l10n/app_localizations.dart';

final class WelcomePage extends StatelessWidget {
  const WelcomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return AuthShell(
      title: l10n.welcomeTitle,
      subtitle: l10n.welcomeBody,
      showBack: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          FilledButton.icon(
            onPressed: () => context.goNamed(AppRoutes.login),
            icon: const Icon(Icons.login),
            label: Text(l10n.login),
          ),
          const SizedBox(height: 12),
          OutlinedButton.icon(
            onPressed: () => context.goNamed(AppRoutes.registerCompany),
            icon: const Icon(Icons.business_outlined),
            label: Text(l10n.createCompany),
          ),
          const SizedBox(height: 12),
          OutlinedButton.icon(
            onPressed: () => context.goNamed(AppRoutes.joinRequest),
            icon: const Icon(Icons.group_add_outlined),
            label: Text(l10n.submitJoinRequest),
          ),
          const SizedBox(height: 12),
          TextButton.icon(
            onPressed: () => context.goNamed(AppRoutes.acceptInvitation),
            icon: const Icon(Icons.mark_email_read_outlined),
            label: Text(l10n.acceptInvitation),
          ),
          const SizedBox(height: 24),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.shield_outlined, size: 18),
              const SizedBox(width: 8),
              Flexible(child: Text(l10n.secureSessionNote)),
            ],
          ),
        ],
      ),
    );
  }
}
