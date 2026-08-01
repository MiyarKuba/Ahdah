import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../l10n/app_localizations.dart';
import '../storage/locale_controller.dart';

final class LanguageSwitcher extends ConsumerWidget {
  const LanguageSwitcher({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final locale = ref.watch(localeControllerProvider);
    return Semantics(
      button: true,
      label: l10n.changeLanguage,
      child: TextButton.icon(
        onPressed: () => ref.read(localeControllerProvider.notifier).toggle(),
        icon: const Icon(Icons.language),
        label: Text(locale.languageCode == 'ar' ? l10n.english : l10n.arabic),
      ),
    );
  }
}
