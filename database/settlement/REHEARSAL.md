# Settlement rehearsal plan — future isolated execution only

No fixture or DDL was executed during preparation. Use a dedicated **empty** `ahdah_settlement_rehearsal_<run>` database, prepared under separate authorization with the existing 91-table schema and this reviewed artifact set. Never clone customer data into this harness. `93_cutover_rehearsal.sql` rejects the deployed database name and any existing company rows, uses synthetic identities and rolls back all data. Fixture identities/password text are nonfunctional synthetic literals.

## Coverage boundaries

The SQL harness directly probes F01/F02/F04/F05 custody arithmetic, F24 uniqueness/nonnegativity/deferred conservation and F14/F24 packet construction/immutability. It deliberately labels structural probes separately from financial command acceptance. It does not claim to simulate the missing authorization, evaluator, reservation/ledger posting or idempotency implementations.

**All F01–F24 below are required future command-level rehearsal cases.** Before claiming them passed, the implementer must bind each Action to the corresponding upgraded W writer, record actual emitted SQL/events, run all assertions, and retain evidence. The commands do not exist at this DDL-only milestone; replacing them with unchecked direct DML would test a different system. Use savepoints/reset to the stated base for independent branches. Never retain synthetic postings in deployed Ahdah.

## Common command-level assertions

For every successful material operation assert exact decimal totals, tenant FKs, currency lineage, expected source versions, immutable movement/effect/audit rows, idempotency response and exactly one revision increment per affected project. Retry the same key/payload and assert unchanged counts/revisions; reuse key with changed payload and require conflict. For every rejected operation capture before/after source, balance, slice, cycle, ledger, audit and replay counts: no partial financial posting. Readiness cannot be inferred from these arithmetic checks alone.

## Full reviewed F01–F24 cases


Shared harness: tenant C and unrelated tenant D; projects A/B/C0; authorized Accountant P, independent Manager M, Supervisor S, holders H1/H2; fixed UTC clock; exact decimal amounts; request keys unique unless replay is tested; all unrelated mandatory categories explicitly evaluated clear, policy v1/configuration known, evidence verified and project basis Completed unless specified. Each test asserts tenant scope, operation/event/audit linkage, expected versions, one material revision per affected project per command, and rollback leaves no partial entries. F01 creates the numerical baseline; F02–F08 explicitly state their base. Race tests require two independent PostgreSQL connections and barriers at gate acquisition, not an in-memory substitute.

