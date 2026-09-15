import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../app/routing/app_routes.dart';
import '../../../core/localization/app_date_time_formatter.dart';
import '../../../core/errors/app_exception.dart';
import '../../../core/localization/error_labels.dart';
import '../../../core/localization/value_labels.dart';
import '../../../core/widgets/form_widgets.dart';
import '../../../l10n/app_localizations.dart';
import '../../access/domain/role_capabilities.dart';
import '../../advances/domain/decimal_money.dart';
import '../../session/presentation/session_controller.dart';
import '../domain/supplier_models.dart';
import '../domain/supplier_requests.dart';
import 'controllers/supplier_controllers.dart';
import 'widgets/supplier_labels.dart';

final class SuppliersPage extends ConsumerStatefulWidget {
  const SuppliersPage({super.key});
  @override
  ConsumerState<SuppliersPage> createState() => _SuppliersPageState();
}

final class _SuppliersPageState extends ConsumerState<SuppliersPage> {
  final _search = TextEditingController();
  bool? _active = true;
  String? _type;
  @override
  void dispose() {
    _search.dispose();
    super.dispose();
  }

  void _apply() {
    final text = _search.text.trim();
    if (text.length == 1) return;
    ref
        .read(supplierListControllerProvider.notifier)
        .applyFilters(
          SupplierFilters(
            isActive: _active,
            supplierType: _type,
            search: text.length >= 2 ? text : null,
          ),
        );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(supplierListControllerProvider);
    final role = ref.watch(sessionControllerProvider).current?.role;
    final capabilities = RoleCapabilities.forRole(role);
    return RefreshIndicator(
      onRefresh: ref.read(supplierListControllerProvider.notifier).refresh,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          _PageHeader(
            title: l10n.suppliers,
            actions: [
              IconButton(
                tooltip: l10n.refresh,
                onPressed: state.refreshing
                    ? null
                    : ref.read(supplierListControllerProvider.notifier).refresh,
                icon: const Icon(Icons.refresh),
              ),
              if (capabilities.canManageSuppliers)
                FilledButton.icon(
                  onPressed: () => context.go(AppRoutes.supplierCreatePath),
                  icon: const Icon(Icons.add),
                  label: Text(l10n.addSupplier),
                ),
            ],
          ),
          const SizedBox(height: 12),
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              SizedBox(
                width: 340,
                child: TextField(
                  controller: _search,
                  maxLength: 50,
                  textInputAction: TextInputAction.search,
                  onSubmitted: (_) => _apply(),
                  decoration: InputDecoration(
                    labelText: l10n.searchSuppliers,
                    prefixIcon: const Icon(Icons.search),
                    suffixIcon: IconButton(
                      tooltip: l10n.searchSuppliers,
                      onPressed: _apply,
                      icon: const Icon(Icons.arrow_forward),
                    ),
                  ),
                ),
              ),
              SizedBox(
                width: 210,
                child: DropdownButtonFormField<bool?>(
                  isExpanded: true,
                  initialValue: _active,
                  decoration: InputDecoration(labelText: l10n.statusFilter),
                  items: [
                    DropdownMenuItem(
                      value: true,
                      child: Text(l10n.statusActive),
                    ),
                    DropdownMenuItem(
                      value: false,
                      child: Text(l10n.statusInactive),
                    ),
                    DropdownMenuItem(
                      value: null,
                      child: Text(l10n.allStatuses),
                    ),
                  ],
                  onChanged: (value) {
                    setState(() => _active = value);
                    _apply();
                  },
                ),
              ),
              SizedBox(
                width: 230,
                child: DropdownButtonFormField<String?>(
                  isExpanded: true,
                  initialValue: _type,
                  decoration: InputDecoration(labelText: l10n.supplierType),
                  items: [
                    DropdownMenuItem(
                      value: null,
                      child: Text(l10n.allStatuses),
                    ),
                    for (final value in supplierTypes)
                      DropdownMenuItem(
                        value: value,
                        child: Text(SupplierLabels.type(l10n, value)),
                      ),
                  ],
                  onChanged: (value) {
                    setState(() => _type = value);
                    _apply();
                  },
                ),
              ),
            ],
          ),
          if (capabilities.canViewSupplierFinancials) ...[
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                _NavChip(
                  label: l10n.supplierInvoices,
                  path: AppRoutes.supplierInvoicesPath,
                ),
                _NavChip(
                  label: l10n.supplierDebts,
                  path: AppRoutes.supplierDebtsPath,
                ),
                _NavChip(
                  label: l10n.supplierPayments,
                  path: AppRoutes.supplierPaymentsPath,
                ),
                _NavChip(
                  label: l10n.supplierCreditNotes,
                  path: AppRoutes.supplierCreditNotesPath,
                ),
                _NavChip(
                  label: l10n.supplierRefunds,
                  path: AppRoutes.supplierRefundsPath,
                ),
              ],
            ),
          ],
          const SizedBox(height: 12),
          _PagedBody<SupplierSummary>(
            state: state,
            emptyMessage: l10n.noSuppliers,
            onRetry: ref.read(supplierListControllerProvider.notifier).load,
            onLoadMore: ref
                .read(supplierListControllerProvider.notifier)
                .loadMore,
            itemBuilder: (item) => Card(
              child: ListTile(
                minVerticalPadding: 14,
                title: Text(item.supplierName),
                subtitle: Text(
                  [
                    if (item.supplierCode != null) item.supplierCode!,
                    SupplierLabels.type(l10n, item.supplierType),
                    SupplierLabels.transactionMode(l10n, item.transactionMode),
                    if (item.contactPersonName != null) item.contactPersonName!,
                  ].join(' · '),
                ),
                trailing: Text(
                  item.isActive ? l10n.statusActive : l10n.statusInactive,
                ),
                onTap: () => context.go('/suppliers/${item.supplierId}'),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

final class SupplierDetailsPage extends ConsumerWidget {
  const SupplierDetailsPage({required this.supplierId, super.key});
  final String supplierId;
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final async = ref.watch(supplierDetailsProvider(supplierId));
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    return async.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (error, _) => _FutureError(
        error: error,
        onRetry: () => ref.invalidate(supplierDetailsProvider(supplierId)),
      ),
      data: (details) {
        final supplier = details.supplier;
        return ListView(
          padding: const EdgeInsets.all(16),
          children: [
            _PageHeader(
              title: l10n.supplierDetails,
              actions: [
                if (capabilities.canManageSuppliers && supplier.isActive)
                  OutlinedButton.icon(
                    onPressed: () => context.go('/suppliers/$supplierId/edit'),
                    icon: const Icon(Icons.edit_outlined),
                    label: Text(l10n.editSupplier),
                  ),
              ],
            ),
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      supplier.supplierName,
                      style: Theme.of(context).textTheme.headlineSmall,
                    ),
                    const SizedBox(height: 12),
                    _Field(l10n.supplierCode, supplier.supplierCode),
                    _Field(
                      l10n.supplierType,
                      SupplierLabels.type(l10n, supplier.supplierType),
                    ),
                    _Field(
                      l10n.transactionMode,
                      SupplierLabels.transactionMode(
                        l10n,
                        supplier.transactionMode,
                      ),
                    ),
                    _Field(l10n.contactPerson, supplier.contactPersonName),
                    _Field(l10n.phoneNumber, supplier.phoneNumber),
                    _Field(
                      l10n.secondaryPhoneNumber,
                      details.secondaryPhoneNumber,
                    ),
                    _Field(l10n.emailAddress, supplier.email),
                    _Field(l10n.city, supplier.city),
                    _Field(l10n.defaultCurrency, supplier.defaultCurrencyCode),
                    _Field(
                      l10n.status,
                      supplier.isActive
                          ? l10n.statusActive
                          : l10n.statusInactive,
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 12),
            if (capabilities.canViewSupplierFinancials)
              Card(
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        l10n.outstandingAmount,
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      if (details.balances.isEmpty)
                        Text(l10n.noSupplierInvoices),
                      for (final balance in details.balances)
                        ListTile(
                          contentPadding: EdgeInsets.zero,
                          title: _Money(
                            balance.outstandingAmount,
                            balance.currencyCode,
                          ),
                          subtitle: Text(
                            '${l10n.supplierDebts}: ${balance.openDebtCount}',
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
                if (capabilities.canViewSupplierFinancials)
                  OutlinedButton(
                    onPressed: () =>
                        context.go('/suppliers/$supplierId/payment-accounts'),
                    child: Text(l10n.paymentAccounts),
                  ),
                if (capabilities.canViewSupplierStatement)
                  OutlinedButton(
                    onPressed: () =>
                        context.go('/suppliers/$supplierId/statement'),
                    child: Text(l10n.viewStatement),
                  ),
                OutlinedButton(
                  onPressed: () => context.go(
                    '${AppRoutes.supplierInvoicesPath}?supplierId=$supplierId',
                  ),
                  child: Text(l10n.viewInvoices),
                ),
                if (capabilities.canViewSupplierFinancials)
                  OutlinedButton(
                    onPressed: () => context.go(
                      '${AppRoutes.supplierPaymentsPath}?supplierId=$supplierId',
                    ),
                    child: Text(l10n.viewPayments),
                  ),
              ],
            ),
            if (!supplier.isActive) ...[
              const SizedBox(height: 12),
              AppMessageBanner(message: l10n.supplierHistoryPreserved),
            ],
          ],
        );
      },
    );
  }
}

final class SupplierPaymentAccountsPage extends ConsumerWidget {
  const SupplierPaymentAccountsPage({required this.supplierId, super.key});
  final String supplierId;
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final async = ref.watch(supplierPaymentAccountsProvider(supplierId));
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _PageHeader(
          title: l10n.paymentAccounts,
          actions: [
            if (capabilities.canCreateSupplierPaymentAccount)
              FilledButton.icon(
                onPressed: () =>
                    _showPaymentAccountDialog(context, ref, supplierId),
                icon: const Icon(Icons.add),
                label: Text(l10n.addPaymentAccount),
              ),
          ],
        ),
        async.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (error, _) => _FutureError(
            error: error,
            onRetry: () =>
                ref.invalidate(supplierPaymentAccountsProvider(supplierId)),
          ),
          data: (page) => page.items.isEmpty
              ? _Empty(l10n.noPaymentAccounts)
              : Column(
                  children: [
                    for (final account in page.items)
                      Card(
                        child: Semantics(
                          label:
                              '${l10n.maskedAccount} ${account.maskedAccountNumber ?? account.maskedIban ?? account.maskedWalletNumber ?? ''}',
                          child: ListTile(
                            title: Text(account.accountLabel),
                            subtitle: Text(
                              [
                                SupplierLabels.accountType(
                                  l10n,
                                  account.accountType,
                                ),
                                account.maskedAccountNumber ??
                                    account.maskedIban ??
                                    account.maskedWalletNumber,
                                account.currencyCode,
                                SupplierLabels.status(
                                  l10n,
                                  account.verificationStatus,
                                ),
                              ].whereType<String>().join(' · '),
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
        ),
      ],
    );
  }
}

Future<void> _showPaymentAccountDialog(
  BuildContext context,
  WidgetRef ref,
  String supplierId,
) async {
  final l10n = AppLocalizations.of(context);
  final label = TextEditingController();
  final holder = TextEditingController();
  final bank = TextEditingController();
  final number = TextEditingController();
  final currency = TextEditingController(text: 'LYD');
  var type = 'BankAccount';
  await showDialog<void>(
    context: context,
    builder: (dialogContext) => StatefulBuilder(
      builder: (context, setState) => AlertDialog(
        title: Text(l10n.addPaymentAccount),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              DropdownButtonFormField<String>(
                isExpanded: true,
                initialValue: type,
                decoration: InputDecoration(labelText: l10n.accountType),
                items: [
                  for (final value in supplierAccountTypes)
                    DropdownMenuItem(
                      value: value,
                      child: Text(SupplierLabels.accountType(l10n, value)),
                    ),
                ],
                onChanged: (value) => setState(() => type = value ?? type),
              ),
              TextField(
                controller: label,
                decoration: InputDecoration(labelText: l10n.accountLabel),
              ),
              TextField(
                controller: holder,
                decoration: InputDecoration(labelText: l10n.accountHolder),
              ),
              TextField(
                controller: bank,
                decoration: InputDecoration(labelText: l10n.bankName),
              ),
              TextField(
                controller: number,
                decoration: InputDecoration(
                  labelText: type == 'MobileWallet'
                      ? l10n.walletNumber
                      : l10n.accountNumber,
                ),
              ),
              TextField(
                controller: currency,
                maxLength: 3,
                decoration: InputDecoration(labelText: l10n.currency),
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext),
            child: Text(l10n.cancelAction),
          ),
          FilledButton(
            onPressed: () async {
              final input = SupplierPaymentAccountInput(
                accountType: type,
                accountLabel: label.text,
                accountHolderName: holder.text,
                bankName: type == 'BankAccount' ? bank.text : null,
                accountNumber: type == 'BankAccount' ? number.text : null,
                walletProvider: type == 'MobileWallet' ? bank.text : null,
                walletNumber: type == 'MobileWallet' ? number.text : null,
                currencyCode: currency.text,
              );
              final result = await ref
                  .read(supplierPaymentAccountCreateControllerProvider.notifier)
                  .create(supplierId, input);
              if (result != null && dialogContext.mounted) {
                Navigator.pop(dialogContext);
                ref.invalidate(supplierPaymentAccountsProvider(supplierId));
              }
            },
            child: Text(l10n.createAction),
          ),
        ],
      ),
    ),
  );
  label.dispose();
  holder.dispose();
  bank.dispose();
  number.dispose();
  currency.dispose();
}

final class SupplierInvoicesPage extends ConsumerWidget {
  const SupplierInvoicesPage({this.debtsOnly = false, super.key});
  final bool debtsOnly;
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(supplierInvoiceListControllerProvider);
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    return _FinancialListPage<SupplierInvoiceSummary>(
      title: debtsOnly ? l10n.supplierDebts : l10n.supplierInvoices,
      state: state,
      empty: l10n.noSupplierInvoices,
      onRefresh: ref
          .read(supplierInvoiceListControllerProvider.notifier)
          .refresh,
      onRetry: ref.read(supplierInvoiceListControllerProvider.notifier).load,
      onLoadMore: ref
          .read(supplierInvoiceListControllerProvider.notifier)
          .loadMore,
      createLabel: capabilities.canCreateSupplierInvoice
          ? l10n.createSupplierInvoice
          : null,
      onCreate: capabilities.canCreateSupplierInvoice
          ? () => context.go(AppRoutes.supplierInvoiceCreatePath)
          : null,
      itemBuilder: (item) => Card(
        child: ListTile(
          title: Text(item.invoiceNumber ?? item.expenseNumber),
          subtitle: Text(
            '${item.supplier.supplierName} · ${SupplierLabels.status(l10n, item.debtStatus)}\n${l10n.dueDate}: ${AppDateTimeFormatter.formatDate(context, item.dueDate)}',
          ),
          isThreeLine: true,
          trailing: _Money(item.outstandingAmount, item.currencyCode),
          onTap: () => context.go(
            debtsOnly
                ? '/supplier-debts/${item.supplierDebtId}'
                : '/supplier-invoices/${item.supplierDebtId}',
          ),
        ),
      ),
    );
  }
}

