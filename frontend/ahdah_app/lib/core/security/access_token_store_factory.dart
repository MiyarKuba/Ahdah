import 'access_token_store.dart';
import 'mobile_access_token_store.dart'
    if (dart.library.js_interop) 'web_access_token_store.dart'
    as platform;

AccessTokenStore createAccessTokenStore() =>
    platform.createPlatformTokenStore();
