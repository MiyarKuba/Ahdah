# Project status

## Project

Ahdah — عُهدة

## Current phase

Suppliers and Supplier Debt Foundation — Backend Phase 1

## Date, workspace, and branch

- Date: `2026-08-04`.
- Workspace: `C:\dev\Ahdah`.
- Branch: `feat/suppliers-foundation`.
- Pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` remains untouched and unstaged.

## Actual schema findings

- No `supplier_invoices`, payable, accounts-payable, supplier-balance, or supplier-statement table/view exists.
- Invoice header: `expenses` with exact payment mode `SupplierCredit`; optional lines: `expense_items`; unique liability: `supplier_debts` by `(company_id, expense_id)`.
- Debt balance authority: database-generated `supplier_debts.outstanding_amount = debt + adjustment - paid - credit-note - written-off`.
- Payment history: `supplier_payments`, debt allocations, funding allocations, immutable debt ledger, and immutable funding ledger.
- Funding attribution: existing `funding_sources` with `manager_contributions` and `company_cashbox_entries` subtype lineage. No supplier-payment relationship to advance balances.
- Credits: `supplier_credit_notes` and unique note/debt allocations.
- Refunds: `supplier_refunds` requires an `expense_returns` row and one-to-one `SupplierRefund` funding source, so writes are deferred.
- Shared safeguards: `company_settings`, `audit_logs`, `idempotency_records`, tenant composite foreign keys, and version fields.

## Exact schema values

- Supplier lifecycle: `is_active`; transaction modes: `CashOnly`, `CreditOnly`, `CashAndCredit`.
- Supplier types: `GeneralSupplier`, `MaterialsSupplier`, `EquipmentSupplier`, `EquipmentRental`, `FuelSupplier`, `Subcontractor`, `TransportProvider`, `MaintenanceProvider`, `ServiceProvider`, `Other`.
- Account types: `BankAccount`, `MobileWallet`, `CashCollection`, `Other`; verification: `PendingVerification`, `Verified`, `Rejected`.
- Expense statuses: `Draft`, `PendingReview`, `CorrectionRequired`, `Approved`, `Rejected`, `Cancelled`, `Reversed`.
- Debt statuses: `Open`, `PartiallySettled`, `Settled`, `Cancelled`, `Reversed`.
- Payment statuses: `Draft`, `PendingApproval`, `Confirmed`, `Rejected`, `Cancelled`, `Reversed`.
- Refund statuses: `PendingVerification`, `Confirmed`, `Cancelled`, `Reversed`.
- Credit statuses: `Draft`, `PendingApproval`, `Approved`, `Rejected`, `Cancelled`, `Reversed`.
- Payment/refund methods: `Cash`, `BankTransfer`, `Cheque`, `Card`, `MobileWallet`, `Other`.
- Funding types: `ProjectOwnerPayment`, `ManagerContribution`, `CompanyCashbox`, `ReturnedAdvance`, `SupplierRefund`, `Other`.
- Funding statuses: `PendingVerification`, `Available`, `PartiallyUsed`, `FullyUsed`, `Cancelled`, `Reversed`.

## Implemented workflows and endpoints

- Directory: paged/filterable `GET /api/v1/suppliers`, detail, Manager creation, versioned metadata update, and history-preserving deactivation.
- Masked accounts: list/create under `/api/v1/suppliers/{supplierId}/payment-accounts`; new accounts are `PendingVerification` and never default.
- Invoices/debts: list/detail under `/api/v1/supplier-invoices` and `/api/v1/supplier-debts`; idempotent invoice creation atomically writes supplier-credit expense, optional lines, open debt, ledger, and audit.
- Payments: list/detail/create plus `/confirm` and `/reject`; partial/full multi-debt and multi-source allocation, pending reservations, exact totals, overpayment prevention, proof/threshold/separation settings, and source consumption/release.
- Credits: list/detail/create/approve/allocate with available-credit, debt, pending-payment, supplier, tenant, and currency safeguards.
- Refunds: safe paged read-only `GET /api/v1/supplier-refunds`.
- Balances/history: per-currency computed debt summaries and immutable-ledger statement at `/api/v1/suppliers/{supplierId}/statement`.

## Role matrix

| Role | Directory/invoice reads | Financial reads | Supplier manage | Invoice create | Payment record/review | Credit manage |
|---|---|---|---|---|---|---|
| Manager | Company | Company | Yes | Yes | Yes | Yes |
| Deputy | Company | Company read-only | No | Yes | No | No |
| Accountant | Company | Company | No | Yes | Yes | Yes |
| Supervisor | Assigned project only | No | No | No | No | No |
| Worker/unknown | None | None | No | No | No | No |

Policies are backed by tenant and record-level predicates. Manager-personal funding requires both `ManagerContribution` subtype and the authenticated Manager owner. Company funding retains existing source subtype. Advance-balance funding is unavailable because the schema has no relationship.

## Financial safety

- C# `decimal` only; quantity `NUMERIC(18,3)`, money `NUMERIC(18,2)`; Phase 1 supplier payment fee is zero.
- Client cannot supply tenant, actor/payer/reviewer, paid/outstanding/available balance, status, version destination, ledger IDs, or timestamps.
- Explicit PostgreSQL transactions cover every multi-table command. Tenant-scoped `FOR UPDATE` locks protect suppliers, debts, payments, credits, and funding sources in stable UUID order.
- Pending payment allocations count against debt before confirmation; funding availability moves to reserved. Confirmation consumes reservations and appends `PaymentApplied`; rejection releases reservations without deleting allocations.
- Credit allocation subtracts pending payment reservations and cannot exceed note availability or debt.
- Existing 16–200 character `Idempotency-Key` fingerprint/replay mechanism is reused for financial/lifecycle commands. Writes are not retried automatically.
- Supplier/payment-account/debt/payment/refund/credit/expense/funding version fields are concurrency tokens. Concurrency and PostgreSQL `23505` map to HTTP 409.

## Deferred workflows and limitations

- Supplier refund POST/verification: blocked on approved expense-return lifecycle and maximum-return rules.
- Payment-account verification/rejection/default/update/deactivation.
- Supplier reactivation/hard delete; debt adjustment/write-off/cancellation/reversal.
- Payment cancellation/reversal/correction; credit rejection/cancellation/reversal.
- Duplicate invoice-number matching: no database constraint; idempotency covers exact command retries only.
- Advance-balance supplier funding and final advance/project settlement.
- Binary upload, OCR, notifications, exchange rates, accounting export, bank/gateway integration, and Flutter supplier UI.

## Verification

- Packages added or changed: none.
- `dotnet restore backend\Ahdah.sln`: passed; all projects up to date.
- `dotnet build backend\Ahdah.sln`: passed, 0 warnings, 0 errors.
- `dotnet test backend\Ahdah.sln`: passed 256 total, 0 failed, 0 skipped (142 unit, 114 integration).
- OpenAPI supplier assertion: passed in integration tests; explicit DTOs are published and generated entities are absent.
- PostgreSQL catalog inspection used a transaction set `READ ONLY` and rolled back. No supplier write endpoint or data mutation was executed.
- PostgreSQL schema changes: none. Generated Database-First changes: none. Migrations: none.
- `EnsureCreated`, `EnsureDeleted`, and `Database.Migrate`: not introduced.
- Flutter source and `devtools_options.yaml`: untouched.
- Staging, commit, and push: not performed.

## Exact recommended next task

Implement the Flutter Suppliers, Invoices, Supplier Debt, Payments, Refunds, Credit Notes, and Supplier Statement UI using the completed Suppliers APIs, without implementing final settlement yet.