| Fixture | Setup and command | Required results |
|---|---|---|
| F01 | Confirm 20,000 LYD custody X/H1; P proposes A10,000/B5,000/pool5,000; Opening enters pool20,000; M separately approves Designate A10,000 and B5,000 | Three unique slices, available sum20,000 reserved0; unassigned Opening plus paired Designate history; A/B revision increment once for posting; source funding not counted again as custody. |
| F02 | From F01 create then approve A expense6,000 | Create A4,000/6,000 and balance14,000/6,000; approval A4,000/0, balance14,000/0, expensed6,000; two commands → A revision+2. |
| F03 | From F02 reserve then confirm A transfer2,000 H1→H2 | H1 A2,000/2,000 pending, H2 A0; confirmed H1 A2,000/0,H2 A2,000/0; H1 balance12,000/H2 2,000; A total4,000; B/pool unchanged. |
| F04 | From F02 reconciled holder, no A reservation/difference; release4,000 approved M | H1 A0,B5,000,pool9,000,balance14,000; no debt reduction/cashbox receipt. Repeat with reserved1 or unresolved difference: reject entirely. |
| F05 | From F02 approved A→B reallocation1,000 | A3,000,B6,000,pool5,000; balance14,000; both revisions+1 atomically; if B closed or Manager approval absent, no change. |
| F06 | From F02 new A expense reservation1,000, then reject | Pending A3,000/1,000; rejection A4,000/0; balance14,000/0; released reservation, no expenditure increase, no surviving claim/debt created solely by rejected item. |
| F07 | From F02 approved return1,000 of original6,000 consumption | A5,000,B5,000,pool5,000; balance15,000, restored1,000; typed return/effect/original allocation; replay identical result; subsequent cumulative return above6,000 rejected. |
| F08 | F01 plus separate USD advance with A100 USD | Reports A10,000 LYD and100 USD separately; LYD transfer from USD slice rejected; no10,100 total; snapshot retains separate category/currency keys. |
| F09 | Legacy balance500 LYD uncertain between A/B; C0 completeness review proves unrelated | Conserving provisional pool500 but A/B Indeterminate, no inferred split; C0 unaffected; Manager reviewed A300/pool200 later resolves only evidenced scope via approved review and separate Designate. |
| F10 | Boundary sender balance available800,reserved200, pending transfer200 A→H2 | Unassigned Opening sender800/200 then separate reviewed Designate to A, destination no200 receipt; original transfer allocation mapped to T05; confirm produces sender800/0,recipient200/0; no1,200 total. Unknown designation keeps scoped gap. |
| F11 | Cancelled A with supplier debt300 LYD | Known blocker, cannot close. After approved payment/effects and complete run-off reconciliation, certificate financial state Closed; projects.status remains Cancelled and completed_at remains null. |
| F12 | Completed A contract10,000 LYD, receipts9,000, qualified retention1,000 with known contractual basis, undisputed release condition and approved schedule | Equation balances; immediate receivable/payable0; retention1,000 remains disclosed; settlement/closure may pass all other gates; no write-off entry. |
| F13 | F12 retention changed to disputed entitlement, or missing basis/currency/classification | Known dispute blocks; unknown mandatory classification produces gap; neither certifies closure. Do not convert retained1,000 into zero. |
| F14 | Submitted/approved snapshot at revision8; permitted new expense10 creates revision9 | Invalidated event, old approved row/hash untouched; close expected8 conflicts; new cycle/current evaluation and independent approval required. |
| F15 | Two Managers close same project revision8 simultaneously, different keys; repeat with same key | One active certificate/one winning transition; loser conflicts or serialization-retries then conflicts. Same-key identical payload replays same certificate, never second event chain. |
| F16 | Expense create and close contend on A gate in both orders | Expense first → revision changes and closure rejects stale snapshot; close first → expense rejects Closed. Never committed expense after certified revision under ordinary path. |
| F17 | Closed Completed A rev8; M reopens for late invoice50 | Old certificate immutable; T11,new Draft,InSettlement rev9,project Completed; invoice posts rev10. Repeat Cancelled: stays Cancelled. Accountant/Worker reopen denied with no state change. |
| F18 | Return supplier refund100, net100 confirmed and funding restoration posted once | Same refund/economic key/ledger cannot also restore custody100; duplicate effect target rejected, retry replays; funding up100 only, not200. Wrong project/currency/direction fail atomically. |
| F19 | Two historical Approved contract changes plus one PendingApproval for A | Evaluator reads all historical changes; pending one blocks; second pending violates existing partial unique index; no singular-navigation loss. |
| F20 | Expense has primary PendingVerification metadata, no verified bytes; paper Required without received original | Existing GET behavior unchanged; future certification detects binary/paper gaps. Verified bytes plus received event or specifically approved evidenced exception resolves only applicable gap. Missing paper policy never passes. |
| F21 | Approved advance closure with zero custody but outstanding supplier debt100 snapshot | Existing generated reconciliation may say ReadyToClose; new project evaluator still blocks debt100. No copy of old expression authorizes closure. |
| F22 | Credit100 intent A60/B40; apply30 to A debt | A unapplied30,B40,actual allocation30; no130 credit. Attempt apply B intent to A debt or allocate closed B rejects entire command; pending unallocated project intent remains visible. |
| F23 | Cross-tenant project/evidence/FK attempt, same preparer/approver, beneficiary-conflicted sole Manager | Composite FK or authorization rejects; one Accountant+independent Manager succeeds; no permanent second Manager requirement. Revocation racing approval synchronizes on admission lock. |
| F24 | Direct balance-only update, negative slice, duplicate unassigned slice, reverse lock-order multi-project race, and policy v2 activation before closure | Deferred conservation/row/key constraints reject malformed writes; sorted gates avoid AB/BA cycle; retry bounded; v1 packet cannot close under active v2, old v1 certificate remains readable. |


