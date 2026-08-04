# Expenses foundation — backend Phase 1

## Scope and database findings

Phase 1 uses the existing PostgreSQL 18 `ahdah` schema without migrations or generated-model edits. The implemented records are:

- `expense_categories`: tenant categories with optional parent, exact group/scope, receipt/supplier/quantity flags, active lifecycle, creator, and version.
- `expenses`: one expense with one optional direct `project_id`, category, incurred-by user, submitter, exact payment mode, amounts, receipt/invoice reference fields, review fields, and version.
- `expense_advance_allocations`: one or more unique allocations from an expense to authoritative `user_advance_balances` rows.
- `user_advance_balances` and immutable `balance_ledger_entries`: advance-balance authority and append-only reservation/confirmation/release history.
- `expense_documents`: supporting-document metadata. The physical table permits multiple rows and one non-rejected primary row per expense. The generated navigation is singular, so Phase 1 queries the document set directly and never relies on that navigation for a collection.
- `expense_items`: existing read-only quantity-detail rows in expense detail. Phase 1 does not create or edit line items.
- `personal_claims`: one optional expense-linked personal-money reimbursement liability.
- `audit_logs`: immutable authoritative expense event history written by Phase 1 and projected without raw JSON or database internals.
- `idempotency_records`: the existing tenant-scoped financial-command replay mechanism.
- `company_settings`: document size/type rules, separation-of-approval rules, optional maximum expense, and personal-claim due days.

There is no expense-project split table, expense payment-component table, separate expense approval table, receipt-status table, original-paper-document custody table, or original-document state field. `expenses.project_id` is the only project association. `expenses.payment_mode` is not the funding-source payment-method list.

## Exact values

Expense statuses are `Draft`, `PendingReview`, `CorrectionRequired`, `Approved`, `Rejected`, `Cancelled`, and `Reversed`. Phase 1 creates `PendingReview`, approves it to `Approved`, or rejects it to `Rejected`. It does not publish draft editing, correction, cancellation, or reversal commands.

Expense payment modes are:

- `AdvanceBalance`
- `SupplierCredit`
- `PersonalFunds`

Creation supports `AdvanceBalance` and `PersonalFunds`. `SupplierCredit` remains readable but cannot be created because supplier debt is excluded from this phase. Cash, bank transfer, card, and cheque are funding-source/transfer values, not expense payment modes.

Category groups are `Materials`, `Labor`, `Subcontracting`, `Transportation`, `Equipment`, `Fuel`, `Services`, `Administrative`, `Utilities`, `Permits`, and `Other`. Category scopes are `ProjectOnly`, `CompanyOnly`, and `Both`.

Document types are `Receipt`, `Invoice`, `Quotation`, `DeliveryNote`, `PaymentProof`, `Contract`, `PurchaseOrder`, and `Other`. Capture sources are `Camera`, `Gallery`, `FileUpload`, `Scanner`, and `Generated`. Verification statuses are `PendingVerification`, `Verified`, and `Rejected`.

The schema has no independent receipt-present/missing lifecycle. Receipt evidence is represented by `receipt_number`, `invoice_number`, and non-rejected `Receipt`/`Invoice` document metadata. A category with `requires_receipt=true` cannot be approved without such document metadata. Phase 1 does not invent a no-receipt exemption or auto-approval.

Personal-claim source type is `PersonalExpense`; statuses are `Open`, `PartiallySettled`, `Settled`, `Cancelled`, and `Reversed`. Expense creation creates only `Open`; rejection cancels an untouched open claim atomically. Claim payment and settlement are deferred.

## API

Categories:

- `GET /api/v1/expense-categories`
- `POST /api/v1/expense-categories`

Expenses:

- `GET /api/v1/expenses`
- `GET /api/v1/expenses/{expenseId}`
- `POST /api/v1/expenses`
- `GET /api/v1/expenses/{expenseId}/allocations`
- `GET /api/v1/expenses/{expenseId}/attachments`
- `POST /api/v1/expenses/{expenseId}/attachments`
- `GET /api/v1/expenses/{expenseId}/history`
- `POST /api/v1/expenses/{expenseId}/approve`
- `POST /api/v1/expenses/{expenseId}/reject`