final class SupplierInvoiceDetailsPage extends ConsumerWidget {
  const SupplierInvoiceDetailsPage({required this.debtId, super.key});
  final String debtId;
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    return ref
        .watch(supplierInvoiceDetailsProvider(debtId))
        .when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (error, _) => _FutureError(
            error: error,
            onRetry: () =>
                ref.invalidate(supplierInvoiceDetailsProvider(debtId)),
          ),
          data: (details) {
            final item = details.invoice;
            return ListView(
              padding: const EdgeInsets.all(16),
              children: [
                _PageHeader(title: item.invoiceNumber ?? item.expenseNumber),
                _DetailCard(
                  fields: [
                    (l10n.supplierName, item.supplier.supplierName),
                    (l10n.debtNumber, item.debtNumber),
                    (
                      l10n.invoiceDate,
                      AppDateTimeFormatter.formatDate(
                        context,
                        item.invoiceDate,
                      ),
                    ),
                    (
                      l10n.dueDate,
                      AppDateTimeFormatter.formatDate(context, item.dueDate),
                    ),
                    (
                      l10n.expenseStatus,
                      SupplierLabels.status(l10n, item.expenseStatus),
                    ),
                    (
                      l10n.debtStatus,
                      SupplierLabels.status(l10n, item.debtStatus),
                    ),
                    (l10n.amount, '${item.amount} ${item.currencyCode}'),
                    (
                      l10n.amountPaid,
                      '${item.paidAmount} ${item.currencyCode}',
                    ),
                    (
                      l10n.appliedCredit,
                      '${item.creditNoteAmount} ${item.currencyCode}',
                    ),
                    (
                      l10n.outstandingAmount,
                      '${item.outstandingAmount} ${item.currencyCode}',
                    ),
                  ],
                ),
                if (details.items.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Text(
                    l10n.expenseItems,
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  for (final line in details.items)
                    Card(
                      child: ListTile(
                        title: Text('${line.lineNumber}. ${line.itemName}'),
                        subtitle: Text(
                          '${line.quantity} ${SupplierLabels.unit(l10n, line.unitCode)} × ${line.unitPrice}',
                        ),
                        trailing: _Money(line.totalAmount, item.currencyCode),
                      ),
                    ),
                ],
              ],
            );
          },
        );
  }
}

