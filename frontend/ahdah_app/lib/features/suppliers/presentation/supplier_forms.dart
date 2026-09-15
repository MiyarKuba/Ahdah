import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../core/localization/error_labels.dart';
import '../../../core/localization/value_labels.dart';
import '../../../core/widgets/form_widgets.dart';
import '../../../l10n/app_localizations.dart';
import '../../advances/domain/decimal_money.dart';
import '../../expenses/presentation/controllers/expense_controllers.dart';
import '../../projects/presentation/controllers/project_controllers.dart';
import '../domain/supplier_models.dart';
import '../domain/supplier_requests.dart';
import 'controllers/supplier_controllers.dart';
import 'widgets/supplier_labels.dart';

final class SupplierFormPage extends ConsumerStatefulWidget {
  const SupplierFormPage({this.supplierId, super.key});
  final String? supplierId;
  @override
  ConsumerState<SupplierFormPage> createState() => _SupplierFormPageState();
}

final class _SupplierFormPageState extends ConsumerState<SupplierFormPage> {
  final _form = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _code = TextEditingController();
  final _contact = TextEditingController();
  final _phone = TextEditingController();
  final _secondaryPhone = TextEditingController();
  final _email = TextEditingController();
  final _city = TextEditingController();
  final _address = TextEditingController();
  final _currency = TextEditingController();
  final _terms = TextEditingController(text: '0');
  final _creditLimit = TextEditingController();
  final _notes = TextEditingController();
  String? _type;
  String? _mode;
  String? _preferredMethod;
  int? _version;
  bool _seeded = false;

  @override
  void dispose() {
    for (final controller in [
      _name,
      _code,
      _contact,
      _phone,
      _secondaryPhone,
      _email,
      _city,
      _address,
      _currency,
      _terms,
      _creditLimit,
      _notes,
    ]) {
      controller.dispose();
    }
    super.dispose();
  }

  void _seed(SupplierDetails details) {
    if (_seeded) return;
    _seeded = true;
    final supplier = details.supplier;
    _name.text = supplier.supplierName;
    _code.text = supplier.supplierCode ?? '';
    _contact.text = supplier.contactPersonName ?? '';
    _phone.text = supplier.phoneNumber ?? '';
    _secondaryPhone.text = details.secondaryPhoneNumber ?? '';
    _email.text = supplier.email ?? '';
    _city.text = supplier.city ?? '';
    _address.text = details.address ?? '';
    _currency.text = supplier.defaultCurrencyCode;
    _terms.text = '${supplier.defaultPaymentTermsDays}';
    _creditLimit.text = supplier.creditLimit ?? '';
    _notes.text = details.notes ?? '';
    _type = supplier.supplierType;
    _mode = supplier.transactionMode;
    _preferredMethod = supplier.preferredPaymentMethod;
    _version = supplier.versionNumber;
  }