Reimbursements:

- `GET /api/v1/reimbursements`
- `GET /api/v1/reimbursements/{reimbursementId}`

Collections use page 1 by default, page size 20, maximum 100, and deterministic ordering. Expense filters are exact status, category, project, incurred-by, submitter, payment mode, and a 2–50 character reference prefix. Incurred-by and submitter filters require company-wide visibility. Filters and record visibility execute in SQL before count and pagination.

## Role and record visibility

| Role | Expense visibility | Creation | Review | Categories | Reimbursements |
|---|---|---|---|---|---|
| Manager | All tenant expenses | Personal | Approve/reject subject to separation settings | List/create | All tenant claims |
| Deputy | All tenant expenses | Personal | Approve/reject subject to separation settings | List | All tenant claims |
| Accountant | All tenant expenses | No | Approve/reject subject to separation settings | List | All tenant claims |
| Supervisor | Personal and actively assigned-project expenses | Personal; assigned projects only | No | List | Personal claims |
| Worker | Personal expenses only | Personal; company-only because no worker/project relation exists | No | List | Personal claims |
| Unknown | None | No | No | None | None |

There is no supervisor/worker hierarchy table, so on-behalf-of creation is omitted. Every Phase 1 creation derives both incurred-by and submitter from the authenticated user. Policies are outer gates; persistence queries still enforce `company_id`, personal ownership, and active supervisor assignment.

## Creation, project, and funding behavior

The request accepts one category, one optional direct project, expense date, positive `NUMERIC(18,2)`-compatible amount, currency, exact payment mode, description, safe optional reference/merchant/location/notes fields, and advance-balance allocations only for `AdvanceBalance`.

`ProjectOnly` requires a project, `CompanyOnly` prohibits one, and `Both` permits either. Every project is resolved by `(company_id, project_id)`. Supervisor project creation additionally requires an active `project_supervisors` row. Worker project creation is unavailable because no worker/project relationship exists. Multiple-project allocation is not represented and is omitted.

For `AdvanceBalance`, each unique allocation points to the authenticated user's active balance on an open same-company advance with matching currency. Allocations total the expense exactly. Balance rows are locked in stable ID order; availability moves to reserved and an immutable `ExpenseReserved` ledger entry is appended. Approval converts reserved to expensed with `ExpenseConfirmed`. Rejection restores availability with `ExpenseReservationReleased`. The API never accepts or reconstructs an available balance.

For `PersonalFunds`, creation atomically opens one `PersonalExpense` claim for the authenticated user. The claim preserves project, currency, amount, description, and optional setting-derived due date. It is not marked paid. Rejection atomically cancels only an untouched open claim; later reimbursement payments, adjustments, write-offs, and settlement are not published.

All newly created expenses enter `PendingReview`. Although company settings contain `Always`, `Threshold`, and `Never` expense-approval modes, automatic approval is deferred: the expense schema requires a concrete reviewer user and review timestamp, and no approved system-review actor rule exists. Review honors `allow_self_approval`, `require_distinct_creator_approver`, and `require_distinct_submitter_approver`; missing settings fail self-approval conservatively.

## Documents, receipts, and custody

The attachment POST records metadata only; it does not upload bytes. It accepts the schema-required safe application-relative file path, file name, MIME type, byte size, SHA-256 checksum, type, capture source, optional document reference fields, and primary flag. Active company settings enforce allowed MIME types, extensions, and maximum size. The response deliberately omits file URL and checksum. New metadata is `PendingVerification`; document verification/rejection commands are deferred.

The schema contains no original-paper custody state, actor, transition, or timestamp. No `/original-status` route exists and no custody state is inferred from a digital attachment.

## Approval, audit, concurrency, and idempotency

Manager, Deputy, and Accountant may review a tenant `PendingReview` expense. Approval/rejection locks the expense and requires `expectedVersion`. Duplicate or stale review conflicts. Rejection preserves the expense; it never deletes financial or history rows.

