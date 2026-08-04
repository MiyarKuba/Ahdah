import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../core/config/app_config.dart';
import '../core/network/api_client.dart';
import '../core/network/auth_interceptor.dart';
import '../core/security/access_token_store.dart';
import '../core/security/access_token_store_factory.dart';
import '../features/authentication/data/auth_repository.dart';
import '../features/access/data/api_access_repository.dart';
import '../features/access/domain/access_repository.dart';
import '../features/company_members/data/api_company_member_repository.dart';
import '../features/company_members/domain/company_member_repository.dart';
import '../features/projects/data/api_project_repository.dart';
import '../features/projects/domain/project_repository.dart';
import '../features/advances/data/api_advance_repository.dart';
import '../features/advances/domain/advance_repository.dart';
import '../features/advances/domain/financial_operation_key.dart';

final appConfigProvider = Provider<AppConfig>(
  (ref) => throw StateError('AppConfig must be supplied during bootstrap.'),
);

final sharedPreferencesProvider = Provider<SharedPreferences>(
  (ref) =>
      throw StateError('SharedPreferences must be supplied during bootstrap.'),
);

final accessTokenStoreProvider = Provider<AccessTokenStore>(
  (ref) => createAccessTokenStore(),
);

final dioProvider = Provider<Dio>((ref) {
  final config = ref.watch(appConfigProvider);
  final dio = Dio(
    BaseOptions(
      baseUrl: config.apiBaseUrl,
      contentType: Headers.jsonContentType,
      responseType: ResponseType.json,
      connectTimeout: const Duration(seconds: 15),
      sendTimeout: const Duration(seconds: 20),
      receiveTimeout: const Duration(seconds: 20),
      headers: const {'Accept': Headers.jsonContentType},
    ),
  );
  dio.interceptors.add(AuthInterceptor(ref.watch(accessTokenStoreProvider)));
  return dio;
});

final apiClientProvider = Provider<ApiClient>(
  (ref) => ApiClient(ref.watch(dioProvider)),
);

final authRepositoryProvider = Provider<AuthRepository>(
  (ref) => ApiAuthRepository(ref.watch(apiClientProvider)),
);

final accessRepositoryProvider = Provider<AccessRepository>(
  (ref) => ApiAccessRepository(ref.watch(apiClientProvider)),
);

final projectRepositoryProvider = Provider<ProjectRepository>(
  (ref) => ApiProjectRepository(ref.watch(apiClientProvider)),
);

final companyMemberRepositoryProvider = Provider<CompanyMemberRepository>(
  (ref) => ApiCompanyMemberRepository(ref.watch(apiClientProvider)),
);

final advanceRepositoryProvider = Provider<AdvanceRepository>(
  (ref) => ApiAdvanceRepository(ref.watch(apiClientProvider)),
);

final financialOperationKeyFactoryProvider =
    Provider<FinancialOperationKeyFactory>(
      (ref) => SecureFinancialOperationKeyFactory(),
    );
