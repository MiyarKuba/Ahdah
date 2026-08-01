import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../app/routing/app_routes.dart';
import '../../../core/widgets/auth_shell.dart';
import '../../../l10n/app_localizations.dart';
import '../domain/pending_result.dart';

final class PendingPage extends StatelessWidget {
  const PendingPage({this.result, super.key});

  final PendingOnboardingResult? result;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final kind = result?.kind ?? PendingResultKind.generic;
    final (title, body) = switch (kind) {
      PendingResultKind.joinRequestSubmitted => (
        l10n.joinPendingTitle,
        l10n.joinPendingBody,
      ),
      PendingResultKind.identityVerificationRequired => (
        l10n.identityPendingTitle,
        l10n.identityPendingBody,
      ),
      PendingResultKind.managerApprovalRequired => (
        l10n.approvalPendingTitle,
        l10n.approvalPendingBody,
      ),
      PendingResultKind.generic => (l10n.pendingTitle, l10n.genericPendingBody),
    };
    return AuthShell(
      title: title,
      subtitle: body,
      showBack: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Icon(
            Icons.hourglass_top_rounded,
            size: 56,
            color: Theme.of(context).colorScheme.primary,
          ),
          const SizedBox(height: 24),
          FilledButton(
            onPressed: () => context.goNamed(AppRoutes.login),
            child: Text(l10n.backToLogin),
          ),
        ],
      ),
    );
  }
}