  Future<void> _submit() async {
    if (!_form.currentState!.validate()) return;
    final l10n = AppLocalizations.of(context);
    final mutation = ref.read(supplierMutationControllerProvider.notifier);
    SupplierDetails? result;
    if (widget.supplierId == null) {
      final limit = _creditLimit.text.trim().isEmpty
          ? null
          : DecimalMoney.canonicalize(_creditLimit.text, allowZero: true);
      if (_creditLimit.text.trim().isNotEmpty && limit == null) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(l10n.invalidMoney)));
        return;
      }
      result = await mutation.create(
        SupplierCreateInput(
          supplierName: _name.text,
          supplierCode: _code.text,
          supplierType: _type!,
          contactPersonName: _contact.text,
          phoneNumber: _phone.text,
          secondaryPhoneNumber: _secondaryPhone.text,
          email: _email.text,
          city: _city.text,
          address: _address.text,
          defaultCurrencyCode: _currency.text,
          transactionMode: _mode!,
          defaultPaymentTermsDays: int.tryParse(_terms.text) ?? 0,
          creditLimit: limit,
          preferredPaymentMethod: _preferredMethod,
          notes: _notes.text,
        ),
      );
    } else {
      result = await mutation.update(
        widget.supplierId!,
        SupplierUpdateInput(
          expectedVersion: _version!,
          contactPersonName: _contact.text,
          phoneNumber: _phone.text,
          secondaryPhoneNumber: _secondaryPhone.text,
          email: _email.text,
          city: _city.text,
          address: _address.text,
          notes: _notes.text,
        ),
      );
    }
    if (result != null && mounted) {
      context.go('/suppliers/${result.supplier.supplierId}');
    }
  }

  Future<void> _deactivate() async {
    final l10n = AppLocalizations.of(context);
    final reason = TextEditingController();
    final accepted = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: Text(l10n.deactivateSupplierTitle),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(l10n.deactivateSupplierBody),
            TextField(
              controller: reason,
              maxLength: 500,
              decoration: InputDecoration(labelText: l10n.rejectionReason),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext, false),
            child: Text(l10n.cancelAction),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(dialogContext, true),
            child: Text(l10n.deactivateSupplier),
          ),
        ],
      ),
    );
    if (accepted == true && reason.text.trim().isNotEmpty) {
      final result = await ref
          .read(supplierMutationControllerProvider.notifier)
          .update(
            widget.supplierId!,
            SupplierUpdateInput(
              expectedVersion: _version!,
              isActive: false,
              deactivationReason: reason.text,
            ),
          );
      if (result != null && mounted) {
        context.go('/suppliers/${widget.supplierId}');
      }
    }
    reason.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (widget.supplierId != null) {
      final async = ref.watch(supplierDetailsProvider(widget.supplierId!));
      if (async.isLoading) {
        return const Center(child: CircularProgressIndicator());
      }
      if (async.hasError) {
        return Center(child: Text(AppLocalizations.of(context).genericError));
      }
      _seed(async.requireValue);
    }
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(supplierMutationControllerProvider);
    return Form(
      key: _form,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            widget.supplierId == null ? l10n.addSupplier : l10n.editSupplier,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 12),
          if (state.error != null)
            AppMessageBanner(
              message: localizedError(l10n, state.error!),
              isError: true,
            ),
          _requiredText(_name, l10n.supplierName, l10n),
          if (widget.supplierId == null)
            _optionalText(_code, l10n.supplierCode, maxLength: 30),
          if (widget.supplierId == null)
            DropdownButtonFormField<String>(
              isExpanded: true,
              initialValue: _type,
              decoration: InputDecoration(labelText: l10n.supplierType),
              items: [
                for (final value in supplierTypes)
                  DropdownMenuItem(
                    value: value,
                    child: Text(SupplierLabels.type(l10n, value)),
                  ),
              ],
              validator: (value) => value == null ? l10n.requiredField : null,
              onChanged: (value) => setState(() => _type = value),
            ),
          _optionalText(_contact, l10n.contactPerson, maxLength: 150),
          _optionalText(
            _phone,
            l10n.phoneNumber,
            maxLength: 20,
            keyboardType: TextInputType.phone,
          ),
          _optionalText(
            _secondaryPhone,
            l10n.secondaryPhoneNumber,
            maxLength: 20,
            keyboardType: TextInputType.phone,
          ),
          _optionalText(
            _email,
            l10n.emailAddress,
            maxLength: 200,
            keyboardType: TextInputType.emailAddress,
          ),
          _optionalText(_city, l10n.city, maxLength: 100),
          _optionalText(
            _address,
            l10n.addressOptional,
            maxLength: 500,
            maxLines: 2,
          ),
          if (widget.supplierId == null) ...[
            _requiredText(_currency, l10n.defaultCurrency, l10n, maxLength: 3),
            DropdownButtonFormField<String>(
              isExpanded: true,
              initialValue: _mode,
              decoration: InputDecoration(labelText: l10n.transactionMode),
              items: [
                for (final value in supplierTransactionModes)
                  DropdownMenuItem(
                    value: value,
                    child: Text(SupplierLabels.transactionMode(l10n, value)),
                  ),
              ],
              validator: (value) => value == null ? l10n.requiredField : null,
              onChanged: (value) => setState(() {
                _mode = value;
                if (value == 'CashOnly') {
                  _terms.text = '0';
                  _creditLimit.clear();
                }
              }),
            ),
            _optionalText(
              _terms,
              l10n.paymentTermsDays,
              keyboardType: TextInputType.number,
              enabled: _mode != 'CashOnly',
            ),
            _optionalText(
              _creditLimit,
              l10n.creditLimit,
              keyboardType: const TextInputType.numberWithOptions(
                decimal: true,
              ),
              enabled: _mode != 'CashOnly',
            ),
            DropdownButtonFormField<String?>(
              isExpanded: true,
              initialValue: _preferredMethod,
              decoration: InputDecoration(
                labelText: l10n.preferredPaymentMethod,
              ),
              items: [
                DropdownMenuItem(value: null, child: Text(l10n.unknownValue)),
                for (final value in supplierPaymentMethods)
                  DropdownMenuItem(
                    value: value,
                    child: Text(ValueLabels.paymentMethod(l10n, value)),
                  ),
              ],
              onChanged: (value) => setState(() => _preferredMethod = value),
            ),
          ],
          _optionalText(
            _notes,
            l10n.notesOptional,
            maxLength: 1000,
            maxLines: 3,
          ),
          const SizedBox(height: 16),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: [
              SubmitButton(
                label: l10n.saveAction,
                loading: state.isSubmitting,
                onPressed: _submit,
              ),
              if (widget.supplierId != null)
                OutlinedButton(
                  onPressed: state.isSubmitting ? null : _deactivate,
                  child: Text(l10n.deactivateSupplier),
                ),
            ],
          ),
        ],
      ),
    );
  }
}

