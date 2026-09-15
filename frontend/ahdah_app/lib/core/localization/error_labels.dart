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
    'invitations.creation_conflict' => l10n.invitationCreationConflict,
    'invitations.concurrency_conflict' ||
    'join_requests.concurrency_conflict' => l10n.accessConflictError,
    'invitations.not_found' => l10n.invitationNotFound,
    'join_requests.not_found' => l10n.joinRequestNotFound,
    'projects.concurrency_conflict' => l10n.projectChangedConflict,
    'projects.not_found' || 'company_members.not_found' => l10n.notFoundError,
    'projects.forbidden' ||
    'projects.members.forbidden' => l10n.permissionDenied,
    'advances.not_found' => l10n.notFoundError,
    'advances.forbidden' => l10n.permissionDenied,
    'advances.conflict' => l10n.staleFinancialState,
    'advances.invalid_operation' ||
    'advances.invalid_idempotency_key' => l10n.validationError,
    'suppliers.not_found' => l10n.notFoundError,
    'suppliers.forbidden' => l10n.permissionDenied,
    'suppliers.conflict' => l10n.staleFinancialState,
    'suppliers.invalid_operation' ||
    'suppliers.invalid_idempotency_key' => l10n.validationError,
    _ => switch (error.kind) {
      AppExceptionKind.validation => l10n.validationError,
      AppExceptionKind.unauthorized => l10n.unauthorizedError,
      AppExceptionKind.forbidden => l10n.permissionDenied,
      AppExceptionKind.notFound => l10n.notFoundError,
      AppExceptionKind.conflict => l10n.conflictError,
      AppExceptionKind.network => l10n.networkError,
      AppExceptionKind.timeout => l10n.timeoutError,
      AppExceptionKind.cancelled => l10n.cancelledError,
      AppExceptionKind.server => l10n.serverError,
      _ => l10n.genericError,
    },
  };
}
