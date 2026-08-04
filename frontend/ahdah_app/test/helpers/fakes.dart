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
import 'package:ahdah_app/features/advances/domain/advance_models.dart';
import 'package:ahdah_app/features/advances/domain/advance_repository.dart';
import 'package:ahdah_app/features/advances/domain/advance_requests.dart';
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

final testAdvanceSummary = AdvanceSummary(
  advanceId: 'advance-1',
  advanceNumber: 'ADV-TEST',
  advanceAmount: '100.00',
  availableAmount: '80.00',
  reservedAmount: '20.00',
  currencyCode: 'LYD',
  issueDate: DateTime(2026, 8, 4),
  purpose: 'Site custody',
  status: 'Open',
  recipient: const AdvanceUserSummary(
    userId: 'recipient-1',
    fullName: 'Recipient',
    role: 'Deputy',
  ),
  versionNumber: 2,
  createdAtUtc: DateTime.utc(2026, 8, 4),
);

final testAdvanceBalance = AdvanceBalanceSummary(
  advanceId: 'advance-1',
  advanceNumber: 'ADV-TEST',
  holder: testAdvanceSummary.recipient,
  totalReceivedAmount: '100.00',
  totalRestoredAmount: '0.00',
  totalExpensedAmount: '0.00',
  totalTransferredOutAmount: '0.00',
  totalReturnedAmount: '0.00',
  availableAmount: '80.00',
  reservedAmount: '20.00',
  currencyCode: 'LYD',
  status: 'Active',
  versionNumber: 2,
  updatedAtUtc: DateTime.utc(2026, 8, 4),
);

final testAdvanceDetails = AdvanceDetails(
  advance: testAdvanceSummary,
  fundings: const [],
  balances: [testAdvanceBalance],
);

final testAdvanceTransfer = AdvanceTransferDetails(
  transferId: 'transfer-1',
  transferNumber: 'TRF-TEST',
  transferType: 'InternalTransfer',
  advanceId: 'advance-1',
  amount: '10.00',
  currencyCode: 'LYD',
  sender: const AdvanceUserSummary(
    userId: 'sender-1',
    fullName: 'Sender',
    role: 'Deputy',
  ),
  recipient: testAdvanceSummary.recipient,
  transferMethod: 'Cash',
  status: 'PendingConfirmation',
  versionNumber: 1,
  createdAtUtc: DateTime.utc(2026, 8, 4),
);

final class FakeAdvanceRepository implements AdvanceRepository {
  AdvancePage advancePage = const AdvancePage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  AdvanceMovementPage movementPage = const AdvanceMovementPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  FundingSourcePage fundingPage = const FundingSourcePage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  AdvanceBalancePage balancePage = const AdvanceBalancePage(
    page: AdvanceBalanceResultPage(
      items: [],
      page: 1,
      pageSize: 20,
      totalCount: 0,
      totalPages: 0,
    ),
  );
  AdvanceDetails details = testAdvanceDetails;
  AdvanceTransferDetails transfer = testAdvanceTransfer;
  AppException? readError;
  AppException? writeError;
  Completer<AdvanceDetails>? createCompleter;
  int listCalls = 0;
  int createCalls = 0;
  int distributionCalls = 0;
  int returnCalls = 0;
  int confirmationCalls = 0;
  int rejectionCalls = 0;
  int? lastPage;
  int? lastPageSize;
  String? lastStatus;
  String? lastUserId;
  String? lastReference;
  String? lastIdempotencyKey;
  CreateAdvanceInput? lastCreateInput;
  CreateAdvanceDistributionInput? lastDistributionInput;
  CreateAdvanceReturnInput? lastReturnInput;
  RejectAdvanceTransferInput? lastRejectionInput;

  @override
  Future<AdvancePage> listAdvances({
    required int page,
    required int pageSize,
    String? status,
    String? userId,
    String? reference,
  }) async {
    listCalls++;
    lastPage = page;
    lastPageSize = pageSize;
    lastStatus = status;
    lastUserId = userId;
    lastReference = reference;
    if (readError != null) throw readError!;
    return advancePage;
  }

  @override
  Future<AdvanceDetails> getAdvance(String advanceId) async {
    if (readError != null) throw readError!;
    return details;
  }

  @override
  Future<AdvanceMovementPage> listMovements(
    String advanceId, {
    required int page,
    required int pageSize,
  }) async {
    lastPage = page;
    lastPageSize = pageSize;
    if (readError != null) throw readError!;
    return movementPage;
  }

  @override
  Future<FundingSourcePage> listFundingSources({
    required int page,
    required int pageSize,
  }) async {
    lastPage = page;
    lastPageSize = pageSize;
    if (readError != null) throw readError!;
    return fundingPage;
  }

  @override
  Future<AdvanceBalancePage> getMyBalances({
    required int page,
    required int pageSize,
  }) async {
    lastPage = page;
    lastPageSize = pageSize;
    if (readError != null) throw readError!;
    return balancePage;
  }

  @override
  Future<AdvanceBalancePage> getUserBalances(
    String userId, {
    required int page,
    required int pageSize,
  }) async {
    lastUserId = userId;
    return getMyBalances(page: page, pageSize: pageSize);
  }

  @override
  Future<AdvanceDetails> createAdvance(
    CreateAdvanceInput input,
    String idempotencyKey,
  ) async {
    createCalls++;
    lastCreateInput = input;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return createCompleter?.future ?? details;
  }

  @override
  Future<AdvanceTransferDetails> distribute(
    String advanceId,
    CreateAdvanceDistributionInput input,
    String idempotencyKey,
  ) async {
    distributionCalls++;
    lastDistributionInput = input;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return transfer;
  }

  @override
  Future<AdvanceTransferDetails> returnMoney(
    String advanceId,
    CreateAdvanceReturnInput input,
    String idempotencyKey,
  ) async {
    returnCalls++;
    lastReturnInput = input;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return transfer;
  }

  @override
  Future<AdvanceTransferDetails> confirmTransfer(
    String transferId,
    String idempotencyKey,
  ) async {
    confirmationCalls++;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return transfer;
  }

  @override
  Future<AdvanceTransferDetails> rejectTransfer(
    String transferId,
    RejectAdvanceTransferInput input,
    String idempotencyKey,
  ) async {
    rejectionCalls++;
    lastRejectionInput = input;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return transfer;
  }
}