final class SupplierInvoiceCreatePage extends ConsumerStatefulWidget {
  const SupplierInvoiceCreatePage({super.key});
  @override
  ConsumerState<SupplierInvoiceCreatePage> createState() =>
      _SupplierInvoiceCreatePageState();
}

final class _SupplierInvoiceCreatePageState
    extends ConsumerState<SupplierInvoiceCreatePage> {
  final _form = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _currency = TextEditingController(text: 'LYD');
  final _reference = TextEditingController();
  final _description = TextEditingController();
  final _notes = TextEditingController();
  String? _supplierId;
  String? _categoryId;
  String? _projectId;
  DateTime _invoiceDate = DateTime.now();
  DateTime _dueDate = DateTime.now().add(const Duration(days: 30));
  final List<_InvoiceItemDraft> _items = [];

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref.read(expenseCategoryControllerProvider.notifier).load();
      ref.read(projectListControllerProvider.notifier).load();
    });
  }

  @override
  void dispose() {
    _amount.dispose();
    _currency.dispose();
    _reference.dispose();
    _description.dispose();
    _notes.dispose();
    for (final item in _items) {
      item.dispose();
    }
    super.dispose();
  }

  Future<void> _submit() async {
    final l10n = AppLocalizations.of(context);
    if (!_form.currentState!.validate() ||
        _supplierId == null ||
        _categoryId == null) {
      return;
    }
    final amount = DecimalMoney.canonicalize(_amount.text);
    if (amount == null) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.invalidMoney)));
      return;
    }
    final lines = <SupplierInvoiceItemInput>[];
    for (final draft in _items) {
      final quantity = DecimalQuantity.canonicalize(draft.quantity.text);
      final price = DecimalMoney.canonicalize(draft.price.text);
      final discount = DecimalMoney.canonicalize(
        draft.discount.text,
        allowZero: true,
      );
      final tax = DecimalMoney.canonicalize(draft.tax.text, allowZero: true);
      if (quantity == null ||
          price == null ||
          discount == null ||
          tax == null) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(l10n.invalidQuantity)));
        return;
      }
      lines.add(
        SupplierInvoiceItemInput(
          itemName: draft.name.text,
          quantity: quantity,
          unitCode: draft.unit,
          unitPrice: price,
          discountAmount: discount,
          taxAmount: tax,
        ),
      );
    }
    final input = SupplierInvoiceCreateInput(
      supplierId: _supplierId!,
      expenseCategoryId: _categoryId!,
      projectId: _projectId,
      invoiceDate: _invoiceDate,
      dueDate: _dueDate,
      amount: amount,
      currencyCode: _currency.text,
      invoiceNumber: _reference.text,
      description: _description.text,
      notes: _notes.text,
      items: lines,
    );
    if (!input.itemsMatchAmount) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.allocationTotalsMismatch)));
      return;
    }
    final result = await ref
        .read(supplierInvoiceCreateControllerProvider.notifier)
        .create(input);
    if (result != null && mounted) {
      context.go('/supplier-invoices/${result.invoice.supplierDebtId}');
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final command = ref.watch(supplierInvoiceCreateControllerProvider);
    final suppliers = ref.watch(activeSupplierOptionsProvider);
    final categories = ref
        .watch(expenseCategoryControllerProvider)
        .items
        .where((item) => item.requiresSupplier)
        .toList();
    final projects = ref.watch(projectListControllerProvider).items;
    return Form(
      key: _form,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.createSupplierInvoice,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 12),
          AppMessageBanner(message: l10n.duplicateInvoiceWarning),
          if (command.isUncertain)
            _Uncertain(
              controller: ref.read(
                supplierInvoiceCreateControllerProvider.notifier,
              ),
            ),
          if (command.error != null && !command.isUncertain)
            AppMessageBanner(
              message: localizedError(l10n, command.error!),
              isError: true,
            ),
          suppliers.when(
            loading: () => const LinearProgressIndicator(),
            error: (_, _) => Text(l10n.genericError),
            data: (page) => DropdownButtonFormField<String>(
              isExpanded: true,
              initialValue: _supplierId,
              decoration: InputDecoration(labelText: l10n.selectSupplier),
              items: [
                for (final item in page.items.where(
                  (item) => item.transactionMode != 'CashOnly',
                ))
                  DropdownMenuItem(
                    value: item.supplierId,
                    child: Text(item.supplierName),
                  ),
              ],
              onChanged: (value) => setState(() {
                _supplierId = value;
                final supplier = page.items
                    .where((item) => item.supplierId == value)
                    .firstOrNull;
                if (supplier != null) {
                  _currency.text = supplier.defaultCurrencyCode;
                }
              }),
            ),
          ),
          DropdownButtonFormField<String>(
            isExpanded: true,
            initialValue: _categoryId,
            decoration: InputDecoration(labelText: l10n.selectCategory),
            items: [
              for (final item in categories)
                DropdownMenuItem(
                  value: item.expenseCategoryId,
                  child: Text(item.categoryName),
                ),
            ],
            onChanged: (value) => setState(() => _categoryId = value),
          ),
          DropdownButtonFormField<String?>(
            isExpanded: true,
            initialValue: _projectId,
            decoration: InputDecoration(labelText: l10n.projectOptional),
            items: [
              DropdownMenuItem(value: null, child: Text(l10n.unknownValue)),
              for (final item in projects)
                DropdownMenuItem(value: item.id, child: Text(item.projectName)),
            ],
            onChanged: (value) => setState(() => _projectId = value),
          ),
          _DateRow(
            label: l10n.invoiceDate,
            value: _invoiceDate,
            onChanged: (value) => setState(() => _invoiceDate = value),
          ),
          _DateRow(
            label: l10n.dueDate,
            value: _dueDate,
            onChanged: (value) => setState(() => _dueDate = value),
          ),
          _requiredText(
            _amount,
            l10n.amount,
            l10n,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
          ),
          _requiredText(_currency, l10n.currency, l10n, maxLength: 3),
          _optionalText(_reference, l10n.invoiceNumber, maxLength: 100),
          _requiredText(
            _description,
            l10n.invoiceDescription,
            l10n,
            maxLength: 1000,
            maxLines: 3,
          ),
          _optionalText(
            _notes,
            l10n.notesOptional,
            maxLength: 1000,
            maxLines: 3,
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                l10n.invoiceItemsOptional,
                style: Theme.of(context).textTheme.titleMedium,
              ),
              TextButton.icon(
                onPressed: () =>
                    setState(() => _items.add(_InvoiceItemDraft())),
                icon: const Icon(Icons.add),
                label: Text(l10n.addInvoiceItem),
              ),
            ],
          ),
          for (var index = 0; index < _items.length; index++)
            _InvoiceItemEditor(
              draft: _items[index],
              onRemove: () => setState(() {
                _items[index].dispose();
                _items.removeAt(index);
              }),
            ),
          const SizedBox(height: 12),
          SubmitButton(
            label: l10n.createAction,
            loading: command.isSubmitting,
            onPressed: _submit,
          ),
        ],
      ),
    );
  }
}

