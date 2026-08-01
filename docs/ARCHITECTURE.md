# Architecture

## System shape

Ahdah consists of a Flutter client, an ASP.NET Core Web API, and an existing PostgreSQL database. The Flutter application communicates with the API over HTTPS using JSON REST endpoints. Only the API and approved server-side processes may communicate with PostgreSQL.

```text
Flutter (Android / iOS / Web)
              |
         HTTPS + JSON
              |
ASP.NET Core controller-based API
              |
   Modular Monolith internals
              |
PostgreSQL 18 / ahdah_db / ahdah
```

The backend has Database-First persistence integration, Identity and Company Access Phase 1, and the schema-supported Invitations and Join Requests phase. Other business modules remain unimplemented.

## Flutter client

Flutter provides one shared product experience across Android, iOS, and Web. Presentation, navigation, client-side input feedback, and API transport belong in the client. Platform-specific behavior must be isolated behind a shared interface and must have an iOS-compatible path whenever Android behavior is introduced.

The client is not a trusted enforcement boundary. Authorization, tenant isolation, financial invariants, and audit behavior must be enforced by the API even if the client also provides contextual validation.

The authentication foundation uses feature-first presentation/data/domain boundaries, Riverpod as the authoritative session and locale state mechanism, `go_router` for guarded routes, and one Dio API client. Request/response models are explicit handwritten mappings of the published camelCase API contracts; persistence entities and decoded JWT claims are not client domain models.

`API_BASE_URL` is required through `String.fromEnvironment` and normalized by `AppConfig`. Android/iOS access tokens use platform secure storage. Web access tokens exist only in runtime memory and intentionally disappear on refresh. Startup reads the platform token store and calls `/api/v1/auth/me`; a 401 clears the local token, while a network failure preserves a potentially valid mobile token and shows a retry state. Logout is local token deletion because no refresh or revocation endpoint exists.

Arabic is the default locale, English is optional, and only the non-secret locale preference is persisted. Pages share responsive, keyboard-safe Material 3 layouts across mobile, tablet, and Web rather than duplicating platform screens. See [FLUTTER_DEVELOPMENT.md](FLUTTER_DEVELOPMENT.md) for the complete client structure and platform networking policy.

Authenticated features are hosted by one nested `go_router` shell. Compact layouts use a Material 3 `NavigationBar`; widths of 840 logical pixels and above use a persistent `NavigationRail`, while the feature pages remain shared. Stable URLs are `/home`, `/access/invitations`, `/access/join-requests`, and `/account`. `RoleCapabilities` derives manager access only from the authoritative `/auth/me` role. Navigation hiding and router redirects are both applied, but server authorization remains authoritative. Unknown roles receive only Home and Account.

Access administration has handwritten domain models, an `AccessRepository` over the existing Dio client and bearer interceptor, focused Riverpod list/write controllers, and shared status/error/empty/confirmation widgets. Lists use the backend page-number contract and exact status values. Write controllers prevent duplicate submission and never retry automatically. A 401 invokes centralized session expiry; network failures keep the potentially valid mobile session and remain retryable.

The one-time invitation creation token is returned directly to dialog-local state. It never enters list models, provider state, routes, preferences, secure storage, browser storage, diagnostics, or logs. Closing the result drops the reference and refreshes the non-secret list. SMS/email delivery is outside the client.

## ASP.NET Core Web API

The .NET 10 API is controller based and owns the public HTTP contract. It is responsible for authentication and authorization when introduced, tenant context resolution, input validation, application orchestration, transaction boundaries, persistence access, audit production, and safe integration with external services.

The initial project dependency direction is:

```text
Ahdah.Api            -> Ahdah.Application, Ahdah.Infrastructure
Ahdah.Infrastructure -> Ahdah.Application, Ahdah.Domain
Ahdah.Application    -> Ahdah.Domain
Ahdah.Domain         -> no other Ahdah project
```

Dependencies must continue to point inward toward business policy. Infrastructure details must not leak into the domain.

## Identity and company access

Application defines focused password-hashing, token-issuance, identity-service, and current-user contracts. Infrastructure implements password hashing and the EF Core identity workflow. API owns JWT Bearer middleware, HTTP claim parsing, authorization policies, controllers, and safe Problem Details responses. Generated EF entities never cross the API boundary.

Company registration normalizes supported text inputs and creates one company plus its initial manager in one explicit PostgreSQL transaction. It relies on database-generated UUIDs, timestamps, and `version_number` defaults. Preflight conflict checks are backed by handling PostgreSQL `23505` uniqueness races as a non-disclosing HTTP 409.

Passwords use ASP.NET Core `PasswordHasher<TUser>`. Verification uses the framework API, and `SuccessRehashNeeded` causes a tracked, concurrency-aware hash replacement together with `updated_at` and `version_number` advancement.

Access tokens are signed JWTs validated for issuer, audience, lifetime, signature, and signing key with a small clock skew. Claims are limited to `sub`, `company_id`, `role`, `jti`, and `name`. Signing key material is supplied only through secret configuration.

The safe user summary returned by authentication and `/auth/me` includes `identityVerificationStatus` so the client can present authoritative account state without decoding claims or accessing persistence models.

The HTTP-backed current-user context accepts tenant identity only from validated JWT claims. The authenticated current-user lookup filters `app_users` by both `company_id` and `user_id`, uses a read-only query, and requires both user and company status to be `Active`. Policies are `AuthenticatedUser`, `CompanyMember`, and `ManagerOnly`; `ManagerOnly` uses the exact stored role `Manager`.

Refresh tokens are deferred. `user_devices` has device identity, platform, trust, activity, timestamps, and a push token, but no cryptographic refresh-token hash, expiry, rotation, or revocation semantics. The push token must never be repurposed. Logout is therefore client-side access-token discard, and no misleading server logout endpoint exists.

