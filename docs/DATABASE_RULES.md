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

## Company-structure schema clarification

`projects` requires a tenant-composite reference to `project_owners`, positive `contract_value NUMERIC(18,2)`, contract/start dates, non-blank project name/address, creator, status, and `version_number`. Exact statuses are `Active`, `Paused`, `Completed`, `FinanciallyClosed`, and `Cancelled`. Completion and cancellation statuses require additional database fields, so this phase creates `Active` projects and permits only `Active`/`Paused` metadata transitions.

`project_owners` requires tenant, name, E.164 phone, active flag, creator, and version. Phone is unique per company; non-null email is also case-insensitively unique per company. Owner creation and its intended project are transactional. Existing-owner references are always resolved by `(company_id, project_owner_id)` and require an active owner.

`project_supervisors` is a history-preserving assignment table with tenant-composite project/user foreign keys, assignment actor/time, active state, and required removal actor/time/reason for inactive rows. It has no `version_number`; replacement serializes on a tenant-filtered project row lock and also advances `projects.version_number`. No assignment row is deleted.

No generic project-member, worker-project, site, client, or user-project-assignment table exists among the 91 mapped tables. Project-member reads therefore represent active project-supervisor assignments only. `project_contract_changes` exists as a separate value-change/reason/review/history workflow with exact types `Increase`, `Decrease`, and `Correction`, and statuses `PendingApproval`, `Approved`, `Rejected`, and `Cancelled`; direct contract-value PATCH is intentionally not implemented. Its partial unique index permits only one pending change per project, while historical rows can be multiple. The generated navigation is singular, so a future implementation must review that Database-First relationship limitation rather than relying on the navigation for history.

## Advances schema clarification

`advances` uses UUID primary key and unique `(company_id, advance_id)`/number keys. It requires an exact Deputy recipient, `advance_amount NUMERIC(18,2)`, currency, issue date, purpose, creator, lifecycle, and version. Exact statuses are `Draft`, `PendingConfirmation`, `Open`, `InSettlement`, `ReadyToClose`, `Closed`, `Cancelled`, and `Reversed`. It has no project, parent-advance, or current-balance column.

`advance_funding_sources` allocates existing `funding_sources`. Source types are `ProjectOwnerPayment`, `ManagerContribution`, `CompanyCashbox`, `ReturnedAdvance`, `SupplierRefund`, and `Other`; source statuses are `PendingVerification`, `Available`, `PartiallyUsed`, `FullyUsed`, `Cancelled`, and `Reversed`. Payment methods are `Cash`, `BankTransfer`, `Cheque`, `Card`, `MobileWallet`, and `Other`. Phase 1 reserves and consumes only `Available`/`PartiallyUsed` sources and appends immutable `funding_source_ledger_entries`.

`money_transfers` represents exact types `AdvanceDelivery`, `InternalTransfer`, and `BalanceReturn`; statuses are `Draft`, `PendingConfirmation`, `CorrectionRequired`, `Confirmed`, `Rejected`, `Cancelled`, and `Reversed`. Transfer methods add `BalanceTransfer` to the funding payment-method set. One direct `AdvanceDelivery` is unique per advance; later operations use `transfer_advance_allocations` and do not create child advances.

`user_advance_balances` is authoritative per `(company_id, advance_id, user_id)` and enforces its received/restored versus expensed/transferred/returned/adjustment/available/reserved equation. Exact statuses are `Active`, `InSettlement`, `Settled`, and `Closed`. `balance_ledger_entries` is immutable through PostgreSQL trigger and records every Phase 1 delta and after-state. No trigger or stored function updates balances automatically.

Advance, funding-source, transfer, and user-balance `version_number` properties are configured as EF Core concurrency tokens outside generated files. Every availability/lifecycle command also locks tenant-scoped source, transfer, advance, and/or balance rows before validation.

`idempotency_records` stores tenant-unique keys, operation/payload fingerprint, actor, completion resource, and replay audit. It is committed in the same transaction as each financial command. The company number-sequence tables had no configured rows at read-only inspection time, so Phase 1 uses opaque UUID-based display references while PostgreSQL continues to generate configured primary-key UUIDs and timestamps.

