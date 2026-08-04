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

final class AdvanceReturnPage extends ConsumerStatefulWidget {
  const AdvanceReturnPage({required this.advanceId, super.key});

  final String advanceId;

  @override
  ConsumerState<AdvanceReturnPage> createState() => _AdvanceReturnPageState();
}

class _AdvanceReturnPageState extends ConsumerState<AdvanceReturnPage> {
  final _formKey = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _bank = TextEditingController();
  final _reference = TextEditingController();
  final _description = TextEditingController();
  final _notes = TextEditingController();
  String _method = 'Cash';
  DateTime _transferDate = DateTime.now();

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
      SnackBar(content: Text(AppLocalizations.of(context).returnPending)),
    );
    context.go('/advances/${widget.advanceId}/movements');
  }

  Future<void> _submit(AdvanceBalanceSummary balance) async {
    final l10n = AppLocalizations.of(context);
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final amount = DecimalMoney.canonicalize(_amount.text);
    if (amount == null ||
        !DecimalMoney.lessThanOrEqual(amount, balance.availableAmount)) {
      return;
    }
    final input = CreateAdvanceReturnInput(
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
    final confirmed = await showAccessConfirmation(
      context,
      title: l10n.confirmFinancialOperationTitle,
      body:
          '$amount ${balance.currencyCode}\n\n${l10n.returnDestinationDerived}',
      confirmLabel: l10n.returnUnusedMoney,
    );
    if (!confirmed || !mounted) return;
    await _complete(
      await ref
          .read(advanceReturnControllerProvider.notifier)
          .returnMoney(widget.advanceId, input),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final session = ref.watch(sessionControllerProvider).current!;
    final detailsState = ref.watch(
      advanceDetailsControllerProvider(widget.advanceId),
    );
    final command = ref.watch(advanceReturnControllerProvider);
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
            l10n.returnUnusedMoney,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 8),
          MoneyText(
            amount: balance.availableAmount,
            currency: balance.currencyCode,
            style: Theme.of(context).textTheme.headlineSmall,
          ),
          const SizedBox(height: 8),
          AppMessageBanner(message: l10n.returnDestinationDerived),
          const SizedBox(height: 12),
          FinancialCommandNotice(
            phase: command.phase,
            errorMessage: command.error == null
                ? null
                : localizedError(l10n, command.error!),
            onRetry: () async => _complete(
              await ref
                  .read(advanceReturnControllerProvider.notifier)
                  .retrySameOperation(),
            ),
            onCancel: ref
                .read(advanceReturnControllerProvider.notifier)
                .cancelPending,
          ),
          Form(
            key: _formKey,
            child: Column(
              children: [
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
                  key: const Key('submit-return-action'),
                  label: l10n.returnUnusedMoney,
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
