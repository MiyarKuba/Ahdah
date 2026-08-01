# Project status

## Project

Ahdah — عُهدة

## Current phase

Flutter Authentication and Company Onboarding — Implementation complete; Android debug APK build verified

## Date

2026-08-01

## Platforms and packages

Targets are Android, iOS, and Web on Flutter 3.44.7 / Dart 3.12.2.

Direct packages:

- `dio` 5.11.0
- `flutter_riverpod` 2.6.1
- `go_router` 17.3.0
- `flutter_secure_storage` 10.3.1
- `shared_preferences` 2.5.5
- `flutter_localizations` from the Flutter SDK (0.0.0)

No code-generation framework, database, Firebase, analytics, crash reporting, push notifications, or social-login dependency was added. Flutter localization generation uses the SDK-supported ARB/gen-l10n workflow only.

## Implemented client foundation

- Feature-first Flutter structure with focused app, core, authentication, company-registration, join-request, invitation-acceptance, onboarding, session, and authenticated-home areas.
- One Dio client with endpoint constants, configured timeouts, explicit authenticated-call metadata, no body logging, no automatic write retries, and safe ASP.NET Core ProblemDetails/field-error mapping.
- Required `API_BASE_URL` through `String.fromEnvironment`; trailing slashes are normalized, malformed origins fail safely, and non-loopback production values require HTTPS. Actual development API HTTP port: `5231`.
- Android/iOS token persistence through `flutter_secure_storage`; Web token storage is runtime memory only. SharedPreferences stores only the selected language.
- Authoritative Riverpod session bootstrap through `/api/v1/auth/me`: no token becomes unauthenticated, valid server response becomes authenticated, authenticated 401 clears the token, and network failure preserves a potentially valid mobile token with Retry.
- Local logout deletes the access token and in-memory session only. Refresh tokens and server revocation/logout are not implemented because the API exposes neither.
- Stable guarded routes: `/`, `/welcome`, `/login`, `/register-company`, `/join-request`, `/accept-invitation`, `/pending`, `/home`, and `/unavailable`.
- Responsive Material 3 screens: splash, welcome, login, company registration, join request, invitation acceptance, typed pending result, connection retry, and safe authenticated home placeholder.
- Login uses exact `phoneNumber`/`password` contract, generic invalid-credentials UX, duplicate-submit prevention, token storage, and authoritative `/me` verification.
- Company registration uses exact company/manager fields, uppercase alphanumeric code normalization, 12–128 character onboarding password UX validation, conflict-safe errors, and authenticated result handling.
- Join request sends only the exact public contract, never offers `Manager`, keeps requested roles provisional, clears the password, returns a typed pending result, and never authenticates the applicant.
- Invitation acceptance uses manual in-memory token entry only. The token/password are cleared after submission/disposal. Authentication is established only when the API returns it; sensitive roles navigate to identity-verification pending state.
- Home displays only safe `/me` user/company information and localized role/status labels. IDs, tokens, claims, passwords, and raw JSON are not displayed.
- Arabic is the default locale with full RTL; English provides LTR. Language switching is accessible and persists only the non-secret locale preference. Unknown future role/status values use a safe localized fallback.

## Backend CORS

The named `FlutterClient` policy reads `Cors:AllowedOrigins`, runs before authentication/authorization, and allows only configured origins, `GET`/`POST`/`OPTIONS`, and `Authorization`/`Content-Type`/`Accept`. Development explicitly allows `http://localhost:5173` and `http://127.0.0.1:5173`. No `AllowAnyOrigin`, wildcard origin, or credentials policy exists, and production has no permissive origin default.

## Local platform networking

- Android: the main manifest contains Internet permission. Cleartext emulator/LAN access is enabled only by a debug-source-set network security file; release has no global cleartext setting. Emulator API address is `http://10.0.2.2:5231`.
- iOS: the project uses ATS `NSAllowsLocalNetworking`, not `NSAllowsArbitraryLoads`. Simulator address is `http://localhost:5231`; physical devices require the development computer's LAN address or local HTTPS. iOS compilation/signing/simulator verification requires macOS and Xcode and was not performed on Windows.
- Web: development uses fixed port `5173` and `http://localhost:5231`; deployed production origins and API transport must use HTTPS.

## Verification results

- `dotnet restore backend\Ahdah.sln`: passed; all projects up to date.
- `dotnet build backend\Ahdah.sln --no-restore`: passed with 0 warnings and 0 errors.
- `dotnet test backend\Ahdah.sln --no-build --no-restore`: passed; 67 total, 67 passed, 0 failed, 0 skipped (40 unit, 27 integration). The original 65 tests remain passing and two controlled-CORS tests were added.
- `flutter pub get`: passed.
- `dart format --output=none --set-exit-if-changed lib test`: passed; 49 files, 0 changes required.
- `flutter analyze`: passed with no issues.
- `flutter test`: passed; 33 total tests, 33 passed, 0 failed.
- Flutter Web release build with `API_BASE_URL=http://localhost:5231`: passed; output `frontend/ahdah_app/build/web`.
- Android debug APK build with `API_BASE_URL=http://10.0.2.2:5231`: passed. Gradle 9.1.0 downloaded successfully, and the first build installed the required NDK, Android SDK platforms, and CMake. Output: `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`. The app was not run on an emulator or physical device.
- iOS: inspected/configured only; not built or tested on Windows. macOS/Xcode verification remains required.

## Safety and limitations

- No real registration, join submission, invitation acceptance, or other API write endpoint was called.
- PostgreSQL mutation status: none; no database write endpoint or mutation command was executed.
- Database structure status: unchanged; no migration, `EnsureCreated`, `EnsureDeleted`, or `Database.Migrate` call was added or run.
- Generated EF status: unchanged.
- Access tokens expire after approximately 15 minutes and there is no refresh-token endpoint; users must sign in again after expiry.
- Web refresh intentionally loses the in-memory access token.
- SMS/email invitation delivery, external identity verification, manager invitation/join-review UI, authenticated business shell, role-aware navigation, and all financial/business modules remain deferred.
- The Android debug APK has been built but still requires runtime verification on an emulator or physical device. iOS must be built and verified on macOS/Xcode.

## Exact recommended next task

Implement the authenticated Flutter application shell, role-aware navigation, and company member administration using the existing invitation and join-request management APIs without implementing financial workflows yet.
