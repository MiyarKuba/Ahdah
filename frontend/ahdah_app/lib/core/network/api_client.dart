import 'dart:convert';

import 'package:dio/dio.dart';

import '../../features/authentication/domain/identity_models.dart';
import '../../features/authentication/domain/identity_requests.dart';
import '../../features/access/domain/access_models.dart';
import '../../features/company_members/domain/company_member_models.dart';
import '../../features/projects/domain/project_models.dart';
import '../../features/projects/domain/project_requests.dart';
import '../../features/advances/domain/advance_models.dart';
import '../../features/advances/domain/advance_requests.dart';
import '../../features/expenses/domain/expense_filters.dart';
import '../../features/expenses/domain/expense_models.dart';
import '../../features/expenses/domain/expense_requests.dart';
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

  Future<AdvancePage> listAdvances({
    required int page,
    required int pageSize,
    String? status,
    String? userId,
    String? reference,
  }) async => AdvancePage.fromJson(
    await _financialGet(
      ApiEndpoints.advances,
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
        'status': ?status,
        'userId': ?userId,
        'reference': ?reference,
      },
    ),
  );

  Future<AdvanceDetails> getAdvance(String advanceId) async =>
      AdvanceDetails.fromJson(
        await _financialGet(ApiEndpoints.advance(advanceId)),
      );

  Future<AdvanceMovementPage> listAdvanceMovements(
    String advanceId, {
    required int page,
    required int pageSize,
  }) async => AdvanceMovementPage.fromJson(
    await _financialGet(
      ApiEndpoints.advanceMovements(advanceId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    ),
  );

  Future<FundingSourcePage> listAdvanceFundingSources({
    required int page,
    required int pageSize,
  }) async => FundingSourcePage.fromJson(
    await _financialGet(
      ApiEndpoints.advanceFundingSources,
      queryParameters: {'page': page, 'pageSize': pageSize},
    ),
  );

  Future<AdvanceBalancePage> getMyAdvanceBalances({
    required int page,
    required int pageSize,
  }) async => AdvanceBalancePage.fromJson(
    await _financialGet(
      ApiEndpoints.myAdvanceBalances,
      queryParameters: {'page': page, 'pageSize': pageSize},
    ),
  );

  Future<AdvanceBalancePage> getUserAdvanceBalances(
    String userId, {
    required int page,
    required int pageSize,
  }) async => AdvanceBalancePage.fromJson(
    await _financialGet(
      ApiEndpoints.userAdvanceBalances(userId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    ),
  );

  Future<AdvanceDetails> createAdvance(
    CreateAdvanceInput input,
    String idempotencyKey,
  ) async => AdvanceDetails.fromJson(
    await _financialPost(
      ApiEndpoints.advances,
      data: input.toJsonBody(),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<AdvanceTransferDetails> distributeAdvance(
    String advanceId,
    CreateAdvanceDistributionInput input,
    String idempotencyKey,
  ) async => AdvanceTransferDetails.fromJson(
    await _financialPost(
      ApiEndpoints.advanceDistributions(advanceId),
      data: input.toJsonBody(),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<AdvanceTransferDetails> returnAdvanceMoney(
    String advanceId,
    CreateAdvanceReturnInput input,
    String idempotencyKey,
  ) async => AdvanceTransferDetails.fromJson(
    await _financialPost(
      ApiEndpoints.advanceReturns(advanceId),
      data: input.toJsonBody(),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<AdvanceTransferDetails> confirmAdvanceTransfer(
    String transferId,
    String idempotencyKey,
  ) async => AdvanceTransferDetails.fromJson(
    await _financialPost(
      ApiEndpoints.advanceTransferConfirm(transferId),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<AdvanceTransferDetails> rejectAdvanceTransfer(
    String transferId,
    RejectAdvanceTransferInput input,
    String idempotencyKey,
  ) async => AdvanceTransferDetails.fromJson(
    await _financialPost(
      ApiEndpoints.advanceTransferReject(transferId),
      data: input.toJson(),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<ExpenseCategoryPage> listExpenseCategories({
    required int page,
    required int pageSize,
    String? categoryGroup,
    String? expenseScope,
  }) async {
    final response = await _send(
      () => _dio.get<Map<String, Object?>>(
        ApiEndpoints.expenseCategories,
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
          'categoryGroup': ?categoryGroup,
          'expenseScope': ?expenseScope,
        },
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return ExpenseCategoryPage.fromJson(_body(response));
  }

  Future<ExpenseCategory> createExpenseCategory(
    CreateExpenseCategoryInput input,
  ) async {
    final response = await _send(
      () => _dio.post<Map<String, Object?>>(
        ApiEndpoints.expenseCategories,
        data: input.toJson(),
        options: Options(extra: const {requiresAuthenticationKey: true}),
      ),
    );
    return ExpenseCategory.fromJson(_body(response));
  }

  Future<ExpensePage> listExpenses({
    required int page,
    required int pageSize,
    required ExpenseFilters filters,
  }) async => ExpensePage.fromJson(
    await _financialGet(
      ApiEndpoints.expenses,
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
        'status': ?filters.status,
        'categoryId': ?filters.categoryId,
        'projectId': ?filters.projectId,
        'incurredByUserId': ?filters.incurredByUserId,
        'submittedByUserId': ?filters.submittedByUserId,
        'paymentMode': ?filters.paymentMode,
        'reference': ?filters.reference,
      },
    ),
  );

  Future<ExpenseDetails> getExpense(String expenseId) async =>
      ExpenseDetails.fromJson(
        await _financialGet(ApiEndpoints.expense(expenseId)),
      );

  Future<ExpenseAllocationPage> listExpenseAllocations(
    String expenseId, {
    required int page,
    required int pageSize,
  }) async => ExpenseAllocationPage.fromJson(
    await _financialGet(
      ApiEndpoints.expenseAllocations(expenseId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    ),
  );

  Future<ExpenseDocumentPage> listExpenseDocuments(
    String expenseId, {
    required int page,
    required int pageSize,
  }) async => ExpenseDocumentPage.fromJson(
    await _financialGet(
      ApiEndpoints.expenseAttachments(expenseId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    ),
  );

  Future<ExpenseHistoryPage> listExpenseHistory(
    String expenseId, {
    required int page,
    required int pageSize,
  }) async => ExpenseHistoryPage.fromJson(
    await _financialGet(
      ApiEndpoints.expenseHistory(expenseId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    ),
  );

  Future<ExpenseDetails> createExpense(
    CreateExpenseInput input,
    String idempotencyKey,
  ) async => ExpenseDetails.fromJson(
    await _financialPost(
      ApiEndpoints.expenses,
      data: input.toJsonBody(),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<ExpenseDetails> approveExpense(
    String expenseId,
    ApproveExpenseInput input,
    String idempotencyKey,
  ) async => ExpenseDetails.fromJson(
    await _financialPost(
      ApiEndpoints.approveExpense(expenseId),
      data: input.toJson(),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<ExpenseDetails> rejectExpense(
    String expenseId,
    RejectExpenseInput input,
    String idempotencyKey,
  ) async => ExpenseDetails.fromJson(
    await _financialPost(
      ApiEndpoints.rejectExpense(expenseId),
      data: input.toJson(),
      idempotencyKey: idempotencyKey,
    ),
  );

  Future<ReimbursementPage> listReimbursements({
    required int page,
    required int pageSize,
    String? status,
    String? claimantUserId,
  }) async => ReimbursementPage.fromJson(
    await _financialGet(
      ApiEndpoints.reimbursements,
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
        'status': ?status,
        'claimantUserId': ?claimantUserId,
      },
    ),
  );

  Future<ReimbursementSummary> getReimbursement(String reimbursementId) async =>
      ReimbursementSummary.fromJson(
        await _financialGet(ApiEndpoints.reimbursement(reimbursementId)),
      );

  Future<bool> health() async {
    await _send(() => _dio.get<Map<String, Object?>>(ApiEndpoints.health));
    return true;
  }

  Future<Map<String, Object?>> _financialGet(
    String path, {
    Map<String, Object?>? queryParameters,
  }) async {
    final response = await _sendFinancial(
      () => _dio.get<String>(
        path,
        queryParameters: queryParameters,
        options: Options(
          responseType: ResponseType.plain,
          extra: const {requiresAuthenticationKey: true},
        ),
      ),
    );
    return _financialBody(response);
  }

  Future<Map<String, Object?>> _financialPost(
    String path, {
    Object? data,
    required String idempotencyKey,
  }) async {
    final response = await _sendFinancial(
      () => _dio.post<String>(
        path,
        data: data,
        options: Options(
          contentType: Headers.jsonContentType,
          responseType: ResponseType.plain,
          headers: {'Idempotency-Key': idempotencyKey},
          extra: const {requiresAuthenticationKey: true},
        ),
      ),
    );
    return _financialBody(response);
  }

  static Map<String, Object?> _financialBody(Response<String> response) {
    final source = response.data;
    if (source == null || source.isEmpty) {
      throw const AppException(AppExceptionKind.server);
    }
    final amountPattern = RegExp(
      r'("(?:amount|advanceAmount|availableAmount|reservedAmount|allocatedAmount|totalReceivedAmount|totalRestoredAmount|totalExpensedAmount|totalTransferredOutAmount|totalReturnedAmount|totalAvailableAmount|totalAmount|subtotalAmount|discountAmount|taxAmount|claimAmount|outstandingAmount|unitPrice|quantity)"\s*:\s*)(-?\d+(?:\.\d+)?)',
    );
    final protected = source.replaceAllMapped(
      amountPattern,
      (match) => '${match.group(1)}"${match.group(2)}"',
    );
    final decoded = jsonDecode(protected);
    if (decoded is! Map) throw const AppException(AppExceptionKind.server);
    return Map<String, Object?>.from(decoded);
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

  static Future<Response<String>> _sendFinancial(
    Future<Response<String>> Function() request,
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
