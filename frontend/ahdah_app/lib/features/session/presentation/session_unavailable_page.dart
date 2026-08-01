import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/widgets/auth_shell.dart';
import '../../../l10n/app_localizations.dart';
import 'session_controller.dart';

final class SessionUnavailablePage extends ConsumerWidget {
  const SessionUnavailablePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final loading =
        ref.watch(sessionControllerProvider).status ==
        SessionStatus.bootstrapping;
    return AuthShell(
      title: l10n.connectionUnavailableTitle,
      subtitle: l10n.connectionUnavailableBody,
      showBack: false,
      child: FilledButton.icon(
        onPressed: loading
            ? null
            : () => ref.read(sessionControllerProvider.notifier).retry(),
        icon: loading
            ? const SizedBox.square(
                dimension: 18,
                child: CircularProgressIndicator(strokeWidth: 2),
              )
            : const Icon(Icons.refresh),
        label: Text(l10n.retry),
      ),
    );
  }
}
