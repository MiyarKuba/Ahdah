import 'package:dio/dio.dart';

import '../../features/authentication/domain/identity_models.dart';
import '../../features/authentication/domain/identity_requests.dart';
import '../errors/app_exception.dart';
import '../errors/problem_details.dart';
import 'api_endpoints.dart';
import 'auth_interceptor.dart';

final class ApiClient {
  ApiClient(this._dio);

  final Dio _dio;

  Future<AuthenticationResult> login(LoginRequest request) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.login,
        data: request.toJson(),
      ),
    );
    return AuthenticationResult.fromJson(_body(response));
  }

  Future<RegisterCompanyResult> registerCompany(
    RegisterCompanyRequest request,
  ) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.registerCompany,
        data: request.toJson(),
      ),
    );
    return RegisterCompanyResult.fromJson(_body(response));
  }

  Future<CurrentSessionResult> currentSession() async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.currentUser,
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return CurrentSessionResult.fromJson(_body(response));
  }

  Future<InvitationAcceptanceResult> acceptInvitation(
    AcceptInvitationRequest request,
  ) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.acceptInvitation,
        data: request.toJson(),
      ),
    );
    return InvitationAcceptanceResult.fromJson(_body(response));
  }

  Future<JoinRequestAcknowledgement> submitJoinRequest(
    SubmitJoinRequest request,
  ) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.joinRequests,
        data: request.toJson(),
      ),
    );
    return JoinRequestAcknowledgement.fromJson(_body(response));
  }

  Future<bool> health() async {
    await _send(() => _dio.get<Map<String, Object?>>(ApiEndpoints.health));
    return true;
  }

  static Map<String, Object?> _body(Response<Map<String, Object?>> response) {
    final data = response.data;
    if (data == null) {
      throw const AppException(AppExceptionKind.server);
    }
    return data;
  }

  static Future<Response<Map<String, Object?>>> _send(
    Future<Response<Map<String, Object?>>> Function() request,
  ) async {
    try {
      return await request();
    } on DioException catch (error) {
      throw _mapDioException(error);
    } on AppException {
      rethrow;
    } on FormatException {
      throw const AppException(AppExceptionKind.server);
    } on TypeError {
      throw const AppException(AppExceptionKind.server);
    }
  }

  static AppException _mapDioException(DioException error) {
    if (error.type == DioExceptionType.cancel) {
      return const AppException(AppExceptionKind.cancelled);
    }
    if (error.type == DioExceptionType.connectionTimeout ||
        error.type == DioExceptionType.sendTimeout ||
        error.type == DioExceptionType.receiveTimeout) {
      return const AppException(AppExceptionKind.timeout);
    }
    if (error.type == DioExceptionType.connectionError ||
        error.type == DioExceptionType.unknown && error.response == null) {
      return const AppException(AppExceptionKind.network);
    }

    final problem = ProblemDetails.fromJson(error.response?.data);
    return switch (error.response?.statusCode) {
      400 => AppException(AppExceptionKind.validation, problem: problem),
      401 => AppException(AppExceptionKind.unauthorized, problem: problem),
      403 => AppException(AppExceptionKind.forbidden, problem: problem),
      409 => AppException(AppExceptionKind.conflict, problem: problem),
      final int status when status >= 500 => AppException(
        AppExceptionKind.server,
        problem: problem,
      ),
      _ => AppException(AppExceptionKind.unexpected, problem: problem),
    };
  }
}
