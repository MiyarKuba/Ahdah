# Advances foundation — backend Phase 1

## Scope and database findings

Phase 1 uses the existing PostgreSQL 18 `ahdah` schema without a migration or generated-model edit. Read-only catalog inspection confirmed these primary records:

- `advances`: one top-level company advance issued to `deputy_user_id`. It has no `project_id` and no parent-advance column.
- `advance_funding_sources`: immutable-by-application allocations from one or more existing `funding_sources` to an advance. `(company_id, advance_id, funding_source_id)` is unique.
- `funding_sources`, `funding_source_payment_methods`, and `funding_source_ledger_entries`: authoritative source availability, exact payment methods, and immutable source history.
- `money_transfers`: initial delivery, internal distribution, and balance return. Exactly one `AdvanceDelivery` may reference an advance directly.
- `transfer_advance_allocations`: associates `InternalTransfer` and `BalanceReturn` records with an advance. It does not create a child advance.
- `user_advance_balances`: authoritative per-user, per-advance balance. `(company_id, advance_id, user_id)` is unique.
- `balance_ledger_entries`: immutable balance history. PostgreSQL prevents update and delete.
- `idempotency_records`: tenant-scoped financial-command deduplication, resource identity, and original safe response replay.
- `advance_settlements`, `advance_settlement_resolutions`, settlement documents, `advance_closures`, and closure documents: inspected but not written in this phase.

There are no generated entities or base tables named `AdvanceDistribution`, `AdvanceTransfer`, `AdvanceReturn`, `AdvanceApproval`, or `AdvanceBalance`. Distribution, return, confirmation, and balance behavior is represented by the records above. No PostgreSQL function performs these workflows and no balance trigger updates totals; the application therefore writes the authoritative balance row and its immutable ledger entry in one explicit transaction.

## Exact values

Advance statuses are:

- `Draft`
- `PendingConfirmation`
- `Open`
- `InSettlement`
- `ReadyToClose`
- `Closed`
- `Cancelled`
- `Reversed`

Funding-source types are:

- `ProjectOwnerPayment`
- `ManagerContribution`
- `CompanyCashbox`
- `ReturnedAdvance`
- `SupplierRefund`
- `Other`

Funding-source statuses are `PendingVerification`, `Available`, `PartiallyUsed`, `FullyUsed`, `Cancelled`, and `Reversed`. Only `Available` and `PartiallyUsed` sources with positive authoritative availability may be allocated by Phase 1.

Funding-source payment methods are `Cash`, `BankTransfer`, `Cheque`, `Card`, `MobileWallet`, and `Other`. Money-transfer methods add `BalanceTransfer` to that set. Bank transfer and cheque require bank and reference data; card and mobile wallet require a reference; `Other` requires a description.

Money-transfer types are `AdvanceDelivery`, `InternalTransfer`, and `BalanceReturn`. Their statuses are `Draft`, `PendingConfirmation`, `CorrectionRequired`, `Confirmed`, `Rejected`, `Cancelled`, and `Reversed`.

User-balance statuses are `Active`, `InSettlement`, `Settled`, and `Closed`. All money columns used here are `NUMERIC(18,2)` and map only to C# `decimal`. Currency is derived from the allocated funding sources; allocations with different currency codes are rejected. No exchange or rounding rule is invented.

## API

Reads:

- `GET /api/v1/advances`
- `GET /api/v1/advances/{advanceId}`
- `GET /api/v1/advances/{advanceId}/movements`
- `GET /api/v1/advance-balances/me`
- `GET /api/v1/advance-balances/users/{userId}`
- `GET /api/v1/advance-funding-sources`

Commands:

- `POST /api/v1/advances`
- `POST /api/v1/advances/{advanceId}/distributions`
- `POST /api/v1/advances/{advanceId}/returns`
- `POST /api/v1/advance-transfers/{transferId}/confirm`
- `POST /api/v1/advance-transfers/{transferId}/reject`

Collection routes use `page` starting at 1, default page size 20, and maximum 100. Advance listing accepts exact `status`, bounded reference-prefix search, and an authorized user filter. Tenant, role, and record visibility predicates execute before count and pagination. Ordering is stable.

