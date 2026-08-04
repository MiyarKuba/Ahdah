import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/decimal_money.dart';
import '../controllers/advance_controllers.dart';
import '../widgets/advance_widgets.dart';

final class AdvanceDetailsPage extends ConsumerStatefulWidget {
  const AdvanceDetailsPage({required this.advanceId, super.key});

  final String advanceId;

  @override
  ConsumerState<AdvanceDetailsPage> createState() => _AdvanceDetailsPageState();
}

class _AdvanceDetailsPageState extends ConsumerState<AdvanceDetailsPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
          .load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(advanceDetailsControllerProvider(widget.advanceId));
    final session = ref.watch(sessionControllerProvider).current!;
    final capabilities = RoleCapabilities.forRole(session.role);

    if (state.phase == AdvanceDetailsPhase.initial ||
        state.phase == AdvanceDetailsPhase.loading && state.details == null) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == AdvanceDetailsPhase.failure && state.details == null) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: ref
            .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
            .load,
      );
    }

    final details = state.details!;
    final advance = details.advance;
    final currentBalance = details.balanceFor(session.user.userId);
    final hasAvailable =
        currentBalance != null &&
        (DecimalMoney.toMinorUnits(currentBalance.availableAmount) ??
                BigInt.zero) >
            BigInt.zero;
    final canDistribute =
        capabilities.canDistributeAdvances &&
        advance.status == 'Open' &&
        hasAvailable;
    final canReturn =
        capabilities.canReturnHeldBalance &&
        advance.status == 'Open' &&
        hasAvailable;
    final personalOnly =
        session.role == 'Supervisor' || session.role == 'Worker';
    final balances = personalOnly
        ? details.balances
              .where((item) => item.holder.userId == session.user.userId)
              .toList(growable: false)
        : details.balances;

    return Scaffold(
      body: RefreshIndicator(
        onRefresh: ref
            .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
            .load,
        child: ListView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.all(16),
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        l10n.advanceDetails,
                        style: Theme.of(context).textTheme.headlineMedium,
                      ),
                      SelectableText(advance.advanceNumber),
                    ],
                  ),
                ),
                AdvanceStatusChip(status: advance.status),
              ],
            ),
            const SizedBox(height: 16),
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    MoneyText(
                      amount: advance.advanceAmount,
                      currency: advance.currencyCode,
                      style: Theme.of(context).textTheme.headlineMedium
                          ?.copyWith(fontWeight: FontWeight.w700),
                    ),
                    AccessDetail(
                      label: l10n.availableAmount,
                      value:
                          '${advance.availableAmount} ${advance.currencyCode}',
                    ),
                    AccessDetail(
                      label: l10n.reservedAmount,
                      value:
                          '${advance.reservedAmount} ${advance.currencyCode}',
                    ),
                    AccessDetail(
                      label: l10n.recipient,
                      value: advance.recipient.fullName,
                    ),
                    AccessDetail(label: l10n.purpose, value: advance.purpose),
                    AccessDetail(
                      label: l10n.issueDate,
                      value: AppDateTimeFormatter.formatDate(
                        context,
                        advance.issueDate,
                      ),
                    ),
                    if (advance.settlementDueDate != null)
                      AccessDetail(
                        label: l10n.settlementDueDateOptional,
                        value: AppDateTimeFormatter.formatDate(
                          context,
                          advance.settlementDueDate,
                        ),
                      ),
                    if (advance.notes != null)
                      AccessDetail(
                        label: l10n.notesOptional,
                        value: advance.notes!,
                      ),
                    AccessDetail(
                      label: l10n.createdAt,
                      value: AppDateTimeFormatter.format(
                        context,
                        advance.createdAtUtc,
                      ),
                    ),
                    if (advance.confirmedAtUtc != null)
                      AccessDetail(
                        label: l10n.confirmed,
                        value: AppDateTimeFormatter.format(
                          context,
                          advance.confirmedAtUtc,
                        ),
                      ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                FilledButton.tonalIcon(
                  onPressed: () => context.go(
                    '${AppRoutes.advancesPath}/${widget.advanceId}/movements',
                  ),
                  icon: const Icon(Icons.timeline),
                  label: Text(l10n.viewMovements),
                ),
                if (canDistribute)
                  FilledButton.icon(
                    key: const Key('distribute-advance-action'),
                    onPressed: () => context.go(
                      '${AppRoutes.advancesPath}/${widget.advanceId}/distribute',
                    ),
                    icon: const Icon(Icons.call_split),
                    label: Text(l10n.distributeMoney),
                  ),
                if (canReturn)
                  OutlinedButton.icon(
                    key: const Key('return-advance-action'),
                    onPressed: () => context.go(
                      '${AppRoutes.advancesPath}/${widget.advanceId}/return',
                    ),
                    icon: const Icon(Icons.keyboard_return),
                    label: Text(l10n.returnUnusedMoney),
                  ),
              ],
            ),
            const SizedBox(height: 20),
            Text(
              l10n.fundingSources,
              style: Theme.of(context).textTheme.titleLarge,
            ),
            if (details.fundings.isEmpty)
              Text(l10n.noFundingSources)
            else
              for (final funding in details.fundings)
                Card(
                  child: ListTile(
                    leading: const Icon(Icons.account_balance_outlined),
                    title: Text(
                      ValueLabels.fundingType(l10n, funding.sourceType),
                    ),
                    subtitle: Text(
                      '${ValueLabels.fundingStatus(l10n, funding.fundingStatus)} · '
                      '${funding.paymentMethods.map((item) => ValueLabels.paymentMethod(l10n, item)).join(', ')}',
                    ),
                    trailing: MoneyText(
                      amount: funding.allocatedAmount,
                      currency: advance.currencyCode,
                    ),
                  ),
                ),
            const SizedBox(height: 20),
            Text(
              l10n.userBalances,
              style: Theme.of(context).textTheme.titleLarge,
            ),
            if (balances.isEmpty)
              Text(l10n.noBalancesBody)
            else
              for (final balance in balances)
                AdvanceBalanceCard(balance: balance),
            const SizedBox(height: 12),
            Text(l10n.pendingReservationsNotice),
            Text(l10n.expensesNotImplemented),
            Text(l10n.settlementNotImplemented),
          ],
        ),
      ),
    );
  }
}