## Additional packet sealing schedule

1. Under one project gate, create a Draft T06 with null snapshot pointer and preallocate T07 UUID S.
2. Keep constraints deferred. Insert T08 categories for S first, in the same top-level write subtransaction.
3. Insert immutable T07 S with matching company/project/cycle and canonical bytes/hash; this seals the packet.
4. Set T06 snapshot pointer/submission actor/time/hash. Force constraints immediate; all references resolve. Commit only in an approved fixture run.
5. New transaction inserting another T08 for S must fail. UPDATE/DELETE of T07/T08 must fail.
6. Two-session variant: session A inserts categories and holds gate; session B tries to insert categories/header for S. After A seals/commits, B must reject/retry. Deferred packet validation also demands matching insertion `xmin`; never use xmin as durable certification identity. SAVEPOINTs may wrap the **whole** packet, never only one side of it.
7. Repeat with header-first category insert, missing header, no categories, other project's snapshot, other cycle's approved snapshot and wrong revision/hash: each must fail or remain uncertifiable as documented.

## Staging result record

For each F case record run/database label (no connection string), schema artifact commit, fixture seed hash, operator, UTC, isolation, input versions/idempotency fingerprint, expected/actual result, SQLSTATE, remaining rows after rollback, and pass/fail. An unrun case is **Not run**, never Pass. F15/F16 and the schedules in RACES.md require independent sessions and cannot be proved by this single-session harness.

## Clarified source/authority acceptance probes (required with F01/F09/F10/F23/F24)

For each rejection assert transaction rollback, unchanged balance/position/revision, and no posted movement. These are future upgraded-command tests, not executed results.

| Setup/action | Required result |
|---|---|
| Confirm AdvanceDelivery20,000 to H1, matching Advance and BalanceCreated ledger; post Opening | Pool20,000, project slices0, no T12 or project revision; one immutable receipt group. |
| Same source with project T02, fake/unrelated T12, missing source, unconfirmed/wrong-recipient transfer, different amount/currency or wrong ledger | Reject each independently. |
| Replay same source; attempt another approved legacy review for the already opened balance | Same idempotent command result or uniqueness rejection; never second custody receipt. |
| Designate A10,000 then B5,000 with independent Manager decisions | Pool5,000/A10,000/B5,000; each command paired, total20,000, each affected revision +1. Wrong action/role, missing approval or closed project rejects. |
| Approved legacy Balance review for available800/reserved200 plus evidence/version; Opening then reviewed Designate and final original-allocation T05 rows | Pool first800/200; final approved project attribution conserves800/200; no historical gross receipt replay or recipient receipt until pending transfer confirmed. Wrong review/boundary/evidence rejects. |
| InternalTransfer/BalanceReturn creates recipient balance | ConfirmTransfer preserves source project designation; Opening path rejects these transfer types. |
| Company-only Correction with original posted operation and approved unused balance resolution or evidenced Balance review | Unassigned source balance only, no T12; apply only approved correction budget once with existing balance/ledger lineage. Fake project, unrelated balance, source reuse, missing original or self-reference rejects. |
| Project Correction | CorrectCustody Manager decision per affected project, canonical gates and revision +1; missing/wrong action, different project, or closed gate rejects. |

The DDL checks source linkage and structural authority. Command-level tests must additionally prove independent membership, source consumption budgets, material revision increments and exact idempotent replay; raw DML does not demonstrate those guarantees.