final class SupplierPaymentsPage extends ConsumerWidget {
  const SupplierPaymentsPage({super.key});
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(supplierPaymentListControllerProvider);
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    return _FinancialListPage<SupplierPaymentSummary>(
      title: l10n.supplierPayments,
      state: state,
      empty: l10n.noSupplierPayments,
      onRefresh: ref
          .read(supplierPaymentListControllerProvider.notifier)
          .refresh,
      onRetry: ref.read(supplierPaymentListControllerProvider.notifier).load,
      onLoadMore: ref
          .read(supplierPaymentListControllerProvider.notifier)
          .loadMore,
      createLabel: capabilities.canRecordSupplierPayment
          ? l10n.recordPayment
          : null,
      onCreate: capabilities.canRecordSupplierPayment
          ? () => context.go(AppRoutes.supplierPaymentCreatePath)
          : null,
      itemBuilder: (item) => Card(
        child: ListTile(
          title: Text(item.paymentNumber),
          subtitle: Text(
            '${item.supplierName} · ${ValueLabels.paymentMethod(l10n, item.paymentMethod)} · ${SupplierLabels.status(l10n, item.status)}',
          ),
          trailing: _Money(item.paymentAmount, item.currencyCode),
          onTap: () =>
              context.go('/supplier-payments/${item.supplierPaymentId}'),
        ),
      ),
    );
  }
}

