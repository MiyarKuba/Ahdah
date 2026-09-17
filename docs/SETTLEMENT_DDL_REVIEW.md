# Settlement DDL review

**2026-09-17 — DDL preparation only; execution is not approved.** Policy v1: `f962d6119281906b5144b80fe1ffdac7e4525d9e`. Physical design: `a8df2e8d48c110959c172a1f35835ee973b495aa`. [Artifact entry point](../database/settlement/README.md), [design](SETTLEMENT_SCHEMA_CUTOVER_DESIGN.md), [policy authority](SETTLEMENT_POLICY_DECISIONS.md).

## Review outcome and counts

Prepared all T01–T26 definitions, five reviewed existing-table alterations, functions/triggers, read-only metadata checks, guarded future business reconciliation, staged enforcement, empty-schema rollback and rehearsal specifications. **18 SQL files**, 26 new tables, 420 new-table columns, **500 named table constraints**, **219 indexes**, **61 triggers**, **11 new functions**. Eleven triggers are deferred constraint triggers and add eleven pg_constraint entries beyond the 500; PostgreSQL 18 automatic NOT NULL entries are not included. Two existing trigger functions are reused unchanged. No additional existing table is altered.

## Baseline revalidation

Metadata-only READ ONLY/ROLLBACK inspection found PostgreSQL 18.4, ahdah_db/ahdah and 91 tables. The 49 recorded relevant table signatures/keys/FKs/triggers match the design appendix. Preflight pins all 7,966 metadata signature rows with MD5 `d697e4fbc6699faed6076d576a814d38` and passed against the deployed catalog. No business rows, fixture rows or sequence values were read. Secrets stayed in the existing configuration/process environment and were not output or committed. There was no schema execution, transaction posting, EF scaffold or application change.

## Physical decisions preserved

- T01 one company/project gate; revision/version/cycle constraints, attribution status, same-project deferred active pointers; separate financial/operational states.
- T02 uses UNIQUE NULLS NOT DISTINCT(company_id,user_advance_balance_id,project_id), no duplicated currency. T03–T05 record approved operation groups, immutable movements and typed reservations. Deferred validation locks authoritative balances, validates current available/reserved/unassigned/reservation equations, and never repairs amounts.
- T06–T13 preserve cycles, sealed snapshots/categories, immutable events, certificates, reopening, action-specific authorization and physical acknowledgment. Nullable Draft pointer plus deferred references makes packet construction possible; header/categories must be constructed together, categories first. Later child insertion and historical edits fail.
- T14–T18 hold immutable policy/configuration/content/hash/approval/effective date/predecessor, active company binding, stable verified object keys/hashes, original-paper events and scoped exceptions. No policy data is seeded and no binary storage implementation is included.
- T19–T21 preserve per-currency owner schedules. Unknown amounts may remain null before approval, approved amounts must be complete, the signed row equation is checked, and retention entitlement/classification remains distinct from dispute/unknown/unclassified difference. Cross-row retention/source completeness is application-enforced, not a hidden CHECK query.
- T22 requires exactly one source, one typed economic target, legal type/direction, unique effect key/ordinal/target and explicit reversal reference. Other needs evidence and ExplicitOther direction but remains uncertifiable until the evaluator validates approved treatment; row existence never closes the gap.
- T23 Project/CompanyUnassigned intent and T24 immutable allocation applications preserve exact amounts without automatically posting or counting twice. Cross-note/project/currency and aggregate budgets remain transaction/evaluator duties. Actual allocations must consume intent rather than count as additional credit.
- T25/T26 preserve polymorphic source digest/version, reviewed uncertainty and affected project scope. No fictitious FK targets polymorphic source_id and no expense-ratio inference or old ledger rewrite occurs.
- Only projects, project_contract_changes and owner_payment_refunds receive nullable denomination columns/checks. projects, user_advance_balances and advances receive the reviewed enforcement triggers; existing-table triggers start disabled until reviewed population/verification/activation.

## Approved Opening and Correction clarification — 2026-09-17