Phase 1 writes immutable `audit_logs` events for expense creation, approval, rejection, and document-metadata addition. History returns stable chronological event/action/outcome/description, actor summary, version, and time only. Raw JSON, SQL, request details, and idempotency keys are not exposed.

Expense, category, document, personal-claim, advance, and balance versions are EF concurrency tokens outside generated files. Financial/lifecycle writes use explicit PostgreSQL transactions and tenant-filtered `FOR UPDATE` locks. `DbUpdateConcurrencyException`, insufficient/reserved-balance conflicts, stale lifecycle, idempotency mismatch, and SQLSTATE `23505` return safe HTTP 409 results. Writes are never automatically retried.

Expense creation, document-metadata addition, approval, and rejection require a 16–200 character `Idempotency-Key`. The existing `idempotency_records` mechanism binds company, operation, actor, and payload, stores the original safe response in the same transaction, replays exact completed requests, and rejects mismatched reuse. Keys are never logged or returned. Category creation is not a financial/lifecycle command and does not require the header.

## Deferred workflows

- Multi-project splits: no expense project-allocation table exists.
- Split payment and cash/card/cheque expense methods: no expense payment-component table exists.
- Supplier-credit creation and supplier debt: explicitly outside Phase 1.
- On-behalf-of Worker creation: no supervisor/worker hierarchy exists.
- Expense PATCH/correction, cancellation, reversal, and returns: require separate approved lifecycle rules.
- Automatic approval modes: no approved system reviewer identity rule exists.
- Document verification/rejection and binary upload: no existing storage abstraction is implemented.
- Original-paper custody: no schema representation exists.
- No-receipt exemption: no schema state or approved rule exists.
- Claim payment, write-off, adjustment, and final settlement: separate financial workflows.
- OCR, receipt similarity/fraud scoring, notifications, offline synchronization, and project financial closure.

## Safety statement

No PostgreSQL table, column, index, trigger, function, enum, or constraint changed. No migration or initialization call was added. Generated Database-First files remain untouched. Automated API tests use a fake expense service and do not invoke a real expense write endpoint.

## Flutter consumption

Flutter consumes every safely usable Phase 1 route with a feature-first `features/expenses/{domain,data,presentation}` implementation. The authenticated shell and router expose `/expenses`, `/expenses/new`, `/expenses/:expenseId`, metadata documents, history, review, category list/Manager creation, reimbursement list, and reimbursement detail. Manager/Deputy/Accountant review; Accountant cannot create; Supervisor creation can use only API-visible assigned projects; Worker has no project selector; unknown roles are denied.

Lists use page 1, page size 20, exact allow-listed status/payment/reference filters, refresh, stable ID deduplication, and recoverable load-more failures. Reimbursements, documents, and history remain paged and read-only. Detail displays safe DTO fields and never exposes company IDs, storage paths, hashes, raw audit JSON, or ledger internals.

Creation offers only `AdvanceBalance` and `PersonalFunds`. It accepts at most one direct project and validates category scope before submission while the API remains authoritative. Advance allocations use one or more unique active caller-owned balances, one currency, positive exact decimal strings, and integer-minor-unit equality with the expense total. Personal funds create an open unpaid claim; Flutter exposes no claim-payment action.

The authoritative advance-balance read originally omitted `userAdvanceBalanceId`, although expense creation requires it. The minimal backend correction adds that existing UUID to `AdvanceBalanceSummary` and its projection plus an OpenAPI assertion. No PostgreSQL or generated EF file changed.

Expense creation, approval, and rejection reuse the existing ephemeral 32-byte operation-key session. No write retries automatically. A timeout/network result is shown as uncertain; explicit retry reuses the same key only for the identical payload, while changed payload, success, definitive rejection, cancellation, or disposal clears it.

Attachment metadata creation is deliberately not exposed in Flutter. The backend contract requires an application-relative stored-file path and SHA-256 checksum, but this phase has no binary storage/upload provider that can safely produce them. Flutter lists metadata and states clearly that no file was uploaded. Binary upload, document verification, paper custody, supplier credit/debt, mixed payments, project splits, correction/cancellation/reversal, claim payment, and final settlement remain absent.
