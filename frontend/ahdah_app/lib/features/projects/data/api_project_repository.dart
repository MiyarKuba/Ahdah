import '../../../core/network/api_client.dart';
import '../domain/project_models.dart';
import '../domain/project_repository.dart';
import '../domain/project_requests.dart';

final class ApiProjectRepository implements ProjectRepository {
  const ApiProjectRepository(this._client);

  final ApiClient _client;

  @override
  Future<ProjectPage> listProjects({
    required int page,
    required int pageSize,
    String? status,
    String? search,
  }) => _client.listProjects(
    page: page,
    pageSize: pageSize,
    status: status,
    search: search,
  );

  @override
  Future<ProjectDetails> getProject(String projectId) =>
      _client.getProject(projectId);

  @override
  Future<ProjectDetails> createProject(ProjectCreateInput input) =>
      _client.createProject(input);

  @override
  Future<ProjectDetails> updateProject(
    String projectId,
    ProjectUpdateInput input,
  ) => _client.updateProject(projectId, input);

  @override
  Future<ProjectDetails> assignSupervisor(
    String projectId,
    SupervisorAssignmentInput input,
  ) => _client.assignProjectSupervisor(projectId, input);

  @override
  Future<ProjectMemberPage> listProjectMembers(
    String projectId, {
    required int page,
    required int pageSize,
  }) => _client.listProjectMembers(projectId, page: page, pageSize: pageSize);
}
