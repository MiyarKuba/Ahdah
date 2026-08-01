import 'package:flutter/material.dart';

import '../../../core/widgets/language_switcher.dart';
import '../../../l10n/app_localizations.dart';

final class ConfigurationErrorPage extends StatelessWidget {
  const ConfigurationErrorPage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Scaffold(
      body: SafeArea(
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 560),
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Align(
                    alignment: AlignmentDirectional.centerEnd,
                    child: LanguageSwitcher(),
                  ),
                  const Icon(Icons.settings_ethernet, size: 52),
                  const SizedBox(height: 20),
                  Text(
                    l10n.configurationTitle,
                    style: Theme.of(context).textTheme.headlineSmall,
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 12),
                  SelectableText(
                    l10n.configurationBody,
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