final class SupplierPaymentDetailsPage extends ConsumerWidget {
  const SupplierPaymentDetailsPage({required this.paymentId, super.key});
  final String paymentId;
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    return ref
        .watch(supplierPaymentDetailsProvider(paymentId))
        .when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (error, _) => _FutureError(
            error: error,
            onRetry: () =>
                ref.invalidate(supplierPaymentDetailsProvider(paymentId)),
          ),
          data: (details) {
            final payment = details.payment;
            return ListView(
              padding: const EdgeInsets.all(16),
              children: [
                _PageHeader(
                  title: payment.paymentNumber,
                  actions:
                      payment.status == 'PendingApproval' &&
                          capabilities.canConfirmSupplierPayment
                      ? [
                          FilledButton(
                            onPressed: () =>
                                _reviewPayment(context, ref, details, true),
                            child: Text(l10n.confirmPayment),
                          ),
                          OutlinedButton(
                            onPressed: () =>
                                _reviewPayment(context, ref, details, false),
                            child: Text(l10n.rejectPayment),
                          ),
                        ]
                      : const [],
                ),
                _DetailCard(
                  fields: [
                    (l10n.supplierName, payment.supplierName),
                    (
                      l10n.paymentDate,
                      AppDateTimeFormatter.formatDate(
                        context,
                        payment.paymentDate,
                      ),
                    ),
                    (
                      l10n.paymentAmount,
                      '${payment.paymentAmount} ${payment.currencyCode}',
                    ),
                    (
                      l10n.paymentMethod,
                      ValueLabels.paymentMethod(l10n, payment.paymentMethod),
                    ),
                    (l10n.status, SupplierLabels.status(l10n, payment.status)),
                    (l10n.paymentReference, payment.referenceNumber),
                  ],
                ),
                const SizedBox(height: 12),
                Text(
                  l10n.debtAllocations,
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                for (final allocation in details.debtAllocations)
                  ListTile(
                    title: Text(allocation.debtNumber),
                    trailing: _Money(
                      allocation.allocatedAmount,
                      payment.currencyCode,
                    ),
                  ),
                const SizedBox(height: 12),
                Text(
                  l10n.fundingAllocations,
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                for (final allocation in details.fundingSources)
                  ListTile(
                    title: Text(
                      ValueLabels.fundingType(l10n, allocation.sourceType),
                    ),
                    trailing: _Money(
                      allocation.allocatedAmount,
                      payment.currencyCode,
                    ),
                  ),
              ],
            );
          },
        );
  }
}