## Invitations and join requests

Application defines explicit access contracts, paged response models, role rules, invitation-token security, and focused invitation/join-request service interfaces. Infrastructure implements the EF Core workflows and cryptographic token service; API controllers own authorization and Problem Details mapping. Persistence entities and secret hashes never cross the API boundary.

Manager operations derive `company_id` and user ID only from `ICurrentUserContext`, revalidate an active same-company manager, and include `company_id` in every tenant-owned query. Invitation cancellation and join-request decisions use explicit transactions and PostgreSQL `FOR UPDATE` row locks because neither table has `version_number`. Invitation acceptance also locks the invitation, locks its active company for the transaction, creates the user, and marks the invitation accepted atomically.

Invitation creation uses validated `InvitationOptions` bound from `Access:Invitations:LifetimeHours`; development configures 168 hours. The token service generates 32 random bytes, returns a URL-safe token once, and persists only its SHA-256 hash. The lifetime remains application configuration and is not stored separately in PostgreSQL.

Invitation acceptance uses the invitation's phone, company, and assigned role. The caller supplies only the one-time token, full name, password, and optional email. `Supervisor`/`Worker` become `Active` with identity `NotRequired` and receive authentication. `Deputy`/`Accountant` remain `PendingApproval` with identity `Pending` and receive no token.

Public join submission resolves an active company by normalized code and creates the pending user plus pending request in one transaction. The password is hashed immediately and stored only on `app_users`. Approval locks and updates both records: non-sensitive roles activate, while sensitive roles require verified identity. Rejection marks the request and linked user rejected without deletion. SMS/email delivery, reapplication/account recovery, and Flutter UI are outside this phase.

## Company structure and projects

Application owns explicit company-member/project requests, response models, visibility capabilities, lifecycle validation, and service interfaces. Infrastructure owns tenant-filtered EF queries and the generated `app_users`, `projects`, `project_owners`, and `project_supervisors` mappings. API owns the company-directory and project controllers, focused authorization policies, HTTP statuses, and safe Problem Details. Generated persistence entities never cross these boundaries.

`CompanyDirectoryViewer` permits only Manager and Deputy because Accountant directory access is not approved by the existing business documentation. `ProjectViewer` admits the five verified roles to the controller, but record-level filtering remains authoritative: Manager, Deputy, and Accountant see tenant projects; Supervisor queries require an active same-project supervisor assignment; Worker queries return no records because no worker/project assignment exists.

Project reads are projected into explicit DTOs. Contract value is selected only for Manager and is JSON-omitted when unavailable. Creation derives company and creator from `ICurrentUserContext`, uses the required schema owner relationship, and atomically creates a new owner/project/optional supervisor assignment or verifies an existing same-company active owner. Supervisor replacement locks the tenant project, ends prior active assignment rows without deletion, creates a new row when needed, and advances project optimistic concurrency.

Only `Active` and `Paused` are mutable lifecycle values in this phase. `Completed`, `FinanciallyClosed`, and `Cancelled` require additional fields and/or deferred financial/business checks. Direct contract-value changes are also deferred because `project_contract_changes` provides a distinct reason/review/history workflow. No financial module is implemented by the company-structure services.

## Modular Monolith

A modular monolith provides one deployable backend while keeping high-cohesion business areas explicit. Modules will share process hosting and operational tooling but should communicate through defined application contracts rather than reaching into one another's internals.

Candidate future module boundaries are:

- Companies and tenant administration
- Users, roles, and access control
- Projects and construction operations
- Funding sources and custody advances
- Transfers and custody balances
- Expenses, receipts, and reimbursements
- Suppliers, debts, and payments
- Worker claims
- Advance settlement and closure
- Documents and attachments
- Notifications
- Audit
- Background jobs, scheduled reports, generation, and delivery

These boundaries require validation against the existing schema and approved business rules before implementation. They are not permission to restructure the database.

## PostgreSQL and database-first integration

The existing PostgreSQL 18 `ahdah_db` database, `ahdah` schema, and 91 mapped base tables are the source of truth. EF Core and Npgsql use a controlled Database-First workflow. Generated persistence models are isolated in `Ahdah.Infrastructure/Persistence/Generated`; they do not define the domain model. The connection string is supplied through configuration, using User Secrets locally and environment-based secret configuration in production. The application must not call `EnsureCreated`, apply migrations, or make destructive schema changes.

Financial values use PostgreSQL `NUMERIC` and C# `decimal`. Financial ledger changes require explicit transactions and auditable behavior.

## Multi-tenancy

Tenant ownership is based on `company_id`. Every tenant-owned query and command must scope and verify `company_id` at the server boundary and persistence layer. An identifier supplied by a client is never sufficient proof that the active tenant owns a record.

Cross-tenant access must be denied by default. Future background jobs and reports must carry an explicit tenant context and apply the same rules as interactive requests.

Unauthenticated phone login is unambiguous because the database enforces global uniqueness on `app_users.phone_number`. Email login is not implemented because email uniqueness is only within a company. Authenticated requests never accept tenant scope from a query string, route value, body, or custom client header.

## REST communication

The Flutter client uses versionable `/api` REST endpoints with JSON payloads, UTC timestamps, consistent errors, correlation IDs, pagination, validation, and idempotency controls for financial commands. Detailed conventions are in [API_CONVENTIONS.md](API_CONVENTIONS.md).

## Why Flutter never connects directly to PostgreSQL

A direct database connection from a mobile or browser client would expose credentials, bypass authorization and tenant enforcement, couple releases to physical schema details, and make financial auditing and transaction policy unreliable. Browser environments also cannot safely hold database secrets. All database access therefore remains behind the API, where credentials can be protected and server-side policy can be enforced consistently.
