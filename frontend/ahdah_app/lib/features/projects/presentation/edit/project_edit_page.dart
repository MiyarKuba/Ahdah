import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../core/validation/validators.dart';
import '../../../../core/widgets/form_widgets.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../domain/project_models.dart';
import '../../domain/project_requests.dart';
import '../controllers/project_controllers.dart';
import '../widgets/project_form_widgets.dart';

final class ProjectEditPage extends ConsumerStatefulWidget {
  const ProjectEditPage({required this.projectId, super.key});

  final String projectId;

  @override
  ConsumerState<ProjectEditPage> createState() => _ProjectEditPageState();
}

class _ProjectEditPageState extends ConsumerState<ProjectEditPage> {
  final _formKey = GlobalKey<FormState>();
  final _projectName = TextEditingController();
  final _siteAddress = TextEditingController();
  final _contactPhone = TextEditingController();
  final _description = TextEditingController();
  final _notes = TextEditingController();
  DateTime? _contractDate;
  DateTime? _startDate;
  DateTime? _expectedEndDate;
  String _status = 'Active';
  bool _initialized = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(projectDetailsControllerProvider(widget.projectId).notifier)
          .load();
    });
  }

  void _initialize(ProjectDetails project) {
    if (_initialized) return;
    _initialized = true;
    _projectName.text = project.projectName;
    _siteAddress.text = project.siteAddress;
    _contactPhone.text = project.contactPhoneNumber ?? '';
    _description.text = project.description ?? '';
    _notes.text = project.notes ?? '';
    _contractDate = project.contractDate;
    _startDate = project.startDate;
    _expectedEndDate = project.expectedEndDate;
    _status = const {'Active', 'Paused'}.contains(project.status)
        ? project.status
        : 'Active';
  }

  @override
  void dispose() {
    _projectName.dispose();
    _siteAddress.dispose();
    _contactPhone.dispose();
    _description.dispose();
    _notes.dispose();
    super.dispose();
  }

  String? _changedText(String current, String? original) {
    final normalized = current.trim();
    final old = original?.trim() ?? '';
    if (normalized == old || normalized.isEmpty) return null;
    return normalized;
  }

  bool _sameDate(DateTime? left, DateTime? right) =>
      left?.year == right?.year &&
      left?.month == right?.month &&
      left?.day == right?.day;

  Future<void> _submit(ProjectDetails project) async {
    final l10n = AppLocalizations.of(context);
    if (!_formKey.currentState!.validate()) return;
    if (_expectedEndDate != null &&
        _startDate != null &&
        _expectedEndDate!.isBefore(_startDate!)) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.invalidDateOrder)));
      return;
    }
    final input = ProjectUpdateInput(
      expectedVersion: project.versionNumber,
      projectName: _changedText(_projectName.text, project.projectName),
      siteAddress: _changedText(_siteAddress.text, project.siteAddress),
      contactPhoneNumber: _changedText(
        _contactPhone.text,
        project.contactPhoneNumber,
      ),
      contractDate: _sameDate(_contractDate, project.contractDate)
          ? null
          : _contractDate,
      startDate: _sameDate(_startDate, project.startDate) ? null : _startDate,
      expectedEndDate: _sameDate(_expectedEndDate, project.expectedEndDate)
          ? null
          : _expectedEndDate,
      status: _status == project.status ? null : _status,
      description: _changedText(_description.text, project.description),
      notes: _changedText(_notes.text, project.notes),
    );
    if (input.toJson().length == 1) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.validationError)));
      return;
    }
    final result = await ref
        .read(projectUpdateControllerProvider.notifier)
        .update(project.id, input);
    if (!mounted || result == null) return;
    await ref.read(projectListControllerProvider.notifier).refresh();
    ref.invalidate(projectDetailsControllerProvider(project.id));
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(l10n.projectUpdated)));
    context.goNamed(
      AppRoutes.projectDetails,
      pathParameters: {'projectId': project.id},
    );
  }

  Future<void> _reload() async {
    ref.read(projectUpdateControllerProvider.notifier).reset();
    _initialized = false;
    await ref
        .read(projectDetailsControllerProvider(widget.projectId).notifier)
        .load();
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final details = ref.watch(
      projectDetailsControllerProvider(widget.projectId),
    );
    final submit = ref.watch(projectUpdateControllerProvider);
    if (details.phase == ProjectDetailsPhase.initial ||
        details.phase == ProjectDetailsPhase.loading &&
            details.project == null) {
      return const Center(child: CircularProgressIndicator());
    }
    if (details.phase == ProjectDetailsPhase.failure &&
        details.project == null) {
      return AccessFailureState(
        message: localizedError(l10n, details.error!),
        onRetry: _reload,
      );
    }
    final project = details.project!;
    _initialize(project);
    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              l10n.editProject,
              style: Theme.of(context).textTheme.headlineSmall,
            ),
            const SizedBox(height: 16),
            if (submit.error != null) ...[
              AppMessageBanner(
                message: submit.isConflict
                    ? l10n.projectChangedConflict
                    : localizedError(l10n, submit.error!),
                isError: true,
              ),
              if (submit.isConflict)
                Align(
                  alignment: AlignmentDirectional.centerStart,
                  child: TextButton.icon(
                    key: const Key('reload-project-action'),
                    onPressed: _reload,
                    icon: const Icon(Icons.refresh),
                    label: Text(l10n.reloadProject),
                  ),
                ),
              const SizedBox(height: 12),
            ],
            Card(
              child: Padding(
                padding: const EdgeInsets.all(22),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    TextFormField(
                      key: const Key('edit-project-name-field'),
                      controller: _projectName,
                      enabled: !submit.isSubmitting,
                      maxLength: 200,
                      decoration: InputDecoration(labelText: l10n.projectName),
                      validator: (value) => Validators.boundedText(
                        value,
                        200,
                        l10n.requiredField,
                        l10n.invalidRequiredText,
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _siteAddress,
                      enabled: !submit.isSubmitting,
                      maxLength: 500,
                      decoration: InputDecoration(labelText: l10n.siteAddress),
                      validator: (value) => Validators.boundedText(
                        value,
                        500,
                        l10n.requiredField,
                        l10n.invalidRequiredText,
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _contactPhone,
                      enabled: !submit.isSubmitting,
                      maxLength: 20,
                      textDirection: TextDirection.ltr,
                      decoration: InputDecoration(
                        labelText: l10n.contactPhoneOptional,
                      ),
                      validator: (value) =>
                          value == null || value.trim().isEmpty
                          ? null
                          : Validators.phone(
                              value,
                              l10n.requiredField,
                              l10n.invalidPhone,
                            ),
                    ),
                    const SizedBox(height: 12),
                    DropdownButtonFormField<String>(
                      key: const Key('project-status-field'),
                      initialValue: _status,
                      decoration: InputDecoration(labelText: l10n.statusFilter),
                      items: [
                        for (final status in const ['Active', 'Paused'])
                          DropdownMenuItem(
                            value: status,
                            child: Text(
                              ValueLabels.projectStatus(l10n, status),
                            ),
                          ),
                      ],
                      onChanged: submit.isSubmitting
                          ? null
                          : (value) =>
                                setState(() => _status = value ?? _status),
                    ),
                    const SizedBox(height: 12),
                    ProjectDateField(
                      label: l10n.contractDate,
                      value: _contractDate,
                      required: true,
                      enabled: !submit.isSubmitting,
                      onChanged: (value) =>
                          setState(() => _contractDate = value),
                    ),
                    const SizedBox(height: 12),
                    ProjectDateField(
                      label: l10n.startDate,
                      value: _startDate,
                      required: true,
                      enabled: !submit.isSubmitting,
                      onChanged: (value) => setState(() => _startDate = value),
                    ),
                    const SizedBox(height: 12),
                    ProjectDateField(
                      label: l10n.expectedEndDate,
                      value: _expectedEndDate,
                      enabled: !submit.isSubmitting,
                      onChanged: (value) =>
                          setState(() => _expectedEndDate = value),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _description,
                      enabled: !submit.isSubmitting,
                      minLines: 2,
                      maxLines: 5,
                      maxLength: 1500,
                      decoration: InputDecoration(
                        labelText: l10n.descriptionOptional,
                      ),
                      validator: (value) => Validators.optionalBoundedText(
                        value,
                        1500,
                        l10n.invalidRequiredText,
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _notes,
                      enabled: !submit.isSubmitting,
                      minLines: 2,
                      maxLines: 4,
                      maxLength: 1000,
                      decoration: InputDecoration(
                        labelText: l10n.notesOptional,
                      ),
                      validator: (value) => Validators.optionalBoundedText(
                        value,
                        1000,
                        l10n.invalidRequiredText,
                      ),
                    ),
                    const SizedBox(height: 8),
                    AppMessageBanner(
                      message:
                          '${l10n.ownerClient}: ${project.owner.ownerName}. '
                          '${l10n.directoryReadOnly}',
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 20),
            Align(
              alignment: AlignmentDirectional.centerEnd,
              child: SubmitButton(
                label: l10n.saveAction,
                loading: submit.isSubmitting,
                onPressed: () => _submit(project),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