Future<void> _reviewPayment(
  BuildContext context,
  WidgetRef ref,
  SupplierPaymentDetails details,
  bool confirm,
) async {
  final l10n = AppLocalizations.of(context);
  final reason = TextEditingController();
  final accepted = await showDialog<bool>(
    context: context,
    builder: (dialogContext) => AlertDialog(
      title: Text(confirm ? l10n.confirmPayment : l10n.rejectPayment),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(confirm ? l10n.confirmPaymentBody : l10n.rejectPaymentBody),
          if (!confirm)
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
          child: Text(confirm ? l10n.confirmPayment : l10n.rejectPayment),
        ),
      ],
    ),
  );
  if (accepted == true) {
    final input = SupplierPaymentReviewInput(
      expectedVersion: details.payment.versionNumber,
      rejectionReason: confirm ? null : reason.text,
    );
    final controller = ref.read(
      supplierPaymentReviewControllerProvider.notifier,
    );
    final result = confirm
        ? await controller.confirm(details.payment.supplierPaymentId, input)
        : await controller.reject(details.payment.supplierPaymentId, input);
    if (result != null) {
      ref.invalidate(
        supplierPaymentDetailsProvider(details.payment.supplierPaymentId),
      );
    }
  }
  reason.dispose();
}

final class SupplierCreditNotesPage extends ConsumerWidget {
  const SupplierCreditNotesPage({super.key});
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(supplierCreditListControllerProvider);
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    return _FinancialListPage<SupplierCreditNoteSummary>(
      title: l10n.supplierCreditNotes,
      state: state,
      empty: l10n.noCreditNotes,
      onRefresh: ref
          .read(supplierCreditListControllerProvider.notifier)
          .refresh,
      onRetry: ref.read(supplierCreditListControllerProvider.notifier).load,
      onLoadMore: ref
          .read(supplierCreditListControllerProvider.notifier)
          .loadMore,
      createLabel: capabilities.canManageSupplierCredits
          ? l10n.createCreditNote
          : null,
      onCreate: capabilities.canManageSupplierCredits
          ? () => context.go(AppRoutes.supplierCreditCreatePath)
          : null,
      itemBuilder: (item) => Card(
        child: ListTile(
          title: Text(item.creditNoteNumber),
          subtitle: Text(
            '${item.supplierName} · ${SupplierLabels.reason(l10n, item.reasonType)} · ${SupplierLabels.status(l10n, item.status)}',
          ),
          trailing: _Money(item.availableAmount, item.currencyCode),
          onTap: () =>
              context.go('/supplier-credit-notes/${item.supplierCreditNoteId}'),
        ),
      ),
    );
  }
}

