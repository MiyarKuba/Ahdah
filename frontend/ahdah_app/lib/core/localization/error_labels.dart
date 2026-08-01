import '../../l10n/app_localizations.dart';
import '../errors/app_exception.dart';

String localizedError(AppLocalizations l10n, AppException error) {
  return switch (error.problem?.code) {
    'authentication.invalid_credentials' => l10n.invalidCredentials,
    'authentication.stale_token' => l10n.unauthorizedError,
    'identity.registration_conflict' => l10n.registrationConflict,
    'invitations.invalid_token' => l10n.invitationInvalid,
    'invitations.acceptance_conflict' => l10n.conflictError,
    'join_requests.submission_unavailable' => l10n.joinUnavailable,
    'join_requests.submission_conflict' => l10n.conflictError,
    _ => switch (error.kind) {
      AppExceptionKind.validation => l10n.validationError,
      AppExceptionKind.unauthorized ||
      AppExceptionKind.forbidden => l10n.unauthorizedError,
      AppExceptionKind.conflict => l10n.conflictError,
      AppExceptionKind.network => l10n.networkError,
      AppExceptionKind.timeout => l10n.timeoutError,
      AppExceptionKind.cancelled => l10n.cancelledError,
      AppExceptionKind.server => l10n.serverError,
      _ => l10n.genericError,
    },
  };
}
