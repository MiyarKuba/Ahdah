# Project status

## Project

Ahdah — عُهدة

## Current phase

Identity and Company Access — Phase 1

## Date

2026-07-28

## Completed work

- Added `POST /api/v1/auth/register-company`, `POST /api/v1/auth/login`, and authenticated `GET /api/v1/auth/me` controller endpoints.
- Preserved anonymous `GET /api/system/health`.
- Implemented company plus initial-manager creation in one explicit PostgreSQL transaction with preflight uniqueness checks, rollback, and safe `23505` conflict handling.
- Added ASP.NET Core `PasswordHasher<TUser>` behind `IPasswordHashingService`, framework verification, and explicit `SuccessRehashNeeded` persistence.
- Added JWT Bearer authentication with issuer, audience, lifetime, signing-key, signed-token, and 30-second clock-skew validation.
- Added safe claims: `sub`, `company_id`, `role`, `jti`, and `name`.
- Added `ICurrentUserContext` with an HTTP-backed implementation that reads only validated claims.
- Added `AuthenticatedUser`, `CompanyMember`, and `ManagerOnly` authorization policies using exact database role casing.
- Enforced `company_id` plus `user_id` on the authenticated current-user database query and rejected stale tokens for non-active users or companies.
- Added custom non-generated optimistic-concurrency configuration for `app_users.version_number` and `companies.version_number`.
- Added built-in Problem Details exception handling and generic, non-disclosing authentication/conflict responses.
- Added unit and API integration tests with fake identity persistence and in-memory test signing configuration; tests do not connect to the real database.
- Verified development OpenAPI generation and confirmed persistence entities and password hashes are absent from the document.

## Packages added

- `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.10
- `Microsoft.Extensions.Identity.Core` 10.0.10
- `Microsoft.AspNetCore.Mvc.Testing` 10.0.10 for integration tests
- `Microsoft.EntityFrameworkCore` 10.0.10 and `Microsoft.EntityFrameworkCore.Relational` 10.0.10 as direct unit-test references to keep test dependencies version-aligned

## Refresh-token status

Deferred. The existing `user_devices` table has no refresh-token hash, expiration, rotation, or revocation fields. Push-notification tokens were not repurposed. No refresh or server logout endpoint was added; logout is client-side access-token discard.

## Verification results

- `dotnet restore backend\Ahdah.sln`: passed.
- `dotnet build backend\Ahdah.sln`: passed with 0 warnings and 0 errors.
- `dotnet test backend\Ahdah.sln`: passed; 21 total tests, 21 passed, 0 failed, 0 skipped.
- Unit tests: 9 passed.
- Integration tests: 12 passed.
- OpenAPI document test: passed.
- Database mutation status: none. Only one SELECT-only PostgreSQL metadata query was executed; no registration/login endpoint was called against the real database.
- Migration status: none. No migration or database-initialization API exists.

## Tenant-isolation rules

- Authenticated tenant scope comes only from the validated `company_id` claim.
- Current-user lookup filters by both `company_id` and `user_id` and uses `AsNoTracking`.
- Client query, route, body, and custom-header values cannot override authenticated tenant scope.
- Login is phone-only because phone is globally unique; tenant-scoped email is not used to guess a company.

## Known limitations

- The owner-bootstrap flow records the initial active manager with identity status `Verified` because the existing database check forbids an active manager otherwise. Phone and email are not marked verified; a future identity-proofing decision should review this bootstrap semantic.
- Failed-login counters and lockout progression are not changed because no lockout threshold/business rule was approved. Existing future `locked_until` values are honored.
- Refresh-token rotation, revocation, reuse detection, and server logout are unavailable without an approved schema decision.
- The scaffolded `Company.AppUser` navigation is singular because EF inferred cardinality from the partial unique active-manager index, although PostgreSQL permits other non-active/non-manager company users. Phase 1 creates only the one initial manager; future membership work must query users explicitly by tenant rather than rely on that navigation.
- Invitations and join requests are not implemented.
- Flutter authentication UI and API integration are not implemented.
- Existing PostgreSQL expression-index and nullable relationship scaffolding warnings remain unchanged.

## Exact recommended next task

Implement Invitations and Join Requests using the existing `invitations` and `join_requests` tables without changing the database schema.
