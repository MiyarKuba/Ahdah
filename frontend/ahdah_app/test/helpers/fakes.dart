import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';

import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/core/security/access_token_store.dart';
import 'package:ahdah_app/features/authentication/data/auth_repository.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/authentication/domain/identity_requests.dart';
import 'package:ahdah_app/features/access/domain/access_models.dart';
import 'package:ahdah_app/features/access/domain/access_repository.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_models.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_repository.dart';
import 'package:ahdah_app/features/projects/domain/project_models.dart';
import 'package:ahdah_app/features/projects/domain/project_repository.dart';
import 'package:ahdah_app/features/projects/domain/project_requests.dart';
import 'package:dio/dio.dart';

const testUser = AuthenticatedUser(
  userId: '2e77a184-b59e-4e99-a276-37582a14b669',
  fullName: 'Test User',
  role: 'Worker',
  status: 'Active',
  identityVerificationStatus: 'NotRequired',
);
const testCompany = CompanySummary(
  companyId: 'f29b18bb-9954-46c5-ad07-bd48db68bb55',
  companyName: 'Test Company',
  companyCode: 'TEST01',
  status: 'Active',
);
const testCurrentSession = CurrentSessionResult(
  user: testUser,
  company: testCompany,
  role: 'Worker',
);
final testAuthentication = AuthenticationResult(
  accessToken: 'test-token-never-logged',
  tokenType: 'Bearer',
  accessTokenExpiresAtUtc: DateTime.utc(2026, 7, 28, 12, 15),
  user: testUser,
  company: testCompany,
  role: 'Worker',
);

const testManager = AuthenticatedUser(
  userId: 'a2c125f6-7cb1-4c41-a99a-7bcc847ba533',
  fullName: 'Test Manager',
  role: 'Manager',
  status: 'Active',
  identityVerificationStatus: 'Verified',
);
const testManagerSession = CurrentSessionResult(
  user: testManager,
  company: testCompany,
  role: 'Manager',
);

final class FakeAccessTokenStore implements AccessTokenStore {
  FakeAccessTokenStore([this.token]);
  String? token;
  int clearCalls = 0;

  @override
  Future<String?> read() async => token;
  @override
  Future<void> write(String token) async => this.token = token;
  @override
  Future<void> clear() async {
    clearCalls++;
    token = null;
  }
}

final class FakeAuthRepository implements AuthRepository {
  CurrentSessionResult current = testCurrentSession;
  AppException? currentError;
  Completer<CurrentSessionResult>? currentCompleter;
  Completer<AuthenticationResult>? loginCompleter;
  int loginCalls = 0;
  int currentCalls = 0;
  InvitationAcceptanceResult invitationResult =
      const InvitationAcceptanceResult(
        outcome: 'AcceptedPendingIdentityVerification',
        userStatus: 'PendingApproval',
        assignedRole: 'Accountant',
      );

  @override
  Future<AuthenticationResult> login(LoginRequest request) {
    loginCalls++;
    return loginCompleter?.future ?? Future.value(testAuthentication);
  }

  @override
  Future<RegisterCompanyResult> registerCompany(
    RegisterCompanyRequest request,
  ) async => RegisterCompanyResult(
    company: testCompany,
    manager: testUser,
    authentication: testAuthentication,
  );

  @override
  Future<CurrentSessionResult> currentSession() async {
    currentCalls++;
    if (currentError != null) throw currentError!;
    return currentCompleter?.future ?? current;
  }

  @override
  Future<InvitationAcceptanceResult> acceptInvitation(
    AcceptInvitationRequest request,
  ) async => invitationResult;

  @override
  Future<JoinRequestAcknowledgement> submitJoinRequest(
    SubmitJoinRequest request,
  ) async => const JoinRequestAcknowledgement(status: 'Pending');

  @override
  Future<bool> health() async => true;
}

final class RecordingAdapter implements HttpClientAdapter {
  RecordingAdapter({this.statusCode = 200, this.body = const {}});
  final int statusCode;
  final Map<String, Object?> body;
  RequestOptions? lastRequest;