final class SupplierPaymentCreatePage extends ConsumerStatefulWidget {
  const SupplierPaymentCreatePage({super.key});
  @override
  ConsumerState<SupplierPaymentCreatePage> createState() =>
      _SupplierPaymentCreatePageState();
}

final class _SupplierPaymentCreatePageState
    extends ConsumerState<SupplierPaymentCreatePage> {
  final _form = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _currency = TextEditingController(text: 'LYD');
  final _reference = TextEditingController();
  final _bank = TextEditingController();
  final _description = TextEditingController();
  final _proofPath = TextEditingController();
  String? _supplierId;
  String? _paymentAccountId;
  String _method = 'Cash';
  DateTime _date = DateTime.now();
  final Map<String, TextEditingController> _debts = {};
  final Map<String, TextEditingController> _funding = {};
  @override
  void dispose() {
    _amount.dispose();
    _currency.dispose();
    _reference.dispose();
    _bank.dispose();
    _description.dispose();
    _proofPath.dispose();
    for (final value in [..._debts.values, ..._funding.values]) {
      value.dispose();
    }
    super.dispose();
  }

  Future<void> _submit() async {
    final l10n = AppLocalizations.of(context);
    if (!_form.currentState!.validate() || _supplierId == null) return;
    final amount = DecimalMoney.canonicalize(_amount.text);
    if (amount == null) return;
    final debtAllocations = <SupplierPaymentDebtAllocationInput>[];
    final fundingAllocations = <SupplierPaymentFundingAllocationInput>[];
    for (final entry in _debts.entries) {
      final value = DecimalMoney.canonicalize(entry.value.text);
      if (value != null) {
        debtAllocations.add(
          SupplierPaymentDebtAllocationInput(
            supplierDebtId: entry.key,
            amount: value,
          ),
        );
      }
    }
    for (final entry in _funding.entries) {
      final value = DecimalMoney.canonicalize(entry.value.text);
      if (value != null) {
        fundingAllocations.add(
          SupplierPaymentFundingAllocationInput(
            fundingSourceId: entry.key,
            amount: value,
          ),
        );
      }
    }
    final input = SupplierPaymentCreateInput(
      supplierId: _supplierId!,
      supplierPaymentAccountId: _paymentAccountId,
      paymentDate: _date,
      paymentAmount: amount,
      currencyCode: _currency.text,
      paymentMethod: _method,
      payerBankName: _bank.text,
      referenceNumber: _reference.text,
      proofFileUrl: _proofPath.text,
      description: _description.text,
      debtAllocations: debtAllocations,
      fundingAllocations: fundingAllocations,
    );
    if (!input.allocationsMatch) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.allocationTotalsMismatch)));
      return;
    }
    final result = await ref
        .read(supplierPaymentCreateControllerProvider.notifier)
        .create(input);
    if (result != null && mounted) {
      context.go('/supplier-payments/${result.payment.supplierPaymentId}');
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final command = ref.watch(supplierPaymentCreateControllerProvider);
    final suppliers = ref.watch(activeSupplierOptionsProvider);
    final key = _supplierId == null
        ? null
        : (
            supplierId: _supplierId!,
            currency: _currency.text.trim().toUpperCase(),
          );
    final debts = key == null
        ? null
        : ref.watch(eligibleSupplierDebtsProvider(key));
    final funding = ref.watch(
      supplierFundingSourcesProvider((
        currency: _currency.text.trim().toUpperCase(),
        method: _method,
      )),
    );
    return Form(
      key: _form,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.recordPayment,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 12),
          AppMessageBanner(
            message:
                '${l10n.paymentFeesUnsupported}\n${l10n.advanceFundingUnavailable}\n${l10n.finalSettlementUnavailable}',
          ),
          if (command.isUncertain)
            _Uncertain(
              controller: ref.read(
                supplierPaymentCreateControllerProvider.notifier,
              ),
            ),
          if (command.error != null && !command.isUncertain)
            AppMessageBanner(
              message: localizedError(l10n, command.error!),
              isError: true,
            ),
          suppliers.when(
            loading: () => const LinearProgressIndicator(),
            error: (_, _) => Text(l10n.genericError),
            data: (page) => DropdownButtonFormField<String>(
              isExpanded: true,
              initialValue: _supplierId,
              decoration: InputDecoration(labelText: l10n.selectSupplier),
              items: [
                for (final item in page.items)
                  DropdownMenuItem(
                    value: item.supplierId,
                    child: Text(item.supplierName),
                  ),
              ],
              onChanged: (value) => setState(() {
                _supplierId = value;
                _paymentAccountId = null;
                _debts.clear();
                final item = page.items
                    .where((item) => item.supplierId == value)
                    .firstOrNull;
                if (item != null) _currency.text = item.defaultCurrencyCode;
              }),
            ),
          ),
          _DateRow(
            label: l10n.paymentDate,
            value: _date,
            onChanged: (value) => setState(() => _date = value),
          ),
          _requiredText(
            _amount,
            l10n.paymentAmount,
            l10n,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
          ),
          _requiredText(_currency, l10n.currency, l10n, maxLength: 3),
          DropdownButtonFormField<String>(
            isExpanded: true,
            initialValue: _method,
            decoration: InputDecoration(labelText: l10n.paymentMethod),
            items: [
              for (final value in supplierPaymentMethods)
                DropdownMenuItem(
                  value: value,
                  child: Text(ValueLabels.paymentMethod(l10n, value)),
                ),
            ],
            onChanged: (value) => setState(() {
              _method = value ?? _method;
              _funding.clear();
            }),
          ),
          if (_supplierId != null &&
              (_method == 'BankTransfer' || _method == 'MobileWallet'))
            ref
                .watch(supplierPaymentAccountsProvider(_supplierId!))
                .when(
                  loading: () => const LinearProgressIndicator(),
                  error: (_, _) => Text(l10n.genericError),
                  data: (page) => DropdownButtonFormField<String>(
                    isExpanded: true,
                    initialValue: _paymentAccountId,
                    decoration: InputDecoration(
                      labelText: l10n.paymentAccounts,
                    ),
                    items: [
                      for (final account in page.items.where(
                        (account) =>
                            account.isActive &&
                            account.verificationStatus == 'Verified' &&
                            account.currencyCode ==
                                _currency.text.trim().toUpperCase(),
                      ))
                        DropdownMenuItem(
                          value: account.supplierPaymentAccountId,
                          child: Text(
                            '${account.accountLabel} · ${account.maskedAccountNumber ?? account.maskedIban ?? account.maskedWalletNumber ?? ''}',
                          ),
                        ),
                    ],
                    onChanged: (value) =>
                        setState(() => _paymentAccountId = value),
                  ),
                ),
          if (_method == 'BankTransfer' || _method == 'Cheque')
            _requiredText(_bank, l10n.payerBankName, l10n, maxLength: 150),
          _optionalText(_reference, l10n.paymentReference, maxLength: 150),
          _optionalText(_proofPath, l10n.proofPathOptional, maxLength: 1000),
          if (_method == 'Other')
            _requiredText(
              _description,
              l10n.description,
              l10n,
              maxLength: 1000,
            ),
          const SizedBox(height: 12),
          Text(
            l10n.debtAllocations,
            style: Theme.of(context).textTheme.titleMedium,
          ),
          if (debts != null)
            debts.when(
              loading: () => const LinearProgressIndicator(),
              error: (_, _) => Text(l10n.genericError),
              data: (page) => Column(
                children: [
                  for (final debt in page.items)
                    _AllocationTile(
                      label:
                          '${debt.debtNumber} · ${debt.outstandingAmount} ${debt.currencyCode}',
                      selected: _debts.containsKey(debt.supplierDebtId),
                      controller: _debts[debt.supplierDebtId],
                      onChanged: (selected) => setState(() {
                        if (selected) {
                          _debts[debt.supplierDebtId] = TextEditingController(
                            text: debt.outstandingAmount,
                          );
                        } else {
                          _debts.remove(debt.supplierDebtId)?.dispose();
                        }
                      }),
                    ),
                ],
              ),
            ),
          const SizedBox(height: 12),
          Text(
            l10n.fundingAllocations,
            style: Theme.of(context).textTheme.titleMedium,
          ),
          funding.when(
            loading: () => const LinearProgressIndicator(),
            error: (_, _) => Text(l10n.genericError),
            data: (page) => Column(
              children: [
                for (final source in page.items)
                  _AllocationTile(
                    label:
                        '${ValueLabels.fundingType(l10n, source.sourceType)} · ${source.availableAmount} ${source.currencyCode}',
                    selected: _funding.containsKey(source.fundingSourceId),
                    controller: _funding[source.fundingSourceId],
                    onChanged: (selected) => setState(() {
                      if (selected) {
                        _funding[source.fundingSourceId] =
                            TextEditingController();
                      } else {
                        _funding.remove(source.fundingSourceId)?.dispose();
                      }
                    }),
                  ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          SubmitButton(
            label: l10n.recordPayment,
            loading: command.isSubmitting,
            onPressed: _submit,
          ),
        ],
      ),
    );
  }
}