Settlement and closure tables are not written. Their schemas depend on expense snapshots, documents, difference resolution, supplier debt, personal claims, pending-operation counts, reconciliation, and approvals. No zero-balance shortcut is implemented.

## Expenses schema clarification

`expenses` uses UUID primary key, unique tenant number, one optional direct `project_id`, one category, incurred-by and submitted-by users, `NUMERIC(18,2)` subtotal/discount/tax/total, exact payment mode, lifecycle review fields, and `version_number`. Exact statuses are `Draft`, `PendingReview`, `CorrectionRequired`, `Approved`, `Rejected`, `Cancelled`, and `Reversed`. Exact payment modes are `AdvanceBalance`, `SupplierCredit`, and `PersonalFunds`.

`expense_categories` is tenant-owned and hierarchical. Groups are `Materials`, `Labor`, `Subcontracting`, `Transportation`, `Equipment`, `Fuel`, `Services`, `Administrative`, `Utilities`, `Permits`, and `Other`; scopes are `ProjectOnly`, `CompanyOnly`, and `Both`. Name and optional code are not uniquely constrained within a tenant, so Phase 1 does not invent application uniqueness. Inactive rows require actor/time/reason, but no delete/deactivate endpoint is published.

`expense_advance_allocations` uniquely links an expense and user balance with positive `NUMERIC(18,2)` amount. `user_advance_balances` remains authoritative. Exact balance ledger operations used by expenses are `ExpenseReserved`, `ExpenseReservationReleased`, and `ExpenseConfirmed`; the ledger is database-immutable and no trigger mutates balance totals.

`expense_documents` stores metadata and exact types `Receipt`, `Invoice`, `Quotation`, `DeliveryNote`, `PaymentProof`, `Contract`, `PurchaseOrder`, and `Other`; capture sources `Camera`, `Gallery`, `FileUpload`, `Scanner`, and `Generated`; and verification states `PendingVerification`, `Verified`, and `Rejected`. The physical schema supports multiple rows and one non-rejected primary document, while the generated navigation is singular. Application collection queries use the `DbSet` directly.

`personal_claims` permits one expense-linked `PersonalExpense` claim. Claim statuses are `Open`, `PartiallySettled`, `Settled`, `Cancelled`, and `Reversed`; outstanding amount is computed. Phase 1 creates open claims and cancels untouched claims only when their pending expense is rejected. No claim payment table is written.

No expense project-allocation, expense payment-component, receipt-status, original-paper custody, or separate expense-approval table exists. `audit_logs` is immutable and supplies safe expense history. Expense, category, document, and personal-claim versions are configured as concurrency tokens outside generated files.

## Supplier schema clarification

There is no `supplier_invoices`, payable, accounts-payable, supplier-balance, or supplier-statement table. Invoice headers are `expenses` rows with exact `SupplierCredit` mode; each debt is unique by `(company_id, expense_id)`. Optional lines use `expense_items` with `quantity NUMERIC(18,3)` and generated totals.

`supplier_debts.outstanding_amount NUMERIC(18,2)` is generated as debt plus adjustment minus paid, credit-note, and written-off amounts. Debt statuses are `Open`, `PartiallySettled`, `Settled`, `Cancelled`, and `Reversed`. `supplier_debt_ledger_entries` is immutable by trigger.

Supplier payments have unique debt/funding allocation rows. Statuses are `Draft`, `PendingApproval`, `Confirmed`, `Rejected`, `Cancelled`, and `Reversed`; methods are `Cash`, `BankTransfer`, `Cheque`, `Card`, `MobileWallet`, and `Other`. Existing funding sources—not advance balances—provide payment funding.

Credit-note statuses are `Draft`, `PendingApproval`, `Approved`, `Rejected`, `Cancelled`, and `Reversed`; allocations are unique per note/debt. Refunds require `expense_returns` and a one-to-one `SupplierRefund` funding source; statuses are `PendingVerification`, `Confirmed`, `Cancelled`, and `Reversed`.

Supplier, account, debt, payment, refund, and credit-note versions are concurrency tokens outside generated files. No generated file or PostgreSQL object changed.
