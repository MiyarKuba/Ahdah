import 'problem_details.dart';

enum AppExceptionKind {
  configuration,
  validation,
  unauthorized,
  forbidden,
  notFound,
  conflict,
  network,
  timeout,
  cancelled,
  server,
  unexpected,
}

final class AppException implements Exception {
  const AppException(this.kind, {this.problem});

  final AppExceptionKind kind;
  final ProblemDetails? problem;

  FieldValidationErrors get fieldErrors =>
      problem?.fieldErrors ?? const FieldValidationErrors({});

  @override
  String toString() =>
      'AppException(kind: ${kind.name}, code: ${problem?.code ?? 'none'})';
}