final class SupplierCreditNoteDetailsPage extends ConsumerWidget {
  const SupplierCreditNoteDetailsPage({required this.creditNoteId, super.key});
  final String creditNoteId;
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    return ref
        .watch(supplierCreditDetailsProvider(creditNoteId))
        .when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (error, _) => _FutureError(
            error: error,
            onRetry: () =>
                ref.invalidate(supplierCreditDetailsProvider(creditNoteId)),
          ),
          data: (details) {
            final note = details.creditNote;
            return ListView(
              padding: const EdgeInsets.all(16),
              children: [
                _PageHeader(
                  title: note.creditNoteNumber,
                  actions: capabilities.canManageSupplierCredits
                      ? [
                          if (note.status == 'PendingApproval')
                            FilledButton(
                              onPressed: () =>
                                  _approveCredit(context, ref, details),
                              child: Text(l10n.approveCreditNote),
                            ),
                          if (note.status == 'Approved' &&
                              DecimalMoney.toMinorUnits(note.availableAmount) !=
                                  BigInt.zero)
                            OutlinedButton(
                              onPressed: () => context.go(
                                '/supplier-credit-notes/$creditNoteId/allocate',
                              ),
                              child: Text(l10n.applyCredit),
                            ),
                        ]
                      : const [],
                ),
                _DetailCard(
                  fields: [
                    (l10n.supplierName, note.supplierName),
                    (
                      l10n.creditReason,
                      SupplierLabels.reason(l10n, note.reasonType),
                    ),
                    (
                      l10n.amount,
                      '${note.creditNoteAmount} ${note.currencyCode}',
                    ),
                    (
                      l10n.appliedCredit,
                      '${note.appliedAmount} ${note.currencyCode}',
                    ),
                    (
                      l10n.availableCredit,
                      '${note.availableAmount} ${note.currencyCode}',
                    ),
                    (l10n.status, SupplierLabels.status(l10n, note.status)),
                    (l10n.description, note.description),
                  ],
                ),
                for (final allocation in details.allocations)
                  ListTile(
                    title: Text(allocation.debtNumber),
                    trailing: _Money(
                      allocation.allocatedAmount,
                      note.currencyCode,
                    ),
                  ),
              ],
            );
          },
        );
  }
}

