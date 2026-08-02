import 'package:dio/dio.dart';

import '../../features/authentication/domain/identity_models.dart';
import '../../features/authentication/domain/identity_requests.dart';
import '../../features/access/domain/access_models.dart';
import '../../features/company_members/domain/company_member_models.dart';
import '../../features/projects/domain/project_models.dart';
import '../../features/projects/domain/project_requests.dart';
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

  Future<AccessPage<Invitation>> listInvitations({
    required int page,
    required int pageSize,
    String? status,
  }) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.invitations,
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
          'status': ?status,
        },
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return AccessPage.fromJson(_body(response), Invitation.fromJson);
  }

  Future<CreatedInvitation> createInvitation(
    CreateInvitationInput input,
  ) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.invitations,
        data: input.toJson(),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return CreatedInvitation.fromJson(_body(response));
  }

  Future<Invitation> cancelInvitation(String invitationId) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.cancelInvitation(invitationId),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return Invitation.fromJson(_body(response));
  }

  Future<AccessPage<JoinRequest>> listJoinRequests({
    required int page,
    required int pageSize,
    String? status,
  }) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.joinRequests,
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
          'status': ?status,
        },
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return AccessPage.fromJson(_body(response), JoinRequest.fromJson);
  }

  Future<JoinRequestDecision> approveJoinRequest(
    String joinRequestId,
    ApproveJoinRequestInput input,
  ) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.approveJoinRequest(joinRequestId),
        data: input.toJson(),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return JoinRequestDecision.fromJson(_body(response));
  }

  Future<JoinRequestDecision> rejectJoinRequest(
    String joinRequestId,
    RejectJoinRequestInput input,
  ) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.rejectJoinRequest(joinRequestId),
        data: input.toJson(),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return JoinRequestDecision.fromJson(_body(response));
  }

  Future<ProjectPage> listProjects({
    required int page,
    required int pageSize,
    String? status,
    String? search,
  }) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.projects,
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
          'status': ?status,
          'search': ?search,
        },
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return ProjectPage.fromJson(_body(response));
  }

  Future<ProjectDetails> getProject(String projectId) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.project(projectId),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return ProjectDetails.fromJson(_body(response));
  }

  Future<ProjectDetails> createProject(ProjectCreateInput input) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.projects,
        data: input.toJsonBody(),
        options: Options(
          contentType: Headers.jsonContentType,
          extra: const {requiresAuthenticationKey: true},
        ),
      ),
    );
    return ProjectDetails.fromJson(_body(response));
  }

  Future<ProjectDetails> updateProject(
    String projectId,
    ProjectUpdateInput input,
  ) async {
    final response = await _send(
      () => _dio.patch<Map<String, Object?>>(
        ApiEndpoints.project(projectId),
        data: input.toJson(),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return ProjectDetails.fromJson(_body(response));
  }

  Future<ProjectDetails> assignProjectSupervisor(
    String projectId,
    SupervisorAssignmentInput input,
  ) async {
    final response = await _send(
      () => _dio.put<Map<String, Object?>>(
        ApiEndpoints.projectSupervisor(projectId),
        data: input.toJson(),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return ProjectDetails.fromJson(_body(response));
  }

  Future<ProjectMemberPage> listProjectMembers(
    String projectId, {
    required int page,
    required int pageSize,
  }) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.projectMembers(projectId),
        queryParameters: {'page': page, 'pageSize': pageSize},
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return ProjectMemberPage.fromJson(_body(response));
  }

  Future<CompanyMemberPage> listCompanyMembers({
    required int page,
    required int pageSize,
    String? role,
    String? status,
    String? search,
  }) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.companyMembers,
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
          'role': ?role,
          'status': ?status,
          'search': ?search,
        },
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return CompanyMemberPage.fromJson(_body(response));
  }

  Future<CompanyMemberDetails> getCompanyMember(String memberId) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.companyMember(memberId),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return CompanyMemberDetails.fromJson(_body(response));
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
      404 => AppException(AppExceptionKind.notFound, problem: problem),
      409 => AppException(AppExceptionKind.conflict, problem: problem),
      final int status when status >= 500 => AppException(
        AppExceptionKind.server,
        problem: problem,
      ),
      _ => AppException(AppExceptionKind.unexpected, problem: problem),
    };
  }
}
