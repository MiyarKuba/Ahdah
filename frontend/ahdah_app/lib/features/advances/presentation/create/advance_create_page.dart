import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../core/widgets/form_widgets.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../domain/advance_models.dart';
import '../../domain/advance_requests.dart';
import '../../domain/decimal_money.dart';
import '../controllers/advance_controllers.dart';
import '../widgets/advance_form_widgets.dart';
import '../widgets/advance_widgets.dart';

final class AdvanceCreatePage extends ConsumerStatefulWidget {
  const AdvanceCreatePage({super.key});

  @override
  ConsumerState<AdvanceCreatePage> createState() => _AdvanceCreatePageState();
}

class _AdvanceCreatePageState extends ConsumerState<AdvanceCreatePage> {
  final _formKey = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _purpose = TextEditingController();
  final _bank = TextEditingController();
  final _reference = TextEditingController();
  final _description = TextEditingController();
  final _notes = TextEditingController();
  final _fundingAmounts = <String, TextEditingController>{};
  String? _recipientUserId;
  String _method = 'Cash';
  DateTime _issueDate = DateTime.now();
  DateTime _transferDate = DateTime.now();
  DateTime? _settlementDueDate;
  String? _fundingError;

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref.read(advanceRecipientControllerProvider('Manager').notifier).load();
      ref.read(fundingSourceControllerProvider.notifier).load();
    });
  }

  @override
  void dispose() {
    _amount.dispose();
    _purpose.dispose();
    _bank.dispose();
    _reference.dispose();
    _description.dispose();
    _notes.dispose();
    for (final controller in _fundingAmounts.values) {
      controller.dispose();
    }
    super.dispose();
  }

  Future<DateTime?> _pickDate(DateTime initial) => showDatePicker(
    context: context,
    initialDate: initial,
    firstDate: DateTime(2000),
    lastDate: DateTime(2100),
  );

  AdvanceTransferFields _transfer(String amount) => AdvanceTransferFields(
    amount: amount,
    transferDate: _transferDate,
    transferMethod: _method,
    bankName: _bank.text,
    referenceNumber: _reference.text,
    description: _description.text,
    notes: _notes.text,
  );

  Future<void> _complete(AdvanceDetails? result) async {
    if (result == null || !mounted) return;
    ref.read(advanceListControllerProvider.notifier).refresh();
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(AppLocalizations.of(context).advanceCreatedPending),
      ),
    );
    context.go('/advances/${result.advance.advanceId}');
  }

  Future<void> _submit(List<AvailableFundingSource> sources) async {
    final l10n = AppLocalizations.of(context);
    setState(() => _fundingError = null);
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final amount = DecimalMoney.canonicalize(_amount.text);
    if (amount == null || _recipientUserId == null) return;

    final allocations = <FundingAllocationInput>[];
    final selectedSources = <AvailableFundingSource>[];
    for (final source in sources) {
      final raw = _fundingAmounts[source.fundingSourceId]?.text.trim() ?? '';
      if (raw.isEmpty) continue;
      final value = DecimalMoney.canonicalize(raw);
      if (value == null ||
          !DecimalMoney.lessThanOrEqual(value, source.availableAmount)) {
        setState(() => _fundingError = l10n.invalidMoney);
        return;
      }
      allocations.add(
        FundingAllocationInput(
          fundingSourceId: source.fundingSourceId,
          amount: value,
        ),
      );
      selectedSources.add(source);
    }
    final currencies = selectedSources.map((item) => item.currencyCode).toSet();
    if (allocations.isEmpty ||
        currencies.length != 1 ||
        !DecimalMoney.sumEquals(
          allocations.map((item) => item.amount),
          amount,
        )) {
      setState(() => _fundingError = l10n.fundingTotalMismatch);
      return;
    }
    if (_settlementDueDate != null &&
        _settlementDueDate!.isBefore(_issueDate)) {
      setState(() => _fundingError = l10n.invalidDateOrder);
      return;
    }
    final input = CreateAdvanceInput(
      recipientUserId: _recipientUserId!,
      issueDate: _issueDate,
      settlementDueDate: _settlementDueDate,
      purpose: _purpose.text,
      fundings: allocations,
      transfer: _transfer(amount),
    );
    final recipient = ref
        .read(advanceRecipientControllerProvider('Manager'))
        .items
        .where((item) => item.id == _recipientUserId)
        .firstOrNull;
    final confirmed = await showAccessConfirmation(
      context,
      title: l10n.confirmFinancialOperationTitle,
      body:
          '${recipient?.fullName ?? ''}\n$amount ${currencies.single}\n\n'
          '${l10n.distributionEffectBody}',
      confirmLabel: l10n.createAdvance,
    );
    if (!confirmed || !mounted) return;
    await _complete(
      await ref.read(advanceCreateControllerProvider.notifier).create(input),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final recipients = ref.watch(advanceRecipientControllerProvider('Manager'));
    final funding = ref.watch(fundingSourceControllerProvider);
    final command = ref.watch(advanceCreateControllerProvider);
    for (final source in funding.items) {
      _fundingAmounts.putIfAbsent(
        source.fundingSourceId,
        TextEditingController.new,
      );
    }

    return Scaffold(
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.createAdvance,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 12),
          FinancialCommandNotice(
            phase: command.phase,
            errorMessage: command.error == null
                ? null
                : localizedError(l10n, command.error!),
            onRetry: () async => _complete(
              await ref
                  .read(advanceCreateControllerProvider.notifier)
                  .retrySameOperation(),
            ),
            onCancel: ref
                .read(advanceCreateControllerProvider.notifier)
                .cancelPending,
          ),
          if (recipients.phase == PagedReadPhase.loading ||
              funding.phase == PagedReadPhase.loading)
            const LinearProgressIndicator(),
          Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: 12),
                TextField(
                  key: const Key('advance-recipient-search'),
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
                            'Manager',
                          ).notifier,
                        )
                        .setSearch(value);
                  },
                ),
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  key: const Key('advance-recipient-field'),
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
                    padding: const EdgeInsets.only(top: 8),
                    child: Text(l10n.noEligibleRecipients),
                  ),
                if (recipients.hasMore)
                  TextButton.icon(
                    onPressed: recipients.phase == PagedReadPhase.loadingMore
                        ? null
                        : ref
                              .read(
                                advanceRecipientControllerProvider(
                                  'Manager',
                                ).notifier,
                              )
                              .loadMore,
                    icon: const Icon(Icons.expand_more),
                    label: Text(l10n.retryLoadingMore),
                  ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _purpose,
                  maxLength: 1000,
                  maxLines: 3,
                  decoration: InputDecoration(labelText: l10n.purpose),
                  validator: (value) {
                    final normalized = value?.trim() ?? '';
                    return normalized.isEmpty ||
                            normalized.length > 1000 ||
                            normalized.contains('\n') ||
                            normalized.contains('\r')
                        ? l10n.invalidRequiredText
                        : null;
                  },
                ),
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: Text(l10n.issueDate),
                  subtitle: Text(
                    AppDateTimeFormatter.formatDate(context, _issueDate),
                  ),
                  trailing: TextButton(
                    onPressed: () async {
                      final picked = await _pickDate(_issueDate);
                      if (picked != null) setState(() => _issueDate = picked);
                    },
                    child: Text(l10n.selectDate),
                  ),
                ),
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: Text(l10n.settlementDueDateOptional),
                  subtitle: Text(
                    _settlementDueDate == null
                        ? l10n.unknownValue
                        : AppDateTimeFormatter.formatDate(
                            context,
                            _settlementDueDate,
                          ),
                  ),
                  trailing: Wrap(
                    children: [
                      if (_settlementDueDate != null)
                        IconButton(
                          tooltip: l10n.clearAction,
                          onPressed: () =>
                              setState(() => _settlementDueDate = null),
                          icon: const Icon(Icons.clear),
                        ),
                      TextButton(
                        onPressed: () async {
                          final picked = await _pickDate(
                            _settlementDueDate ?? _issueDate,
                          );
                          if (picked != null) {
                            setState(() => _settlementDueDate = picked);
                          }
                        },
                        child: Text(l10n.selectDate),
                      ),
                    ],
                  ),
                ),
                AdvanceTransferFormFields(
                  amountController: _amount,
                  transferDate: _transferDate,
                  transferMethod: _method,
                  onSelectDate: () async {
                    final picked = await _pickDate(_transferDate);
                    if (picked != null) setState(() => _transferDate = picked);
                  },
                  onMethodChanged: (value) => setState(() => _method = value),
                  bankNameController: _bank,
                  referenceController: _reference,
                  descriptionController: _description,
                  notesController: _notes,
                  currencyCode:
                      funding.items
                              .map((item) => item.currencyCode)
                              .toSet()
                              .length ==
                          1
                      ? funding.items.first.currencyCode
                      : l10n.currency,
                ),
                const SizedBox(height: 20),
                Text(
                  l10n.fundingAllocation,
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                if (funding.items.isEmpty)
                  Text(l10n.noFundingSources)
                else
                  for (final source in funding.items)
                    Card(
                      child: Padding(
                        padding: const EdgeInsets.all(12),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              ValueLabels.fundingType(l10n, source.sourceType),
                            ),
                            Text(
                              '${source.availableAmount} ${source.currencyCode} · '
                              '${AppDateTimeFormatter.formatDate(context, source.sourceDate)}',
                            ),
                            TextFormField(
                              key: Key('funding-${source.fundingSourceId}'),
                              controller:
                                  _fundingAmounts[source.fundingSourceId],
                              keyboardType:
                                  const TextInputType.numberWithOptions(
                                    decimal: true,
                                  ),
                              textDirection: TextDirection.ltr,
                              decoration: InputDecoration(
                                labelText:
                                    '${l10n.fundingAllocation} (${source.currencyCode})',
                              ),
                              validator: (value) {
                                if (value == null || value.trim().isEmpty) {
                                  return null;
                                }
                                final canonical = DecimalMoney.canonicalize(
                                  value,
                                );
                                return canonical == null ||
                                        !DecimalMoney.lessThanOrEqual(
                                          canonical,
                                          source.availableAmount,
                                        )
                                    ? l10n.invalidMoney
                                    : null;
                              },
                            ),
                          ],
                        ),
                      ),
                    ),
                if (funding.hasMore)
                  TextButton.icon(
                    onPressed: ref
                        .read(fundingSourceControllerProvider.notifier)
                        .loadMore,
                    icon: const Icon(Icons.expand_more),
                    label: Text(l10n.retryLoadingMore),
                  ),
                if (_fundingError != null)
                  Padding(
                    padding: const EdgeInsets.symmetric(vertical: 8),
                    child: Text(
                      _fundingError!,
                      style: TextStyle(
                        color: Theme.of(context).colorScheme.error,
                      ),
                    ),
                  ),
                const SizedBox(height: 16),
                SubmitButton(
                  label: l10n.createAdvance,
                  loading: command.isSubmitting,
                  onPressed: () => _submit(funding.items),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
