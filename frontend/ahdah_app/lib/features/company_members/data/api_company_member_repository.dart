import '../../../core/network/api_client.dart';
import '../domain/company_member_models.dart';
import '../domain/company_member_repository.dart';

final class ApiCompanyMemberRepository implements CompanyMemberRepository {
  const ApiCompanyMemberRepository(this._client);

  final ApiClient _client;

  @override
  Future<CompanyMemberPage> listMembers({
    required int page,
    required int pageSize,
    String? role,
    String? status,
    String? search,
  }) => _client.listCompanyMembers(
    page: page,
    pageSize: pageSize,
    role: role,
    status: status,
    search: search,
  );

  @override
  Future<CompanyMemberDetails> getMember(String memberId) =>
      _client.getCompanyMember(memberId);
}