Future<void> _approveCredit(
  BuildContext context,
  WidgetRef ref,
  SupplierCreditNoteDetails details,
) async {
  final result = await ref
      .read(supplierCreditCommandControllerProvider.notifier)
      .approve(
        details.creditNote.supplierCreditNoteId,
        SupplierCreditApproveInput(
          expectedVersion: details.creditNote.versionNumber,
        ),
      );
  if (result != null) {
    ref.invalidate(
      supplierCreditDetailsProvider(details.creditNote.supplierCreditNoteId),
    );
  }
}

final class SupplierRefundsPage extends ConsumerWidget {
  const SupplierRefundsPage({super.key});
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(supplierRefundListControllerProvider);
    return _FinancialListPage<SupplierRefundSummary>(
      title: l10n.supplierRefunds,
      notice: l10n.refundHistoryOnly,
      state: state,
      empty: l10n.noSupplierRefunds,
      onRefresh: ref
          .read(supplierRefundListControllerProvider.notifier)
          .refresh,
      onRetry: ref.read(supplierRefundListControllerProvider.notifier).load,
      onLoadMore: ref
          .read(supplierRefundListControllerProvider.notifier)
          .loadMore,
      itemBuilder: (item) => Card(
        child: ListTile(
          title: Text(item.refundNumber),
          subtitle: Text(
            '${item.supplierName} · ${ValueLabels.paymentMethod(l10n, item.refundMethod)} · ${SupplierLabels.status(l10n, item.status)}',
          ),
          trailing: _Money(item.netReceivedAmount, item.currencyCode),
        ),
      ),
    );
  }
}

final class SupplierStatementPage extends ConsumerWidget {
  const SupplierStatementPage({required this.supplierId, super.key});
  final String supplierId;
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    return ref
        .watch(supplierStatementProvider(supplierId))
        .when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (error, _) => _FutureError(
            error: error,
            onRetry: () =>
                ref.invalidate(supplierStatementProvider(supplierId)),
          ),
          data: (statement) => ListView(
            padding: const EdgeInsets.all(16),
            children: [
              _PageHeader(title: l10n.supplierStatement),
              AppMessageBanner(message: l10n.differentCurrenciesSeparate),
              const SizedBox(height: 12),
              for (final balance in statement.balances)
                Card(
                  child: ListTile(
                    title: Text(balance.currencyCode),
                    trailing: _Money(
                      balance.outstandingAmount,
                      balance.currencyCode,
                    ),
                    subtitle: Text(
                      '${l10n.supplierDebts}: ${balance.openDebtCount}',
                    ),
                  ),
                ),
              if (statement.entries.items.isEmpty)
                _Empty(l10n.noStatementEntries),
              for (final entry in statement.entries.items)
                Card(
                  child: ListTile(
                    title: Text(entry.reference),
                    subtitle: Text(
                      '${entry.description}\n${AppDateTimeFormatter.formatDate(context, entry.eventDate)} · ${SupplierLabels.status(l10n, entry.status)}',
                    ),
                    isThreeLine: true,
                    trailing: _Money(entry.amount, entry.currencyCode),
                  ),
                ),
            ],
          ),
        );
  }
}

