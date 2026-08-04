# Project status

## Project

Ahdah — عُهدة

## Current phase

Flutter Advances and User Balances

## Date

2026-08-04

## Workspace and branch

- Workspace confirmed before edits: `C:\dev\Ahdah`.
- Branch confirmed before edits: `feat/flutter-advances-balances`.
- One pre-existing untracked Flutter file, `frontend/ahdah_app/devtools_options.yaml`, was present and left untouched.

## Actual schema findings

- `advances` is the top-level Manager-to-Deputy record. Exact statuses are `Draft`, `PendingConfirmation`, `Open`, `InSettlement`, `ReadyToClose`, `Closed`, `Cancelled`, and `Reversed`.
- `advance_funding_sources` allocates existing `funding_sources`; multiple unique sources may fund one fixed advance amount.
- Funding-source types are `ProjectOwnerPayment`, `ManagerContribution`, `CompanyCashbox`, `ReturnedAdvance`, `SupplierRefund`, and `Other`. Statuses are `PendingVerification`, `Available`, `PartiallyUsed`, `FullyUsed`, `Cancelled`, and `Reversed`.
- Funding payment methods are `Cash`, `BankTransfer`, `Cheque`, `Card`, `MobileWallet`, and `Other`.
- `money_transfers` represents `AdvanceDelivery`, `InternalTransfer`, and `BalanceReturn`. Transfer methods add `BalanceTransfer`; statuses are `Draft`, `PendingConfirmation`, `CorrectionRequired`, `Confirmed`, `Rejected`, `Cancelled`, and `Reversed`.
- `transfer_advance_allocations` links distributions/returns to an advance. Distribution does not create a child advance.
- `user_advance_balances` is authoritative. Exact statuses are `Active`, `InSettlement`, `Settled`, and `Closed`.
- `balance_ledger_entries` and `funding_source_ledger_entries` preserve immutable history. PostgreSQL blocks ledger update/delete. No trigger or function performs balance mutations.
- No `AdvanceDistribution`, `AdvanceReturn`, `AdvanceApproval`, or direct `AdvanceBalance` table exists.
- Advances have no direct project, parent-advance, or current-balance field.
- Company number-sequence tables contained no configured rows during read-only inspection.

## Workflows implemented

- Paginated tenant/role-filtered advance listing, detail, and movement history.
- Manager-only safe available-funding-source listing.
- Atomic Manager top-level creation for an active same-company Deputy using one or more existing usable same-currency sources whose allocations exactly total the advance.
- Pending initial `AdvanceDelivery`; confirmation consumes funding reservation, creates the Deputy balance, appends ledgers, and opens the advance.
- Partial held-balance distribution: Manager to Deputy, or Deputy to Supervisor/Worker.
- Recipient confirmation with paired sender/recipient authoritative balance and immutable ledger effects.
- Recipient rejection for pending internal distribution and balance return, with reservation release and preserved transfer history.
- Unused-money return to exactly one upstream sender derived from confirmed lineage; no client destination is accepted.
- Current-user balance and Manager/Deputy/Accountant same-company user-balance lookup.
- Tenant-scoped idempotency key/fingerprint/original-response replay records for every financial command.
- Flutter role-aware advance navigation, guarded routes, paginated list/detail/movement history, and authoritative balance pages for Android, iOS, and Web.
- Manager top-level creation with active-Deputy selection, usable funding sources, and exact multi-source allocation validation.
- Manager/Deputy held-balance distribution with the approved recipient chain, expected-recipient confirmation, supported internal/return rejection, and lineage-derived unused-money returns.
- Exact decimal-string response/request handling without `double`, plus 32-byte secure ephemeral `Idempotency-Key` generation and explicit same-payload retry after ambiguous network/timeout outcomes.
- Arabic RTL and English LTR financial labels, forms, states, confirmations, and safe unknown-value fallbacks.

## Workflows omitted and reasons

- New funding-source creation/verification: each type has separate subtype and verification semantics not approved in this phase.
- Separate post-creation funding: funding is complete and atomic at creation because the advance amount is fixed and confirmation requires complete allocation.
- Project association/filter: no direct schema relationship exists, and funding-source project origin does not prove a multi-source advance belongs to one project.
- Supervisor distribution: the database can store it, but product authority is not explicitly approved.
- Initial advance-delivery rejection: the advance has no rejected state and its unique delivery could not be safely reissued.
- Transfer correction/cancellation/reversal: separate reviewed lifecycles remain unimplemented.
- Expense allocation, receipts, supplier debt, and claims: outside Phase 1.
- Settlement and closure writes: depend on expense snapshots, documents, difference resolutions, supplier debt, claims, reconciliation, and review. Zero cash balance is not treated as financial settlement.
- FIFO settlement: deferred with expenses/settlement.

## Endpoints

