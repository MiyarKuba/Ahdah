# Project status

## Project

Ahdah — عُهدة

## Current phase

Invitations and Join Requests — Complete

## Date

2026-07-28

## Completed endpoints

- Manager-only `POST /api/v1/invitations`
- Manager-only `GET /api/v1/invitations`
- Public `POST /api/v1/invitations/accept`
- Manager-only `POST /api/v1/invitations/{invitationId}/cancel`
- Public `POST /api/v1/join-requests`
- Manager-only `GET /api/v1/join-requests`
- Manager-only `POST /api/v1/join-requests/{joinRequestId}/approve`
- Manager-only `POST /api/v1/join-requests/{joinRequestId}/reject`

Existing authentication and health endpoints remain unchanged.

## Invitation behavior

- `Access:Invitations:LifetimeHours` is strongly typed, bound at startup, validated from 1 through 720 hours, and set to 168 hours (7 days) in development.
- Creation revalidates the active same-company manager, rejects registered phones and conflicting pending invitations, generates 32 random bytes, stores only a SHA-256 hash, and returns the URL-safe raw token once.
- Creation never creates a user. SMS/email delivery remains out of band.
- Acceptance locks the invitation and company and creates the user plus acceptance transition atomically.
- `Supervisor`/`Worker` acceptance creates an `Active` user with identity `NotRequired` and returns authentication.
- `Deputy`/`Accountant` acceptance creates a `PendingApproval` user with identity `Pending` and no access token.
- Invalid, expired, cancelled, and already-used tokens share a generic response.

## Join-request behavior

- Public submission normalizes company code, resolves only an active company, hashes the password immediately, and atomically creates a `PendingApproval` `app_user` plus linked `Pending` request.
- The password hash is stored only on `app_users`; `join_requests` contains no password data.
- Requested role is provisional and limited to `Deputy`, `Accountant`, `Supervisor`, or `Worker`.
- Initial identity is `Pending` for `Deputy`/`Accountant` and `NotRequired` for `Supervisor`/`Worker`.
- Approval locks both records, requires a pending request and pending user, stores the manager-selected role, and updates the existing linked user rather than creating another.
- Approval produces `ApprovedAndActivated` for `Supervisor`/`Worker` and legitimately verified sensitive users; otherwise it produces `ApprovedPendingIdentityVerification`.
- Rejection updates the request and linked user to `Rejected` atomically without deleting the user or erasing its password hash.

## Security, tenancy, and concurrency

- `Manager` is rejected from all invitation and join-request role inputs.
- Manager tenant scope comes only from `ICurrentUserContext`; active manager/company status is revalidated and every tenant resource query includes `company_id`.
- Invitation acceptance takes company, phone, and role only from the invitation.
- Unique violation `23505` maps to generic HTTP 409 responses without database detail.
- Invitations and join requests have no `version_number`; explicit transactions and PostgreSQL row locks serialize lifecycle transitions. Linked `app_users.version_number` remains an optimistic-concurrency token and is advanced on approval/rejection.
- No real write endpoint is used during automated verification.

## Schema limitations

- Phone is globally unique, so one phone can currently belong to only one account/tenant membership.
- Reapplication after rejection and company transfer require a future approved account-recovery/reapplication workflow.
- Invitation cancellation has no cancellation-actor column.
- SMS/email delivery and Flutter UI are not implemented.

## Packages added

None. Existing framework, options, cryptography, identity, EF Core, and Npgsql capabilities are reused.

## Verification results

- `dotnet restore backend\Ahdah.sln`: passed; all projects were up to date.
- `dotnet build backend\Ahdah.sln`: passed with 0 warnings and 0 errors.
- `dotnet test backend\Ahdah.sln`: passed; 65 total tests, 65 passed, 0 failed, 0 skipped (40 unit, 25 integration).
- Existing 43 tests remain passing.
- Development OpenAPI verification passed; generated persistence entities, invitation-token hashes, and password hashes are absent.
- PostgreSQL mutation status: none. No real write endpoint was invoked.
- Generated EF status: unchanged.
- Migration status: none.

## Exact recommended next task

Implement Flutter authentication and company onboarding for Android, iOS, and Web using the completed Identity, Invitation, and Join Request APIs.
