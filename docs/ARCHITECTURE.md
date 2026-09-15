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

Projects and company members follow the same feature-first boundaries. `features/projects` contains handwritten immutable transport/domain models, exact create/update/supervisor inputs, an API repository, focused list/detail/member/write controllers, and responsive list/detail/forms. `features/company_members` contains separate list/detail models because the list intentionally omits contact fields, a read-only repository, focused controllers, and responsive pages. Neither feature imports persistence models or adds another state-management package.

The authenticated shell additionally owns `/projects`, `/projects/new`, `/projects/:projectId`, `/projects/:projectId/edit`, `/projects/:projectId/supervisor`, `/projects/:projectId/members`, `/company/members`, and `/company/members/:memberId`. Central `RoleCapabilities` guards project visibility, Manager mutation routes, project-supervisor visibility, directory access, contract-value rendering, and Supervisor assigned-only messaging. Navigation hiding is paired with route redirects; backend authorization and record filtering remain authoritative.

Project/member lists preserve server filters while refreshing, reset to page 1 when filters change, debounce bounded searches without a dependency, ignore stale search responses, deduplicate by stable IDs, and retain loaded data after a load-more failure. Creation and mutation controllers suppress duplicate submissions and do not retry writes. A 401 expires the central session; 403 remains a permission error; 404 is safe/unavailable; and 409 presents an explicit reload flow.

Contract value crosses Flutter as decimal text. Creation validates its exact decimal grammar and replaces a private JSON marker with that validated token so the API receives a JSON number without a `double` conversion. No financial arithmetic occurs. Presentation additionally checks Manager capability, so a malformed response containing `contractValue` is still not rendered for Deputy, Accountant, or Supervisor.

The Flutter Advances feature follows the same feature-first boundary under `features/advances`. Handwritten immutable DTOs, exact request objects, a repository interface/API adapter, focused paged-read controllers, and focused financial-command controllers remain independent of EF persistence types. Financial responses are read as text and amount tokens are protected before JSON decoding so `NUMERIC(18,2)` values do not pass through binary floating point. Request amounts remain canonical decimal text until validated JSON-number emission; the only client arithmetic is checked integer minor-unit comparison for form validation and exact funding allocation equality.

`RoleCapabilities` adds advance visibility, top-level creation, held-balance distribution, authorized-balance discovery, confirmation, supported rejection, and return capabilities. Routes are guarded in addition to hidden navigation. Record actions further require an `Open` advance, an authoritative current-user balance, or a pending movement whose exact recipient matches `/auth/me`. The initial delivery is never rejectable in Flutter.

Financial command controllers generate 32 secure random bytes as URL-safe, unpadded `Idempotency-Key` values. Keys and payloads live only in auto-disposed controller memory. A timeout/network error preserves the exact payload/key and exposes an explicit retry; success, definitive 4xx, user cancellation, payload replacement, and disposal clear it. No write is automatically retried and no key enters preferences, secure storage, browser storage, routes, UI, or logs.

The Flutter Expenses feature follows the same feature-first boundary under `features/expenses`. Handwritten immutable category, expense, allocation, document, item, reimbursement, and history models mirror only the published camelCase contracts. The API adapter preserves financial JSON number tokens as decimal text, while request values remain validated strings until exact JSON numeric emission. Focused Riverpod controllers separate list/detail/category/document/history/reimbursement reads from create/approve/reject commands.

Authenticated expense routes cover list, create, detail, metadata documents, history, review, categories, Manager-only category creation, reimbursements, and reimbursement detail. Central role capabilities and router redirects provide broad guards; loaded status, actor/project relationships, and backend authorization remain authoritative. Worker receives no project selector, Supervisor sees API-assigned projects, Accountant is review-only, and unknown roles receive no expense capability.

Advance-balance expense creation consumes authoritative personal balance rows. A minimal backend contract correction exposes the existing `userAdvanceBalanceId` in `AdvanceBalanceSummary`; no persistence mapping or schema changed. Flutter supports multiple unique same-currency allocations and compares their sum in integer minor units. Personal-funds creation presents the returned claim as unpaid. Document metadata is read-only because the current metadata POST requires storage-generated path/checksum input and no binary storage abstraction exists.

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

## Advances foundation

Application owns explicit advance requests/responses, money validation, exact lifecycle values, role capabilities, and the `IAdvanceService` contract. Infrastructure owns tenant-filtered EF Core projections, explicit transactions, PostgreSQL row locks, source/balance updates, immutable ledger appends, and tenant-scoped idempotency. API owns focused advance policies, controllers, status mapping, and Problem Details. Generated persistence entities never cross the API boundary.

`advances` represents a top-level Manager-to-Deputy custody issue. Its unique direct `AdvanceDelivery` uses `money_transfers`; later `InternalTransfer` and `BalanceReturn` records link through `transfer_advance_allocations`. A distribution does not create a child advance. `user_advance_balances` is current-state authority, while `balance_ledger_entries` and `funding_source_ledger_entries` preserve immutable history.