  @override
  Future<ResponseBody> fetch(
    RequestOptions options,
    Stream<Uint8List>? requestStream,
    Future<void>? cancelFuture,
  ) async {
    lastRequest = options;
    return ResponseBody.fromString(
      jsonEncode(body),
      statusCode,
      headers: {
        Headers.contentTypeHeader: [Headers.jsonContentType],
      },
    );
  }

  @override
  void close({bool force = false}) {}
}

final class FakeAccessRepository implements AccessRepository {
  AccessPage<Invitation> invitationPage = const AccessPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  AccessPage<JoinRequest> joinRequestPage = const AccessPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  AppException? invitationListError;
  AppException? joinListError;
  AppException? writeError;
  Completer<CreatedInvitation>? createCompleter;
  int invitationListCalls = 0;
  int joinListCalls = 0;
  int createCalls = 0;
  int cancelCalls = 0;
  int approveCalls = 0;
  int rejectCalls = 0;
  int? lastPage;
  int? lastPageSize;
  String? lastFilter;
  CreateInvitationInput? lastCreateInput;
  ApproveJoinRequestInput? lastApprovalInput;
  RejectJoinRequestInput? lastRejectionInput;

  @override
  Future<AccessPage<Invitation>> listInvitations({
    required int page,
    required int pageSize,
    String? status,
  }) async {
    invitationListCalls++;
    lastPage = page;
    lastPageSize = pageSize;
    lastFilter = status;
    if (invitationListError != null) throw invitationListError!;
    return invitationPage;
  }

  @override
  Future<AccessPage<JoinRequest>> listJoinRequests({
    required int page,
    required int pageSize,
    String? status,
  }) async {
    joinListCalls++;
    lastPage = page;
    lastPageSize = pageSize;
    lastFilter = status;
    if (joinListError != null) throw joinListError!;
    return joinRequestPage;
  }

  @override
  Future<CreatedInvitation> createInvitation(
    CreateInvitationInput input,
  ) async {
    createCalls++;
    lastCreateInput = input;
    if (writeError != null) throw writeError!;
    return createCompleter?.future ??
        CreatedInvitation(
          invitation: Invitation(
            id: 'invitation-1',
            invitedPhoneNumber: input.phoneNumber,
            assignedRole: input.assignedRole,
            status: 'Pending',
            expiresAtUtc: DateTime.utc(2026, 8, 8),
          ),
          token: 'one-time-token-never-persisted',
        );
  }

  @override
  Future<Invitation> cancelInvitation(String invitationId) async {
    cancelCalls++;
    if (writeError != null) throw writeError!;
    return Invitation(
      id: invitationId,
      invitedPhoneNumber: '+218912345678',
      assignedRole: 'Worker',
      status: 'Cancelled',
    );
  }

  @override
  Future<JoinRequestDecision> approveJoinRequest(
    String joinRequestId,
    ApproveJoinRequestInput input,
  ) async {
    approveCalls++;
    lastApprovalInput = input;
    if (writeError != null) throw writeError!;
    return JoinRequestDecision(
      outcome: input.assignedRole == 'Accountant'
          ? 'ApprovedPendingIdentityVerification'
          : 'ApprovedAndActivated',
      userStatus: input.assignedRole == 'Accountant'
          ? 'PendingApproval'
          : 'Active',
      request: JoinRequest(
        id: joinRequestId,
        fullName: 'Applicant',
        phoneNumber: '+218912345679',
        requestedRole: 'Worker',
        assignedRole: input.assignedRole,
        status: 'Approved',
      ),
    );
  }

  @override
  Future<JoinRequestDecision> rejectJoinRequest(
    String joinRequestId,
    RejectJoinRequestInput input,
  ) async {
    rejectCalls++;
    lastRejectionInput = input;
    if (writeError != null) throw writeError!;
    return JoinRequestDecision(
      outcome: 'Rejected',
      userStatus: 'Rejected',
      request: JoinRequest(
        id: joinRequestId,
        fullName: 'Applicant',
        phoneNumber: '+218912345679',
        requestedRole: 'Worker',
        status: 'Rejected',
        rejectionReason: input.reason,
      ),
    );
  }
}