final class SupplierCreditCreatePage extends ConsumerStatefulWidget {
  const SupplierCreditCreatePage({super.key});
  @override
  ConsumerState<SupplierCreditCreatePage> createState() =>
      _SupplierCreditCreatePageState();
}

final class _SupplierCreditCreatePageState
    extends ConsumerState<SupplierCreditCreatePage> {
  final _form = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _currency = TextEditingController(text: 'LYD');
  final _reference = TextEditingController();
  final _description = TextEditingController();
  String? _supplierId;
  String _reason = 'ReturnedGoods';
  DateTime _date = DateTime.now();
  @override
  void dispose() {
    _amount.dispose();
    _currency.dispose();
    _reference.dispose();
    _description.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_form.currentState!.validate() || _supplierId == null) return;
    final amount = DecimalMoney.canonicalize(_amount.text);
    if (amount == null) return;
    final input = SupplierCreditCreateInput(
      supplierId: _supplierId!,
      creditNoteDate: _date,
      amount: amount,
      currencyCode: _currency.text,
      supplierReferenceNumber: _reference.text,
      reasonType: _reason,
      description: _description.text,
    );
    final result = await ref
        .read(supplierCreditCommandControllerProvider.notifier)
        .create(input);
    if (result != null && mounted) {
      context.go(
        '/supplier-credit-notes/${result.creditNote.supplierCreditNoteId}',
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final command = ref.watch(supplierCreditCommandControllerProvider);
    return Form(
      key: _form,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.createCreditNote,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          if (command.isUncertain)
            _Uncertain(
              controller: ref.read(
                supplierCreditCommandControllerProvider.notifier,
              ),
            ),
          ref
              .watch(activeSupplierOptionsProvider)
              .when(
                loading: () => const LinearProgressIndicator(),
                error: (_, _) => Text(l10n.genericError),
                data: (page) => DropdownButtonFormField<String>(
                  isExpanded: true,
                  initialValue: _supplierId,
                  decoration: InputDecoration(labelText: l10n.selectSupplier),
                  items: [
                    for (final item in page.items)
                      DropdownMenuItem(
                        value: item.supplierId,
                        child: Text(item.supplierName),
                      ),
                  ],
                  onChanged: (value) => setState(() {
                    _supplierId = value;
                    final item = page.items
                        .where((item) => item.supplierId == value)
                        .firstOrNull;
                    if (item != null) _currency.text = item.defaultCurrencyCode;
                  }),
                ),
              ),
          _DateRow(
            label: l10n.creditNoteDate,
            value: _date,
            onChanged: (value) => setState(() => _date = value),
          ),
          _requiredText(
            _amount,
            l10n.amount,
            l10n,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
          ),
          _requiredText(_currency, l10n.currency, l10n, maxLength: 3),
          _optionalText(_reference, l10n.referenceNumber, maxLength: 100),
          DropdownButtonFormField<String>(
            isExpanded: true,
            initialValue: _reason,
            decoration: InputDecoration(labelText: l10n.creditReason),
            items: [
              for (final value in supplierCreditReasons)
                DropdownMenuItem(
                  value: value,
                  child: Text(SupplierLabels.reason(l10n, value)),
                ),
            ],
            onChanged: (value) => setState(() => _reason = value ?? _reason),
          ),
          _requiredText(
            _description,
            l10n.description,
            l10n,
            maxLength: 1000,
            maxLines: 3,
          ),
          const SizedBox(height: 12),
          SubmitButton(
            label: l10n.createAction,
            loading: command.isSubmitting,
            onPressed: _submit,
          ),
        ],
      ),
    );
  }
}

