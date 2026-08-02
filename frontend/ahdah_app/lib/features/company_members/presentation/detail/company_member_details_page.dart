import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../controllers/company_member_controllers.dart';

final class CompanyMemberDetailsPage extends ConsumerStatefulWidget {
  const CompanyMemberDetailsPage({required this.memberId, super.key});

  final String memberId;

  @override
  ConsumerState<CompanyMemberDetailsPage> createState() =>
      _CompanyMemberDetailsPageState();
}

class _CompanyMemberDetailsPageState
    extends ConsumerState<CompanyMemberDetailsPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(
            companyMemberDetailsControllerProvider(widget.memberId).notifier,
          )
          .load();
    });
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(
      companyMemberDetailsControllerProvider(widget.memberId),
    );
    final l10n = AppLocalizations.of(context);
    if (state.phase == CompanyMemberDetailsPhase.initial ||
        state.phase == CompanyMemberDetailsPhase.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == CompanyMemberDetailsPhase.failure) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: () => ref
            .read(
              companyMemberDetailsControllerProvider(widget.memberId).notifier,
            )
            .load(),
      );
    }
    final member = state.member!;
    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Card(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      member.fullName,
                      style: Theme.of(context).textTheme.headlineSmall,
                    ),
                  ),
                  Chip(label: Text(l10n.readOnly)),
                ],
              ),
              const Divider(height: 32),
              AccessDetail(
                label: l10n.role,
                value: ValueLabels.role(l10n, member.role),
              ),
              AccessDetail(
                label: l10n.accountStatus,
                value: ValueLabels.userStatus(l10n, member.status),
              ),
              AccessDetail(
                label: l10n.identityStatus,
                value: ValueLabels.identityStatus(
                  l10n,
                  member.identityVerificationStatus,
                ),
              ),
              AccessDetail(label: l10n.phoneNumber, value: member.phoneNumber),
              if (member.email != null)
                AccessDetail(label: l10n.email, value: member.email!),
              AccessDetail(
                label: l10n.createdAt,
                value: AppDateTimeFormatter.format(
                  context,
                  member.createdAtUtc,
                ),
              ),
              AccessDetail(
                label: l10n.updatedAt,
                value: AppDateTimeFormatter.format(
                  context,
                  member.updatedAtUtc,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