Creation locks and allocates existing usable `funding_sources`, reserves their authoritative availability, and creates the pending advance delivery atomically. Confirmation consumes the reservation and creates the recipient balance. Distribution and return initiation lock the holder balance before reserving availability; confirmation creates paired sender/recipient ledger effects. PostgreSQL row locks and EF concurrency tokens protect every mutable financial aggregate. SQLSTATE `23505` and optimistic-concurrency failures map to safe conflict responses.

Every financial command requires tenant-scoped `Idempotency-Key` handling through the existing `idempotency_records` table. Identical completed commands replay their original safe response; key reuse by another actor, operation, or payload conflicts. Writes are never automatically retried.

Manager, Deputy, and Accountant receive company-wide advance reads; Accountant remains read-only. Supervisor and Worker reads are limited to personal balances and transfer participation. Manager creates top-level advances and may distribute held returns to Deputy. Deputy distributes held balances to Supervisor or Worker. Supervisor-to-Worker distribution remains deferred because no approved business rule authorizes it.

There is no advance/project foreign key, so project association is not inferred through a funding source. Settlement and closure writes remain separate future modules because their existing records depend on expenses, documents, difference resolutions, supplier debt, personal claims, and final review. See [ADVANCES.md](ADVANCES.md).

## Expenses foundation

Application owns explicit expense/category/document/reimbursement contracts, exact schema values, money and allocation validation, role capabilities, and `IExpenseService`. Infrastructure owns tenant-filtered EF projections, explicit transactions, expense and balance row locks, authoritative balance changes, immutable ledger/audit appends, and existing idempotency records. API owns focused policies, controllers, status mapping, and safe Problem Details. Generated persistence entities never cross the API boundary.

`expenses` has one optional direct project and exact payment modes `AdvanceBalance`, `SupplierCredit`, and `PersonalFunds`. The implemented create paths are advance balance and personal funds. Advance-backed submission reserves the authenticated holder's authoritative balance and appends `ExpenseReserved`; approval appends `ExpenseConfirmed`, while rejection appends `ExpenseReservationReleased`. Personal-funds submission atomically creates one open expense-linked `personal_claim`; payment remains deferred.

## Suppliers and supplier debt foundation

Application owns explicit supplier, masked-account, invoice/debt, payment/allocation, refund-history, credit-note, balance, and statement contracts plus exact schema values and role capabilities. Infrastructure owns tenant-filtered EF queries, supplier-credit expense/debt creation, explicit transactions, row locks, funding reservation/consumption, computed debt balances, immutable ledger/audit appends, and existing idempotency. API owns focused policies, controllers, status mapping, and safe Problem Details.

There is no `supplier_invoices` table: `expenses(payment_mode='SupplierCredit')` is the invoice header and has one `supplier_debts` row. `expense_items` supplies optional lines. Pending payments reserve `funding_sources`; confirmation consumes reservations and applies allocations to debts. `ManagerContribution` preserves manager-personal attribution. `user_advance_balances` cannot fund supplier payments. Refund writes remain deferred because the schema requires an approved `expense_returns` resolution and one-to-one refund funding source.

Manager, Deputy, and Accountant receive company-wide reads and may review pending expenses subject to settings-based approval separation. Manager, Deputy, Supervisor, and Worker may create personal expenses; Supervisor project use requires active assignment and Worker project use is unavailable because no worker/project relationship exists. Supervisor reads personal plus assigned-project records; Worker reads personal records only.

The physical `expense_documents` table supports multiple metadata rows, but its generated navigation is singular. Infrastructure therefore queries the table set directly. File URLs and hashes remain persistence-only. There is no original-paper custody representation, expense project-split table, or expense payment-component table. See [EXPENSES.md](EXPENSES.md).

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
## Flutter supplier/payables boundary

The Suppliers feature follows the existing `domain` / `data` / `presentation` split. Handwritten domain models and request objects represent the public HTTP contracts only; they are not persistence entities. `ApiSupplierRepository` owns endpoint translation, focused Riverpod controllers own paging or one financial operation, and route widgets contain presentation and confirmation behavior. Central `RoleCapabilities` is the single client source for navigation, routing, and action visibility, with server policies and tenant predicates remaining authoritative.

Financial values cross the Flutter boundary as validated decimal text and exact JSON numeric tokens. Money comparisons use integer minor units and invoice quantities use integer thousandths, preventing binary floating-point drift. Multi-currency balances and statements stay as separate currency groups rather than being summed.

Each financial mutation controller owns one in-memory idempotency operation. It creates a secure random key, binds it to a payload fingerprint, prevents duplicate submission, and permits same-key retry only after an ambiguous transport failure. Replacing the payload or reaching a definitive outcome discards the key. HTTP writes have no automatic retry interceptor.

The supplier funding-source selector is a read model, not a new ledger or persistence abstraction. Infrastructure filters existing funding sources by tenant, lifecycle, availability, payment method/currency, and Manager-contribution ownership. No Flutter contract depends on generated EF models, and no client connects directly to PostgreSQL.
