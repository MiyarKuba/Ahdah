# Two-session race rehearsals — future isolated staging tests

**Not executed.** Use only the dedicated synthetic rehearsal database described in REHEARSAL.md, with separately authorized upgraded writers and valid Policy v1 fixtures. Two physical connections are mandatory. Do not use deployed company/project IDs. The schema installation does not implement financial commands or retries.

## Session setup and synchronization

Use fixture tenant C and project UUIDs A < B by PostgreSQL UUID order. Both sessions validate `current_database() LIKE 'ahdah_settlement_rehearsal_%'`. Enable `ON_ERROR_STOP`, `lock_timeout='10s'`, `statement_timeout='30s'`; a lock timeout is a test observation, not a successful command. Record application command boundaries and use a debugger/test barrier immediately after each indicated lock. Do not commit a partial close or emulate a successful close by setting projects.status.

For a certifier, the transaction starts as:

```sql
BEGIN ISOLATION LEVEL SERIALIZABLE;
-- Claim same logical idempotency key/hash in the real upgraded command.
-- Acquire company/membership/policy admission locks in reviewed order first.
SELECT project_id, financial_revision, version_number, financial_state
FROM ahdah.project_financial_controls
WHERE company_id = :'fixture_company_id'::uuid
  AND project_id = ANY (ARRAY[:'project_a'::uuid, :'project_b'::uuid])
ORDER BY project_id FOR UPDATE;
-- PAUSE AT BARRIER; continue the entire command under these locks.
```

For ordinary writers use READ COMMITTED and the same gate order. Policy activation locks the company binding exclusively before any project gate. Every command discovers/rechecks the complete affected project set. A changed set requires restart, never late acquisition of an earlier-ranked lock. Use actual upgraded command test hooks; these lock snippets alone are not the command.

## A. Closure versus new expense (F16)

* Seed A Completed, fully evaluated/approved at revision R, no blockers. Session 1 begins close, holds A gate, pauses before evaluator. Session 2 attempts expense create against A: it must block before inserting expense/reservation/claim/debt.
* Release session 1 to complete certificate/control/project/audit/idempotency commit. Session 2 resumes, rechecks state and rejects FinanciallyClosed; no expense or partial ledger/reservation survives.
* Reset seed. Session 2 takes gate first, posts expense and revision R+1. Session 1 starts from expected R and waits: serialization retry or business conflict occurs; no certificate for R may commit. A retry must reevaluate and find the new expense or stale approval.

## B. Closure versus supplier invoice/debt

Repeat A using SupplierService.CreateInvoiceAsync's future upgraded implementation. Require both the new expense and supplier_debt creation to wait on A before inserts. Closure-first rejects the entire invoice, including idempotency/audit outcome semantics; invoice-first increments A once and invalidates the packet. Test zero existing debt rows: locking only existing debts must not pass the test.

## C. Closure versus project custody transfer

Seed an otherwise eligible project using a transfer that will change an explicitly designated slice; run a closing attempt with a packet that precedes this change. Session 1 holds A gate; session 2 reserves/confirms A-designated custody. Closing must itself reject if current A custody/reservations are blockers; it must never certify a nonzero unresolved custody position. Also use a valid zero-custody closed fixture and attempt a newly designated inbound transfer: close-first blocks/rejects transfer, transfer-first invalidates approval and close fails. For shared A/B transfer, acquire both gates in UUID order; a closed participant rejects the whole transfer, not just its leg. Unassigned-only unrelated transfers do not lock A.

## D. Two simultaneous closure attempts (F15)

Both sessions target current approved A/R. With different keys, first gate holder closes once; second waits then returns conflict or serialization-retries into closed-state rejection. Assert one active T01 pointer, one certificate and one winning event/audit/result chain. With the same key and payload, the unique idempotency record serializes work and both responses identify the same certificate. Same key/different payload conflicts. Inspect both old/new transaction IDs on retry; never continue the failed transaction.

## E. Cross-project operations requested in opposite order

Session 1 requests A→B designation reallocation; session 2 B→A. Both must sort A then B before balance/slice locks, independently of request direction. Pause session 1 after A lock; session 2 waits on A and must not hold B. Release session 1, then session 2; both reconcile if funds/versions remain valid, otherwise the stale request conflicts atomically. Also reverse sender/recipient balance input order and verify balances sorted by UUID. Inject a legacy out-of-order writer to prove any deadlock yields 40P01/full rollback, never balancing repair.

## F. Reopen versus new close

Seed Closed A with certificate C and revision R. Session 1 holds A for Manager reopen; session 2 attempts close using old approved cycle/expected R. Reopen commits immutable reopening record, new Draft cycle and R+1, returns normal project to Completed (Cancelled stays Cancelled). Session 2 cannot reuse old approval: retry/conflict, no new certificate. Inverse order: a new close against already Closed rejects; then reopen can proceed. Same reopen key replays; competing distinct reopen attempts produce one T11 per previous certificate. Reclosing is possible only after new complete reconciliation/independent approval.

## G. Policy activation versus closure

Seed active binding v1 and approved packet v1/R. Session 1 closes while holding binding FOR SHARE and A gate; session 2's activation UPDATE waits for the shared binding lock. Closure may commit v1 before activation; immutable certificate continues resolving v1 afterward. Reverse order: activation commits v2 first; close with packet v1 rejects policy mismatch even though revision R is unchanged. Serializable retry reacquires binding and membership and reevaluates; no cached v1 authority. Activation never locks projects first or bulk rewrites old certificates.

## Serializable retry contract

On SQLSTATE **40001** or **40P01**, abort the entire transaction, dispose tracked state and perform a bounded full restart (proposed maximum three attempts; operational retry budget must be confirmed when implementing commands). Preserve the same logical key and payload. Revalidate membership, role/conflict authority, policy, versions and evaluator on every attempt. No independently committed pending idempotency stub, certificate, snapshot or audit/result exists between failed attempts. A semantic version mismatch is a conflict, not permission to change payload and retry. Do not automatically retry permission failures, invalid requests, uniqueness violations representing conflicting commands, or unknown commit outcomes with a new key. For uncertain commit outcome, query/replay using the original key after authorized access validation.

## Evidence required

Record both session timelines and gate order, blocking interval, SQLSTATE/retry count, winning certificate/result ID, source versions before/after, exact currency-separated totals, no-partial-write assertions and role/policy snapshot. Test both acquisition orders for A/B/C/G. No race is marked passed on the basis of static SQL parsing.