The inherited T03 rule required project-scoped T12 authorization for every Posted Opening, although company-unassigned custody has no project. The owner resolved this inconsistency: **Opening establishes unassigned custody; separate Designate assigns it to a project.** This corrects physical design, not approved Policy v1. T12.project_id remains mandatory.

| Operation | Authority and source | Scope and effects |
|---|---|---|
| Opening, live | Confirmed existing `money_transfers` AdvanceDelivery, linked advance, recipient confirmation and `balance_ledger_entries` BalanceCreated/Advance receipt | T03 money_transfer_id set, legacy review null, authorization_decision_id null; one unassigned T02, received/available equal delivery, no reserved amount; no project revision. |
| Opening, legacy | Approved T25 Balance review with evidence, source balance/version and retained boundary digest | T03 legacy_attribution_review_id set, delivery/auth null; current available/reserved enters unassigned T02; no replayed gross receipt or existing balance-ledger posting. A zero balance requires its unassigned position but no all-zero T04 row. |
| Designate | Allowed Manager T12 action `Designate`; affected project; reviewed legacy source when reserved boundary attribution is needed | Separate immutable paired unassigned debit/project credit; net available/reserved zero per balance; project gate and financial revision +1. |
| Release | Allowed Manager T12 action `Release` | Paired project debit/unassigned credit; no reserved movement; D01 reconciled holder, no project reservation or unresolved difference still required. |
| Reallocate | Allowed Manager T12 action `Reallocate` for both projects | Paired project effects, canonical gates, both revisions +1; no reserved or cross-currency move. |
| Correction, project-affecting | Allowed Manager T12 action `CorrectCustody` for every affected project, linked original Posted operation | Independent appropriate project authority; gates/revision +1; closed project must reopen; compensating immutable effects. |
| Correction, company-only | Approved existing advance_settlement_resolutions linked through advance_settlements to the target balance/currency, or approved evidenced T25 Balance review; original Posted operation | authorization_decision_id null; only the authoritative source balance's unassigned position; no invented project. Approved source is the authority, not a generic approval flag. |

Source inspection: `AdvanceService.ConfirmTransferAsync` dispatches AdvanceDelivery to `ConfirmDeliveryAsync`, which verifies recipient/advance, consumes funding allocations, creates the balance and BalanceCreated ledger referencing **Advance**, then records confirmation. InternalTransfer and BalanceReturn instead use `ConfirmAllocatedTransferAsync`: retain `ConfirmTransfer` movements and existing designation, even for a newly created recipient balance. They cannot be relabeled Opening.

DDL amendments: T03 distinguishes live/legacy Opening source paths and requires null project authorization; a typed nullable advance_settlement_resolution_id supports existing company-correction authority. T12 adds CorrectCustody. T03/T04 deferred authority triggers reject project Opening, validate receipt/review lineage and paired disposition effects, and require matching Manager action/scope. Two partial unique indexes prevent repeated posted Opening per source; a balance lock also checks duplicate opening across source paths. The new resolution FK has tenant-leading support. Posted headers/movements stay frozen. No sixth baseline table alteration or company-scoped T12 was introduced.

All writers must still validate actor membership/independence, approved source budgets and unapplied residual, original correction lineage, whole-command idempotency, evidence authenticity, exact currency and revision +1 under the universal gate. Source approval alone does not authorize reuse of an already consumed resolution. SQL structural checks do not replace those application duties. All five existing-table changes, T02 uniqueness, conservation, immutable history and closure controls remain intact.

Legacy sequence: preserve declarations, source/boundary versions/digests and project impacts in T25/T26; approved Opening initializes **unassigned** current custody; separate Manager-approved Designate operations establish evidenced project slices. For reserved boundary custody, construct final T05 reservations with the original allocation lineage in the same cutover transaction and force conservation before commit; never reparent an existing live reservation. Indeterminate review cannot authorize Posted Opening; provisional conserving positions remain explicitly unapproved cutover work until reviewed, and cannot certify closure.