final class SupplierCreditAllocatePage extends ConsumerStatefulWidget {
  const SupplierCreditAllocatePage({required this.creditNoteId, super.key});
  final String creditNoteId;
  @override
  ConsumerState<SupplierCreditAllocatePage> createState() =>
      _SupplierCreditAllocatePageState();
}

final class _SupplierCreditAllocatePageState
    extends ConsumerState<SupplierCreditAllocatePage> {
  final Map<String, TextEditingController> _allocations = {};
  @override
  void dispose() {
    for (final value in _allocations.values) {
      value.dispose();
    }
    super.dispose();
  }

  Future<void> _submit(SupplierCreditNoteDetails details) async {
    final l10n = AppLocalizations.of(context);
    final allocations = <SupplierCreditAllocationInput>[];
    for (final entry in _allocations.entries) {
      final amount = DecimalMoney.canonicalize(entry.value.text);
      if (amount != null) {
        allocations.add(
          SupplierCreditAllocationInput(
            supplierDebtId: entry.key,
            amount: amount,
          ),
        );
      }
    }
    final input = SupplierCreditApplyInput(
      expectedVersion: details.creditNote.versionNumber,
      allocations: allocations,
    );
    if (!input.totalWithin(details.creditNote.availableAmount)) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.creditAllocationExceeded)));
      return;
    }
    final result = await ref
        .read(supplierCreditCommandControllerProvider.notifier)
        .apply(widget.creditNoteId, input);
    if (result != null && mounted) {
      context.go('/supplier-credit-notes/${widget.creditNoteId}');
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return ref
        .watch(supplierCreditDetailsProvider(widget.creditNoteId))
        .when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (_, _) => Center(child: Text(l10n.genericError)),
          data: (details) {
            final key = (
              supplierId: details.creditNote.supplierId,
              currency: details.creditNote.currencyCode,
            );
            final command = ref.watch(supplierCreditCommandControllerProvider);
            return ListView(
              padding: const EdgeInsets.all(16),
              children: [
                Text(
                  l10n.applyCredit,
                  style: Theme.of(context).textTheme.headlineMedium,
                ),
                AppMessageBanner(
                  message:
                      '${l10n.availableCredit}: ${details.creditNote.availableAmount} ${details.creditNote.currencyCode}',
                ),
                if (command.isUncertain)
                  _Uncertain(
                    controller: ref.read(
                      supplierCreditCommandControllerProvider.notifier,
                    ),
                  ),
                ref
                    .watch(eligibleSupplierDebtsProvider(key))
                    .when(
                      loading: () => const LinearProgressIndicator(),
                      error: (_, _) => Text(l10n.genericError),
                      data: (page) => Column(
                        children: [
                          for (final debt in page.items)
                            _AllocationTile(
                              label:
                                  '${debt.debtNumber} · ${debt.outstandingAmount} ${debt.currencyCode}',
                              selected: _allocations.containsKey(
                                debt.supplierDebtId,
                              ),
                              controller: _allocations[debt.supplierDebtId],
                              onChanged: (selected) => setState(() {
                                if (selected) {
                                  _allocations[debt.supplierDebtId] =
                                      TextEditingController();
                                } else {
                                  _allocations
                                      .remove(debt.supplierDebtId)
                                      ?.dispose();
                                }
                              }),
                            ),
                        ],
                      ),
                    ),
                const SizedBox(height: 12),
                SubmitButton(
                  label: l10n.applyAction,
                  loading: command.isSubmitting,
                  onPressed: () => _submit(details),
                ),
              ],
            );
          },
        );
  }
}

