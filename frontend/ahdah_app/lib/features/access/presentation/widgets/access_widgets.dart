import 'package:flutter/material.dart';

import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';

enum AccessStatusKind { invitation, joinRequest }

final class AccessStatusChip extends StatelessWidget {
  const AccessStatusChip({required this.status, required this.kind, super.key});

  final String status;
  final AccessStatusKind kind;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final label = kind == AccessStatusKind.invitation
        ? ValueLabels.invitationStatus(l10n, status)
        : ValueLabels.joinRequestStatus(l10n, status);
    final scheme = Theme.of(context).colorScheme;
    final color = switch (status) {
      'Pending' => scheme.tertiaryContainer,
      'Accepted' || 'Approved' => scheme.primaryContainer,
      'Rejected' || 'Expired' => scheme.errorContainer,
      'Cancelled' => scheme.surfaceContainerHighest,
      _ => scheme.surfaceContainerHighest,
    };
    return Chip(
      avatar: Icon(_icon, size: 17),
      label: Text(label),
      backgroundColor: color,
      side: BorderSide.none,
    );
  }

  IconData get _icon => switch (status) {
    'Pending' => Icons.schedule,
    'Accepted' || 'Approved' => Icons.check_circle_outline,
    'Rejected' => Icons.block,
    'Expired' => Icons.timer_off_outlined,
    'Cancelled' => Icons.cancel_outlined,
    _ => Icons.help_outline,
  };
}

final class AccessEmptyState extends StatelessWidget {
  const AccessEmptyState({required this.title, required this.body, super.key});
  final String title;
  final String body;

  @override
  Widget build(BuildContext context) => Center(
    child: ConstrainedBox(
      constraints: const BoxConstraints(maxWidth: 440),
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.inbox_outlined,
              size: 52,
              color: Theme.of(context).colorScheme.primary,
            ),
            const SizedBox(height: 16),
            Text(title, style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: 8),
            Text(body, textAlign: TextAlign.center),
          ],
        ),
      ),
    ),
  );
}

final class AccessFailureState extends StatelessWidget {
  const AccessFailureState({
    required this.message,
    required this.onRetry,
    super.key,
  });

  final String message;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) => Center(
    child: ConstrainedBox(
      constraints: const BoxConstraints(maxWidth: 440),
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.cloud_off_outlined,
              size: 52,
              color: Theme.of(context).colorScheme.error,
            ),
            const SizedBox(height: 16),
            Text(message, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton.icon(
              onPressed: onRetry,
              icon: const Icon(Icons.refresh),
              label: Text(AppLocalizations.of(context).retry),
            ),
          ],
        ),
      ),
    ),
  );
}

final class AccessDetail extends StatelessWidget {
  const AccessDetail({required this.label, required this.value, super.key});
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(top: 8),
    child: Wrap(
      spacing: 8,
      runSpacing: 4,
      children: [
        Text(label, style: const TextStyle(fontWeight: FontWeight.w700)),
        SelectableText(value),
      ],
    ),
  );
}

Future<bool> showAccessConfirmation(
  BuildContext context, {
  required String title,
  required String body,
  required String confirmLabel,
}) async =>
    await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: Text(body),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: Text(AppLocalizations.of(context).cancelAction),
          ),
          FilledButton(
            autofocus: true,
            onPressed: () => Navigator.pop(context, true),
            child: Text(confirmLabel),
          ),
        ],
      ),
    ) ??
    false;