- `GET /api/v1/advances`
- `GET /api/v1/advances/{advanceId}`
- `GET /api/v1/advances/{advanceId}/movements`
- `GET /api/v1/advance-funding-sources`
- `POST /api/v1/advances`
- `POST /api/v1/advances/{advanceId}/distributions`
- `POST /api/v1/advances/{advanceId}/returns`
- `POST /api/v1/advance-transfers/{transferId}/confirm`
- `POST /api/v1/advance-transfers/{transferId}/reject`
- `GET /api/v1/advance-balances/me`
- `GET /api/v1/advance-balances/users/{userId}`

All command routes require a 16–200 character `Idempotency-Key` header.

## Role visibility and capabilities

| Role | Visibility | Balances | Writes |
|---|---|---|---|
| Manager | All company advances/movements | Own and all same-company users | Create, distribute held money to Deputy, confirm, return |
| Deputy | All company advances/movements | Own and all same-company users | Distribute held money to Supervisor/Worker, confirm, return |
| Accountant | All company advances/movements | Own and all same-company users | None |
| Supervisor | Personal participation only | Own only | Confirm and return |
| Worker | Personal participation only | Own only | Confirm and return |
| Unknown | None | None | None |

Policies added: `AdvanceViewer`, `AdvanceCreator`, `AdvanceDistributor`, `AdvanceParticipant`, and `AdvanceBalanceViewer`. Every persistence query still applies `company_id` and record-level predicates.

## Financial integrity

- Money precision: PostgreSQL `NUMERIC(18,2)` and C# `decimal` only; maximum `9999999999999999.99`.
- Balance authority: `user_advance_balances`; the API does not accept or reconstruct client balances.
- Funding authority: `funding_sources.available_amount`/`reserved_amount`/`used_amount` under row lock.
- Transactions: every command uses an explicit PostgreSQL transaction.
- Locking: tenant-filtered `FOR UPDATE` locks protect funding sources, transfers, advances, and user balances before lifecycle/availability validation.
- Ledger behavior: append only; no update, delete, repair, or recalculation code.
- Concurrency: `advances`, `funding_sources`, `money_transfers`, and `user_advance_balances` versions are EF concurrency tokens. Concurrency and SQLSTATE `23505` map to HTTP 409.
- IDs/timestamps: configured primary UUIDs and timestamps are PostgreSQL-generated. Opaque `ADV-<uuid>`/`TRF-<uuid>` display references are used because no sequence is configured.
- Idempotency: operation/payload/actor fingerprint, resource identity, and original safe response share the financial transaction; key reuse with a different request conflicts.

## Packages changed

None.

## Verification status

- Flutter format check: passed for `lib` and `test` after generation.
- Flutter analyze: passed with no issues.
- Flutter tests: passed 137 total, including the pre-existing 104 and 33 focused Advances tests; 0 failed.
- Flutter Web release build: passed with `API_BASE_URL=http://localhost:5231`; artifact generated under `frontend/ahdah_app/build/web`. The compiler emitted a non-blocking missing Cupertino icon-font warning from outside application source; no application source references `CupertinoIcons`.
- Android debug APK build: passed with `API_BASE_URL=http://10.0.2.2:5231`; artifact generated at `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`. The application was not launched on an emulator or device.
- iOS: source compatibility inspected; build, signing, simulator, and device verification remain pending macOS/Xcode.
- Backend: unchanged. Backend restore/build/tests were not rerun; the existing recorded result remains 176 passed from Backend Phase 1.
- OpenAPI: existing `AdvanceApiTests` development-document verification was inspected; no backend/OpenAPI contract changed.
- PostgreSQL mutation: none. No real financial endpoint or database command was invoked.
- Generated EF files: unchanged.
- Migrations: none created or run.
- Packages: no dependency added or changed.
- The pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` remains untouched.

## Known limitations

- Existing funding sources must be created and verified by a future focused source-management workflow before an advance can be created.
- No configured company number sequence exists; references are opaque and not chronological.
- Return initiation rejects ambiguous multi-sender lineage rather than inventing FIFO routing.
- Movement output projects safe allocation/transfer history, not raw ledger internals.
- Settlement status is not collapsed into one advance-level field because settlements are balance-specific.
- The authorized user-balance API requires a user ID. Manager/Deputy use the safe company directory; Accountant has no directory discovery, so Flutter exposes personal balances but no raw-ID lookup UI for Accountant.
- Funding-source creation/editing remains unavailable; Manager creation depends on usable sources already returned by the API.
- iOS compilation/signing and runtime verification require macOS/Xcode.
- No live financial workflow was exercised; automated tests use fakes and controlled Dio adapters.

## Exact recommended next task

Design and implement the backend Expenses foundation using the existing expense, category, allocation, payment, attachment, approval, receipt-original-status, reimbursement, and audit tables without modifying the PostgreSQL schema.
