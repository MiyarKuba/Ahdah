# Project status

## Project

Ahdah — عُهدة

## Current phase

Expenses Foundation — Backend Phase 1

## Date, workspace, and branch

- Date: `2026-08-04`.
- Workspace confirmed before edits: `C:\dev\Ahdah`.
- Branch confirmed before edits: `feat/expenses-foundation`.
- The pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` was left untouched.

## Actual schema findings

- Tables used directly are `expense_categories`, `expenses`, `expense_advance_allocations`, `expense_documents`, `expense_items`, `personal_claims`, `user_advance_balances`, `balance_ledger_entries`, `advances`, `projects`, `users`, `company_members`, `project_assignments`, `company_settings`, `idempotency_records`, and `audit_logs`.
- `expenses` has one nullable direct `project_id`; there is no project-split table or expense payment-component table.
- Expense statuses are `Draft`, `PendingReview`, `CorrectionRequired`, `Approved`, `Rejected`, `Cancelled`, and `Reversed`.
- Payment modes are `AdvanceBalance`, `SupplierCredit`, and `PersonalFunds`.
- `expense_advance_allocations` links an expense to one or more `user_advance_balances`; allocations must be positive and unique per balance.
- `balance_ledger_entries` already defines `ExpenseReserved`, `ExpenseReservationReleased`, and `ExpenseConfirmed`; PostgreSQL prevents ledger update/delete.
- `expense_documents` stores multiple attachments with document type, capture method, verification status, and a single non-rejected primary document. There is no separate original-paper custody object or lifecycle.
- Document types are `Receipt`, `Invoice`, `Quotation`, `DeliveryNote`, `PaymentProof`, `Contract`, `PurchaseOrder`, and `Other`; verification statuses are `PendingVerification`, `Verified`, and `Rejected`. Receipt/invoice presence is derived from non-rejected document rows plus the optional receipt/invoice numbers on the expense; there is no separate receipt-state enum.
- `personal_claims` represents reimbursement obligations for personal expenses. Statuses are `Open`, `PartiallySettled`, `Settled`, `Cancelled`, and `Reversed`.
- `company_settings` defines expense approval/document modes and thresholds, self-approval separation, document size/type configuration, and related defaults.
- There is no expense-specific database function, separate expense approval table, configured expense reference sequence, or schema object for document binary storage.
- Category groups are `Materials`, `Labor`, `Subcontracting`, `Transportation`, `Equipment`, `Fuel`, `Services`, `Administrative`, `Utilities`, `Permits`, and `Other`; scopes are `ProjectOnly`, `CompanyOnly`, and `Both`. Listing defaults to active tenant categories, and creation sets active server state without exposing deletion.

## Workflows implemented

- Paginated, tenant-scoped category and expense queries with role- and record-level visibility.
- Manager-only active expense-category creation using exact database groups/scopes and schema-backed validation.
- Expense creation for `AdvanceBalance` and `PersonalFunds` with authenticated-user ownership only.
- One direct project association with category scope enforcement; Supervisors may use only active assigned projects and Workers cannot create project-linked expenses.
- Atomic advance-balance reservation across sorted, tenant-filtered row locks, with immutable `ExpenseReserved` ledger entries.
- Personal-funds expense creation with an auditable `Open` personal claim.
- Manager/Deputy/Accountant approval and rejection with expected-version concurrency, company separation-of-duty settings, explicit transactions, and idempotency.
- Approval confirms reserved advance amounts and appends `ExpenseConfirmed`; rejection releases reservations and appends `ExpenseReservationReleased`.
- Attachment metadata listing/creation with settings-backed MIME type, extension, and size validation; safe application-relative storage paths; no storage URL or hash disclosure in response DTOs.
- Expense allocation, attachment, audit-history, and reimbursement read models.
- Tenant-scoped financial idempotency fingerprint/original-response replay and HTTP 409 mapping for concurrency or uniqueness conflicts.
- Immutable audit events for expense creation, document addition, approval, and rejection.

## Endpoints

- `GET /api/v1/expense-categories`
- `POST /api/v1/expense-categories`
- `GET /api/v1/expenses`
- `POST /api/v1/expenses`
- `GET /api/v1/expenses/{expenseId}`
- `GET /api/v1/expenses/{expenseId}/allocations`
- `GET /api/v1/expenses/{expenseId}/attachments`
- `POST /api/v1/expenses/{expenseId}/attachments`
- `GET /api/v1/expenses/{expenseId}/history`
- `POST /api/v1/expenses/{expenseId}/approve`
- `POST /api/v1/expenses/{expenseId}/reject`
- `GET /api/v1/reimbursements`
- `GET /api/v1/reimbursements/{reimbursementId}`

`POST /api/v1/expenses`, attachment creation, approval, and rejection require a 16–200 character `Idempotency-Key` header.

## Role visibility and capabilities

| Role | Expense visibility | Create | Review | Attachments | Categories | Reimbursements |
|---|---|---|---|---|---|---|
| Manager | All company expenses | Yes | Approve/reject | Yes | List/create | All company claims |
| Deputy | All company expenses | Yes | Approve/reject | Yes | List | All company claims |
| Accountant | All company expenses | No | Approve/reject | Yes | List | All company claims |
| Supervisor | Own plus assigned-project expenses | Yes, assigned projects only | No | Visible records | List | Own claims |
| Worker | Own expenses only | Yes, company-only | No | Own records | List | Own claims |
| Unknown | None | No | No | No | None | None |

Policies added: `ExpenseViewer`, `ExpenseCreator`, `ExpenseReviewer`, `ExpenseDocumentContributor`, `ExpenseCategoryManager`, and `ReimbursementViewer`. Every persistence operation applies `company_id` plus record-level authorization.

## Financial and concurrency integrity

- PostgreSQL `NUMERIC(18,2)` and C# `decimal` are used for all money; no `float` or `double` was introduced.
- `user_advance_balances` remains authoritative; clients cannot submit balances or ledger values.
- Financial commands run in explicit PostgreSQL transactions and lock tenant-filtered mutable records before validation.
- Ledger history is append-only. No update, delete, repair, or silent recalculation path was added.
- Expenses, categories, documents, claims, and user balances use optimistic concurrency where mutation is exposed.
- All actor/company/status/audit values come from authenticated context and server policy, not client identity fields.
- Opaque `EXP-<uuid>` and `CLM-<uuid>` display references are used because no company sequence is configured.

## Deliberately deferred

- `SupplierCredit` creation and supplier-debt effects: this requires a focused supplier-credit workflow and approved business rules.
- Automatic approval for `Never`/threshold modes: the schema requires a concrete reviewer and reviewed timestamp, so no synthetic approver was invented.
- Draft/edit/correction/cancellation/reversal lifecycles: no safe command contract was approved for this foundation phase.
- Expense creation on behalf of another user: the schema does not establish an approved reporting hierarchy for that authority.
- Document binary upload/download, malware scanning, verification/rejection, and original-paper custody: only metadata persistence exists in the current schema.
- Claim settlement/payment writes: reimbursement reads are provided, but financial settlement rules are outside this phase.
- Project splits, mixed payment components, and separate approval history: corresponding schema objects do not exist.
- Receipt-line items and settlement/FIFO integration: deferred until their business workflows are explicitly approved.

## Packages and schema changes

- Packages changed: none.
- PostgreSQL schema changed: no.
- EF migrations created or run: none.
- Generated Database-First entity/configuration files changed: no.
- `EnsureCreated`, automatic migration, and direct Flutter database access were not introduced.

## Verification status

- `dotnet restore backend\Ahdah.sln`: passed; all projects up to date.
- `dotnet build backend\Ahdah.sln --no-restore`: passed with 0 warnings and 0 errors.
- `dotnet test backend\Ahdah.sln --no-restore --no-build`: passed 220 total, 0 failed, 0 skipped (128 unit and 92 integration).
- OpenAPI: the integration suite passed the development-document test for all new expense/category/reimbursement routes and their authorization metadata.
- Scoped formatting verification for all newly added expense files: passed.
- Repository-wide formatting verification remains blocked by pre-existing line-ending diagnostics in unrelated files; those files were not rewritten.
- PostgreSQL inspection was read-only; no real expense, balance, claim, document, or ledger write endpoint was invoked.
- The pre-existing untracked Flutter file remains untouched.

## Known limitations and risks

- Attachment creation records metadata for an already stored application-relative file; this phase does not implement a storage provider or prove the file exists.
- Category code/name uniqueness is not enforced by the database across a tenant; the service performs tenant-scoped conflict checks, with the normal concurrency caveat.
- Database-generated document navigation is singular even though the physical table permits multiple rows; the service queries `ExpenseDocuments` directly.
- Auto-approval and supplier-credit semantics require approved schema/business decisions before safe implementation.
- No live financial workflow was exercised; automated tests use service fakes and persistence/source-contract checks.

## Exact recommended next task

Implement the Flutter Expenses, Receipt Status, Reimbursement, and Document Custody UI using the completed Expenses APIs, without implementing final settlement or supplier debt yet.