Every command requires a 16–200 character `Idempotency-Key` header. The key is unique per company. The stored fingerprint binds operation and payload to the authenticated actor. A completed identical request replays the original safe response; another operation, actor, or payload using the same key returns conflict. Financial writes are not automatically retried.

## Role and record visibility

| Role | Advance visibility | Balance visibility | Commands |
|---|---|---|---|
| Manager | All company advances and movements | Own and any same-company user | Create, distribute held balance to Deputy, confirm, return |
| Deputy | All company advances and movements | Own and any same-company user | Distribute held balance to Supervisor or Worker, confirm, return |
| Accountant | All company advances and movements | Own and any same-company user | Read-only |
| Supervisor | Personally received/held/participated advances only | Own only | Confirm and return; no Phase 1 distribution |
| Worker | Personally received/held/participated advances only | Own only | Confirm and return |
| Unknown | None | None | None |

Authorization policies are only outer gates. Every query also filters `company_id`; personal reads use balance and transfer participation predicates. Cross-tenant and record-invisible IDs return a non-enumerating unavailable result.

## Creation and funding

Only Manager creates a top-level advance. The authenticated company and creator are never accepted from the client. The recipient must be an active same-company exact `Deputy` because `advances.deputy_user_id` is required.

Creation accepts one or more unique IDs of existing verified/usable funding sources. Allocations must be positive, fit `NUMERIC(18,2)`, have one currency, and total the advance amount exactly. Each source row is locked before availability validation. Creation atomically:

1. creates a `PendingConfirmation` advance;
2. creates its `AdvanceDelivery` transfer from Manager to Deputy;
3. records every `advance_funding_sources` allocation;
4. moves each source amount from available to reserved;
5. appends an immutable `AmountReserved` funding ledger entry.

The manager-only available-source read exposes safe source IDs, types, currency, available amount, status, and payment-method names needed by the creation contract. Phase 1 does not create new source records: each source type has a separate subtype and verification workflow, and those product rules remain unapproved. For the same reason there is no separate funding command. Funding is complete and atomic with creation; the fixed advance amount cannot be partially funded.

The schema's company number-sequence tables contain no configured rows. Phase 1 therefore uses opaque unique `ADV-<uuid>` and `TRF-<uuid>` references without implying chronology. PostgreSQL still generates configured record UUID primary keys and timestamps.

## Confirmation, distribution, and return

The initial Deputy confirms `AdvanceDelivery`. Confirmation locks the transfer, advance, and funding sources. It converts source reservation to confirmed use, appends `AdvanceFunding` ledgers, creates the Deputy's authoritative balance and `BalanceCreated` ledger, and changes the advance to `Open`. Only the exact recipient may confirm. Repeating a completed confirmation with the same idempotency fingerprint is safe.

Distribution is allowed only from a caller's own `Active` balance on an `Open` advance. The balance row is locked before checking availability. Initiation moves available to reserved and appends `TransferReserved`; it creates a pending `InternalTransfer` and one advance allocation. Recipient confirmation converts sender reservation to transferred-out and creates or increments recipient received/available totals, with paired immutable `TransferOutConfirmed` and `TransferInConfirmed` ledger entries.

Unused-money return derives the destination from confirmed incoming lineage. It proceeds only when exactly one distinct upstream sender exists; no client destination is accepted. Initiation reserves the holder's available balance with `BalanceReturnReserved`. Confirmation increments sender returned totals and upstream recipient restored/available totals with `BalanceReturnConfirmed` and `AmountRestored` entries.

Recipients may reject pending `InternalTransfer` and `BalanceReturn` records. Rejection releases the sender reservation through `TransferReservationReleased` and preserves the transfer as `Rejected`. Initial `AdvanceDelivery` rejection is omitted: `advances` has no rejected state, and rejecting its unique delivery would leave an unresolvable pending advance. Cancellation/reissue needs a separately approved lifecycle.

## Balance and movement authority

