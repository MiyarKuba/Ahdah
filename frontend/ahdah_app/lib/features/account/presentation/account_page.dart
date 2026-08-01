import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/value_labels.dart';
import '../../../core/widgets/language_switcher.dart';
import '../../../l10n/app_localizations.dart';
import '../../session/presentation/session_controller.dart';

final class AccountPage extends ConsumerWidget {
  const AccountPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final current = ref.watch(sessionControllerProvider).current!;
    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            l10n.accountTitle,
            style: Theme.of(
              context,
            ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w800),
          ),
          const SizedBox(height: 16),
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
                  _AccountDetail(
                    label: l10n.company,
                    value: current.company.companyName,
                  ),
                  _AccountDetail(
                    label: l10n.role,
                    value: ValueLabels.role(l10n, current.role),
                  ),
                  _AccountDetail(
                    label: l10n.accountStatus,
                    value: ValueLabels.userStatus(l10n, current.user.status),
                  ),
                  _AccountDetail(
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
          const SizedBox(height: 20),
          Card(
            child: Padding(
              padding: const EdgeInsets.all(20),
              child: Row(
                children: [
                  const Expanded(child: LanguageSwitcher()),
                  const SizedBox(width: 12),
                  Text(l10n.language),
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
    );
  }
}

final class _AccountDetail extends StatelessWidget {
  const _AccountDetail({required this.label, required this.value});
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: 16),
    child: Wrap(
      spacing: 12,
      runSpacing: 4,
      children: [
        SizedBox(
          width: 170,
          child: Text(
            label,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
        Text(value),
      ],
    ),
  );
}