final class FakeProjectRepository implements ProjectRepository {
  ProjectPage projectPage = const ProjectPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  ProjectMemberPage memberPage = const ProjectMemberPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  ProjectDetails? project;
  AppException? listError;
  AppException? detailError;
  AppException? writeError;
  Completer<ProjectPage>? listCompleter;
  Completer<ProjectDetails>? createCompleter;
  int listCalls = 0;
  int detailCalls = 0;
  int createCalls = 0;
  int updateCalls = 0;
  int assignCalls = 0;
  int memberCalls = 0;
  int? lastPage;
  int? lastPageSize;
  String? lastStatus;
  String? lastSearch;
  ProjectCreateInput? lastCreateInput;
  ProjectUpdateInput? lastUpdateInput;
  SupervisorAssignmentInput? lastAssignmentInput;

  @override
  Future<ProjectPage> listProjects({
    required int page,
    required int pageSize,
    String? status,
    String? search,
  }) async {
    listCalls++;
    lastPage = page;
    lastPageSize = pageSize;
    lastStatus = status;
    lastSearch = search;
    if (listError != null) throw listError!;
    return listCompleter?.future ?? projectPage;
  }

  @override
  Future<ProjectDetails> getProject(String projectId) async {
    detailCalls++;
    if (detailError != null) throw detailError!;
    return project ?? testProject;
  }

  @override
  Future<ProjectDetails> createProject(ProjectCreateInput input) async {
    createCalls++;
    lastCreateInput = input;
    if (writeError != null) throw writeError!;
    return createCompleter?.future ?? project ?? testProject;
  }

  @override
  Future<ProjectDetails> updateProject(
    String projectId,
    ProjectUpdateInput input,
  ) async {
    updateCalls++;
    lastUpdateInput = input;
    if (writeError != null) throw writeError!;
    return project ?? testProject;
  }

  @override
  Future<ProjectDetails> assignSupervisor(
    String projectId,
    SupervisorAssignmentInput input,
  ) async {
    assignCalls++;
    lastAssignmentInput = input;
    if (writeError != null) throw writeError!;
    return project ?? testProject;
  }

  @override
  Future<ProjectMemberPage> listProjectMembers(
    String projectId, {
    required int page,
    required int pageSize,
  }) async {
    memberCalls++;
    lastPage = page;
    lastPageSize = pageSize;
    if (listError != null) throw listError!;
    return memberPage;
  }
}

final class FakeCompanyMemberRepository implements CompanyMemberRepository {
  CompanyMemberPage memberPage = const CompanyMemberPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  CompanyMemberDetails? member;
  AppException? listError;
  AppException? detailError;
  Completer<CompanyMemberPage>? listCompleter;
  int listCalls = 0;
  int detailCalls = 0;
  int? lastPage;
  int? lastPageSize;
  String? lastRole;
  String? lastStatus;
  String? lastSearch;

  @override
  Future<CompanyMemberPage> listMembers({
    required int page,
    required int pageSize,
    String? role,
    String? status,
    String? search,
  }) async {
    listCalls++;
    lastPage = page;
    lastPageSize = pageSize;
    lastRole = role;
    lastStatus = status;
    lastSearch = search;
    if (listError != null) throw listError!;
    return listCompleter?.future ?? memberPage;
  }

  @override
  Future<CompanyMemberDetails> getMember(String memberId) async {
    detailCalls++;
    if (detailError != null) throw detailError!;
    return member ?? testMemberDetails;
  }
}

final testProject = ProjectDetails(
  id: 'project-1',
  projectName: 'Project One',
  owner: const ProjectOwnerSummary(
    id: 'owner-1',
    ownerName: 'Owner One',
    phoneNumber: '+218912345678',
  ),
  siteAddress: 'Tripoli',
  contractValue: '1250.25',
  contractDate: DateTime(2026, 8, 1),
  startDate: DateTime(2026, 8, 2),
  status: 'Active',
  assignedSupervisors: const [],
  versionNumber: 3,
  createdAtUtc: DateTime.utc(2026, 8, 1),
  updatedAtUtc: DateTime.utc(2026, 8, 1),
);

final testMemberDetails = CompanyMemberDetails(
  id: 'member-1',
  fullName: 'Supervisor One',
  role: 'Supervisor',
  status: 'Active',
  identityVerificationStatus: 'NotRequired',
  phoneNumber: '+218912345679',
  createdAtUtc: DateTime.utc(2026, 8, 1),
  updatedAtUtc: DateTime.utc(2026, 8, 1),
);
