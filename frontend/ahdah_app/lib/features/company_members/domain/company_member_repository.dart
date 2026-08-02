import 'company_member_models.dart';

abstract interface class CompanyMemberRepository {
  Future<CompanyMemberPage> listMembers({
    required int page,
    required int pageSize,
    String? role,
    String? status,
    String? search,
  });

  Future<CompanyMemberDetails> getMember(String memberId);
}
