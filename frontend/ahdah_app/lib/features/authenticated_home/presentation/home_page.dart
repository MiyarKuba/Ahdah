import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/value_labels.dart';
import '../../../core/widgets/language_switcher.dart';
import '../../../l10n/app_localizations.dart';
import '../../session/presentation/session_controller.dart';

final class HomePage extends ConsumerWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final current = ref.watch(sessionControllerProvider).current!;
    return Scaffold(
      appBar: AppBar(
        title: Text(l10n.arabicAppName),
        actions: const [LanguageSwitcher(), SizedBox(width: 8)],
      ),
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(20),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 720),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    l10n.homeFoundationTitle,
                    style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(l10n.homeFoundationBody),
                  const SizedBox(height: 24),
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(24),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            current.user.fullName,
                            style: Theme.of(context).textTheme.titleLarge,
                          ),
                          const Divider(height: 32),
                          _SafeDetail(
                            label: l10n.company,
                            value: current.company.companyName,
                          ),
                          _SafeDetail(
                            label: l10n.role,
                            value: ValueLabels.role(l10n, current.role),
                          ),
                          _SafeDetail(
                            label: l10n.accountStatus,
                            value: ValueLabels.userStatus(
                              l10n,
                              current.user.status,
                            ),
                          ),
                          _SafeDetail(
                            label: l10n.companyStatus,
                            value: ValueLabels.companyStatus(
                              l10n,
                              current.company.status,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),
                  Text(l10n.logoutExplanation),
                  const SizedBox(height: 12),
                  OutlinedButton.icon(
                    onPressed: () =>
                        ref.read(sessionControllerProvider.notifier).logout(),
                    icon: const Icon(Icons.logout),
                    label: Text(l10n.logout),
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

final class _SafeDetail extends StatelessWidget {
  const _SafeDetail({required this.label, required this.value});
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: 16),
    child: Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 140,
          child: Text(
            label,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
        Expanded(child: Text(value)),
      ],
    ),
  );
}