## Staging execution prerequisites

- [ ] Verify clarified Opening source paths, unassigned-only effects, separate Manager Designate and scoped Correction rejection/acceptance cases.
- [ ] Review every SQL file and its approved baseline reference; authorize staging execution separately.
- [ ] Prove backup/restore and isolate deployment ownership from runtime role; runtime cannot own/inherit ownership, disable triggers or truncate history.
- [ ] Confirm 00 preflight against staging clone metadata; if fingerprint differs, classify drift and stop affected installation.
- [ ] Confirm installation transaction order 01–10, timeout/lock impact and absence of automatic cutover/fixture execution.
- [ ] Plan company preparation phases and global enforcement activation checkpoint; no per-company activation state exists in the approved schema.
- [ ] Confirm runtime installation grants are read-only and closure remains disabled.

## DDL checklist

- [ ] Tenant predicates and composite tenant FKs on all financial references; same-project pointers/snapshot cycles; actor references valid.
- [ ] Explicit deterministic names <=63 bytes, no duplicate names, intended FK directions and NO ACTION deletion; no cascade loss of history.
- [ ] All exact types/nullability/defaults/status/amount/nonblank/time-pair/version CHECKs reviewed; no guessed currency, float or cross-currency total.
- [ ] Position uniqueness and one unassigned component; reservations/conservation under parent balance locks; multi-row and old/new parents covered.
- [ ] All application writers lock complete sorted project/balance sets before DML; deferred row triggers do not magically establish global order.
- [ ] Snapshot categories-before-header packet construction, deferred circular FKs, same-write-subtransaction sealing and concurrent append rejection tested.
- [ ] Immutable/conditional/child-sealing triggers cover inserts as well as updates/deletes; original baseline immutable mechanism untouched.
- [ ] Closure pointer, approved revision/policy/snapshot and Completed/Cancelled projections consistent; reopening retains history and starts new cycle.
- [ ] Policy content/config/hash retained; active binding locks and v1/v2 history behavior reviewed.
- [ ] Metadata, verified bytes, paper and exceptions remain separate; stable keys only; evidence storage availability still application responsibility.
- [ ] Owner equation/source coverage, qualified retention, disputed gross positions and missing denominations reviewed; no extension of retention exception to supplier debt/claims.
- [ ] Effect target uniqueness, reversal direction/budget, refund fee/net lineage and unsupported Other treatment tested; credit intent consumption cannot double count.
- [ ] Legacy review actor/source scope and opening completeness attested; unbounded uncertainty never silently called CompanyOnly.
- [ ] Every proposed index workload reviewed in INDEX_REVIEW.md, including append-write amplification; no irrelevant baseline index rebuilt.
- [ ] Rollback guard rejects any new data, populated currency or activated enforcement; no DROP after financial history; no CASCADE.
- [ ] Metadata verifier definition outputs compared manually with reviewed CHECK expressions/index predicates/function bodies, not just names.
- [ ] All 24 command fixtures and seven two-session schedules passed before operational enablement; structural probes alone do not qualify.
- [ ] Serializable retries preserve key/payload, restart whole transaction with fresh authority/evaluation, and leave no partial committed result.

## Database-First expectations

Only after separately approved staging DDL/catalog verification and rehearsal should scaffolding run. Never manually edit generated entities/mappings. Expected conceptual entities/DbSets (the scaffolder controls final pluralization):

