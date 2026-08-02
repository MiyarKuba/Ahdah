import 'package:flutter/material.dart';

import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';

final class ProjectStatusChip extends StatelessWidget {
  const ProjectStatusChip({required this.status, super.key});

  final String status;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final color = switch (status) {
      'Active' => scheme.primaryContainer,
      'Paused' => scheme.tertiaryContainer,
      'Completed' => scheme.secondaryContainer,
      'FinanciallyClosed' => scheme.surfaceContainerHighest,
      'Cancelled' => scheme.errorContainer,
      _ => scheme.surfaceContainerHighest,
    };
    return Chip(
      avatar: Icon(_icon, size: 17),
      label: Text(
        ValueLabels.projectStatus(AppLocalizations.of(context), status),
      ),
      backgroundColor: color,
      side: BorderSide.none,
    );
  }

  IconData get _icon => switch (status) {
    'Active' => Icons.play_circle_outline,
    'Paused' => Icons.pause_circle_outline,
    'Completed' => Icons.check_circle_outline,
    'FinanciallyClosed' => Icons.lock_outline,
    'Cancelled' => Icons.cancel_outlined,
    _ => Icons.help_outline,
  };
}

String formatLyd(String value) {
  final parts = value.trim().split('.');
  final whole = parts.first.isEmpty ? '0' : parts.first;
  final fraction = parts.length < 2
      ? '00'
      : parts[1].padRight(2, '0').substring(0, 2);
  final grouped = whole.replaceAllMapped(
    RegExp(r'\B(?=(\d{3})+(?!\d))'),
    (_) => ',',
  );
  return '$grouped.$fraction LYD';
}
