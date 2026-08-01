import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../app/providers.dart';
import '../../../app/routing/app_routes.dart';
import '../../../core/errors/app_exception.dart';
import '../../../core/localization/error_labels.dart';
import '../../../core/localization/value_labels.dart';
import '../../../core/validation/validators.dart';
import '../../../core/widgets/auth_shell.dart';
import '../../../core/widgets/form_widgets.dart';
import '../../../l10n/app_localizations.dart';
import '../../authentication/domain/identity_requests.dart';
import '../../onboarding/domain/pending_result.dart';

final class JoinRequestPage extends ConsumerStatefulWidget {
  const JoinRequestPage({super.key});

  @override
  ConsumerState<JoinRequestPage> createState() => _JoinRequestPageState();
}

class _JoinRequestPageState extends ConsumerState<JoinRequestPage> {
  final _formKey = GlobalKey<FormState>();
  final _companyCode = TextEditingController();
  final _fullName = TextEditingController();
  final _phone = TextEditingController();
  final _email = TextEditingController();
  final _password = TextEditingController();
  final _message = TextEditingController();
  String _role = 'Worker';
  bool _submitting = false;
  AppException? _error;

  @override
  void dispose() {
    _companyCode.dispose();
    _fullName.dispose();
    _phone.dispose();
    _email.dispose();
    _password.dispose();
    _message.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_submitting || !_formKey.currentState!.validate()) return;
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      await ref
          .read(authRepositoryProvider)
          .submitJoinRequest(
            SubmitJoinRequest(
              companyCode: _companyCode.text,
              fullName: _fullName.text,
              phoneNumber: _phone.text,
              email: _email.text,
              password: _password.text,
              requestedRole: _role,
              requestMessage: _message.text,
            ),
          );
      _password.clear();
      if (mounted) {
        context.goNamed(
          AppRoutes.pending,
          extra: const PendingOnboardingResult(
            PendingResultKind.joinRequestSubmitted,
          ),
        );
      }
    } on AppException catch (error) {
      _password.clear();
      if (mounted) setState(() => _error = error);
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  String? _serverError(String field, AppLocalizations l10n) =>
      _error?.fieldErrors.values.containsKey(field) == true
      ? l10n.validationError
      : null;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return AuthShell(
      title: l10n.submitJoinRequest,
      subtitle: l10n.requestRoleNotice,
      child: AutofillGroup(
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              if (_error != null) ...[
                AppMessageBanner(
                  message: localizedError(l10n, _error!),
                  isError: true,
                ),
                const SizedBox(height: 16),
              ],
              TextFormField(
                key: const Key('join-company-code-field'),
                controller: _companyCode,
                enabled: !_submitting,
                textCapitalization: TextCapitalization.characters,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                inputFormatters: const [UpperCaseTextFormatter()],
                decoration: InputDecoration(
                  labelText: l10n.companyCode,
                  helperText: l10n.companyCodeHint,
                  errorText: _serverError('companyCode', l10n),
                ),
                validator: (value) => Validators.companyCode(
                  value,
                  l10n.requiredField,
                  l10n.invalidCompanyCode,
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _fullName,
                enabled: !_submitting,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.name],
                decoration: InputDecoration(
                  labelText: l10n.fullName,
                  errorText: _serverError('fullName', l10n),
                ),
                validator: (value) => Validators.name(
                  value,
                  l10n.requiredField,
                  l10n.invalidFullName,
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _phone,
                enabled: !_submitting,
                keyboardType: TextInputType.phone,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.telephoneNumber],
                decoration: InputDecoration(
                  labelText: l10n.phoneNumber,
                  helperText: l10n.phoneHint,
                  errorText: _serverError('phoneNumber', l10n),
                ),
                validator: (value) => Validators.phone(
                  value,
                  l10n.requiredField,
                  l10n.invalidPhone,
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _email,
                enabled: !_submitting,
                keyboardType: TextInputType.emailAddress,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.email],
                decoration: InputDecoration(
                  labelText: l10n.emailOptional,
                  errorText: _serverError('email', l10n),
                ),
                validator: (value) =>
                    Validators.email(value, l10n.invalidEmail),
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                key: const Key('requested-role-field'),
                initialValue: _role,
                decoration: InputDecoration(
                  labelText: l10n.requestedRole,
                  errorText: _serverError('requestedRole', l10n),
                ),
                items: ValueLabels.assignableRoles
                    .map(
                      (role) => DropdownMenuItem(
                        value: role,
                        child: Text(ValueLabels.role(l10n, role)),
                      ),
                    )
                    .toList(growable: false),
                onChanged: _submitting
                    ? null
                    : (value) {
                        if (value != null) setState(() => _role = value);
                      },
                validator: (value) => Validators.role(value, l10n.invalidRole),
              ),
              const SizedBox(height: 8),
              Text(l10n.identityRoleNotice),
              const SizedBox(height: 16),
              TextFormField(
                controller: _message,
                enabled: !_submitting,
                minLines: 2,
                maxLines: 4,
                textInputAction: TextInputAction.newline,
                decoration: InputDecoration(
                  labelText: l10n.requestMessageOptional,
                  errorText: _serverError('requestMessage', l10n),
                ),
                validator: (value) =>
                    Validators.message(value, l10n.invalidMessage),
              ),
              const SizedBox(height: 16),
              PasswordFormField(
                controller: _password,
                autofillHints: const [AutofillHints.newPassword],
                errorText: _serverError('password', l10n),
                validator: (value) => Validators.onboardingPassword(
                  value,
                  l10n.requiredField,
                  l10n.passwordMinOnboarding,
                ),
                onFieldSubmitted: (_) => _submit(),
              ),
              const SizedBox(height: 24),
              SubmitButton(
                label: l10n.submitJoinRequest,
                loading: _submitting,
                onPressed: _submit,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
