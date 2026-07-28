# Database rules

## Existing database

- Database: `ahdah_db`
- Schema: `ahdah`
- Database engine: PostgreSQL 18
- Current table count: 91
- Integration approach: database first
- Source of truth: the existing database

The database was designed and created before the application. EF Core Database-First integration now maps the existing schema without changing it. The expected mapped base-table count is 91.

## Permanent safeguards

- Do not use EF Core `EnsureCreated`.
- Do not create or run automatic migrations.
- Do not make destructive schema changes.
- Do not assume application models can redefine the existing schema.
- Review generated database-first mappings before accepting them.
- Separate generated persistence mappings from domain and application policy.
- Every tenant-owned query and command must enforce `company_id`.
- Financial values must use C# `decimal` and PostgreSQL `NUMERIC`; never use `float` or `double`.
- Financial ledger records must not be silently edited or deleted.
- Financial changes must be transactional and auditable.
- Connection strings, passwords, and other secrets must not be committed.
- Flutter must never receive database credentials or connect directly to PostgreSQL.

## Database-First integration

Generated context and entity files are isolated under `Ahdah.Infrastructure/Persistence/Generated` and must not be manually edited. Custom persistence extensions belong outside that directory. Re-scaffolding requires a dedicated branch, explicit approval, and review.

The API obtains `ConnectionStrings:AhdahDatabase` through ASP.NET Core configuration. Local development uses .NET User Secrets; production must use environment-based secret configuration. Credentials must never appear in repository files or generated source.

No application startup path may apply migrations, call `EnsureCreated`, or repair schema automatically.

## Identity schema clarification

Identity Phase 1 uses the existing `companies` and `app_users` mappings without schema changes. `app_users.phone_number` is globally unique and constrained to E.164 format; company email uniqueness is scoped to `company_id`; company code uniqueness is case-insensitive. Existing `version_number` fields are configured as optimistic-concurrency tokens outside generated files.

`user_devices` is not a session or refresh-token store. It has device identity, platform, push token, trust/activity flags, and device timestamps, but lacks refresh-token hash, expiration, rotation, and revocation fields. Application code must not store raw refresh tokens or repurpose `push_token`. Adding refresh support requires a separately approved database-first schema decision.

Identity Phase 1 introduced no migration, initialization call, table, column, constraint, or other database structure change.

## Invitation and join-request schema clarification

`invitations` stores a globally unique `invitation_code_hash`, required phone, assigned non-manager role, required expiry, lifecycle status, creator, acceptance user/time, and cancellation time. Valid statuses are `Pending`, `Accepted`, `Expired`, and `Cancelled`. The table has no `version_number`, cancellation actor, recipient name/email, or invitation-lifetime default. The database only requires `expires_at > created_at`.

`join_requests` requires `(company_id, user_id)` to reference an already-existing same-company `app_user`. It stores requested/assigned roles, status, message, reviewer, review notes, rejection reason, request/review/cancellation timestamps, and audit timestamps. Valid statuses are `Pending`, `Approved`, `Rejected`, and `Cancelled`. It has no applicant name, phone, email, password hash, approved-user reference distinct from `user_id`, or `version_number`.

The completed access phase introduced no database change. Invitation lifetime is application configuration, while each invitation continues to store its required `expires_at`. Public join onboarding creates the required pending `app_user` first and then its linked request in one transaction; password hashes remain exclusively on `app_users`. Where invitation/join-request `version_number` is absent, transactional PostgreSQL row locks protect lifecycle transitions.
