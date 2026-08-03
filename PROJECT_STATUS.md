# Project status

## Project

Ahdah — عُهدة

## Current phase

Advances Foundation — Backend Phase 1

## Date

2026-08-03

## Workspace and branch

- Workspace confirmed before edits: `C:\dev\Ahdah`.
- Branch confirmed before edits: `feat/advances-foundation`.
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

- Restore: passed for `backend\Ahdah.sln`.
- Build: passed with 0 warnings and 0 errors.
- Tests: passed 176 total (104 unit and 72 integration), 0 failed and 0 skipped.
- OpenAPI: development `/openapi/v1.json` built and was verified by the passing integration suite; advance routes/DTOs are present and generated EF entities are absent.
- PostgreSQL mutation: none. Only read-only catalog/configuration queries were executed; no real write endpoint was called.
- Generated EF files: unchanged.
- Migrations: none created or run.
- Flutter: no Flutter source or command was touched; the pre-existing untracked devtools file remains untouched.

## Known limitations

- Existing funding sources must be created and verified by a future focused source-management workflow before an advance can be created.
- No configured company number sequence exists; references are opaque and not chronological.
- Return initiation rejects ambiguous multi-sender lineage rather than inventing FIFO routing.
- Movement output projects safe allocation/transfer history, not raw ledger internals.
- Settlement status is not collapsed into one advance-level field because settlements are balance-specific.
- iOS, Android, and Web Flutter Advances UI is not implemented in this phase.

## Exact recommended next task

Implement the Flutter Advances and User Balance UI using the completed Advances APIs, without implementing expenses or settlement workflows yet.