`user_advance_balances` is the current-balance authority. Reads do not reconstruct it from transfers. `balance_ledger_entries` and `funding_source_ledger_entries` are immutable audit history; application code never updates or deletes them. Movement responses project safe funding allocations and transfer records, rather than exposing raw ledger implementation columns.

The service increments `version_number` on every changed advance, funding source, money transfer, and user balance. Those mapped fields are EF Core concurrency tokens. PostgreSQL row locks serialize availability/lifecycle checks; `DbUpdateConcurrencyException` and SQLSTATE `23505` map to safe HTTP 409 responses.

## Project, settlement, and closure decisions

No advance route accepts `project_id`, and no project filter is published. The `advances`, `money_transfers`, allocations, and balance tables have no direct project relationship. A funding source may originate from a project-owner payment, but that does not prove the whole advance belongs to one project because an advance may use multiple sources.

Settlement and closure commands are deferred. Expenses now reserve authoritative balance availability with `ExpenseReserved`, convert it to expensed totals with `ExpenseConfirmed` on approval, and release it with `ExpenseReservationReleased` on rejection. Settlements still require reviewed snapshots, documents, difference resolution, cash recovery, claim offset or waiver, supplier debt, pending-operation checks, and final review. Zero cash balance alone never marks an advance financially settled or closed. FIFO settlement is not implemented.

Supplier payments allocate existing `funding_sources`; the schema has no supplier-payment allocation to `user_advance_balances`. Phase 1 therefore does not spend advance balances for supplier payments or treat supplier debt payment as final advance settlement.

## Flutter consumption

Flutter now consumes every safely supported Phase 1 route with handwritten immutable models and the existing authenticated Dio/session stack. Stable routes cover advance list/create/detail/movements/distribution/return plus personal and selected-user balances. Navigation and router guards use centralized exact-role capabilities, while record actions also require authoritative loaded state and exact current-user relationships.

The list applies server-side status, bounded reference, and safely selected user filters with page size 20, refresh, stable deduplication, and recoverable load-more errors. Detail renders exact advance, funding, and balance DTO fields; Supervisor/Worker balance rendering remains personal. Movement history preserves API chronological order and never reconstructs running balances. Pending transfer IDs provide confirmation/rejection targets; initial `AdvanceDelivery` is confirmable but not rejectable.

Manager creation offers active Deputies and manager-only usable funding sources. Allocation amounts and the total are strict positive decimal strings with at most two fractional digits and 16 whole digits. Exact equality uses checked integer minor units, not `double`. Financial response amount tokens are preserved lexically before JSON decoding, and requests emit validated decimal JSON numbers. Different currencies are never added.

Manager distributes to active Deputies; Deputy distributes to active Supervisors/Workers. Return accepts no recipient because the server derives one unambiguous upstream sender. Personal and authorized balance pages display authoritative server balance rows. Manager/Deputy user lookup uses the directory; Accountant user lookup is not exposed through raw ID because Accountant has no safe directory discovery endpoint.

Every command creates 32 cryptographically secure random bytes and sends unpadded URL-safe Base64 in `Idempotency-Key`. The value exists only in auto-disposed controller memory. No automatic retry exists. Network/timeout results are explicitly uncertain and permit only an explicit retry of the identical payload with the same key. Success, definitive 4xx, cancellation, payload replacement, or disposal clears it. It is never shown, logged, or persisted.

Arabic/English localization covers routes, values, forms, errors, confirmation summaries, and uncertain operations. The same responsive Material 3 pages support Android, iOS, tablet, and Web. Automated tests use fakes/controlled Dio only and make no live financial call.

Expense-backed balance reservation/confirmation/release is now implemented by the backend. Flutter expenses, settlement, closure, cancellation, reversal, supplier debt, arbitrary balance adjustment, funding-source mutation, offline write queues, and background synchronization remain absent.

## Safety statement

This phase adds no table, column, index, constraint, trigger, function, enum, migration, `EnsureCreated`, `EnsureDeleted`, or `Database.Migrate` call. No generated Database-First file is edited. Automated verification uses fake API services and metadata/source tests; it does not call a real write endpoint or mutate PostgreSQL.