| Table | Expected entity | Expected DbSet |
|---|---|---|
| project_financial_controls | ProjectFinancialControl | ProjectFinancialControls |
| project_custody_positions | ProjectCustodyPosition | ProjectCustodyPositions |
| custody_operations | CustodyOperation | CustodyOperations |
| custody_movement_entries | CustodyMovementEntry | CustodyMovementEntries |
| custody_reservations | CustodyReservation | CustodyReservations |
| project_settlements | ProjectSettlement | ProjectSettlements |
| project_settlement_snapshots | ProjectSettlementSnapshot | ProjectSettlementSnapshots |
| project_settlement_snapshot_categories | ProjectSettlementSnapshotCategory | ProjectSettlementSnapshotCategories |
| project_settlement_events | ProjectSettlementEvent | ProjectSettlementEvents |
| project_closure_certificates | ProjectClosureCertificate | ProjectClosureCertificates |
| project_reopenings | ProjectReopening | ProjectReopenings |
| project_authorization_decisions | ProjectAuthorizationDecision | ProjectAuthorizationDecisions |
| project_operational_events | ProjectOperationalEvent | ProjectOperationalEvents |
| settlement_policy_versions | SettlementPolicyVersion | SettlementPolicyVersions |
| company_settlement_policy_bindings | CompanySettlementPolicyBinding | CompanySettlementPolicyBindings |
| document_custody_events | DocumentCustodyEvent | DocumentCustodyEvents |
| document_policy_exceptions | DocumentPolicyException | DocumentPolicyExceptions |
| verified_evidence_objects | VerifiedEvidenceObject | VerifiedEvidenceObjects |
| project_owner_reconciliations | ProjectOwnerReconciliation | ProjectOwnerReconciliations |
| project_owner_reconciliation_positions | ProjectOwnerReconciliationPosition | ProjectOwnerReconciliationPositions |
| project_owner_retention_items | ProjectOwnerRetentionItem | ProjectOwnerRetentionItems |
| financial_resolution_effects | FinancialResolutionEffect | FinancialResolutionEffects |
| supplier_credit_intents | SupplierCreditIntent | SupplierCreditIntents |
| supplier_credit_intent_applications | SupplierCreditIntentApplication | SupplierCreditIntentApplications |
| legacy_attribution_reviews | LegacyAttributionReview | LegacyAttributionReviews |
| legacy_attribution_review_projects | LegacyAttributionReviewProject | LegacyAttributionReviewProjects |

Review nullable-project unique relationships, partial unique indexes and singular-navigation inference as sets; composite company/project FKs; deferred cycle/snapshot references; trigger-only invariants and immutable grants absent from generated models; JSONB and bytea canonical payload/hash fields; nullable legacy currencies and exact numeric decimals. Add concurrency configuration in non-generated partials only after implementation authorization. Generated properties do not authorize updates to immutable rows.

## Validation record

Final static validation accepted 845 SQL statements across all 18 files, with native PL/pgSQL syntax checks for all 11 CREATE FUNCTION bodies and anonymous DO blocks, using a temporary pglast 8.4 parser. Psql variables were replaced with typed synthetic literals for parsing; psql control flow was reviewed separately. The high-level PL/pgSQL JSON decoder fails on trivial trigger ASTs; the native parser was used for syntax acceptance. No database binding/execution validation is claimed. Checks include identifiers, duplicate constraint/index names, 26 object IDs, FK local/remote columns and candidate unique keys against the read-only catalog, all FKs after table creation, and scope restricted to SQL/Markdown artifacts. Metadata preflight alone ran against deployed PostgreSQL, under READ ONLY and ROLLBACK. All forward DDL, privilege, invariant, business inventory, rehearsal, activation and rollback files remain unexecuted. No safe disposable database was assumed or created.

[REHEARSAL.md](../database/settlement/REHEARSAL.md) specifies all F01–F24. The rollback-only SQL harness provides structural probes, not full application acceptance. [RACES.md](../database/settlement/RACES.md) specifies closure versus expense, invoice/debt and custody transfer; two closers; reverse-order multi-project requests; reopen versus close; and policy activation versus close. No fixture or race passed by execution in this milestone.

## Exact next task

Review the artifacts including the owner-approved Opening/Correction physical-design correction. If the reviewed artifact set is then explicitly authorized for **staging execution**, verify backup/restore and role separation, rerun metadata preflight, execute the approved installation on the named isolated staging target, compare catalog/deparsed definitions, and run the authorized structural fixture probes. Do not enable closure, populate real cutover data, scaffold EF or implement application commands without the separately approved scope for those steps. Production execution remains separately gated.