final class _FinancialListPage<T> extends StatelessWidget {
  const _FinancialListPage({
    required this.title,
    required this.state,
    required this.empty,
    required this.onRefresh,
    required this.onRetry,
    required this.onLoadMore,
    required this.itemBuilder,
    this.notice,
    this.createLabel,
    this.onCreate,
  });
  final String title;
  final String? notice;
  final SupplierListState<T> state;
  final String empty;
  final Future<void> Function() onRefresh;
  final VoidCallback onRetry;
  final VoidCallback onLoadMore;
  final Widget Function(T) itemBuilder;
  final String? createLabel;
  final VoidCallback? onCreate;
  @override
  Widget build(BuildContext context) => RefreshIndicator(
    onRefresh: onRefresh,
    child: ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _PageHeader(
          title: title,
          actions: [
            if (onCreate != null)
              FilledButton.icon(
                onPressed: onCreate,
                icon: const Icon(Icons.add),
                label: Text(createLabel!),
              ),
          ],
        ),
        if (notice != null) ...[
          const SizedBox(height: 12),
          AppMessageBanner(message: notice!),
        ],
        const SizedBox(height: 12),
        _PagedBody<T>(
          state: state,
          emptyMessage: empty,
          onRetry: onRetry,
          onLoadMore: onLoadMore,
          itemBuilder: itemBuilder,
        ),
      ],
    ),
  );
}

final class _PagedBody<T> extends StatelessWidget {
  const _PagedBody({
    required this.state,
    required this.emptyMessage,
    required this.onRetry,
    required this.onLoadMore,
    required this.itemBuilder,
  });
  final SupplierListState<T> state;
  final String emptyMessage;
  final VoidCallback onRetry;
  final VoidCallback onLoadMore;
  final Widget Function(T) itemBuilder;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    if (state.loading && state.items.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(32),
          child: CircularProgressIndicator(),
        ),
      );
    }
    if (state.error != null && state.items.isEmpty) {
      return _FutureError(error: state.error!, onRetry: onRetry);
    }
    if (state.items.isEmpty) return _Empty(emptyMessage);
    return Column(
      children: [
        for (final item in state.items) itemBuilder(item),
        if (state.loadMoreError != null)
          AppMessageBanner(
            message: localizedError(l10n, state.loadMoreError!),
            isError: true,
          ),
        if (state.hasMore)
          TextButton.icon(
            onPressed: state.loadingMore ? null : onLoadMore,
            icon: state.loadingMore
                ? const SizedBox.square(
                    dimension: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.expand_more),
            label: Text(l10n.loadMore),
          ),
      ],
    );
  }
}

final class _FutureError extends StatelessWidget {
  const _FutureError({required this.error, required this.onRetry});
  final Object error;
  final VoidCallback onRetry;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final message = error is AppException
        ? localizedError(l10n, error as AppException)
        : l10n.genericError;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(message),
            const SizedBox(height: 8),
            OutlinedButton(onPressed: onRetry, child: Text(l10n.retry)),
          ],
        ),
      ),
    );
  }
}

final class _Empty extends StatelessWidget {
  const _Empty(this.message);
  final String message;
  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.all(32),
    child: Center(child: Text(message, textAlign: TextAlign.center)),
  );
}

final class _PageHeader extends StatelessWidget {
  const _PageHeader({required this.title, this.actions = const []});
  final String title;
  final List<Widget> actions;
  @override
  Widget build(BuildContext context) => Wrap(
    alignment: WrapAlignment.spaceBetween,
    crossAxisAlignment: WrapCrossAlignment.center,
    spacing: 12,
    runSpacing: 8,
    children: [
      Text(title, style: Theme.of(context).textTheme.headlineMedium),
      Wrap(spacing: 8, runSpacing: 8, children: actions),
    ],
  );
}

final class _Field extends StatelessWidget {
  const _Field(this.label, this.value);
  final String label;
  final String? value;
  @override
  Widget build(BuildContext context) {
    if (value == null || value!.isEmpty) return const SizedBox.shrink();
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Text('$label: $value'),
    );
  }
}

final class _DetailCard extends StatelessWidget {
  const _DetailCard({required this.fields});
  final List<(String, String?)> fields;
  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [for (final field in fields) _Field(field.$1, field.$2)],
      ),
    ),
  );
}

final class _Money extends StatelessWidget {
  const _Money(this.amount, this.currency);
  final String amount;
  final String currency;
  @override
  Widget build(BuildContext context) => Directionality(
    textDirection: TextDirection.ltr,
    child: Semantics(
      label: '$amount $currency',
      child: Text(
        '$amount $currency',
        style: const TextStyle(fontFeatures: [FontFeature.tabularFigures()]),
      ),
    ),
  );
}

final class _NavChip extends StatelessWidget {
  const _NavChip({required this.label, required this.path});
  final String label;
  final String path;
  @override
  Widget build(BuildContext context) =>
      ActionChip(label: Text(label), onPressed: () => context.go(path));
}
