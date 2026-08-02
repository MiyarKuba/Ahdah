import 'project_models.dart';
import 'project_requests.dart';

abstract interface class ProjectRepository {
  Future<ProjectPage> listProjects({
    required int page,
    required int pageSize,
    String? status,
    String? search,
  });

  Future<ProjectDetails> getProject(String projectId);

  Future<ProjectDetails> createProject(ProjectCreateInput input);

  Future<ProjectDetails> updateProject(
    String projectId,
    ProjectUpdateInput input,
  );

  Future<ProjectDetails> assignSupervisor(
    String projectId,
    SupervisorAssignmentInput input,
  );

  Future<ProjectMemberPage> listProjectMembers(
    String projectId, {
    required int page,
    required int pageSize,
  });
}