final class _InvoiceItemDraft {
  final name = TextEditingController();
  final quantity = TextEditingController(text: '1');
  final price = TextEditingController();
  final discount = TextEditingController(text: '0');
  final tax = TextEditingController(text: '0');
  String unit = 'Piece';
  void dispose() {
    name.dispose();
    quantity.dispose();
    price.dispose();
    discount.dispose();
    tax.dispose();
  }
}

final class _InvoiceItemEditor extends StatefulWidget {
  const _InvoiceItemEditor({required this.draft, required this.onRemove});
  final _InvoiceItemDraft draft;
  final VoidCallback onRemove;
  @override
  State<_InvoiceItemEditor> createState() => _InvoiceItemEditorState();
}

final class _InvoiceItemEditorState extends State<_InvoiceItemEditor> {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          children: [
            Row(
              children: [
                Expanded(
                  child: _requiredText(widget.draft.name, l10n.itemName, l10n),
                ),
                IconButton(
                  onPressed: widget.onRemove,
                  icon: const Icon(Icons.delete_outline),
                  tooltip: l10n.cancelAction,
                ),
              ],
            ),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                SizedBox(
                  width: 140,
                  child: _requiredText(
                    widget.draft.quantity,
                    l10n.itemQuantity,
                    l10n,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                  ),
                ),
                SizedBox(
                  width: 180,
                  child: DropdownButtonFormField<String>(
                    isExpanded: true,
                    initialValue: widget.draft.unit,
                    decoration: InputDecoration(labelText: l10n.unitCode),
                    items: [
                      for (final value in supplierUnitCodes)
                        DropdownMenuItem(
                          value: value,
                          child: Text(SupplierLabels.unit(l10n, value)),
                        ),
                    ],
                    onChanged: (value) => setState(
                      () => widget.draft.unit = value ?? widget.draft.unit,
                    ),
                  ),
                ),
                SizedBox(
                  width: 150,
                  child: _requiredText(
                    widget.draft.price,
                    l10n.unitPrice,
                    l10n,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                  ),
                ),
                SizedBox(
                  width: 150,
                  child: _requiredText(
                    widget.draft.discount,
                    l10n.discountAmount,
                    l10n,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                  ),
                ),
                SizedBox(
                  width: 150,
                  child: _requiredText(
                    widget.draft.tax,
                    l10n.taxAmount,
                    l10n,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

final class _AllocationTile extends StatelessWidget {
  const _AllocationTile({
    required this.label,
    required this.selected,
    required this.controller,
    required this.onChanged,
  });
  final String label;
  final bool selected;
  final TextEditingController? controller;
  final ValueChanged<bool> onChanged;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(8),
        child: Row(
          children: [
            Checkbox(
              value: selected,
              onChanged: (value) => onChanged(value ?? false),
            ),
            Expanded(child: Text(label)),
            if (selected)
              SizedBox(
                width: 150,
                child: TextFormField(
                  controller: controller,
                  keyboardType: const TextInputType.numberWithOptions(
                    decimal: true,
                  ),
                  decoration: InputDecoration(labelText: l10n.allocationAmount),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

final class _DateRow extends StatelessWidget {
  const _DateRow({
    required this.label,
    required this.value,
    required this.onChanged,
  });
  final String label;
  final DateTime value;
  final ValueChanged<DateTime> onChanged;
  @override
  Widget build(BuildContext context) => ListTile(
    contentPadding: EdgeInsets.zero,
    title: Text(label),
    subtitle: Text(MaterialLocalizations.of(context).formatMediumDate(value)),
    trailing: const Icon(Icons.calendar_today),
    onTap: () async {
      final selected = await showDatePicker(
        context: context,
        firstDate: DateTime(2000),
        lastDate: DateTime(2100),
        initialDate: value,
      );
      if (selected != null) onChanged(selected);
    },
  );
}

final class _Uncertain<T> extends StatelessWidget {
  const _Uncertain({required this.controller});
  final SupplierFinancialCommandController<T> controller;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppMessageBanner(
          message: l10n.operationOutcomeUncertain,
          isError: true,
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          children: [
            FilledButton(
              onPressed: controller.retrySameOperation,
              child: Text(l10n.retrySameOperation),
            ),
            TextButton(
              onPressed: controller.cancelPending,
              child: Text(l10n.cancelUncertainOperation),
            ),
          ],
        ),
      ],
    );
  }
}

Widget _requiredText(
  TextEditingController controller,
  String label,
  AppLocalizations l10n, {
  int? maxLength,
  int maxLines = 1,
  TextInputType? keyboardType,
}) => TextFormField(
  controller: controller,
  maxLength: maxLength,
  maxLines: maxLines,
  keyboardType: keyboardType,
  decoration: InputDecoration(labelText: label),
  validator: (value) =>
      value == null || value.trim().isEmpty ? l10n.requiredField : null,
);
Widget _optionalText(
  TextEditingController controller,
  String label, {
  int? maxLength,
  int maxLines = 1,
  TextInputType? keyboardType,
  bool enabled = true,
}) => TextFormField(
  controller: controller,
  enabled: enabled,
  maxLength: maxLength,
  maxLines: maxLines,
  keyboardType: keyboardType,
  decoration: InputDecoration(labelText: label),
);
