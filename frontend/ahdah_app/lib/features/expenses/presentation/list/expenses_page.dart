import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/expense_filters.dart';
import '../../domain/expense_models.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';
import '../widgets/expense_widgets.dart';

final class ExpensesPage extends ConsumerStatefulWidget {
  const ExpensesPage({super.key});

  @override
  ConsumerState<ExpensesPage> createState() => _ExpensesPageState();
}

class _ExpensesPageState extends ConsumerState<ExpensesPage> {
  final _reference = TextEditingController();
  final _scroll = ScrollController();

  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref.read(expenseListControllerProvider.notifier).load(),
    );
    _scroll.addListener(() {
      if (_scroll.position.extentAfter < 320) {
        ref.read(expenseListControllerProvider.notifier).loadMore();
      }
    });
  }

  @override
  void dispose() {
    _reference.dispose();
    _scroll.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(expenseListControllerProvider);
    final role = ref.watch(sessionControllerProvider).current!.role;
    final capabilities = RoleCapabilities.forRole(role);
    return Scaffold(
      floatingActionButton: capabilities.canCreateExpenses
          ? FloatingActionButton.extended(
              key: const Key('create-expense-action'),
              onPressed: () => context.go(AppRoutes.expenseCreatePath),
              icon: const Icon(Icons.add),
              label: Text(l10n.createExpense),
            )
          : null,
      body: RefreshIndicator(
        onRefresh: ref.read(expenseListControllerProvider.notifier).refresh,
        child: CustomScrollView(
          controller: _scroll,
          physics: const AlwaysScrollableScrollPhysics(),
          slivers: [
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            l10n.expensesTitle,
                            style: Theme.of(context).textTheme.headlineMedium,
                          ),
                        ),
                        IconButton(
                          tooltip: l10n.refresh,
                          onPressed: ref
                              .read(expenseListControllerProvider.notifier)
                              .refresh,
                          icon: const Icon(Icons.refresh),
                        ),
                        IconButton(
                          tooltip: l10n.expenseCategories,
                          onPressed: () =>
                              context.go(AppRoutes.expenseCategoriesPath),
                          icon: const Icon(Icons.category_outlined),
                        ),
                        IconButton(
                          tooltip: l10n.reimbursements,
                          onPressed: () =>
                              context.go(AppRoutes.reimbursementsPath),
                          icon: const Icon(Icons.receipt_long_outlined),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    TextField(
                      controller: _reference,
                      maxLength: 50,
                      onChanged: ref
                          .read(expenseListControllerProvider.notifier)
                          .setReference,
                      decoration: InputDecoration(
                        labelText: l10n.searchExpenseReference,
                        prefixIcon: const Icon(Icons.search),
                        counterText: '',
                      ),
                    ),
                    const SizedBox(height: 12),
                    Wrap(
                      spacing: 12,
                      runSpacing: 12,
                      children: [
                        SizedBox(
                          width: 250,
                          child: DropdownButtonFormField<String?>(
                            initialValue: state.filters.status,
                            isExpanded: true,
                            decoration: InputDecoration(
                              labelText: l10n.statusFilter,
                            ),
                            items: [
                              DropdownMenuItem<String?>(
                                value: null,
                                child: Text(l10n.allStatuses),
                              ),
                              for (final status in expenseStatuses)
                                DropdownMenuItem<String?>(
                                  value: status,
                                  child: Text(expenseStatusLabel(l10n, status)),
                                ),
                            ],
                            onChanged: (value) => ref
                                .read(expenseListControllerProvider.notifier)
                                .setFilters(
                                  ExpenseFilters(
                                    status: value,
                                    paymentMode: state.filters.paymentMode,
                                  ),
                                ),
                          ),
                        ),
                        SizedBox(
                          width: 250,
                          child: DropdownButtonFormField<String?>(
                            initialValue: state.filters.paymentMode,
                            isExpanded: true,
                            decoration: InputDecoration(
                              labelText: l10n.paymentMode,
                            ),
                            items: [
                              DropdownMenuItem<String?>(
                                value: null,
                                child: Text(l10n.allStatuses),
                              ),
                              for (final mode in expensePaymentModes)
                                DropdownMenuItem<String?>(
                                  value: mode,
                                  child: Text(
                                    expensePaymentModeLabel(l10n, mode),
                                  ),
                                ),
                            ],
                            onChanged: (value) => ref
                                .read(expenseListControllerProvider.notifier)
                                .setFilters(
                                  ExpenseFilters(
                                    status: state.filters.status,
                                    paymentMode: value,
                                  ),
                                ),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            if (state.phase == ExpenseReadPhase.initial ||
                state.phase == ExpenseReadPhase.loading)
              const SliverFillRemaining(
                child: Center(child: CircularProgressIndicator()),
              )
            else if (state.phase == ExpenseReadPhase.failure)
              SliverFillRemaining(
                child: AccessFailureState(
                  message: localizedError(l10n, state.error!),
                  onRetry: ref
                      .read(expenseListControllerProvider.notifier)
                      .load,
                ),
              )
            else if (state.items.isEmpty)
              SliverFillRemaining(
                child: AccessEmptyState(
                  title: l10n.noExpensesTitle,
                  body: role == 'Supervisor'
                      ? l10n.supervisorExpensesBody
                      : role == 'Worker'
                      ? l10n.workerExpensesBody
                      : l10n.noExpensesBody,
                ),
              )
            else
              SliverPadding(
                padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
                sliver: SliverList.separated(
                  itemCount: state.items.length,
                  separatorBuilder: (_, _) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final expense = state.items[index];
                    return ExpenseSummaryCard(
                      expense: expense,
                      onOpen: () => context.go(
                        '${AppRoutes.expensesPath}/${expense.expenseId}',
                      ),
                    );
                  },
                ),
              ),
            if (state.phase == ExpenseReadPhase.loadingMore)
              const SliverToBoxAdapter(
                child: Padding(
                  padding: EdgeInsets.all(16),
                  child: Center(child: CircularProgressIndicator()),
                ),
              ),
            if (state.loadMoreError != null)
              SliverToBoxAdapter(
                child: Center(
                  child: TextButton.icon(
                    onPressed: ref
                        .read(expenseListControllerProvider.notifier)
                        .loadMore,
                    icon: const Icon(Icons.refresh),
                    label: Text(l10n.retryLoadingMore),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
