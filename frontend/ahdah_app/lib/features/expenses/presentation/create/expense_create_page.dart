import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../advances/domain/decimal_money.dart';
import '../../../advances/presentation/controllers/advance_controllers.dart';
import '../../../projects/presentation/controllers/project_controllers.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/expense_requests.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';

final class ExpenseCreatePage extends ConsumerStatefulWidget {
  const ExpenseCreatePage({super.key});
  @override
  ConsumerState<ExpenseCreatePage> createState() => _ExpenseCreatePageState();
}

class _ExpenseCreatePageState extends ConsumerState<ExpenseCreatePage> {
  final _form = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _currency = TextEditingController(text: 'LYD');
  final _description = TextEditingController();
  final _invoice = TextEditingController();
  final _receipt = TextEditingController();
  final _merchant = TextEditingController();
  final _location = TextEditingController();
  final _notes = TextEditingController();
  final Map<String, TextEditingController> _allocations = {};
  String? _categoryId;
  String? _projectId;
  String _paymentMode = 'PersonalFunds';
  DateTime _expenseDate = DateTime.now();
  String? _allocationError;

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref.read(expenseCategoryControllerProvider.notifier).load();
      final role = ref.read(sessionControllerProvider).current!.role;
      final capabilities = RoleCapabilities.forRole(role);
      if (capabilities.canCreateProjectLinkedExpense) {
        ref.read(projectListControllerProvider.notifier).load();
      }
      if (capabilities.canUseAdvanceBalanceForExpense) {
        ref.read(personalAdvanceBalanceControllerProvider.notifier).load();
      }
    });
  }

  @override
  void dispose() {
    _amount.dispose();
    _currency.dispose();
    _description.dispose();
    _invoice.dispose();
    _receipt.dispose();
    _merchant.dispose();
    _location.dispose();
    _notes.dispose();
    for (final controller in _allocations.values) {
      controller.dispose();
    }
    super.dispose();
  }

  Future<void> _pickDate() async {
    final selected = await showDatePicker(
      context: context,
      initialDate: _expenseDate,
      firstDate: DateTime(2000),
      lastDate: DateTime.now(),
    );
    if (selected != null) setState(() => _expenseDate = selected);
  }

  Future<void> _submit() async {
    final l10n = AppLocalizations.of(context);
    if (!_form.currentState!.validate()) return;
    final amount = DecimalMoney.canonicalize(_amount.text);
    if (amount == null) return;
    final category = ref
        .read(expenseCategoryControllerProvider)
        .items
        .where((item) => item.expenseCategoryId == _categoryId)
        .firstOrNull;
    if (category == null) return;
    if (category.expenseScope == 'ProjectOnly' && _projectId == null ||
        category.expenseScope == 'CompanyOnly' && _projectId != null) {
      setState(() => _allocationError = l10n.invalidRequiredText);
      return;
    }
    final balanceState = ref.read(personalAdvanceBalanceControllerProvider);
    final allocationInputs = <ExpenseAdvanceAllocationInput>[];
    if (_paymentMode == 'AdvanceBalance') {
      for (final balance in balanceState.items) {
        final raw = _allocations[balance.userAdvanceBalanceId]?.text ?? '';
        if (raw.trim().isEmpty) continue;
        final value = DecimalMoney.canonicalize(raw);
        if (value == null ||
            balance.currencyCode != _currency.text.trim().toUpperCase()) {
          setState(() => _allocationError = l10n.allocationTotalMismatch);
          return;
        }
        allocationInputs.add(
          ExpenseAdvanceAllocationInput(
            userAdvanceBalanceId: balance.userAdvanceBalanceId,
            amount: value,
          ),
        );
      }
      if (allocationInputs.isEmpty ||
          !DecimalMoney.sumEquals(
            allocationInputs.map((item) => item.amount),
            amount,
          )) {
        setState(() => _allocationError = l10n.allocationTotalMismatch);
        return;
      }
    }
    setState(() => _allocationError = null);
    final result = await ref
        .read(expenseCreateControllerProvider.notifier)
        .create(
          CreateExpenseInput(
            expenseCategoryId: category.expenseCategoryId,
            projectId: _projectId,
            expenseDate: _expenseDate,
            amount: amount,
            currencyCode: _currency.text,
            paymentMode: _paymentMode,
            description: _description.text,
            invoiceNumber: _invoice.text,
            receiptNumber: _receipt.text,
            merchantName: _merchant.text,
            expenseLocation: _location.text,
            notes: _notes.text,
            advanceAllocations: allocationInputs,
          ),
        );
    if (!mounted || result == null) return;
    await ref.read(expenseListControllerProvider.notifier).refresh();
    await ref.read(reimbursementListControllerProvider.notifier).load();
    await ref.read(personalAdvanceBalanceControllerProvider.notifier).refresh();
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(l10n.expenseCreatedPending)));
    context.go('${AppRoutes.expensesPath}/${result.expense.expenseId}');
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final command = ref.watch(expenseCreateControllerProvider);
    final categories = ref.watch(expenseCategoryControllerProvider).items;
    final projects = ref.watch(projectListControllerProvider).items;
    final balances = ref.watch(personalAdvanceBalanceControllerProvider).items;
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current!.role,
    );
    final selectedCategory = categories
        .where((item) => item.expenseCategoryId == _categoryId)
        .firstOrNull;
    final showProject =
        capabilities.canCreateProjectLinkedExpense &&
        selectedCategory?.expenseScope != 'CompanyOnly';
    return Form(
      key: _form,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.createExpense,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 16),
          DropdownButtonFormField<String>(
            initialValue: _categoryId,
            isExpanded: true,
            decoration: InputDecoration(labelText: l10n.category),
            items: [
              for (final category in categories.where((item) => item.isActive))
                DropdownMenuItem(
                  value: category.expenseCategoryId,
                  child: Text(category.categoryName),
                ),
            ],
            validator: (value) =>
                value == null ? l10n.invalidRequiredText : null,
            onChanged: command.isSubmitting
                ? null
                : (value) => setState(() {
                    _categoryId = value;
                    final category = categories
                        .where((item) => item.expenseCategoryId == value)
                        .firstOrNull;
                    if (category?.expenseScope == 'CompanyOnly') {
                      _projectId = null;
                    }
                  }),
          ),
          if (selectedCategory != null)
            Padding(
              padding: const EdgeInsets.only(top: 8),
              child: Text(
                expenseScopeLabel(l10n, selectedCategory.expenseScope),
              ),
            ),
          if (showProject) ...[
            const SizedBox(height: 12),
            DropdownButtonFormField<String?>(
              initialValue: _projectId,
              isExpanded: true,
              decoration: InputDecoration(labelText: l10n.projectOptional),
              items: [
                DropdownMenuItem<String?>(
                  value: null,
                  child: Text(l10n.companyExpense),
                ),
                for (final project in projects)
                  DropdownMenuItem<String?>(
                    value: project.id,
                    child: Text(project.projectName),
                  ),
              ],
              validator: (value) =>
                  selectedCategory?.expenseScope == 'ProjectOnly' &&
                      value == null
                  ? l10n.invalidRequiredText
                  : null,
              onChanged: command.isSubmitting
                  ? null
                  : (value) => setState(() => _projectId = value),
            ),
          ],
          const SizedBox(height: 12),
          InkWell(
            onTap: command.isSubmitting ? null : _pickDate,
            child: InputDecorator(
              decoration: InputDecoration(labelText: l10n.expenseDate),
              child: Text(
                AppDateTimeFormatter.formatDate(context, _expenseDate),
              ),
            ),
          ),
          const SizedBox(height: 12),
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                flex: 2,
                child: TextFormField(
                  controller: _amount,
                  keyboardType: const TextInputType.numberWithOptions(
                    decimal: true,
                  ),
                  decoration: InputDecoration(labelText: l10n.amount),
                  validator: (value) => DecimalMoney.canonicalize(value) == null
                      ? l10n.invalidMoney
                      : null,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: TextFormField(
                  controller: _currency,
                  maxLength: 3,
                  textCapitalization: TextCapitalization.characters,
                  decoration: InputDecoration(
                    labelText: l10n.currency,
                    counterText: '',
                  ),
                  validator: (value) =>
                      RegExp(
                        r'^[A-Z]{3}$',
                      ).hasMatch((value ?? '').trim().toUpperCase())
                      ? null
                      : l10n.invalidRequiredText,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          DropdownButtonFormField<String>(
            key: const Key('expense-payment-mode'),
            initialValue: _paymentMode,
            decoration: InputDecoration(labelText: l10n.paymentMode),
            items: [
              if (capabilities.canUseAdvanceBalanceForExpense)
                DropdownMenuItem(
                  value: 'AdvanceBalance',
                  child: Text(l10n.advanceBalance),
                ),
              if (capabilities.canCreatePersonalFundsExpense)
                DropdownMenuItem(
                  value: 'PersonalFunds',
                  child: Text(l10n.personalFunds),
                ),
            ],
            onChanged: command.isSubmitting
                ? null
                : (value) =>
                      setState(() => _paymentMode = value ?? 'PersonalFunds'),
          ),
          const SizedBox(height: 8),
          Text(
            _paymentMode == 'AdvanceBalance'
                ? l10n.advanceReservationNotice
                : l10n.personalFundsClaimNotice,
          ),
          if (_paymentMode == 'AdvanceBalance') ...[
            const SizedBox(height: 12),
            Text(
              l10n.advanceAllocations,
              style: Theme.of(context).textTheme.titleMedium,
            ),
            for (final balance in balances.where(
              (item) =>
                  item.status == 'Active' && item.availableAmount != '0.00',
            ))
              Padding(
                padding: const EdgeInsets.only(top: 8),
                child: TextFormField(
                  controller: _allocations.putIfAbsent(
                    balance.userAdvanceBalanceId,
                    TextEditingController.new,
                  ),
                  keyboardType: const TextInputType.numberWithOptions(
                    decimal: true,
                  ),
                  decoration: InputDecoration(
                    labelText:
                        '${balance.advanceNumber} · '
                        '${balance.availableAmount} ${balance.currencyCode}',
                  ),
                ),
              ),
            if (_allocationError != null)
              Padding(
                padding: const EdgeInsets.only(top: 8),
                child: Text(
                  _allocationError!,
                  style: TextStyle(color: Theme.of(context).colorScheme.error),
                ),
              ),
          ],
          const SizedBox(height: 12),
          TextFormField(
            controller: _description,
            maxLength: 1000,
            maxLines: 3,
            decoration: InputDecoration(labelText: l10n.description),
            validator: (value) => value == null || value.trim().isEmpty
                ? l10n.invalidRequiredText
                : null,
          ),
          TextFormField(
            controller: _receipt,
            maxLength: 100,
            decoration: InputDecoration(labelText: l10n.receiptNumberOptional),
          ),
          TextFormField(
            controller: _invoice,
            maxLength: 100,
            decoration: InputDecoration(labelText: l10n.invoiceNumberOptional),
          ),
          TextFormField(
            controller: _merchant,
            maxLength: 200,
            decoration: InputDecoration(labelText: l10n.merchantNameOptional),
          ),
          TextFormField(
            controller: _location,
            maxLength: 500,
            decoration: InputDecoration(
              labelText: l10n.expenseLocationOptional,
            ),
          ),
          TextFormField(
            controller: _notes,
            maxLength: 1000,
            maxLines: 3,
            decoration: InputDecoration(labelText: l10n.notesOptional),
          ),
          Text(l10n.binaryUploadNotImplemented),
          if (command.phase == ExpenseCommandPhase.failure)
            Text(
              localizedError(l10n, command.error!),
              style: TextStyle(color: Theme.of(context).colorScheme.error),
            ),
          if (command.isUncertain)
            Card(
              color: Theme.of(context).colorScheme.errorContainer,
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(l10n.operationStatusUncertain),
                    Text(l10n.idempotentRetryExplanation),
                    Wrap(
                      children: [
                        TextButton(
                          onPressed: ref
                              .read(expenseCreateControllerProvider.notifier)
                              .retrySameOperation,
                          child: Text(l10n.retrySameOperation),
                        ),
                        TextButton(
                          onPressed: ref
                              .read(expenseCreateControllerProvider.notifier)
                              .cancelPending,
                          child: Text(l10n.cancelUncertainOperation),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          const SizedBox(height: 16),
          FilledButton.icon(
            onPressed: command.isSubmitting || command.isUncertain
                ? null
                : _submit,
            icon: command.isSubmitting
                ? const SizedBox.square(
                    dimension: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.save_outlined),
            label: Text(l10n.createExpense),
          ),
        ],
      ),
    );
  }
}
