import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/providers.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../company_members/domain/company_member_models.dart';

final class ProjectDateField extends StatelessWidget {
  const ProjectDateField({
    required this.label,
    required this.value,
    required this.onChanged,
    this.required = false,
    this.enabled = true,
    super.key,
  });

  final String label;
  final DateTime? value;
  final ValueChanged<DateTime?> onChanged;
  final bool required;
  final bool enabled;

  @override
  Widget build(BuildContext context) {
    final material = MaterialLocalizations.of(context);
    final l10n = AppLocalizations.of(context);
    return FormField<DateTime>(
      initialValue: value,
      validator: (_) => required && value == null ? l10n.requiredField : null,
      builder: (field) => InkWell(
        onTap: enabled
            ? () async {
                final now = DateTime.now();
                final selected = await showDatePicker(
                  context: context,
                  initialDate: value ?? now,
                  firstDate: DateTime(2000),
                  lastDate: DateTime(2100),
                );
                if (selected != null) {
                  field.didChange(selected);
                  onChanged(selected);
                }
              }
            : null,
        borderRadius: BorderRadius.circular(12),
        child: InputDecorator(
          decoration: InputDecoration(
            labelText: label,
            errorText: field.errorText,
            suffixIcon: const Icon(Icons.calendar_month_outlined),
          ),
          child: Text(
            value == null ? l10n.selectDate : material.formatMediumDate(value!),
          ),
        ),
      ),
    );
  }
}

final class SupervisorPickerField extends StatelessWidget {
  const SupervisorPickerField({
    required this.label,
    required this.selected,
    required this.onChanged,
    this.enabled = true,
    super.key,
  });

  final String label;
  final CompanyMemberSummary? selected;
  final ValueChanged<CompanyMemberSummary?> onChanged;
  final bool enabled;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return InputDecorator(
      decoration: InputDecoration(labelText: label),
      child: Row(
        children: [
          Expanded(child: Text(selected?.fullName ?? l10n.noSupervisor)),
          if (selected != null)
            IconButton(
              tooltip: l10n.clearAction,
              onPressed: enabled ? () => onChanged(null) : null,
              icon: const Icon(Icons.clear),
            ),
          TextButton.icon(
            onPressed: enabled
                ? () async {
                    final result = await showDialog<CompanyMemberSummary>(
                      context: context,
                      builder: (_) => const _SupervisorPickerDialog(),
                    );
                    if (result != null) onChanged(result);
                  }
                : null,
            icon: const Icon(Icons.search),
            label: Text(l10n.chooseSupervisor),
          ),
        ],
      ),
    );
  }
}

final class _SupervisorPickerDialog extends ConsumerStatefulWidget {
  const _SupervisorPickerDialog();

  @override
  ConsumerState<_SupervisorPickerDialog> createState() =>
      _SupervisorPickerDialogState();
}

class _SupervisorPickerDialogState
    extends ConsumerState<_SupervisorPickerDialog> {
  final _search = TextEditingController();
  final _scroll = ScrollController();
  List<CompanyMemberSummary> _items = const [];
  AppException? _error;
  int _page = 0;
  int _totalPages = 0;
  int _generation = 0;
  bool _loading = false;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    _scroll.addListener(() {
      if (_scroll.position.extentAfter < 220) _load(more: true);
    });
    WidgetsBinding.instance.addPostFrameCallback((_) => _load());
  }

  Future<void> _load({bool more = false}) async {
    if (_loading || more && _page >= _totalPages) return;
    final search = _search.text.trim();
    if (search.length == 1) {
      setState(() => _items = const []);
      return;
    }
    _loading = true;
    final generation = more ? _generation : ++_generation;
    setState(() => _error = null);
    try {
      final page = await ref
          .read(companyMemberRepositoryProvider)
          .listMembers(
            page: more ? _page + 1 : 1,
            pageSize: 20,
            role: 'Supervisor',
            status: 'Active',
            search: search.length >= 2 ? search : null,
          );
      if (!mounted || generation != _generation) return;
      final safeItems = page.items
          .where((member) => member.isEligibleSupervisor)
          .toList(growable: false);
      final byId = {
        if (more)
          for (final item in _items) item.id: item,
        for (final item in safeItems) item.id: item,
      };
      setState(() {
        _items = byId.values.toList(growable: false);
        _page = page.page;
        _totalPages = page.totalPages;
      });
    } on AppException catch (error) {
      if (mounted && generation == _generation) setState(() => _error = error);
    } finally {
      _loading = false;
      if (mounted) setState(() {});
    }
  }

  void _searchChanged(String _) {
    _timer?.cancel();
    _timer = Timer(const Duration(milliseconds: 350), _load);
  }

  @override
  void dispose() {
    _timer?.cancel();
    _search.dispose();
    _scroll.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return AlertDialog(
      title: Text(l10n.chooseSupervisor),
      content: SizedBox(
        width: 540,
        height: 520,
        child: Column(
          children: [
            TextField(
              key: const Key('supervisor-search-field'),
              controller: _search,
              maxLength: 100,
              autofocus: true,
              decoration: InputDecoration(
                labelText: l10n.searchMembers,
                counterText: '',
                prefixIcon: const Icon(Icons.search),
              ),
              onChanged: _searchChanged,
            ),
            const SizedBox(height: 12),
            Expanded(
              child: _error != null && _items.isEmpty
                  ? AccessFailureState(
                      message: localizedError(l10n, _error!),
                      onRetry: _load,
                    )
                  : _loading && _items.isEmpty
                  ? const Center(child: CircularProgressIndicator())
                  : _items.isEmpty
                  ? AccessEmptyState(
                      title: l10n.supervisorPickerEmpty,
                      body: _search.text.trim().length == 1
                          ? l10n.searchMinimum
                          : l10n.noMembersBody,
                    )
                  : ListView.builder(
                      controller: _scroll,
                      itemCount: _items.length + 1,
                      itemBuilder: (context, index) {
                        if (index == _items.length) {
                          return _loading
                              ? const Center(child: CircularProgressIndicator())
                              : const SizedBox.shrink();
                        }
                        final member = _items[index];
                        return ListTile(
                          leading: const Icon(Icons.engineering_outlined),
                          title: Text(member.fullName),
                          subtitle: Text(
                            '${ValueLabels.role(l10n, member.role)} · '
                            '${ValueLabels.userStatus(l10n, member.status)}',
                          ),
                          onTap: () => Navigator.pop(context, member),
                        );
                      },
                    ),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: Text(l10n.cancelAction),
        ),
      ],
    );
  }
}
