import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/localization/error_labels.dart';
import '../../../../core/widgets/form_widgets.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/advance_models.dart';
import '../../domain/advance_requests.dart';
import '../../domain/decimal_money.dart';
import '../controllers/advance_controllers.dart';
import '../widgets/advance_form_widgets.dart';
import '../widgets/advance_widgets.dart';

final class AdvanceDistributionPage extends ConsumerStatefulWidget {
  const AdvanceDistributionPage({required this.advanceId, super.key});

  final String advanceId;

  @override
  ConsumerState<AdvanceDistributionPage> createState() =>
      _AdvanceDistributionPageState();
}

class _AdvanceDistributionPageState
    extends ConsumerState<AdvanceDistributionPage> {
  final _formKey = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _bank = TextEditingController();
  final _reference = TextEditingController();
  final _description = TextEditingController();
  final _notes = TextEditingController();
  String? _recipientUserId;
  String _method = 'Cash';
  DateTime _transferDate = DateTime.now();

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      final role = ref.read(sessionControllerProvider).current!.role;
      ref
          .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
          .load();
      ref.read(advanceRecipientControllerProvider(role).notifier).load();
    });
  }

  @override
  void dispose() {
    _amount.dispose();
    _bank.dispose();
    _reference.dispose();
    _description.dispose();
    _notes.dispose();
    super.dispose();
  }

  Future<void> _complete(AdvanceTransferDetails? result) async {
    if (result == null || !mounted) return;
    await Future.wait([
      ref
          .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
          .load(),
      ref.read(advanceListControllerProvider.notifier).refresh(),
      ref.read(personalAdvanceBalanceControllerProvider.notifier).refresh(),
    ]);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(AppLocalizations.of(context).distributionPending)),
    );
    context.go('/advances/${widget.advanceId}/movements');
  }

  Future<void> _submit(AdvanceBalanceSummary balance) async {
    final l10n = AppLocalizations.of(context);
    if (!(_formKey.currentState?.validate() ?? false) ||
        _recipientUserId == null) {
      return;
    }
    final amount = DecimalMoney.canonicalize(_amount.text);
    if (amount == null ||
        !DecimalMoney.lessThanOrEqual(amount, balance.availableAmount)) {
      return;
    }
    final input = CreateAdvanceDistributionInput(
      recipientUserId: _recipientUserId!,
      transfer: AdvanceTransferFields(
        amount: amount,
        transferDate: _transferDate,
        transferMethod: _method,
        bankName: _bank.text,
        referenceNumber: _reference.text,
        description: _description.text,
        notes: _notes.text,
      ),
    );
    final role = ref.read(sessionControllerProvider).current!.role;
    final recipient = ref
        .read(advanceRecipientControllerProvider(role))
        .items
        .where((item) => item.id == _recipientUserId)
        .firstOrNull;
    final confirmed = await showAccessConfirmation(
      context,
      title: l10n.confirmFinancialOperationTitle,
      body:
          '${recipient?.fullName ?? ''}\n$amount ${balance.currencyCode}\n\n'
          '${l10n.distributionEffectBody}',
      confirmLabel: l10n.distributeMoney,
    );
    if (!confirmed || !mounted) return;
    await _complete(
      await ref
          .read(advanceDistributionControllerProvider.notifier)
          .distribute(widget.advanceId, input),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final session = ref.watch(sessionControllerProvider).current!;
    final detailsState = ref.watch(
      advanceDetailsControllerProvider(widget.advanceId),
    );
    final recipients = ref.watch(
      advanceRecipientControllerProvider(session.role),
    );
    final command = ref.watch(advanceDistributionControllerProvider);
    if (detailsState.details == null) {
      if (detailsState.phase == AdvanceDetailsPhase.failure) {
        return AccessFailureState(
          message: localizedError(l10n, detailsState.error!),
          onRetry: ref
              .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
              .load,
        );
      }
      return const Center(child: CircularProgressIndicator());
    }
    final details = detailsState.details!;
    final balance = details.balanceFor(session.user.userId);
    final supported =
        details.advance.status == 'Open' &&
        balance != null &&
        (DecimalMoney.toMinorUnits(balance.availableAmount) ?? BigInt.zero) >
            BigInt.zero;
    if (!supported) {
      return AccessFailureState(
        message: l10n.staleFinancialState,
        onRetry: ref
            .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
            .load,
      );
    }

    return Scaffold(
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.distributeMoney,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 8),
          MoneyText(
            amount: balance.availableAmount,
            currency: balance.currencyCode,
            style: Theme.of(context).textTheme.headlineSmall,
          ),
          Text(l10n.distributionEffectBody),
          const SizedBox(height: 12),
          FinancialCommandNotice(
            phase: command.phase,
            errorMessage: command.error == null
                ? null
                : localizedError(l10n, command.error!),
            onRetry: () async => _complete(
              await ref
                  .read(advanceDistributionControllerProvider.notifier)
                  .retrySameOperation(),
            ),
            onCancel: ref
                .read(advanceDistributionControllerProvider.notifier)
                .cancelPending,
          ),
          Form(
            key: _formKey,
            child: Column(
              children: [
                TextField(
                  key: const Key('distribution-recipient-search'),
                  maxLength: 100,
                  decoration: InputDecoration(
                    labelText: l10n.searchMembers,
                    prefixIcon: const Icon(Icons.search),
                    counterText: '',
                  ),
                  onChanged: (value) {
                    setState(() => _recipientUserId = null);
                    ref
                        .read(
                          advanceRecipientControllerProvider(
                            session.role,
                          ).notifier,
                        )
                        .setSearch(value);
                  },
                ),
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  key: const Key('distribution-recipient-field'),
                  initialValue: _recipientUserId,
                  isExpanded: true,
                  decoration: InputDecoration(labelText: l10n.selectRecipient),
                  items: [
                    for (final recipient in recipients.items)
                      DropdownMenuItem(
                        value: recipient.id,
                        child: Text(recipient.fullName),
                      ),
                  ],
                  validator: (value) =>
                      value == null ? l10n.requiredField : null,
                  onChanged: (value) =>
                      setState(() => _recipientUserId = value),
                ),
                if (recipients.phase == PagedReadPhase.empty)
                  Padding(
                    padding: const EdgeInsets.all(8),
                    child: Text(l10n.noEligibleRecipients),
                  ),
                if (recipients.hasMore)
                  TextButton.icon(
                    onPressed: recipients.phase == PagedReadPhase.loadingMore
                        ? null
                        : ref
                              .read(
                                advanceRecipientControllerProvider(
                                  session.role,
                                ).notifier,
                              )
                              .loadMore,
                    icon: const Icon(Icons.expand_more),
                    label: Text(l10n.retryLoadingMore),
                  ),
                const SizedBox(height: 12),
                AdvanceTransferFormFields(
                  amountController: _amount,
                  transferDate: _transferDate,
                  transferMethod: _method,
                  onSelectDate: () async {
                    final picked = await showDatePicker(
                      context: context,
                      initialDate: _transferDate,
                      firstDate: DateTime(2000),
                      lastDate: DateTime(2100),
                    );
                    if (picked != null) setState(() => _transferDate = picked);
                  },
                  onMethodChanged: (value) => setState(() => _method = value),
                  bankNameController: _bank,
                  referenceController: _reference,
                  descriptionController: _description,
                  notesController: _notes,
                  currencyCode: balance.currencyCode,
                  availableAmount: balance.availableAmount,
                ),
                const SizedBox(height: 16),
                SubmitButton(
                  label: l10n.distributeMoney,
                  loading: command.isSubmitting,
                  onPressed: () => _submit(balance),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
