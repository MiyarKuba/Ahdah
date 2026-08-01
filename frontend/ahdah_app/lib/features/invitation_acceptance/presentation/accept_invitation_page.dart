import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../app/providers.dart';
import '../../../app/routing/app_routes.dart';
import '../../../core/errors/app_exception.dart';
import '../../../core/localization/error_labels.dart';
import '../../../core/validation/validators.dart';
import '../../../core/widgets/auth_shell.dart';
import '../../../core/widgets/form_widgets.dart';
import '../../../l10n/app_localizations.dart';
import '../../authentication/domain/identity_requests.dart';
import '../../onboarding/domain/pending_result.dart';
import '../../session/presentation/session_controller.dart';

final class AcceptInvitationPage extends ConsumerStatefulWidget {
  const AcceptInvitationPage({super.key});

  @override
  ConsumerState<AcceptInvitationPage> createState() =>
      _AcceptInvitationPageState();
}

class _AcceptInvitationPageState extends ConsumerState<AcceptInvitationPage> {
  final _formKey = GlobalKey<FormState>();
  final _token = TextEditingController();
  final _fullName = TextEditingController();
  final _email = TextEditingController();
  final _password = TextEditingController();
  final _tokenFocus = FocusNode();
  bool _submitting = false;
  AppException? _error;

  @override
  void initState() {
    super.initState();
    _tokenFocus.addListener(_refreshTokenMask);
  }

  void _refreshTokenMask() {
    if (mounted) setState(() {});
  }

  @override
  void dispose() {
    _token.clear();
    _password.clear();
    _tokenFocus
      ..removeListener(_refreshTokenMask)
      ..dispose();
    _token.dispose();
    _fullName.dispose();
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_submitting || !_formKey.currentState!.validate()) return;
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final result = await ref
          .read(authRepositoryProvider)
          .acceptInvitation(
            AcceptInvitationRequest(
              token: _token.text,
              fullName: _fullName.text,
              password: _password.text,
              email: _email.text,
            ),
          );
      _token.clear();
      _password.clear();
      if (result.authentication != null) {
        await ref
            .read(sessionControllerProvider.notifier)
            .establish(result.authentication!);
      } else if (mounted) {
        context.goNamed(
          AppRoutes.pending,
          extra: PendingOnboardingResult(
            result.requiresIdentityVerification
                ? PendingResultKind.identityVerificationRequired
                : PendingResultKind.managerApprovalRequired,
          ),
        );
      }
    } on AppException catch (error) {
      _token.clear();
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
      title: l10n.acceptInvitation,
      subtitle: l10n.invitationTokenHint,
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
                key: const Key('invitation-token-field'),
                controller: _token,
                focusNode: _tokenFocus,
                enabled: !_submitting,
                obscureText: !_tokenFocus.hasFocus,
                autocorrect: false,
                enableSuggestions: false,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                decoration: InputDecoration(
                  labelText: l10n.invitationToken,
                  errorText: _serverError('token', l10n),
                ),
                validator: (value) => Validators.invitationToken(
                  value,
                  l10n.requiredField,
                  l10n.invalidInvitationToken,
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
                label: l10n.acceptInvitation,
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
