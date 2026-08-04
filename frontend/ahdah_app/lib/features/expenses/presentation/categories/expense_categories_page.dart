import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/expense_models.dart';
import '../../domain/expense_requests.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';

final class ExpenseCategoriesPage extends ConsumerStatefulWidget {
  const ExpenseCategoriesPage({super.key});
  @override
  ConsumerState<ExpenseCategoriesPage> createState() =>
      _ExpenseCategoriesPageState();
}

class _ExpenseCategoriesPageState extends ConsumerState<ExpenseCategoriesPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref.read(expenseCategoryControllerProvider.notifier).load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(expenseCategoryControllerProvider);
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current!.role,
    );
    return Scaffold(
      floatingActionButton: capabilities.canManageExpenseCategories
          ? FloatingActionButton.extended(
              key: const Key('create-expense-category-action'),
              onPressed: () => context.go(AppRoutes.expenseCategoryCreatePath),
              icon: const Icon(Icons.add),
              label: Text(l10n.createExpenseCategory),
            )
          : null,
      body: RefreshIndicator(
        onRefresh: ref.read(expenseCategoryControllerProvider.notifier).load,
        child: ListView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.all(16),
          children: [
            Text(
              l10n.expenseCategories,
              style: Theme.of(context).textTheme.headlineMedium,
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 12,
              runSpacing: 12,
              children: [
                SizedBox(
                  width: 260,
                  child: DropdownButtonFormField<String?>(
                    initialValue: state.group,
                    isExpanded: true,
                    decoration: InputDecoration(labelText: l10n.categoryGroup),
                    items: [
                      DropdownMenuItem<String?>(
                        value: null,
                        child: Text(l10n.allStatuses),
                      ),
                      for (final group in expenseCategoryGroups)
                        DropdownMenuItem<String?>(
                          value: group,
                          child: Text(expenseCategoryGroupLabel(l10n, group)),
                        ),
                    ],
                    onChanged: (value) => ref
                        .read(expenseCategoryControllerProvider.notifier)
                        .setGroup(value),
                  ),
                ),
                SizedBox(
                  width: 260,
                  child: DropdownButtonFormField<String?>(
                    initialValue: state.scope,
                    isExpanded: true,
                    decoration: InputDecoration(labelText: l10n.categoryScope),
                    items: [
                      DropdownMenuItem<String?>(
                        value: null,
                        child: Text(l10n.allStatuses),
                      ),
                      for (final scope in expenseCategoryScopes)
                        DropdownMenuItem<String?>(
                          value: scope,
                          child: Text(expenseScopeLabel(l10n, scope)),
                        ),
                    ],
                    onChanged: (value) => ref
                        .read(expenseCategoryControllerProvider.notifier)
                        .setScope(value),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            if (state.phase == ExpenseReadPhase.initial ||
                state.phase == ExpenseReadPhase.loading)
              const Padding(
                padding: EdgeInsets.all(32),
                child: Center(child: CircularProgressIndicator()),
              )
            else if (state.phase == ExpenseReadPhase.failure)
              AccessFailureState(
                message: localizedError(l10n, state.error!),
                onRetry: ref
                    .read(expenseCategoryControllerProvider.notifier)
                    .load,
              )
            else if (state.items.isEmpty)
              AccessEmptyState(
                title: l10n.expenseCategories,
                body: l10n.noCategories,
              )
            else
              for (final category in state.items)
                Card(
                  child: ListTile(
                    leading: const Icon(Icons.category_outlined),
                    title: Text(category.categoryName),
                    subtitle: Text(
                      '${expenseCategoryGroupLabel(l10n, category.categoryGroup)} · '
                      '${expenseScopeLabel(l10n, category.expenseScope)}',
                    ),
                    trailing: category.requiresReceipt
                        ? Tooltip(
                            message: l10n.requiresReceipt,
                            child: const Icon(Icons.receipt_long_outlined),
                          )
                        : null,
                  ),
                ),
          ],
        ),
      ),
    );
  }
}

final class ExpenseCategoryCreatePage extends ConsumerStatefulWidget {
  const ExpenseCategoryCreatePage({super.key});
  @override
  ConsumerState<ExpenseCategoryCreatePage> createState() =>
      _ExpenseCategoryCreatePageState();
}

class _ExpenseCategoryCreatePageState
    extends ConsumerState<ExpenseCategoryCreatePage> {
  final _form = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _code = TextEditingController();
  final _description = TextEditingController();
  final _order = TextEditingController(text: '0');
  String _group = 'Other';
  String _scope = 'Both';
  bool _requiresSupplier = false;
  bool _requiresReceipt = true;
  bool _supportsQuantity = false;

  @override
  void dispose() {
    _name.dispose();
    _code.dispose();
    _description.dispose();
    _order.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_form.currentState!.validate()) return;
    final result = await ref
        .read(expenseCategoryCreateControllerProvider.notifier)
        .create(
          CreateExpenseCategoryInput(
            categoryName: _name.text,
            categoryCode: _code.text,
            categoryGroup: _group,
            expenseScope: _scope,
            description: _description.text,
            requiresSupplier: _requiresSupplier,
            requiresReceipt: _requiresReceipt,
            supportsQuantityDetails: _supportsQuantity,
            displayOrder: int.parse(_order.text),
          ),
        );
    if (!mounted || result == null) return;
    await ref.read(expenseCategoryControllerProvider.notifier).load();
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(AppLocalizations.of(context).categoryCreated)),
    );
    context.go(AppRoutes.expenseCategoriesPath);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(expenseCategoryCreateControllerProvider);
    return Form(
      key: _form,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.createExpenseCategory,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 16),
          TextFormField(
            controller: _name,
            maxLength: 150,
            decoration: InputDecoration(labelText: l10n.categoryName),
            validator: (value) => value == null || value.trim().isEmpty
                ? l10n.invalidRequiredText
                : null,
          ),
          TextFormField(
            controller: _code,
            maxLength: 30,
            textCapitalization: TextCapitalization.characters,
            decoration: InputDecoration(labelText: l10n.categoryCodeOptional),
            validator: (value) {
              final text = value?.trim() ?? '';
              if (text.isEmpty) return null;
              return RegExp(r'^[A-Z0-9][A-Z0-9_-]{1,29}$').hasMatch(text)
                  ? null
                  : l10n.invalidRequiredText;
            },
          ),
          DropdownButtonFormField<String>(
            initialValue: _group,
            decoration: InputDecoration(labelText: l10n.categoryGroup),
            items: [
              for (final group in expenseCategoryGroups)
                DropdownMenuItem(
                  value: group,
                  child: Text(expenseCategoryGroupLabel(l10n, group)),
                ),
            ],
            onChanged: state.phase == CategoryCommandPhase.submitting
                ? null
                : (value) => setState(() => _group = value ?? 'Other'),
          ),
          const SizedBox(height: 12),
          DropdownButtonFormField<String>(
            initialValue: _scope,
            decoration: InputDecoration(labelText: l10n.categoryScope),
            items: [
              for (final scope in expenseCategoryScopes)
                DropdownMenuItem(
                  value: scope,
                  child: Text(expenseScopeLabel(l10n, scope)),
                ),
            ],
            onChanged: state.phase == CategoryCommandPhase.submitting
                ? null
                : (value) => setState(() => _scope = value ?? 'Both'),
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: _description,
            maxLength: 500,
            maxLines: 3,
            decoration: InputDecoration(labelText: l10n.descriptionOptional),
          ),
          TextFormField(
            controller: _order,
            keyboardType: TextInputType.number,
            decoration: InputDecoration(labelText: l10n.displayOrder),
            validator: (value) =>
                int.tryParse(value ?? '') == null || int.parse(value!) < 0
                ? l10n.invalidRequiredText
                : null,
          ),
          CheckboxListTile(
            value: _requiresReceipt,
            title: Text(l10n.requiresReceipt),
            onChanged: (value) =>
                setState(() => _requiresReceipt = value ?? true),
          ),
          CheckboxListTile(
            value: _requiresSupplier,
            title: Text(l10n.requiresSupplier),
            onChanged: (value) =>
                setState(() => _requiresSupplier = value ?? false),
          ),
          CheckboxListTile(
            value: _supportsQuantity,
            title: Text(l10n.supportsQuantityDetails),
            onChanged: (value) =>
                setState(() => _supportsQuantity = value ?? false),
          ),
          if (state.phase == CategoryCommandPhase.failure)
            Text(
              localizedError(l10n, state.error!),
              style: TextStyle(color: Theme.of(context).colorScheme.error),
            ),
          const SizedBox(height: 16),
          FilledButton.icon(
            onPressed: state.phase == CategoryCommandPhase.submitting
                ? null
                : _submit,
            icon: state.phase == CategoryCommandPhase.submitting
                ? const SizedBox.square(
                    dimension: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.save_outlined),
            label: Text(l10n.createExpenseCategory),
          ),
        ],
      ),
    );
  }
}
