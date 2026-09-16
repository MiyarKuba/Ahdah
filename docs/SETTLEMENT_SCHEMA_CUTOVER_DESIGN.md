# Settlement Database-First schema and cutover design

**Status:** proposed physical design for review; Policy v1 is approved, this schema is not approved for execution. **Date:** 2026-09-16. **Baseline:** `f962d6119281906b5144b80fe1ffdac7e4525d9e`; branch `design/settlement-schema-cutover`.

The authority is [approved D01–D16](SETTLEMENT_POLICY_DECISIONS.md). This artifact proposes **26 new tables, 5 modified existing tables, 48 writer/action inventory entries and 24 acceptance fixtures**. It is not a migration. No DDL, business-row inspection, database mutation, scaffold or application change was performed. Existing settlement GET behavior remains unchanged; closure stays disabled.

## 1. Deployed catalog verification

Connected through the existing API user-secret configuration without printing its values. Used `psql -X`, a read-only connection default, an explicit `REPEATABLE READ READ ONLY` transaction, catalog-only queries and `ROLLBACK`. Result: database **ahdah_db**, schema **ahdah**, PostgreSQL **18.4**, transaction read-only **on**, **91 tables**. Inspection covered every column's type/nullability/default/generated/identity metadata, PK/UQ/FK/CHECK definitions, indexes, noninternal triggers, schema functions and sequence definitions. No sequence values, customer rows, names, credentials or production financial values were selected. Appendix A records the relevant structural inventory; numbers in examples below are fictional fixtures.

Catalog-wide totals: 928 indexes, 91 noninternal triggers, 11 bigint identity sequences, 14 stored generated columns. The catalog returns 4,354 constraints including PostgreSQL 18 NOT NULL constraints; this is not a count of business CHECKs. All 91 tables have RLS disabled: explicit tenant predicates and composite FKs remain essential. Functions are `set_updated_at` and `prevent_immutable_record_change`; none provides aggregate financial revision or custody allocation. UUID identifiers generally default to `gen_random_uuid()`; audit and several reporting/job sequences are GENERATED ALWAYS identity, not financial numbering guarantees.

### Catalog vs generated-model differences

Compared all 91 `ToTable` mappings and column sets against the deployed catalog, plus entity scalar nullability, decimal mapping, stored generation, PK/FK names, defaults and index coverage. Inspected material relationship/cardinality differences below. This is a metadata/source comparison, not a runtime EF round-trip test.

| Finding | Classification | Consequence |
|---|---|---|
| All 91 tables and column-name sets match; no missing PK/FK names, scalar nullability or decimal/generated-column mapping discrepancy found | Expected | No observed base-schema drift requiring a refresh now. |
| CLR-default zero/false defaults are often omitted from fluent mapping; generated expressions appear as computed mappings rather than defaults | Harmless | Preserve deployed defaults and stored generation; do not generate DDL from this model. |
| 22 expression indexes are not represented by generated index mappings, including case-insensitive expense invoice, credit reference, category, supplier and project-name indexes; others concern identity/reporting | Expected | Preserve them from catalog when preparing reviewed DDL. EF is not the complete schema source. |
| Generated context contains no `HasCheckConstraint` or `HasTrigger` calls; live CHECKs, timestamp and immutable triggers exist | Expected | Capture/review catalog DDL independently; application validation does not replace database constraints. |
| Partial unique `ux_project_contract_changes_one_pending(company_id,project_id)` is only unique for PendingApproval; generated `Project.ProjectContractChange` is singular | Requires design adjustment | Query changes by tenant/project as a set; historical Approved rows are not singular. Review future scaffolding navigations without manually editing generated files. |
| Partial unique primary/nonrejected expense document index does not limit all expense documents to one | Requires design adjustment | Evidence evaluation uses the document set and verification states, not a singular navigation assumption. |
| `version_number` exists more widely than the 18 entity types configured as EF concurrency tokens in the non-generated partial | Requires design adjustment | Configure each new implemented writer's concurrency token outside generated files; version columns alone do not implement optimistic concurrency. |
| Project/contract-change/refund headers lack complete currency semantics; advance, custody and transfer headers lack project ownership | Requires design adjustment | Add reviewed denomination and explicit attribution below. No default LYD backfill. |
| Advance closure generated readiness omits its separate outstanding supplier-debt/personal-claim snapshot amounts | Requires design adjustment | Never reuse it as project closure authority. New evaluator checks all mandatory categories. |
| No existing financial-control/certificate/reopen structure or universal gate | Blocks implementation | Blocks enabling closure, not this design. Must implement and prove the complete protocol first. |

No deployed constraint directly contradicts D01–D16. In particular the projects completion CHECK requires completed dates for Completed/FinanciallyClosed and null `completed_at` for other statuses. Keeping Cancelled operationally unchanged is compatible; projecting a cancelled project to FinanciallyClosed would not be.

### Material existing structures

* `user_advance_balances`: unique tenant/advance/user; nonnegative cumulative counters and current available/reserved. CHECK: received + restored = expensed + transferred out + returned + adjustment out + available + reserved. Settled/Closed requires zero current available/reserved. Amounts are stored and maintained by writers, not automatically recomputed by a trigger.
* `advance_settlements`: stored generated difference = declared remaining − available snapshot; difference type generated. `advance_closures`: stored generated unsettled count and readiness. Neither has project scope.
* `expenses`, `personal_claims`, `supplier_debts`: optional direct project links. Debt and claim outstanding are stored generated expressions. Payment/credit allocations carry debt/claim lineage; pending headers do not prove payment. Debt/expense project equality requires validation beyond their individual FKs.
* `expense_returns` permits AdvanceBalanceRestore, SupplierCreditNote, SupplierRefund, PersonalClaimReduction, Other. Restore allocations identify the original expense allocation; supplier refunds reference the return and funding source. These links do not prove one complete, nonduplicated economic effect.
* Owner-payment project allocations express provenance. Advances may mix sources. Supplier identity, expense history and owner provenance do not designate unused custody.
* `expense_documents` has metadata, verification and hash fields; metadata presence is not proof of available verified bytes or original paper. Company settings include mutable general controls, not immutable settlement policy versions.
* Ledger and audit tables have immutable triggers; most financial workflow tables only have timestamp triggers. Existing approved workflow rows are not universally immutable in PostgreSQL.

## 2. Policy-to-design traceability

| Policy | Required design |
|---|---|
| D01 | T02–T05 custody slices, explicit approvals, release predicates, no inferred attribution; sections 4–5 |
| D02 | T01 independent financial state and T13 operational evidence; Cancelled remains Cancelled |
| D03 | Complete evaluator: known blockers false, missing mandatory evidence indeterminate, true only after complete pass |
| D04 | T10 certificate plus T01 gate and authoritative transaction, not status-only closure |
| D05 | T07–T09 immutable reproducible snapshots/events |
| D06 | T14–T18 versioned digital/paper requirements and evidence/exception records |
| D07 | T22 typed return/difference effects with reconciliation and deduplication |
| D08 | T19–T21 per-currency owner schedule and qualifying retention disclosure |
| D09 | Funding is not debt; claim or explicit approved financing classification required |
| D10 | T25–T26 reviewed openings and scoped unknowns, section 13 |
| D11 | T12 separation/authorization evidence; one Accountant plus one Manager is sufficient |
| D12 | All writers use section 11 gate and material revision protocol |
| D13 | T11 immutable reopen, new cycle, no erased certificate |
| D14 | Cancelled financial run-off without fabricated physical completion |
| D15 | Exact decimal zero per currency after posted authorized resolutions; no silent tolerance/write-off |
| D16 | Section 15 rollout gates, reopening and races before activation |

## 3. Proposed object register and schema conventions

All following names are proposed in `ahdah`. This section defines conceptual columns and constraints, not executable SQL.

**Common conventions (apply to every T object):** `company_id uuid NOT NULL` FK companies; identifier `<singular_table_name>_id uuid NOT NULL DEFAULT gen_random_uuid()` PK unless a natural PK is specified; UNIQUE(company_id,id). All references to tenant-owned rows use `(company_id, referenced_id)` composite FKs, NO ACTION/RESTRICT deletion, never a cross-tenant UUID-only FK. Project-bound parents additionally expose UNIQUE(company_id,project_id,id), used by their project-bound children, control pointers and certificate references. Every FK has a tenant-leading supporting index unless covered by a key. Actor IDs reference app_users with company_id. UTC timestamps are `timestamptz`; dates `date`; amounts `numeric(18,2)`; percentages `numeric(9,6)`; no floating point. `?` explicitly means nullable; other listed fields are NOT NULL. IDs mean uuid, revisions bigint, versions integer. A mutable record has `version_number integer DEFAULT 1 CHECK >=1`, `created_at/updated_at timestamptz DEFAULT CURRENT_TIMESTAMP`; writers advance version, timestamp trigger maintains updated_at. An immutable row has created_at only and rejects UPDATE/DELETE via trigger and runtime grants. Mutable projections are never accepted as the sole history. Freeze triggers protect child INSERT as well as UPDATE/DELETE after parent approval/posting: T20/T21 under approved T19, T04 under posted T03, and reviewed T26 under approved T25. Compose children before the atomic parent finalization; snapshots/categories are inserted as one sealed packet: T08 snapshot FK is DEFERRABLE INITIALLY DEFERRED, category rows are inserted with the preallocated snapshot ID before T07, and insertion of T07 seals the packet; T08 INSERT rejects an already-existing T07 parent. Later category insertion is therefore rejected without a mutable sealed flag. Grants exclude TRUNCATE and trigger disabling for runtime roles. No automatic EF migrations or EnsureCreated.

Text codes use `varchar(40)` with explicit CHECK lists given below; category codes use varchar(80), currency varchar(3) CHECK uppercase three letters, comments/reasons varchar(1000) with nonblank CHECK when required, algorithm/version identifiers varchar(80), hashes `bytea` CHECK length 32 for SHA-256. Optional reason fields cannot be blank if present. IDs and paired actor/time fields are either both null or both nonnull. JSON is jsonb, NOT NULL with object/array shape CHECK as specified, has a documented version and application schema validation. Amounts nonnegative unless explicitly called signed/delta; row-local equations use CHECK. Every action is tenant-scoped and idempotent via existing idempotency records plus immutable operation/event uniqueness.

DB = PostgreSQL-enforced; APP = application transaction/authorization; BOTH = each has an explicit part. Cross-row statements marked DB require the specified constraint trigger, never a CHECK with a hidden table query.

### T01–T05: financial authority and custody

| ID / table | Columns beyond common conventions | Keys, constraints, indexes and enforcement |
|---|---|---|
| T01 `project_financial_controls` | Natural PK(company_id,project_id), FK projects; financial_revision bigint default 1; financial_state; active_settlement_id?; active_closure_id?; last_material_change_at; version_number; next_cycle_number integer default 1; attribution_status | DB: revision/cycle >=1; financial_state in Open, InSettlement, Settled, FinanciallyClosed; attribution_status in PendingReview, Reviewed, Indeterminate; same-project FKs to T06/T10 DEFERRABLE INITIALLY DEFERRED; state FinanciallyClosed iff closure pointer nonnull. Deferred consistency trigger validates approved/current cycle and closure linkage. APP: full policy eligibility. Index(company_id,financial_state). |
| T02 `project_custody_positions` | user_advance_balance_id; project_id?; available_amount default 0; reserved_amount default 0; version_number | DB: FK balance, optional project FK; UNIQUE NULLS NOT DISTINCT(company_id,user_advance_balance_id,project_id); nonnegative amounts. Currency inherited through balance.advance, not duplicated. Balance ID/project ID immutable. Index(company_id,project_id,balance_id). Deferred conservation triggers on positions and balances, parent balance locked first. APP: who may designate and release. |
| T03 `custody_operations` | operation_type; status; requested_by_user_id; requested_at; approved_by_user_id?; approved_at?; authorization_decision_id?; reason; idempotency_record_id; source_operation_id?; money_transfer_id?; expense_id?; expense_return_id?; legacy_attribution_review_id?; version_number | DB: unique(company_id,idempotency_record_id,operation_type); type Opening, Designate, Release, Reallocate, ReserveExpense, ConsumeExpense, RejectExpense, ReserveTransfer, ConfirmTransfer, RejectTransfer, Restore, Correction; status Draft, PendingApproval, Posted, Rejected, Cancelled. Posted header immutable; correction references prior operation. FKs typed to roots; type-specific required references via CHECK. APP validates full participant set, permission and posting; Manager approval mandatory for Designate/Release/Reallocate/Opening. |
| T04 `custody_movement_entries` | custody_operation_id; project_custody_position_id; entry_number integer; delta_available_amount signed; delta_reserved_amount signed; received_amount, restored_amount, expensed_amount, transferred_out_amount, returned_amount, adjustment_out_amount (nonnegative defaults 0); balance_ledger_entry_id?; reverses_entry_id?; occurred_at; actor_user_id; position_version_before/after integer; financial_revision_after? | Immutable. DB: unique(company_id,operation_id,entry_number); version_after=before+1, entry not all zero (opening with zero is represented by position, no entry); FK position/operation/ledger/reversal. APP validates entry pair/group conservation, source lineage and permissions. No duplicated currency. One operation can update a position once with one combined entry. Financial revision null only for unassigned. Index(company_id,position_id,occurred_at). |
| T05 `custody_reservations` | custody_operation_id; project_custody_position_id; expense_advance_allocation_id?; transfer_advance_allocation_id?; amount >0; status; resolved_by_operation_id?; version_number | DB: exactly one allocation FK; status Reserved, Consumed, Released; unique(company_id,position_id,expense_allocation_id) where nonnull, equivalent transfer key; resolution required iff not Reserved. APP: sum across reservation rows equals existing allocation amount, owner/currency/project lineage, exactly-once transition. Deferred position/balance/reservation totals validated at commit. Index(company_id,position_id,status). |

Every project gets T01 in its creation transaction, including Active, Paused, Completed and Cancelled projects. Legacy projects receive one through reviewed cutover; missing control is an error, never implicit Open. New balance creation also creates its zero/unassigned T02 and posted delivery opening in the same transaction. No active/inactive duplicate slices: a zero slice remains reusable and its history stays immutable.

State: Open → InSettlement on draft creation; InSettlement → Settled on current approved reconciliation; Settled → InSettlement when material change invalidates readiness; either open state → FinanciallyClosed only through certificate. Reject/withdraw leaves InSettlement until a replacement cycle or deliberate return to Open. Reopening sets InSettlement and starts a Draft cycle. Reopened is an event, not a redundant persistent state; Settled does not block ordinary lawful activity. Cancelled may be in any financial state, including FinanciallyClosed, while projects.status remains Cancelled. `attribution_status` is separate from workflow state and never authorizes closing.

### T06–T13: certification and independent authority

| ID / table | Columns beyond common conventions | Keys, constraints, indexes and enforcement |
|---|---|---|
| T06 `project_settlements` | project_id; cycle_number integer; previous_settlement_id?; status; operational_basis; captured_project_version integer; captured_financial_revision bigint; policy_version_id; evaluator_version; snapshot_schema_version integer; evaluated_at?; prepared_by_user_id; submitted_by_user_id?; approved_by_user_id?; submitted_at?; approved_at?; snapshot_id?; snapshot_hash?; notes?; version_number | DB unique(company_id,project_id,cycle_number), cycle>=1; basis Completed/Cancelled; status Draft, Submitted, Approved, Rejected, Withdrawn, Superseded. Same-project previous-cycle/snapshot FKs (snapshot FK deferred for packet construction); submitted needs snapshot/time/actor; approved needs all and approver != preparer AND approver != submitter. Approval freezes row permanently. APP transition/authority/staleness; later Invalidated/Superseded/Reopened are immutable events, not updates to an Approved row. Index(company_id,project_id,status). |
| T07 `project_settlement_snapshots` | project_id; project_settlement_id; snapshot_number integer; purpose; project_version; financial_revision; policy_version_id; evaluator_version; schema_version integer; evaluated_at; canonical_payload bytea; payload jsonb object; hash_algorithm varchar(20)='SHA256'; snapshot_hash; evaluated_by_user_id; blocker_count integer; mandatory_gap_count integer | Immutable. DB unique(company_id,settlement_id,snapshot_number), counts>=0; purpose Evaluation, Submission, ApprovalCheck, ClosureCheck. Same-project FKs. APP canonicalization/hash validation and completeness; DB stores exact bytes to reproduce hash without relying on JSONB property order. Index(company_id,project_id,financial_revision). |
| T08 `project_settlement_snapshot_categories` | project_id; snapshot_id; category_code; evaluation_status; currency_code?; amount_meaning; exact_amount?; blocker_count; gap_count; details jsonb array | Immutable. DB unique NULLS NOT DISTINCT(company_id,snapshot_id,category_code,currency_code,amount_meaning); status Evaluated, NotVisible, NotAttributable, Unknown; counts>=0; amount null if currency null; non-evaluated totals null. APP detail schema/reference validation and no crosscategory aggregation. Same-project snapshot FK. |
| T09 `project_settlement_events` | project_id; settlement_id; event_number integer; event_type; actor_user_id; actor_role; occurred_at; reason?; authorization_decision_id?; snapshot_id?; financial_revision; record_version_before/after integer; idempotency_record_id; payload jsonb object | Immutable. DB unique(company_id,settlement_id,event_number), unique(company_id,idempotency_record_id,event_type,settlement_id); event Created, Evaluated, Submitted, Approved, Rejected, Withdrawn, Superseded, Invalidated, Closed, Reopened. APP legal transitions; FK snapshot same project. Ordered event index. |
| T10 `project_closure_certificates` | project_id; project_settlement_id; approved_snapshot_id; closure_snapshot_id; financial_revision; policy_version_id; closure_basis; closed_by_user_id; actor_role; closed_at; reason; pre_close_project_version; post_close_project_version; approved_snapshot_hash; closure_snapshot_hash; closure_version integer; authorization_decision_id; idempotency_record_id | Immutable. DB unique(company_id,project_id,closure_version), unique(company_id,idempotency_record_id); basis Completed/Cancelled; actor_role Manager; version positive, post version=pre+1 for Completed, post=pre for Cancelled (no project-row mutation). Same-project typed FKs. T01 active pointer is the single-active authority; effective Active/Reopened/Superseded computed from pointer, T11 and events. APP full reevaluation and separation. No mutable active flag on certificate. |
| T11 `project_reopenings` | project_id; previous_closure_id; new_settlement_id; actor_user_id; actor_role; reason; reopened_at; prior_financial_state; resulting_financial_state; revision_before/after; pre/post_project_version; authorization_decision_id; audit_log_id; idempotency_record_id | Immutable. DB unique(company_id,previous_closure_id), unique(company_id,new_settlement_id), unique(company_id,idempotency_record_id); same-project FKs; Manager role; prior FinanciallyClosed/result InSettlement; revision_after=before+1. APP approved correction context and correct operational projection. Old certificate/cycle untouched. |
| T12 `project_authorization_decisions` | project_id; action_code; actor_user_id; actor_role; decided_at; decision; policy_version_id; preparer_user_id?; submitter_user_id?; beneficiary_user_id?; conflict_detected boolean; conflict_reason?; evidence_id?; delegation_reference? uuid; request_hash; details jsonb object | Immutable. DB action Prepare, Submit, Approve, Close, Reopen, Acknowledge, Complete, Designate, Release, Reallocate, Exception, ReconcileOwner; decision Allowed/Denied; role Manager/Deputy/Accountant/Supervisor; FK actors/evidence/policy. Delegation reference must be null under v1; no unsupported FK to a nonexistent delegation model. APP active membership, visibility, beneficiary conflict and action matrix. BOTH stored identity inequality plus current authorization checks. |
| T13 `project_operational_events` | project_id; event_type; actor_user_id; actor_role; occurred_at; reason; project_version_before/after; evidence_id?; acknowledgment_event_id?; authorization_decision_id; idempotency_record_id | Immutable. DB event PhysicalAcknowledged, CompletionApproved, CancellationApproved; self-FK acknowledgment same project; unique(company_id,idempotency_record_id,event_type); role Supervisor for acknowledgment, Manager for final decisions; monotonic versions. APP active assignment, freshness, required evidence and transition. Acknowledgment does not itself complete or settle the project. |

Cycle allocation uses the locked T01 next_cycle_number, increments it once, and inserts cycle atomically; no global sequence and no MAX()+1 race. Active-settlement pointer may reference a Draft/Submitted/Approved cycle, but only an effective current Approved cycle satisfies closure. An immutable approved cycle may become stale; an Invalidated event and control state record this without editing the historical approval. Effective state is a query projection of header plus events, never inferred from the header status alone.

Closure checks approved settlement revision R; writing the certificate and closed-state projection increments control version but **does not increment material revision R**. This avoids making its own approved snapshot stale. Reopen increments material revision to R+1 because authority for correction and current reconciliation scope changes. Certificate contains R and both snapshot hashes; closure check must reproduce the same financial facts as approval, while authorization timestamps may differ.

### T14–T18: policy and evidence

| ID / table | Columns beyond common conventions | Keys, constraints, indexes and enforcement |
|---|---|---|
| T14 `settlement_policy_versions` | policy_code varchar(40); major_version integer; configuration_version integer; policy_text text; canonical_rules bytea; rules jsonb object; content_hash; approved_by_user_id; approved_at; effective_from; predecessor_id? | Immutable. DB unique(company_id,policy_code,major_version,configuration_version); positive versions; FK predecessor. APP exact D01–D16 v1 rules plus company/category choices; hash canonical bytes. Digital rules and paper mode Required/NotRequired/CategorySpecific with explicit category maps; no omitted configuration default. |
| T15 `company_settlement_policy_bindings` | Natural PK company_id; active_policy_version_id; activated_by_user_id; activated_at; version_number | DB tenant FK policy; APP valid effective immutable version. Shared lock by evaluators/writers that depend on rules; exclusive lock for activation. Policy activation does not rewrite old certificates. |
| T16 `document_custody_events` | project_id; evidence_id; expense_document_id?; event_type; actor_user_id; actor_role; occurred_at; reason; policy_version_id; category_code; previous_event_id?; authorization_decision_id? | Immutable. DB same-tenant evidence/document/policy FKs; event OriginalReceived, OriginalTransferred, OriginalReturned, OriginalLost, SupplementalRecorded; APP correct physical chain and requirement. OriginalReceived identifies received-by/received-at. Index(company_id,project_id,evidence_id,occurred_at). |
| T17 `document_policy_exceptions` | project_id; evidence_id; expense_document_id?; category_code; requirement_code; policy_version_id; approved_by_user_id; actor_role; approved_at; reason; authorization_decision_id; supersedes_exception_id? | Immutable. DB exact typed FKs, unique(company_id,supersedes_exception_id) where nonnull; APP explicit exception permitted by applicable policy and authorized independent reviewer, no blanket exemption. Evidence required even where original document is absent: evidence of the exception decision. |
| T18 `verified_evidence_objects` | project_id?; expense_document_id?; storage_object_key varchar(500); storage_version varchar(200); sha256_hash; media_type varchar(150); byte_length bigint; verified_by_user_id; verified_at; verification_method varchar(40); source_evidence_id? | Immutable. DB size>0; UNIQUE NULLS NOT DISTINCT(company_id,storage_object_key,storage_version,project_id); FK document/source; APP actual bytes checked against hash, authorized tenant storage access, persistent retention/legal availability. Stable opaque keys only, no credentials, signed/temporary URLs or inline secrets. An unavailable binary is a mandatory gap even if metadata says verified. |

Reuse expense_documents and their existing audit history as metadata; T18 is an independent byte-verification attestation and can also support contracts, owner schedules and physical acknowledgment without a fake expense. T16 records paper custody, T17 exceptions. A company-scoped T18 with null project supports cutover declarations or shared contracts; every project consumer is explicitly linked in its typed parent, and material evidence changes discover and gate all consumers. A null evidence project never authorizes cross-tenant or unrestricted access. Binary storage is an implementation dependency, not delivered here. Subsequent loss/revocation changes certification eligibility through a material event; it does not edit the original attestation. Historical closed evidence remains intact; an append-only supplemental event is allowed only if it does not change certified facts. A discovered deficiency requires controlled reopening.

T14 v1 includes exact approved policy and explicit tenant/category selections, never mutable company_settings alone. Old certificates always resolve their original immutable policy/configuration. New cycles use the active binding. Activating v2 invalidates pending certification by policy-ID mismatch; drafts need reevaluation/new submission. Reopening an old certificate keeps its v1 evidence and creates the new cycle under the currently effective approved policy (v2 when active). Policy activation alone does not bump every project revision; closure must compare both financial revision and policy ID. Changing project-specific exceptions/evidence does bump its revision.

### T19–T26: owner position, effects, intent and cutover evidence

| ID / table | Columns beyond common conventions | Keys, constraints, indexes and enforcement |
|---|---|---|
| T19 `project_owner_reconciliations` | project_id; reconciliation_number integer; financial_revision; policy_version_id; status; prepared_by_user_id; prepared_at; approved_by_user_id?; approved_at?; evidence_id; supersedes_id?; version_number | DB unique(company_id,project_id,reconciliation_number); status Draft, Submitted, Approved, Rejected; same-project evidence/predecessor; approved actor differs preparer; approved row immutable. APP complete per-currency schedule and independent approval. Index(company_id,project_id,status). |
| T20 `project_owner_reconciliation_positions` | project_id; owner_reconciliation_id; currency_code; contract_amount; approved_change_amount signed; payments_received_amount; refunds_amount; immediate_receivable_amount; immediate_payable_amount; retention_amount; disputed_amount signed; unclassified_difference_amount signed; classification_status; source_details jsonb array | DB unique(company_id,owner_reconciliation_id,currency_code); amount columns nullable in Draft/Submitted with explicit unknown reasons; Approved requires every amount nonnull; nonnegative except signed fields; classification Known, Disputed, Unclassified, Unknown; row equation defined in section 8; frozen with approved parent. APP full source coverage, missing denomination, exact source amounts and no duplicated receipts. |
| T21 `project_owner_retention_items` | project_id; owner_position_id; amount >0; percentage?; percentage_base_amount?; contractual_basis varchar(1000); release_condition varchar(1000); due_date?; entitlement_status; classification; evidence_id; approved_by_user_id; approved_at | DB currency inherited from position; percent between 0 and 100 with paired base; entitlement Undisputed/Disputed/Unknown; classification OutsideImmediateSettlement/PendingReview; frozen with approved parent. APP contractual basis and exact percent/amount rounding convention; at least a meaningful release condition, due date when known. Sum items=position retention cross-row transaction validation. |
| T22 `financial_resolution_effects` | project_id; expense_return_id?; advance_settlement_resolution_id?; effect_number integer; effect_type; currency_code; amount >0; direction; custody_movement_entry_id?; funding_source_ledger_entry_id?; personal_claim_adjustment_id?; personal_claim_write_off_id?; supplier_debt_write_off_id?; supplier_credit_note_id?; supplier_refund_id?; economic_effect_key uuid; polarity; authorization_decision_id; evidence_id?; reverses_effect_id?; posted_at | Immutable. DB exactly one source root; polarity Original/Reversal with reverses_effect_id required iff Reversal; type CustodyRestore, FundingRestore, ClaimAdjustment, ClaimWriteOff, DebtWriteOff, CreditGranted, Other; direction IncreaseCustody, IncreaseFunding, ReduceClaim, ReduceDebt, IncreaseCredit, ExplicitOther; CHECK type/direction/typed-target matrix; unique(company_id,economic_effect_key), unique source/effect_number partial indexes, unique nonnull target per posting kind. Refund ID is backing receipt for FundingRestore, not another additive effect. APP currency/project/source budget and effect coverage; unsupported Other cannot certify. |
| T23 `supplier_credit_intents` | supplier_credit_note_id; project_id?; intent_kind; intended_amount >0; consumed_amount default 0; released_amount default 0; status; prepared_by_user_id; approved_by_user_id?; approved_at?; reason; version_number | DB UNIQUE NULLS NOT DISTINCT(company_id,credit_note_id,project_id); kind Project iff project nonnull, otherwise CompanyUnassigned; status Pending, Approved, Exhausted, Cancelled; consumed+released<=intended. Currency inherited from note. APP approved/project-complete intent before credit can be ignored in certification; sum intended−released across intents <= note amount, exactly covers note on completed intent review. |
| T24 `supplier_credit_intent_applications` | supplier_credit_intent_id; supplier_credit_note_allocation_id; amount >0; applied_by_user_id; applied_at; reverses_application_id? | Immutable. DB unique(company_id,credit_allocation_id,intent_id), unique reversal target when nonnull; composite FKs. APP same note, currency, debt project=intent project, sum effective applications=allocation amount (linked reversals subtract), consumed balances update atomically; company intent must explicitly reassign before applying to project debt. |
| T25 `legacy_attribution_reviews` | boundary_key uuid; source_type; source_id uuid; source_version? integer; status; uncertainty_scope; declared_by_user_id?; declared_at?; reviewed_by_user_id?; reviewed_at?; approved_by_user_id?; approved_at?; evidence_id?; declared_details jsonb object; source_digest; reason; supersedes_review_id?; version_number | DB unique(company_id,boundary_key,source_type,source_id); status Pending, Indeterminate, Approved, Rejected; scope CompanyOnly, KnownProjects, Unbounded; source_type Balance, Transfer, Return, Expense, Claim, SupplierDebt, ManagerContribution, OwnerPosition, Credit, Refund, Contract, ProjectCompleteness; source-version positive if present; approved row immutable. APP resolve source allow-list, reviewer independence, complete inventory. Polymorphic legacy reference intentionally has no fake FK; validate source existence/version in cutover transaction and freeze digest. |
| T26 `legacy_attribution_review_projects` | legacy_attribution_review_id; project_id; impact_code | DB unique(company_id,review_id,project_id); composite FKs; impact PossibleAttribution, ConfirmedAttribution, ReviewedUnrelated. Frozen with approved parent. APP conservative, evidenced scope: no guessed relation and no blanket company block when unrelatedness established. Index(company_id,project_id,impact_code). |

### Five modified existing tables (exhaustive proposed list)

| Existing table | Proposed change | Legacy and enforcement |
|---|---|---|
| `projects` | Add nullable `contract_currency_code varchar(3)` with uppercase CHECK; add deferred control-existence/operational-projection consistency trigger | No default/backfill inference. New project command must supply denomination; legacy null is an explicit owner gap until reviewed. Trigger requires T01 per project once cutover is enabled, allows Cancelled plus financial Closed, rejects FinanciallyClosed without certificate. |
| `project_contract_changes` | Add nullable `currency_code varchar(3)` with uppercase CHECK | Validate against reviewed base contract denomination for scalar previous/new values; legacy unknown remains unknown. Multicurrency contractual components belong in the approved owner schedule with evidence, never summed into scalar contract_value. |
| `owner_payment_refunds` | Add nullable `currency_code varchar(3)` with uppercase CHECK | Derive only from verified linked payment when available; otherwise explicit reviewed designation. APP checks all refund funding allocations same currency. |
| `user_advance_balances` | Add deferred conservation trigger with T02/T05 counterpart triggers | After reviewed cutover, every balance must equal sum of current slices/reservations. Existing counters/equation retained. Trigger acquires/requires parent balance lock; detects balance-only writes and deletions. |
| `advances` | Add trigger rejecting currency changes after any balance/custody history exists | Makes inherited custody currency stable. Historical conversion is never an in-place currency update. |

No other existing column/constraint/trigger change is proposed. New immutable triggers/functions, supporting indexes and runtime privileges are part of the future DDL artifact, not extra tables. Existing financial writers must change application protocol; this design does **not** claim that PostgreSQL can detect every bypassing legacy writer. Closure activation requires exclusive use of the upgraded service role/workflows, no legacy/manual/import DML route, and operational control of privileged maintenance. Adding a second uncontrolled writer would invalidate this safety proof and requires a new reviewed enforcement design.

## 4. Custody representation and exact conservation

Choose **A: nullable project_id position**. A unique NULLS NOT DISTINCT key permits exactly one unassigned slice per balance and one per project. A synthetic project would pollute project authorization/lifecycle and tenant reporting; a separate unassigned balance column would create two reservation/movement mechanisms. A separate company-position table has the same duplication problem. NULL means explicitly company-unassigned, not unknown attribution: T25/T26 express uncertainty separately.

For balance b, with positions S(b), at every committed state:

* sum(s.available)=b.available_amount; sum(s.reserved)=b.reserved_amount; both components nonnegative.
* For each s, sum(active T05 amounts)=s.reserved. Each existing expense/transfer allocation's active lifecycle has matching slice reservations; sum matches its amount while reserved.
* Existing balance equation remains: received + restored − expensed − transferred_out − returned − adjustment_out = available + reserved.
* Movement history rebuilds current slice available/reserved from reviewed Opening entries plus subsequent signed deltas. Gross designation over time is not current money. Expense consumption removes reserved custody and increments existing expensed counter; history still records its original designation.
* Transfer/return within the same advance moves custody between holders, preserves designation and currency, and has paired entries. Advance currency is inherited; no slice currency column. Any historical snapshot copies currency with source advance ID/version, validated at capture.
* Designate/release/reallocate changes slice ownership only: net balance available/reserved delta zero; no funding/cashbox/claim reduction. Restoration references original consumption and restores original project/advance; closed projects require reopening first.

DB CHECKs handle row nonnegativity and the existing balance equation. Deferred constraint triggers on T02, T05 and existing balances verify cross-row totals under balance locks, including deletion/update old and new parents. APP posts complete movement groups, verifies original lineage, authorization and cumulative return budgets. Pair/group totals, ledger correlation and type-specific semantics are also checked in the posting transaction; rollout tests deliberately attempt malformed groups. A CHECK cannot safely enforce cross-row sums. [PostgreSQL constraints](https://www.postgresql.org/docs/18/ddl-constraints.html).

**Numerical baseline:** 20,000 LYD in holder H1/advance X: A available 10,000; B 5,000; unassigned 5,000; all reserved zero. A expense 6,000 reservation makes A=(4,000 available,6,000 reserved), whole balance=(14,000,6,000). Approval makes A=(4,000,0), balance=(14,000,0), expensed=6,000. Independent branches from this state: transfer 2,000 → H1 A2,000, H2 A2,000; release 4,000 → H1 A0, B5,000, pool9,000; reallocate 1,000 → A3,000, B6,000, pool5,000. Rejecting a new 1,000 reservation returns A to4,000. Approved return of 1,000 of the original expense restores A to5,000 and balance to15,000, restored counter1,000; it is not extra funding or a second refund. All have immutable movement lineage.

Release additionally requires reconciled holder custody, **zero project reservations and no unresolved project difference**, reason and Manager approval. Transferring or returning to an upstream holder does not silently release designation. Reallocation locks both projects, is separately prepared/approved and cannot consume reserved amounts or cross currencies. Unknown holder attribution prevents release; unrelated reviewed positions remain evaluable.

## 5. Reservation/posting action matrix

All rows use section 11 ordering; P=all affected project controls, B=balances, S=positions, R=reservations, O=operation roots. x is positive and in one inherited currency. Each successful material command increments each affected project's revision **once**, even when several tables/entries change. Failure of permission, version, funds, currency, conservation or audit/idempotency persistence rolls back the entire transaction: no partial counters, events or revisions.

| Action | Locked rows after P | Slice and existing balance changes | Immutable entries / revision |
|---|---|---|---|
| Expense create funded by custody | B,S; expense root/allocation,R,O | available−x; reserved+x on selected project slices AND balances | ReserveExpense, T05 Reserved; project +1. No implicit draw from unassigned. Explicit designation must precede or be authorized atomically. |
| Expense approve | B,S; expense,R,O | reserved−x; existing balance expensed+x; T05 Consumed | ConsumeExpense linked existing balance ledger; project +1. |
| Expense reject | B,S; claim/debt if created; expense,R,O | reserved−x, available+x; cancel related untouched liability as existing rules permit | RejectExpense; T05 Released; project +1. |
| Transfer create | sender and destination B (create zero destination safely), selected S; transfer,R,O | sender available−x,reserved+x; destination unchanged | ReserveTransfer; T05 Reserved per slice; each designated project +1. |
| Transfer confirm | sender/destination B,S; transfer,R,O | sender reserved−x/transferred_out+x; destination available+x/received+x | ConfirmTransfer paired entries preserving project; T05 Consumed; each project +1. |
| Transfer reject | sender B,S; transfer,R,O | sender reserved−x,available+x | RejectTransfer; T05 Released; each project +1. |
| Balance return | same reserve/confirm/reject phases and locks | confirmation sender reserved−x/returned+x; upstream recipient available+x/restored+x | ConfirmTransfer with existing BalanceReturn source; project unchanged, project revision +1; no automatic company funding restoration. |
| Designation release | affected B, all project S/R relevant to release; O | project available−x; same balance unassigned available+x; authoritative balance unchanged | Release pair plus approval evidence; project +1, only when D01 predicates hold. |
| Reallocation A→B | both P then B,S; O | A available−x; B available+x; balance unchanged | Reallocate pair; A and B each +1 atomically. |
| Expense return/restoration | original project P, original balance B,S; return/effect/O | project available+x; balance restored+x; source consumption budget reduced | Restore linked T22/original ledger; project +1, no duplicate economic effect. |
| Approved correction | all old/new P,B,S and typed liability/source roots | compensating entry group, never editing old movements | Correction with reason, independent authority, source reference and +1 per project. Closed project must first reopen. |

Unassigned-only custody changes lock balances but do not invent project revisions. A mixed transfer explicitly names each slice including unassigned; all designated projects are discovered before locks and revalidated afterward. Project closure is not prevented by unrelated company custody.

## 6. Snapshot shape and workflow

Choose **immutable header + relational category/currency/amount-meaning rows + versioned canonical detailed payload** (T07/T08). Typed category rows enable reporting and normal Database-First generation; typed header FKs protect project/cycle/policy integrity. A single polymorphic source-ID table cannot have valid FKs to every source type; dozens of typed snapshot tables would duplicate the source domain and obstruct version evolution. Historical source references inside JSON intentionally are not FKs: they include copied facts and versions, are checked at capture, and remain readable if a source is later legitimately superseded. A UUID lookup alone is never a snapshot.

Payload schema v1 requires: tenant/project/cycle; project and financial versions; policy/config/evaluator/schema versions; evaluation UTC; every required category's status; every considered record's type, ID, record version, lifecycle status, source lineage, currency, exact decimal-as-string amount and meaning; blockers; mandatory gap code/reason/scope; source completeness counts; owner schedule and retention items; custody available/reserved/reservations; verified evidence hashes/object versions; paper events; approved exceptions; preparer/submitter/reviewer identities and role-at-time authorization references. Categories with no records still have an evaluated empty result. Restricted UI packets do not reveal hidden details; the server's complete certificate evaluation cannot infer readiness from a role-restricted GET. Manager supplies/reviews restricted owner section without granting Accountant/Deputy broader read permissions.

Amount meanings include CurrentAvailable, CurrentReserved, OutstandingDebt, OutstandingClaim, PendingPayment, UnappliedCredit, ImmediateOwnerReceivable, ImmediateOwnerPayable, QualifiedRetention, DisputedOwnerPosition and UnclassifiedDifference. Overlapping debt/payment/credit categories never sum into one balance. Decimal string serialization has fixed scale, canonical UTF-8 bytes and SHA-256; algorithm/schema are versioned. Store no passwords, authentication tokens or temporary URLs. Stable evidence object keys are authorization-protected.

Workflow: create Draft; evaluate repeatedly with distinct immutable snapshots; submit binds exactly one submission snapshot; approve re-evaluates, compares material revision/policy and financial payload, and freezes T06/T07/T08; reject or withdraw records reason/event; replacement cycle links previous cycle. Approved cycle supersession/invalidation is event-derived; no UPDATE of the frozen row. Event numbering is allocated while holding cycle lock. Projection/event/control changes are one transaction. A snapshot hash proves the stored bytes have not changed relative to the hash, not that the accounting was correct; authorization and full evaluation remain mandatory.

## 7. Completion, separation, closure and reopening

Current project dates lack Supervisor acknowledgment and Manager decision evidence. T13 adds minimal event records with role, actor, time, project version, reason and evidence. Manager completion uses the current valid Supervisor acknowledgment required by policy; the acknowledgment never grants financial approval. Cancelled run-off uses cancellation basis/evidence, not fabricated completed dates.

Default authority: Accountant/Deputy prepares and submits; independent Manager approves and closes; Supervisor acknowledges physical completion; Worker performs none of these transitions. T06 inequality CHECKs enforce preparer/submitter != approver. APP verifies active membership and action-specific permission, approver/closer independence from preparer/submitter and beneficiary conflict; current role cannot overwrite role-at-time evidence. Approver and closer may be the **same independent Manager**. If that Manager is conflicted, another authorized reviewer is needed; there is no permanent two-Manager requirement or implicit self-approval exemption. T12 records each decision, not just the current role. Delegated approval requires later explicit policy/schema support; v1 stores no invented active delegation and grants no delegated closure.

T10 is immutable certificate history. Its effective state is Active if selected by T01, Reopened if linked by T11, otherwise Superseded only with a valid supersession event; never infer Active merely from absence of a reopen row. Exactly one control per project and a same-project certificate FK prevent two active closures. A deferred trigger checks that the certificate's approved cycle, revision, policy and snapshot belong to that project. APP proves blocker/gap absence. Completed closure updates projects.status to FinanciallyClosed and project version+1; Cancelled stays Cancelled with unchanged project version, but financial control closes and version advances. Certificate records both versions explicitly.

Reopening is a Manager command with nonblank reason, idempotency, current certificate/version and authorization. Under the same gate: append T11 and Reopened event; preserve old approved cycle/certificate/snapshots; clear active closure; increment material revision once; create new Draft cycle, set active cycle and InSettlement; normal project returns to Completed with project version+1, Cancelled remains Cancelled with unchanged project version; append audit and commit. A reopen is not a destructive reversal of posted transactions and not permission to resume physical work. Subsequent late invoices/approved corrections use ordinary gated writers and increment revision again. Closure cannot launch until this complete path exists and has race tests.

## 8. Per-currency owner reconciliation and retention

T19 owns T20 per-currency signed reconciliation and T21 individually evidenced retention. Its approval is material: bind financial_revision to the resulting revision after that command, not its stale pre-command value. Later evaluation validates the source versions/facts again; the owner schedule is not itself the final settlement approval. For currency c, require:

`contract_amount + approved_change_amount − payments_received_amount + refunds_amount = immediate_receivable_amount − immediate_payable_amount + retention_amount + disputed_amount + unclassified_difference_amount`.

Disputed amount is signed: positive disputed receivable, negative disputed payable. Source details retain separate gross disputed receivable and payable amounts, even if their net is zero; any unresolved dispute blocks closure. It cannot be silently treated as known payable or qualify for closure. Unknown figures stay Unknown with mandatory gaps; do not fabricate zeros to satisfy the equation. Submitted/approved known schedules must fully balance; Draft may preserve explicit unknowns in source_details and cannot certify. Source details explain each contractual component and link recorded payments/refunds/changes with versions; source coverage prevents counting a payment both at header and allocation level. Different currencies get different rows and equations, never a converted or net total.

| Owner state | Settlement | Final financial closure |
|---|---|---|
| Known immediate receivable/payable outstanding | Blocks final cash settlement until collected/refunded or an explicitly approved applicable disposition is posted | Blocks |
| Qualified undisputed contractual retention with known amount/currency, basis, release condition/date when applicable and explicit OutsideImmediateSettlement classification | Does not automatically block immediate cash settlement | Does not block by itself; remains outstanding and disclosed in T10 snapshot, never zeroed or written off |
| Unknown or unclassified amount/basis/currency | Mandatory evaluation gap; cannot certify a complete settlement | Blocks readiness/indeterminate if no known blocker |
| Disputed owner entitlement | Known blocker; cannot certify final reconciliation as resolved | Blocks |
| Pending approval, incomplete refund or contract-change effects | Blocks or gap according to known evidence | Blocks |

This implements D08's qualification, not a new exemption for supplier debt or claims. Percentage is optional descriptive support; exact retained amount is authoritative and must reconcile to its documented base/rounding. Contract terms and currency must be reviewed; the design supplies no made-up retention rate, due date or tax rule. Retention release after closure is a material change and requires reopening before receipt/adjustment.

## 9. Return/difference effects and credit intent

T22 models **one economic posting per row**, with typed root and typed target. Legal matrix: CustodyRestore→custody movement, IncreaseCustody; FundingRestore→funding ledger, IncreaseFunding (supplier_refund may be backing receipt); ClaimAdjustment→claim adjustment, ReduceClaim; ClaimWriteOff→claim write-off, ReduceClaim; DebtWriteOff→debt write-off, ReduceDebt; CreditGranted→supplier credit note, IncreaseCredit. Other requires explicit typed target among these, documented approved treatment and direction; unsupported Other remains a reconciliation gap and cannot close. An approved label or free text is never a posted effect.

The target uniqueness is per actual posting, not per supplier. Store one economic_effect_key assigned when posting; reuse it across retries. Custody/funding/claim/credit target partial unique indexes prevent the same posting being attributed to two returns/resolutions. An expense return may have several effect rows only if permitted by its approved resolution and exact total; no mix invented by the posting service. Under source and target locks, validate same company, project, inherited currency, correct direction, original consumed/paid amounts, cumulative prior effects and reversal budget. For a supplier refund, `net_received + fee = gross_refund`; the funding-restoration row records the net, backing refund records the fee, and the reconciliation explicitly accounts for both. A fee is not unexplained missing cash or a second restoration.

Refund confirmation and funding ledger insertion are one posting with one T22 effect, not separate amounts to add. CreditGranted records issuance; later application reduces unapplied credit and debt through existing allocations/T24, not a second return credit. Reversal is a new linked compensating effect with original immutable target retained; its positive amount has negative reconciliation sign through polarity Reversal and a new compensating target posting; APP prevents cumulative reversals exceeding the original. Approved advance difference resolutions must similarly link real claim/funding/write-off outcomes. No company tolerance auto-forgives differences; explicit approved write-off reason/evidence remains required. This closes ApprovedReturnEffectsRequireReconciliation only when every expected effect is verified and no unexplained remainder exists.

Credit lifecycle: note has one or more T23 intentions; nullable project is explicitly CompanyUnassigned, not supplier-derived project intent. Pending project-intended amount is visible to that project's evaluator even before debt allocation. Manager-approved intent review must cover the note amount; unknown attribution stays a scoped gap. At allocation, lock all affected projects and note/debts, validate intent project matches debt, insert actual allocation+T24, increment consumed, and update debt in one transaction. `unapplied intended = intended − consumed − released`; report that amount as intent and actual allocated amount through debt/credit allocation, never both as unapplied. Company intent must first be explicitly reallocated/approved to the target project; reassignment locks old/new projects and increments both. Intent changes do not rewrite posted allocations; compensating operations preserve history. Every intent edit/approval/reassignment appends the prior/new values, actor and reason to existing immutable audit_logs in the same transaction.

## 10. Complete writer/action inventory

Search scope: entire backend source for SaveChanges, raw SQL, ExecuteUpdate/Delete, hosted/background services, controllers, imports/admin entry points and generated-only workflow tables. Current SaveChanges families are AdvanceService, ExpenseService, SupplierService, ProjectService, IdentityService, InvitationService and JoinRequestService. No implemented financial job/import/admin bypass or hosted worker was found; generated background_jobs/outbox/scheduled_job tables do not constitute implemented writers. ProjectSettlementService only runs a read-only evaluator. Future scripts/jobs must call the same command protocol; direct DML imports are disallowed once closure is enabled.

There are **48 inventory entries**, including current endpoints, internal posting helpers, future missing command families and nonfinancial boundary exclusions. This is not a claim of 48 implemented endpoints. `F` means future/no current writer, locks or implemented concurrency token. Current `V` means configured per-record VersionNumber optimistic token; allocation/ledger inserts alone have no token. All financial gate entries also compare T01 version/revision. `G`=gate, `R`=material revision, `C`=ordinary action blocked after closure, `M`=may involve multiple projects. `Y*` means only explicitly affected designated projects; `conditional` is detailed in the row. `L` denotes full section 11 order, even when current lock order differs. Service names resolve under Infrastructure; corresponding API controllers are named explicitly.

| # | Writer/action and service/controller | Project source / relationship | Current locks; token | G / R / C / M | Required order or special rule |
|---|---|---|---|---|---|
| W01 | Expense create: ExpenseService.CreateAsync / ExpensesController | request project direct; custody allocations indirect | idempotency, sorted balances; balance V, new expense | Y/Y/Y/possible | L; include explicitly designated slices and new claim/debt |
| W02 | Expense approve: ApproveAsync→ReviewAsync / ExpensesController | expense direct + allocations | idempotency, expense then balances; expense/balance V | Y/Y/Y/possible | L; reorder root lock after project/balances |
| W03 | Expense reject: RejectAsync→ReviewAsync / ExpensesController | expense + claim/debt + slices | expense, balances, linked claim/debt; V | Y/Y/Y/possible | L; atomic reservation and liability cancellation |
| W04 | Evidence metadata add: ExpenseService.AddDocumentAsync / ExpensesController | document→expense direct | idempotency, expense; expense/document V | Y/Y/Y/no | L when settlement-relevant; supplemental-only postclose exception must not alter certified facts |
| W05 | Expense amount/project/payment edit, cancel, reverse: F | old and new expense project, slices | F | Y/Y/Y/yes | L; both old/new project gates, compensating history |
| W06 | Evidence binary verification/revocation/delete or paper receipt/loss: F | evidence project/document→expense | F | Y/Y/Y/possible | L; original immutable, new events; substantive postclose requires reopen |
| W07 | Claim creation embedded in ExpenseService.CreateAsync | claim/expense project direct | transaction, new claim; claim V configured | Y/Y/Y/no | Same W01 command: do not increment twice |
| W08 | Standalone/manager-contribution claim creation: F | explicit claim project, contribution indirect | F | Y/Y/Y/possible | L; contribution itself is not liability |
| W09 | Claim review/adjust/reduce/write-off/cancel/reverse: F (except W03) | claim project direct, return indirect | F; claim V configured only | Y/Y/Y/possible | L; typed effect, authority, zero only after posted resolution |
| W10 | Claim payment create/confirm/reject/reverse and allocations: F | payment→claim allocations indirect | F | Y/Y/Y/yes | L sorted claims and funding sources |
| W11 | Supplier invoice: SupplierService.CreateInvoiceAsync / SupplierInvoicesController | debt + expense explicit project | idempotency, supplier; new expense/debt | Y/Y/Y/no | L; project existence check currently has no closure guard |
| W12 | Supplier debt standalone edit/adjust/write-off/cancel/reverse: F | debt and expense project | F; debt V configured | Y/Y/Y/possible | L; mismatch is gap, never choose whichever link convenient |
| W13 | Supplier payment create: SupplierService.CreatePaymentAsync / SupplierPaymentsController | allocations→debts indirect | supplier, sorted debts/sources, idempotency; debt/source V | Y/Y/Y/yes | L; include pending reservations and immediate-confirm branch |
| W14 | Supplier payment confirm: ConfirmPaymentAsync→ReviewPayment / SupplierPaymentsController | all allocated debts | payment, debts/sources, idempotency; V | Y/Y/Y/yes | L; current payment-first pattern must change |
| W15 | Supplier payment reject: RejectPaymentAsync→ReviewPayment / SupplierPaymentsController | all allocated debts | payment, sources/debts as applicable; V | Y/Y/Y/yes | L; release pending source reservation atomically |
| W16 | Supplier payment reverse/correct/change allocation: F | old/new allocated debts | F | Y/Y/Y/yes | L; freeze source set during discovery |
| W17 | Credit create: SupplierService.CreateCreditNoteAsync / SupplierCreditsController | future intent, currently none at header | supplier/idempotency; new credit | Y*/Y*/Y*/yes | L; pending intent must be visible before allocations exist |
| W18 | Credit approve: SupplierService.ApproveCreditNoteAsync / SupplierCreditsController | future intents, existing allocations | credit/idempotency; credit V | Y*/Y*/Y*/yes | L; approved header alone does not resolve a project |
| W19 | Credit apply: SupplierService.ApplyCreditNoteAsync / SupplierCreditsController | allocated debts indirect | credit, sorted debts/idempotency; credit/debt V | Y/Y/Y/yes | L; T24 plus consumed intent; exactly once |
| W20 | Credit intent create/reassign/release, reject/cancel/reverse note: F | explicit old/new intent projects | F | Y*/Y*/Y*/yes | L; company-only note no fabricated project; reviewed uncertainty scope |
| W21 | Expense return submit/approve/reject/reverse: F | return→expense and original allocation | F | Y/Y/Y/possible | L; T22/source budget, immutable compensations |
| W22 | Supplier refund confirm/cancel/reverse: F; SupplierService.ListRefundsAsync is read-only | refund→return→expense | F; refund V configured | Y/Y/Y/possible | L funding/source roots and T22 dedup |
| W23 | Owner receipts and project allocation changes: F | explicit owner allocations, old/new projects | F | Y/Y/Y/yes | L; provenance does not designate custody |
| W24 | Owner refund lifecycle/funding allocation: F | refund.project + original payment | F | Y/Y/Y/yes | L; exact verified currency; source consistency |
| W25 | Contract change lifecycle: F | direct project | F | Y/Y/Y/no | L; all approved historical rows, not singular navigation |
| W26 | Owner reconciliation/retention/classification approval or correction: F | explicit project | F | Y/Y/Y/no | L; independent approval; material fact changes invalidate packet |
| W27 | Manager contribution funding/classification: F | explicit claim/designation if any | F | conditional/conditional/conditional/possible | L if linked certification facts change; company capital alone creates no project debt |
| W28 | Advance create: AdvanceService.CreateAsync / AdvancesController | currently no project; future explicit slices only | idempotency, sorted funding sources; source V | Y*/Y*/Y*/possible | L; source provenance alone does not make all source projects affected |
| W29 | Advance delivery confirm: ConfirmTransferAsync→ConfirmDeliveryAsync / AdvancesController | future opening designation | transfer, advance, funding sources; V; creates balance | Y*/Y*/Y*/possible | L; zero destination + unassigned opening; no inference |
| W30 | Advance delivery reject/cancel: F; current RejectTransferAsync explicitly refuses AdvanceDelivery | future designated pending scopes | current path returns Invalid after transfer lock; no posting | Y*/Y*/Y*/possible | L if a separately authorized future cancellation workflow is implemented; no custody before delivery |
| W31 | Internal transfer create: AdvanceService.DistributeAsync / AdvancesController | future named slices | idempotency, sender balance; balance V | Y*/Y*/Y*/yes | L; reserve exact slice set |
| W32 | Internal transfer confirm: ConfirmTransferAsync→ConfirmAllocatedTransferAsync / AdvancesController | sender/reservation slices preserved at recipient | transfer then sender/recipient balances; V | Y*/Y*/Y*/yes | L; deterministic holder balance order replaces directional order |
| W33 | Internal transfer reject: RejectTransferAsync / AdvancesController | original reserved slices | transfer then sender balance; V | Y*/Y*/Y*/yes | L; exact reservation restoration |
| W34 | Balance return create: AdvanceService.ReturnAsync / AdvancesController | exact original designated slices | sender balance/idempotency, upstream lineage query; V | Y*/Y*/Y*/yes | L; existing upstream recipient rule retained |
| W35 | Balance return confirm/reject: AdvanceService transfer review / AdvancesController | same project at upstream recipient | transfer and balances; V | Y*/Y*/Y*/yes | L; return does not clear designation/company liability |
| W36 | Designate/release/reallocate/project opening: F | explicit source/destination projects | F | Y/Y/Y/yes | L; Accountant/Deputy prepares, Manager approves |
| W37 | Advance settlement/difference resolution/advance closure: F | all relevant project slices and linked claims/effects | F | Y*/Y*/Y*/yes | L; cannot rely on existing advance computed readiness |
| W38 | Transfer correction requests, financial reversal/adjustment imports: F | all old/new slice/expense/liability links | F | Y*/Y*/Y*/yes | L; imports invoke gated commands, no direct DML |
| W39 | Company cashbox/funding-source ledger writer: current internal advance/supplier helpers; standalone F | explicit effect/payment projects only | source locks; source V | conditional/conditional/conditional/yes | L; parent command increments once; unrelated funding must not block all projects |
| W40 | Settlement create/evaluate/submit/approve/reject/withdraw/supersede: F | direct project | F | Y/no/no/no | L; closed state rejects ordinary cycle transitions; version/events advance, financial revision does not; evaluate read-only remains allowed |
| W41 | Financial closure: F | direct project | F | Y/no/special/no | L; active cert rejects distinct second request; same idempotent replay allowed |
| W42 | Financial reopening: F | direct project/certificate | F | Y/Y/special/no | L; only authorized transition unlocks; old history frozen |
| W43 | Operational completion/cancellation/acknowledgment: F | direct project | F | Y/Y/Y/no | L; basis/evidence changes are certification material |
| W44 | Project create/update/supervisor: ProjectService / ProjectsController | direct project; metadata/assignment | create transaction; update optimistic project V; assignment project FOR UPDATE + V | conditional/conditional/conditional/no | Create T01; contract/owner/status changes gated+revision; cosmetic name/address no material bump; project version/fingerprint checks still apply |
| W45 | Category create; supplier create/update/payment-account create: ExpenseService / ExpenseCategoriesController, SupplierService / SuppliersController, SuppliersController | usually company master data | category new; supplier/account locks+V | conditional/conditional/conditional/possible | Nonfinancial names/contacts excluded; changes to applicable evidence rules or certified recipient identity require versioned policy/affected-project protocol |
| W46 | Policy binding/category exception/configuration activation: F | explicit policy binding or project exception | F | conditional/conditional/conditional/possible | Binding lock first; policy-ID mismatch invalidates pending packet without bulk revisions; project exception gates+revision |
| W47 | IdentityService, InvitationService, JoinRequestService / corresponding controllers | membership authority, not financial attribution | current membership/company/join/invitation transaction/version rules | no/no/no/no | Admission/role changes synchronize with actor membership lock; no financial revision for login/invite; revalidate authority at commit transaction |
| W48 | Audit/idempotency helper inserts; jobs/outbox/report/import/admin boundary | parent command or no project | parent transaction; immutable append, idempotency key lock | inherited/inherited/inherited/inherited | Never standalone second revision; no financial background/admin implementation found; future equivalent writer must be registered and tested |

`W40 C=no` means no material financial posting; it does not permit mutation of frozen cycles or new ordinary settlement commands while financially closed. `W41/W42 special` are the only certification transitions of closed authority. Cosmetic changes must not silently change a field included in a certified financial fact; administrative corrections after close require their own narrow audit policy and cannot revise immutable snapshots.

## 11. Financial revision and universal synchronization

**Material revision contract:** one increment per affected project per committed logical command, not per SaveChanges or changed row. Include new/changed expense, amount/project/payment mode, approve/reject/cancel/reversal; claim/debt and payment/credit lifecycle/allocation; return/refund/effect; owner contract/position/retention; custody designation/transfer/return/restoration/reservation/consumption/release/reallocation; relevant binary verification/paper/exception; physical basis/acknowledgment and reopen. An edit moving A→B increments both even if amount unchanged. Store last_material_change_at in same update. Enqueue Invalidated once when current submitted/approved evidence is made stale, without editing frozen approval.

No increment for GET, failed command, exact idempotent replay, cosmetic notes not used in certification, notification/report generation, authentication, ordinary settlement workflow events or closure itself. These still advance their own versions/events where applicable. Any notes/evidence actually changing the evaluator's findings are material regardless of UI label. Policy binding changes are detected by immutable policy ID as well as revision. Project version captures operational changes independently; a harmless project-version mismatch may require recapture/reapproval, never silent acceptance of a stale expected version.

### Canonical lock order (all updated writers, including current ones)

1. Begin transaction; acquire/insert existing tenant idempotency record keyed to operation + request hash. Unique-key conflict waits/retries; no committed pending stub outside the financial transaction.
2. Admission: company status row **FOR SHARE** only when needed for company-active authorization; actor membership (`app_users`) FOR SHARE; relevant T15 policy binding FOR SHARE. Revocation/status/policy writers lock their corresponding row exclusively first, in company→user UUID→binding order. Shared locks coexist; no global company FOR UPDATE lock on ordinary financial writers.
3. Discover full candidate project set from requested IDs AND existing allocations/intents/source lineage, old and new. Lock T01 controls by ascending PostgreSQL UUID order within company; re-read lineage. If set changes, rollback/retry discovery; never append a newly discovered lower-order project lock. Every project-affecting insert must lock control before insertion, so a closure protects even an empty category.
4. Projects themselves, then advances (UUID order), then **user_advance_balances sorted by balance UUID**, then T02 slices sorted(balance UUID, nullable project key with NULL first, position UUID). For absent destination balances/positions, lock advance then use unique-key create/reload before position locks; all paths use this same order. No unordered sender→recipient locks.
5. Claims sorted UUID, supplier master rows sorted UUID when needed, supplier debts sorted UUID, funding sources sorted UUID, then payment/credit/expense/transfer/return/refund/contract roots by fixed table rank (in that listed order) then UUID. T05 reservations/T03 operations/effect/intent rows follow their parent roots in fixed table-name/UUID order. Source discovery reads before this point are provisional only.
6. Settlement cycles sorted UUID, owner reconciliations then positions/retention, evidence workflow rows sorted table-name/UUID; snapshots/events/certificate inserts; control/project updates; audit/idempotency result. No late acquisition of an earlier-ranked mutable row. If evaluator finds an additional required mutable root, restart with complete lock set.

All affected rows include company predicates. New project-owned records require T01 lock even if they have no previous balance. A shared payment touches every allocation project. If any is closed, reject the whole command; do not partially pay the others. Reopen required projects explicitly first. Unassigned company-only work locks only its actual source/balance and does not take a company-wide exclusive project gate. Existing current lock orders in W02/W14/W32 must be refactored together before activation; adding the new lock at the end is unsafe.

**Isolation recommendation:** ordinary upgraded writers ReadCommitted with the complete gate/row protocol and expected tokens. Approval and closure Serializable with bounded full-transaction retries; reopening Serializable for the certificate/control/new-cycle transition. Read-only previews may retain RepeatableRead. RepeatableRead plus a universally shared, updated control row can protect project invariants, but a snapshot acquired before waiting may be stale or force a retry; it cannot by itself solve omitted writers or wider policy/attribution predicates. Serializable is defense for certification's multi-source predicate reads, not a substitute for enrolling all writers. Retry SQLSTATE 40001 and 40P01 from the start using the same idempotency key; do not retry a business version mismatch as if approved. [Isolation](https://www.postgresql.org/docs/18/transaction-iso.html), [row locks and deadlocks](https://www.postgresql.org/docs/18/explicit-locking.html).

Finance and evidence writers that can change a certification predicate obey the gate; policy/authority changes obey admission locks. No network upload/remote fetch occurs while holding these database locks: establish a durable verification attestation before the command, then validate its continued availability/version. Privileged DBA changes remain outside this protocol and require maintenance-mode closure suspension and review.

## 12. Exact future closure transaction

1. Validate request shape, authenticated tenant, idempotency key and expected project/control/cycle versions outside transaction; this grants no financial authority.
2. Begin Serializable; reserve/read the tenant operation-scoped idempotency record. Same key+same hash completed result replays after access validation; different hash conflicts. A concurrent duplicate yields exactly one committed certificate/result.
3. Revalidate active company/membership and Manager action authority under admission locks; capture role-at-time and beneficiary/separation decision. Resolve immutable active policy binding. Reject unsupported delegation.
4. Discover and lock full project set (normally one) in UUID order; lock T01. Require expected control version/revision, correct operational basis, no active certificate, no missing/indeterminate attribution. A different-key second close conflicts rather than creates another certificate.
5. Lock project and all required existing mutable financial roots in canonical order, then current settlement. Validate expected project/cycle version, current effective Approved cycle, same project, same current policy and captured revision. Current closed projects cannot accept ordinary late financial records.
6. Run full authoritative evaluator in this transaction over all required categories, including empty sets, pending records, evidence/paper, actual return effects, scoped legacy reviews, owner schedule, custody and qualified retention. Never reuse the GET response or role-filtered totals as authority.
7. Require zero known blockers and zero mandatory gaps, exact per-currency resolution and D01 release conditions where used. Validate completed/cancelled basis and approval separation. Missing evidence/configuration fails, never defaults to pass.
8. Compare approved financial snapshot facts/hash inputs with current evaluated facts; permitted contextual differences are only evaluation/actor timestamps and closure purpose. Any material difference, stale revision or policy mismatch requires new reconciliation/approval; do not silently refresh approval.
9. Insert immutable ClosureCheck snapshot/category rows; compute/store exact canonical bytes and hash. Insert T12 allowed closure decision and T10 certificate referencing both approved snapshot and closure snapshot. Allocate closure_version under T01 (prior maximum+1 is safe only under this exclusive per-project gate).
10. Set T01 financial_state FinanciallyClosed, active_closure_id and control version+1; retain material revision R. Completed project → FinanciallyClosed, project version+1 with existing completed dates retained. Cancelled project remains Cancelled, project version unchanged. Validate deferred same-project/certificate/projection constraints.
11. Append Closed workflow event and existing audit record with actor/reason/revision/versions; save completed idempotency response in the same transaction. Failures roll back certificate, state, snapshot, audit and replay result together.
12. Commit. On serialization/deadlock failure restart entire transaction with bounded retry; on version/eligibility/auth failure return conflict/forbidden/validation as appropriate. No external notification is emitted before commit; later notifications can consume committed outbox work if implemented.

## 13. Legacy cutover and rollback

This milestone inspects metadata only. Actual cutover necessarily requires authorized staging/business-data review later; no current tenant has been declared reconciled here.

1. Approve design and separately approve future DDL preparation. Back up staging, verify restore, record schema/application versions and immutable artifact hashes. Before production repeat backup/restore proof and record recovery point.
2. Rehearse under a **financial write maintenance window per cutover company**, draining in-flight writes and disabling old jobs/imports/admin DML. Record boundary UUID/time and authoritative row versions/totals. This is a one-time boundary, not a permanent global company lock. Keep existing read-only reporting available as permitted.
3. Inventory every balance and counter, pending delivery/transfer/return, expense allocation/reservation, expense/project link, claim/payment, supplier debt/payment/credit, manager contribution/claim classification, owner contract/receipts/refunds/retention, approved returns/refunds/difference effects, evidence and cancellation/completion basis. Reconcile source ledgers to authoritative current totals without editing them.
4. Obtain holder declarations of **current available and reserved** custody by project/unassigned, with evidence. Accountant/Deputy reviews; independent Manager approves opening designations. Do not infer remaining custody from historical expense ratios, supplier identity or source provenance. Historical postings remain untouched.
5. T25 records each source/boundary/version/digest; T26 captures affected projects. Approved opening T03/T04 entries establish current slices, not a replay of all historical gross receipts. Persist baseline history reconciliation separately in evidence. For an uncertain balance use a provisional unassigned slice to conserve arithmetic, mark review Indeterminate and explicitly state that unassigned does not certify ownership.
6. Pending transfers/returns keep amounts reserved at sender until actual confirmation; no destination opening receipt and no double custody. Link original allocation to T05 slices only when reviewed. Unresolved reservation attribution stays a gap for affected projects and prevents their closure. Pending delivery with no existing custody stays a source reservation, not an invented balance. Reconcile pending expenses similarly.
7. Unknown sources affect explicitly plausible projects; reviewed unrelated projects remain evaluable. If scope is truly unbounded, document that uncertainty and require project completeness review before declaring any project unrelated; never silently assign CompanyOnly. Resolve unknowns through new reviewed evidence/opening corrections, not ledger edits.
8. Each project gets T01 revision 1, version 1, appropriate Open/InSettlement state and attribution status. Existing `projects.status=FinanciallyClosed` without a valid certificate is a **cutover exception**, not grandfathered certification: keep closure feature disabled for it, obtain Manager-controlled reviewed reconciliation/reopen plan; do not manufacture T10 from status. Reviewed Completed/Cancelled projects enter reconciliation under their genuine basis.
9. Validate exact slice/balance/reservation sums, currency, tenant/link consistency, zero duplicated effects, reviewed credit intent, owner denomination, required evidence and project completeness. Record reviewer signoff and hashes. Enable constraints and upgraded writers atomically with cutover activation; no mixed old/new custody writer period. Openings cannot be recomputed after writers resume.
10. Before activation rollback restores the staging/cutover checkpoint or abandons unactivated new records via an approved rollback artifact, preserving original ledgers. After any new live financial posting/certificate, **no destructive schema downgrade or ledger replay**: disable new commands, keep evidence, recover with approved compensating fixes or a full coordinated database/application restore that accounts for every post-boundary transaction. Never restore just the slices while keeping newer balances. Resume only after reconciled audit approval.

## 14. Acceptance fixtures (specified, not executed)

Shared harness: tenant C and unrelated tenant D; projects A/B/C0; authorized Accountant P, independent Manager M, Supervisor S, holders H1/H2; fixed UTC clock; exact decimal amounts; request keys unique unless replay is tested; all unrelated mandatory categories explicitly evaluated clear, policy v1/configuration known, evidence verified and project basis Completed unless specified. Each test asserts tenant scope, operation/event/audit linkage, expected versions, one material revision per affected project per command, and rollback leaves no partial entries. F01 creates the numerical baseline; F02–F08 explicitly state their base. Race tests require two independent PostgreSQL connections and barriers at gate acquisition, not an in-memory substitute.

| Fixture | Setup and command | Required results |
|---|---|---|
| F01 | Confirm 20,000 LYD custody X/H1; P proposes A10,000/B5,000/pool5,000; M approves reviewed opening | Three unique slices, available sum20,000 reserved0; paired/opening history; A/B revision increment once for posting; source funding not counted again as custody. |
| F02 | From F01 create then approve A expense6,000 | Create A4,000/6,000 and balance14,000/6,000; approval A4,000/0, balance14,000/0, expensed6,000; two commands → A revision+2. |
| F03 | From F02 reserve then confirm A transfer2,000 H1→H2 | H1 A2,000/2,000 pending, H2 A0; confirmed H1 A2,000/0,H2 A2,000/0; H1 balance12,000/H2 2,000; A total4,000; B/pool unchanged. |
| F04 | From F02 reconciled holder, no A reservation/difference; release4,000 approved M | H1 A0,B5,000,pool9,000,balance14,000; no debt reduction/cashbox receipt. Repeat with reserved1 or unresolved difference: reject entirely. |
| F05 | From F02 approved A→B reallocation1,000 | A3,000,B6,000,pool5,000; balance14,000; both revisions+1 atomically; if B closed or Manager approval absent, no change. |
| F06 | From F02 new A expense reservation1,000, then reject | Pending A3,000/1,000; rejection A4,000/0; balance14,000/0; released reservation, no expenditure increase, no surviving claim/debt created solely by rejected item. |
| F07 | From F02 approved return1,000 of original6,000 consumption | A5,000,B5,000,pool5,000; balance15,000, restored1,000; typed return/effect/original allocation; replay identical result; subsequent cumulative return above6,000 rejected. |
| F08 | F01 plus separate USD advance with A100 USD | Reports A10,000 LYD and100 USD separately; LYD transfer from USD slice rejected; no10,100 total; snapshot retains separate category/currency keys. |
| F09 | Legacy balance500 LYD uncertain between A/B; C0 completeness review proves unrelated | Conserving provisional pool500 but A/B Indeterminate, no inferred split; C0 unaffected; Manager reviewed A300/pool200 later resolves only evidenced scope via posted opening correction. |
| F10 | Boundary sender balance available800,reserved200, pending transfer200 A→H2 | Opening sender slice800/200, destination no200 receipt; original transfer allocation mapped to T05; confirm produces sender800/0,recipient200/0; no1,200 total. Unknown designation keeps scoped gap. |
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

## 15. Database-First rollout and acceptance gates

1. Explicitly approve this design and remaining implementation inputs. Next task prepares **reviewable DDL artifacts only**, without executing them, scaffolding or changing application behavior.
2. Backup staging and prove restore; record current catalog again. Write reviewed forward/rollback DDL with every composite FK, index, status CHECK, immutable/deferred trigger and privilege change, including staged handling of nullable legacy denominations.
3. Obtain separate execution approval, apply to staging only; verify catalog, trigger timing, permissions and rejection tests; inspect PostgreSQL constraint/index names and generated expressions.
4. Rehearse maintenance boundary, full cutover inventory, declarations and openings; reconcile every balance/reservation and scoped uncertainty. Test interrupted cutover and rollback before activation.
5. Scaffold Database-First entities/context from the verified staging schema into the isolated generated directories; review complete generated diff including partial-unique navigations, decimals/nullability, computed columns and circular/deferred references. Never manually edit generated entity/mapping files. Keep tokens/conversions in non-generated partial configuration.
6. Implement Domain policy, Application command contracts/orchestration and Infrastructure transactions/persistence in the documented dependency direction; API controllers only route/authenticate. Enroll every applicable W entry, including current commands, before closure. Implement reopening before enabling closing.
7. Run backend validation/unit/integration tests, real PostgreSQL concurrency/serialization tests F01–F24, tenant/role/idempotency tests, source/ledger reconciliation and malicious incomplete posting checks. Prove every future registered writer participates in the gate; no unreviewed direct database writer.
8. Stabilize backend contracts first; then update Flutter for Android/iOS/Web without persistence-model coupling or direct PostgreSQL access. Existing read-only readiness remains until complete true-readiness and authority flows pass.
9. Rehearse production on an authorized restored copy, obtain cutover signoff, backup production and verify restore, drain financial writes and apply controlled approved DDL/cutover. Reinspect catalog, reconcile openings and version boundaries, verify tenant/configuration completeness and replay recovery.
10. Enable behind controlled rollout only for reviewed companies/projects, with closure and reopen together; monitor audit/reconciliation failures. Stop new certification if any writer/gate/data gap appears. Rollback boundaries are section 13; post-posting rollback is not dropping new tables or rewriting ledgers.

**Database invariants:** tenant FKs, one control/project, unique nullable slice, positive versions, nonnegative current custody, valid row statuses/actor pairs, same-project pointers, cycle/certificate numbering uniqueness, typed effect target/uniqueness, immutable posted evidence/history, control/projection consistency, stable inherited custody currency, deferred current custody/reservation conservation. DB triggers require dedicated staging tests and catalog verification; none exists yet.

**Application transaction invariants:** complete writer enrollment/project discovery; authorization, beneficiary separation beyond row identity, evidence availability, policy completeness, legacy uncertainty scope, economic meaning/currency agreement of composite effects and owner schedules, exact reservation/allocation/source budgets, source-to-snapshot completeness, movement pairing, cross-source duplicate prevention, full current evaluation, current policy/revision comparison, audit and idempotency atomicity. Both layers enforce the portions assigned in T01–T26; a valid FK/hash never proves financial correctness.

**Acceptance to prepare DDL:** reviewers can trace every D01–D16 rule to objects, fields, constraints, writer protocol and fixture; table/alteration counts reconcile; no hidden application behavior was changed. **Acceptance to activate closure later:** all mandatory categories evaluable, reviewed attribution/effects/owner/evidence, upgraded writers, immutable history, independent approval, working reopen and race tests. Schema existence alone does not satisfy D16.

## 16. Unresolved inputs and implementation risks

No D01–D16 policy decision is reopened. Remaining inputs are company/category digital and paper choices, actual contract denominations/retention terms, reviewed legacy attribution and financing classifications, durable binary storage/retention verification, and authorized cutover declarations. Missing inputs are explicit gaps, never invented financial rules. No unsupported Other resolution can be certified.

No customer-row audit was authorized/performed here: real legacy quantities, preexisting FinanciallyClosed projects, inconsistent debt/expense links or orphan effect budgets are unknown. Staging rehearsal must measure these. Closure remains blocked until the application-only universal gate has exclusive writer coverage; manual/privileged bypass is an operational risk, not something these five alterations magically prevent. A future second write platform requires reviewed DB enforcement or the same proven command boundary.

DDL review must prove deferred circular references, aggregate-trigger locking, approved-parent child immutability, canonical serialization and Database-First mapping behavior. These are implementation verification tasks, not approval to execute. No backend/Flutter build or test was run for this documentation-only milestone; F01–F24 are acceptance specifications, not passing test claims.

**Exact next task:** after explicit approval of this schema design, prepare the reviewed Database-First forward/rollback DDL and metadata verification/cutover rehearsal scripts for T01–T26 and the five listed alterations, tied to this artifact and Policy v1. Do not execute scripts, mutate any database, scaffold models or implement commands in that preparation task. Submit artifacts and a staging execution/rollback checklist for separate execution authorization.

## Appendix A. Relevant deployed catalog signatures
These **49 tables** were selected from the complete 91-table inspection for this evidence register. All entries below describe the deployed baseline, not the proposed schema. `?` means nullable; `= expression` is an actual default; `GENERATED` identifies stored computed values. PK/UQ definitions and FK target sets are reported; tenant relationships were checked in the full definitions. CHECK names list the inspected constraints; repeated lifecycle actor/time CHECKs are retained by name instead of copying thousands of expressions. Index/trigger counts include all catalog entries for that table. This compact register is not an executable schema dump.
Metadata capture SHA-256: `eef20a6bae7c651f9a8ebe571ff5a8f5c2ad830988766f391ff524bb0e8e1a0e`. The temporary local metadata capture and inspection helper are removed before commit; no database configuration or business rows are retained.

<details>
<summary>projects</summary>

**Columns:** `project_id: uuid = gen_random_uuid()`; `company_id: uuid`; `project_owner_id: uuid`; `project_name: varchar(200)`; `site_address: varchar(500)`; `latitude: numeric(9,6)?`; `longitude: numeric(9,6)?`; `contact_phone_number: varchar(20)?`; `contract_value: numeric(18,2)`; `contract_date: date`; `start_date: date`; `expected_end_date: date?`; `actual_end_date: date?`; `status: varchar(30) = 'Active'::character varying`; `description: varchar(1500)?`; `notes: varchar(1000)?`; `created_by_user_id: uuid`; `completed_at: timestamptz?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (project_id)`; `UNIQUE (company_id, project_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id) REFERENCES companies(company_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_owner_id) REFERENCES project_owners(company_id, project_owner_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_projects_actual_end_date`, `ck_projects_cancellation_fields`, `ck_projects_cancelled_after_creation`, `ck_projects_completed_after_creation`, `ck_projects_completed_fields`, `ck_projects_contact_phone_format`, `ck_projects_contract_value`, `ck_projects_coordinates_together`, `ck_projects_description_not_blank`, `ck_projects_expected_end_date`, `ck_projects_latitude`, `ck_projects_longitude`, `ck_projects_name_not_blank`, `ck_projects_notes_not_blank`, `ck_projects_site_address_not_blank`, `ck_projects_status`, `ck_projects_version_number`.

**Indexes:** 7 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_projects_set_updated_at BEFORE UPDATE ON ahdah.projects FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>project_supervisors</summary>

**Columns:** `project_supervisor_id: uuid = gen_random_uuid()`; `company_id: uuid`; `project_id: uuid`; `supervisor_user_id: uuid`; `assigned_by_user_id: uuid`; `assigned_at: timestamptz = CURRENT_TIMESTAMP`; `is_active: bool = true`; `removed_by_user_id: uuid?`; `removed_at: timestamptz?`; `removal_reason: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (project_supervisor_id)`; `UNIQUE (company_id, project_supervisor_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, assigned_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_id) REFERENCES projects(company_id, project_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, removed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supervisor_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_project_supervisors_active_fields`, `ck_project_supervisors_removed_after_assignment`.

**Indexes:** 7 total. Partial: `CREATE UNIQUE INDEX ux_project_supervisors_active_assignment ON ahdah.project_supervisors USING btree (company_id, project_id, supervisor_user_id) WHERE (is_active = true)`.

**Triggers:** `CREATE TRIGGER trg_project_supervisors_set_updated_at BEFORE UPDATE ON ahdah.project_supervisors FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>advances</summary>

**Columns:** `advance_id: uuid = gen_random_uuid()`; `company_id: uuid`; `advance_number: varchar(50)`; `deputy_user_id: uuid`; `advance_amount: numeric(18,2)`; `currency_code: varchar(3) = 'LYD'::character varying`; `issue_date: date`; `settlement_due_date: date?`; `purpose: varchar(1000)`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `confirmed_by_user_id: uuid?`; `confirmed_at: timestamptz?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `closed_at: timestamptz?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (advance_id)`; `UNIQUE (company_id, advance_id)`; `UNIQUE (company_id, advance_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id) REFERENCES companies(company_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, confirmed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, deputy_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_advances_amount`, `ck_advances_cancelled_after_creation`, `ck_advances_cancelled_fields`, `ck_advances_closed_after_confirmation`, `ck_advances_closed_fields`, `ck_advances_confirmed_after_creation`, `ck_advances_confirmed_fields`, `ck_advances_currency_code`, `ck_advances_notes`, `ck_advances_number`, `ck_advances_purpose`, `ck_advances_reversed_after_confirmation`, `ck_advances_reversed_fields`, `ck_advances_settlement_due_date`, `ck_advances_status`, `ck_advances_unconfirmed_fields`, `ck_advances_version`.

**Indexes:** 9 total. Partial: `CREATE INDEX ix_advances_settlement_due_date ON ahdah.advances USING btree (company_id, settlement_due_date) WHERE ((settlement_due_date IS NOT NULL) AND ((status)::text <> ALL ((ARRAY['Closed'::character varying, 'Cancelled'::character varying, 'Reversed'::character varying])::text[])))`.

**Triggers:** `CREATE TRIGGER trg_advances_set_updated_at BEFORE UPDATE ON ahdah.advances FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>user_advance_balances</summary>

**Columns:** `user_advance_balance_id: uuid = gen_random_uuid()`; `company_id: uuid`; `advance_id: uuid`; `user_id: uuid`; `total_received_amount: numeric(18,2) = 0`; `total_restored_amount: numeric(18,2) = 0`; `total_expensed_amount: numeric(18,2) = 0`; `total_transferred_out_amount: numeric(18,2) = 0`; `total_returned_amount: numeric(18,2) = 0`; `total_adjustment_out_amount: numeric(18,2) = 0`; `available_amount: numeric(18,2) = 0`; `reserved_amount: numeric(18,2) = 0`; `status: varchar(30) = 'Active'::character varying`; `created_by_user_id: uuid`; `settled_at: timestamptz?`; `closed_at: timestamptz?`; `notes: varchar(1000)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `UNIQUE (company_id, advance_id, user_id)`; `UNIQUE (company_id, user_advance_balance_id)`; `PRIMARY KEY (user_advance_balance_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, advance_id) REFERENCES advances(company_id, advance_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_user_advance_balances_active_fields`, `ck_user_advance_balances_balance_equation`, `ck_user_advance_balances_closed_after_settlement`, `ck_user_advance_balances_closed_fields`, `ck_user_advance_balances_non_negative`, `ck_user_advance_balances_notes`, `ck_user_advance_balances_settled_after_creation`, `ck_user_advance_balances_settled_fields`, `ck_user_advance_balances_status`, `ck_user_advance_balances_version`.

**Indexes:** 8 total. Partial: `CREATE INDEX ix_user_advance_balances_available ON ahdah.user_advance_balances USING btree (company_id, user_id, available_amount) WHERE ((status)::text = ANY ((ARRAY['Active'::character varying, 'InSettlement'::character varying])::text[]))`.

**Triggers:** `CREATE TRIGGER trg_user_advance_balances_set_updated_at BEFORE UPDATE ON ahdah.user_advance_balances FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>money_transfers</summary>

**Columns:** `money_transfer_id: uuid = gen_random_uuid()`; `company_id: uuid`; `transfer_number: varchar(50)`; `transfer_type: varchar(30)`; `advance_id: uuid?`; `sender_user_id: uuid`; `recipient_user_id: uuid`; `transfer_amount: numeric(18,2)`; `currency_code: varchar(3) = 'LYD'::character varying`; `transfer_date: date`; `transfer_method: varchar(30)`; `bank_name: varchar(150)?`; `reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `description: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `initiated_by_user_id: uuid`; `confirmed_by_user_id: uuid?`; `confirmed_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (money_transfer_id)`; `UNIQUE (company_id, money_transfer_id)`; `UNIQUE (company_id, transfer_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, advance_id) REFERENCES advances(company_id, advance_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, confirmed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, initiated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, recipient_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, sender_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_money_transfers_advance_reference`, `ck_money_transfers_amount`, `ck_money_transfers_bank_fields`, `ck_money_transfers_bank_name`, `ck_money_transfers_cancelled_after_creation`, `ck_money_transfers_cancelled_fields`, `ck_money_transfers_confirmed_after_creation`, `ck_money_transfers_confirmed_fields`, `ck_money_transfers_currency`, `ck_money_transfers_description`, `ck_money_transfers_different_users`, `ck_money_transfers_method`, `ck_money_transfers_notes`, `ck_money_transfers_number`, `ck_money_transfers_open_fields`, `ck_money_transfers_other_description`, `ck_money_transfers_proof`, `ck_money_transfers_reference`, `ck_money_transfers_reference_required`, `ck_money_transfers_rejected_after_creation`, `ck_money_transfers_rejected_fields`, `ck_money_transfers_reversed_after_confirmation`, `ck_money_transfers_reversed_fields`, `ck_money_transfers_status`, `ck_money_transfers_type`, `ck_money_transfers_version`.

**Indexes:** 11 total. Partial: `CREATE UNIQUE INDEX ux_money_transfers_advance_delivery ON ahdah.money_transfers USING btree (company_id, advance_id) WHERE ((transfer_type)::text = 'AdvanceDelivery'::text)`; `CREATE INDEX ix_money_transfers_recipient_pending ON ahdah.money_transfers USING btree (company_id, recipient_user_id, transfer_date) WHERE ((status)::text = 'PendingConfirmation'::text)`.

**Triggers:** `CREATE TRIGGER trg_money_transfers_set_updated_at BEFORE UPDATE ON ahdah.money_transfers FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>transfer_advance_allocations</summary>

**Columns:** `transfer_advance_allocation_id: uuid = gen_random_uuid()`; `company_id: uuid`; `money_transfer_id: uuid`; `advance_id: uuid`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (transfer_advance_allocation_id)`; `UNIQUE (company_id, transfer_advance_allocation_id)`; `UNIQUE (company_id, money_transfer_id, advance_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, advance_id) REFERENCES advances(company_id, advance_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, money_transfer_id) REFERENCES money_transfers(company_id, money_transfer_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_transfer_advance_allocations_amount`, `ck_transfer_advance_allocations_notes`.

**Indexes:** 7 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_transfer_advance_allocations_set_updated_at BEFORE UPDATE ON ahdah.transfer_advance_allocations FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>advance_funding_sources</summary>

**Columns:** `advance_funding_source_id: uuid = gen_random_uuid()`; `company_id: uuid`; `advance_id: uuid`; `funding_source_id: uuid`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (advance_funding_source_id)`; `UNIQUE (company_id, advance_id, funding_source_id)`; `UNIQUE (company_id, advance_funding_source_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, advance_id) REFERENCES advances(company_id, advance_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_advance_funding_sources_amount`, `ck_advance_funding_sources_notes`.

**Indexes:** 6 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_advance_funding_sources_set_updated_at BEFORE UPDATE ON ahdah.advance_funding_sources FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>advance_settlements</summary>

**Columns:** `advance_settlement_id: uuid = gen_random_uuid()`; `company_id: uuid`; `settlement_number: varchar(50)`; `user_advance_balance_id: uuid`; `balance_version_number: int`; `settlement_date: date`; `currency_code: varchar(3) = 'LYD'::character varying`; `received_snapshot_amount: numeric(18,2)`; `restored_snapshot_amount: numeric(18,2)`; `expensed_snapshot_amount: numeric(18,2)`; `transferred_out_snapshot_amount: numeric(18,2)`; `returned_snapshot_amount: numeric(18,2)`; `adjustment_out_snapshot_amount: numeric(18,2)`; `available_snapshot_amount: numeric(18,2)`; `reserved_snapshot_amount: numeric(18,2)`; `declared_remaining_amount: numeric(18,2)`; `difference_amount: numeric(18,2)? GENERATED (declared_remaining_amount - available_snapshot_amount)`; `difference_type: varchar(20)? GENERATED  CASE WHEN (declared_remaining_amount = available_snapshot_amount) THEN 'Balanced'::text WHEN (declared_remaining_amount < available_snapshot_amount) THEN 'Shortage'::text ELSE 'Surplus'::text END`; `settlement_explanation: varchar(1000)?`; `supporting_document_url: varchar(1000)?`; `resolution_notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `reviewed_by_user_id: uuid?`; `reviewed_at: timestamptz?`; `review_notes: varchar(1000)?`; `correction_reason: varchar(1000)?`; `rejection_reason: varchar(1000)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (advance_settlement_id)`; `UNIQUE (company_id, advance_settlement_id)`; `UNIQUE (company_id, settlement_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, user_advance_balance_id) REFERENCES user_advance_balances(company_id, user_advance_balance_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reviewed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_advance_settlements_approved_difference`, `ck_advance_settlements_approved_fields`, `ck_advance_settlements_balance_version`, `ck_advance_settlements_cancelled_after_creation`, `ck_advance_settlements_cancelled_fields`, `ck_advance_settlements_correction_fields`, `ck_advance_settlements_correction_reason`, `ck_advance_settlements_currency`, `ck_advance_settlements_difference_explanation`, `ck_advance_settlements_document`, `ck_advance_settlements_draft_fields`, `ck_advance_settlements_explanation`, `ck_advance_settlements_no_pending_reservations`, `ck_advance_settlements_number`, `ck_advance_settlements_pending_fields`, `ck_advance_settlements_rejected_fields`, `ck_advance_settlements_rejection_reason`, `ck_advance_settlements_resolution_notes`, `ck_advance_settlements_reversed_after_approval`, `ck_advance_settlements_reversed_fields`, `ck_advance_settlements_review_notes`, `ck_advance_settlements_review_pair`, `ck_advance_settlements_reviewed_after_submission`, `ck_advance_settlements_snapshot_equation`, `ck_advance_settlements_snapshot_non_negative`, `ck_advance_settlements_status`, `ck_advance_settlements_submission_pair`, `ck_advance_settlements_submitted_after_creation`, `ck_advance_settlements_version`.

**Indexes:** 10 total. Partial: `CREATE UNIQUE INDEX ux_advance_settlements_one_active ON ahdah.advance_settlements USING btree (company_id, user_advance_balance_id) WHERE ((status)::text = ANY ((ARRAY['Draft'::character varying, 'PendingReview'::character varying, 'CorrectionRequired'::character varying, 'Approved'::character varying])::text[]))`; `CREATE INDEX ix_advance_settlements_pending_review ON ahdah.advance_settlements USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingReview'::text)`; `CREATE INDEX ix_advance_settlements_difference ON ahdah.advance_settlements USING btree (company_id, difference_type) WHERE ((difference_type)::text <> 'Balanced'::text)`.

**Triggers:** `CREATE TRIGGER trg_advance_settlements_set_updated_at BEFORE UPDATE ON ahdah.advance_settlements FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>advance_settlement_documents</summary>

**Columns:** `advance_settlement_document_id: uuid = gen_random_uuid()`; `company_id: uuid`; `advance_settlement_id: uuid`; `document_type: varchar(40)`; `document_number: varchar(100)?`; `document_date: date?`; `issuer_name: varchar(200)?`; `original_file_name: varchar(255)`; `file_url: varchar(1000)`; `mime_type: varchar(150)`; `file_size_bytes: bigint`; `sha256_hash: character(64)`; `capture_source: varchar(30) = 'FileUpload'::character varying`; `is_primary: bool = false`; `verification_status: varchar(30) = 'PendingVerification'::character varying`; `uploaded_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `rejection_reason: varchar(500)?`; `notes: varchar(1000)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (advance_settlement_document_id)`; `UNIQUE (company_id, advance_settlement_document_id)`; `UNIQUE (company_id, advance_settlement_id, sha256_hash)`.

**Foreign keys:** `FOREIGN KEY (company_id, advance_settlement_id) REFERENCES advance_settlements(company_id, advance_settlement_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, uploaded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_advance_settlement_documents_capture_source`, `ck_advance_settlement_documents_file_name`, `ck_advance_settlement_documents_file_size`, `ck_advance_settlement_documents_file_url`, `ck_advance_settlement_documents_issuer`, `ck_advance_settlement_documents_mime_type`, `ck_advance_settlement_documents_notes`, `ck_advance_settlement_documents_number`, `ck_advance_settlement_documents_other_notes`, `ck_advance_settlement_documents_pending_fields`, `ck_advance_settlement_documents_primary`, `ck_advance_settlement_documents_rejected_fields`, `ck_advance_settlement_documents_sha256`, `ck_advance_settlement_documents_type`, `ck_advance_settlement_documents_verification_status`, `ck_advance_settlement_documents_verified_after_upload`, `ck_advance_settlement_documents_verified_fields`, `ck_advance_settlement_documents_version`.

**Indexes:** 9 total. Partial: `CREATE UNIQUE INDEX ux_advance_settlement_documents_one_primary ON ahdah.advance_settlement_documents USING btree (company_id, advance_settlement_id) WHERE ((is_primary = true) AND ((verification_status)::text <> 'Rejected'::text))`.

**Triggers:** `CREATE TRIGGER trg_advance_settlement_documents_set_updated_at BEFORE UPDATE ON ahdah.advance_settlement_documents FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>advance_settlement_resolutions</summary>

**Columns:** `advance_settlement_resolution_id: uuid = gen_random_uuid()`; `company_id: uuid`; `advance_settlement_id: uuid`; `resolution_number: varchar(50)`; `resolution_date: date`; `resolution_type: varchar(40)`; `resolution_amount: numeric(18,2)`; `funding_source_id: uuid?`; `personal_claim_id: uuid?`; `payment_method: varchar(30)?`; `bank_name: varchar(150)?`; `reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `description: varchar(1000)`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (advance_settlement_resolution_id)`; `UNIQUE (company_id, advance_settlement_resolution_id)`; `UNIQUE (company_id, resolution_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, personal_claim_id) REFERENCES personal_claims(company_id, personal_claim_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, advance_settlement_id) REFERENCES advance_settlements(company_id, advance_settlement_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_advance_settlement_resolutions_amount`, `ck_advance_settlement_resolutions_approval_pair`, `ck_advance_settlement_resolutions_approved_fields`, `ck_advance_settlement_resolutions_approved_time`, `ck_advance_settlement_resolutions_bank_fields`, `ck_advance_settlement_resolutions_bank_name`, `ck_advance_settlement_resolutions_cancellation_pair`, `ck_advance_settlement_resolutions_cancelled_fields`, `ck_advance_settlement_resolutions_cancelled_time`, `ck_advance_settlement_resolutions_description`, `ck_advance_settlement_resolutions_draft_fields`, `ck_advance_settlement_resolutions_notes`, `ck_advance_settlement_resolutions_number`, `ck_advance_settlement_resolutions_payment_fields`, `ck_advance_settlement_resolutions_payment_method`, `ck_advance_settlement_resolutions_payment_proof`, `ck_advance_settlement_resolutions_pending_fields`, `ck_advance_settlement_resolutions_proof`, `ck_advance_settlement_resolutions_reference`, `ck_advance_settlement_resolutions_reference_required`, `ck_advance_settlement_resolutions_rejected_fields`, `ck_advance_settlement_resolutions_rejected_time`, `ck_advance_settlement_resolutions_rejection_pair`, `ck_advance_settlement_resolutions_reversal_pair`, `ck_advance_settlement_resolutions_reversed_fields`, `ck_advance_settlement_resolutions_reversed_time`, `ck_advance_settlement_resolutions_source_reference`, `ck_advance_settlement_resolutions_status`, `ck_advance_settlement_resolutions_submission_pair`, `ck_advance_settlement_resolutions_submitted_time`, `ck_advance_settlement_resolutions_type`, `ck_advance_settlement_resolutions_version`, `ck_advance_settlement_resolutions_waiver_proof`.

**Indexes:** 10 total. Partial: `CREATE UNIQUE INDEX ux_advance_settlement_resolutions_funding_source ON ahdah.advance_settlement_resolutions USING btree (company_id, funding_source_id) WHERE (funding_source_id IS NOT NULL)`; `CREATE INDEX ix_advance_settlement_resolutions_pending ON ahdah.advance_settlement_resolutions USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`; `CREATE INDEX ix_advance_settlement_resolutions_personal_claim ON ahdah.advance_settlement_resolutions USING btree (company_id, personal_claim_id) WHERE (personal_claim_id IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_advance_settlement_resolutions_set_updated_at BEFORE UPDATE ON ahdah.advance_settlement_resolutions FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>advance_closures</summary>

**Columns:** `advance_closure_id: uuid = gen_random_uuid()`; `company_id: uuid`; `closure_number: varchar(50)`; `advance_id: uuid`; `advance_version_number: int`; `closure_date: date`; `closure_type: varchar(30) = 'Normal'::character varying`; `currency_code: varchar(3) = 'LYD'::character varying`; `balance_count: int = 0`; `settled_balance_count: int = 0`; `unsettled_balance_count: int? GENERATED (balance_count - settled_balance_count)`; `pending_operation_count: int = 0`; `unresolved_issue_count: int = 0`; `total_available_amount: numeric(18,2) = 0`; `total_reserved_amount: numeric(18,2) = 0`; `unresolved_difference_amount: numeric(18,2) = 0`; `settled_shortage_amount: numeric(18,2) = 0`; `settled_surplus_amount: numeric(18,2) = 0`; `outstanding_supplier_debt_amount: numeric(18,2) = 0`; `outstanding_personal_claim_amount: numeric(18,2) = 0`; `reconciliation_status: varchar(20)? GENERATED  CASE WHEN ((balance_count = settled_balance_count) AND (pending_operation_count = 0) AND (unresolved_issue_count = 0) AND (total_available_amount = (0)::numeric) AND (total_reserved_amount = (0)::numeric) AND (unresolved_difference_amount = (0)::numeric)) THEN 'ReadyToClose'::text ELSE 'NotReady'::text END`; `closure_reason: varchar(1000)`; `review_notes: varchar(1000)?`; `authorization_document_url: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (advance_closure_id)`; `UNIQUE (company_id, advance_closure_id)`; `UNIQUE (company_id, closure_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, advance_id) REFERENCES advances(company_id, advance_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_advance_closures_admin_document`, `ck_advance_closures_advance_version`, `ck_advance_closures_amounts`, `ck_advance_closures_approval_pair`, `ck_advance_closures_approved_fields`, `ck_advance_closures_approved_time`, `ck_advance_closures_authorization_document`, `ck_advance_closures_cancellation_pair`, `ck_advance_closures_cancelled_fields`, `ck_advance_closures_cancelled_time`, `ck_advance_closures_counts`, `ck_advance_closures_currency`, `ck_advance_closures_draft_fields`, `ck_advance_closures_number`, `ck_advance_closures_pending_fields`, `ck_advance_closures_ready_for_approval`, `ck_advance_closures_reason`, `ck_advance_closures_rejected_fields`, `ck_advance_closures_rejected_time`, `ck_advance_closures_rejection_pair`, `ck_advance_closures_reversal_pair`, `ck_advance_closures_reversed_fields`, `ck_advance_closures_reversed_time`, `ck_advance_closures_review_notes`, `ck_advance_closures_status`, `ck_advance_closures_submission_pair`, `ck_advance_closures_submitted_time`, `ck_advance_closures_type`, `ck_advance_closures_version`.

**Indexes:** 10 total. Partial: `CREATE UNIQUE INDEX ux_advance_closures_one_active ON ahdah.advance_closures USING btree (company_id, advance_id) WHERE ((status)::text = ANY ((ARRAY['Draft'::character varying, 'PendingApproval'::character varying, 'Approved'::character varying])::text[]))`; `CREATE INDEX ix_advance_closures_pending ON ahdah.advance_closures USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_advance_closures_set_updated_at BEFORE UPDATE ON ahdah.advance_closures FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>advance_closure_documents</summary>

**Columns:** `advance_closure_document_id: uuid = gen_random_uuid()`; `company_id: uuid`; `advance_closure_id: uuid`; `document_type: varchar(50)`; `document_title: varchar(250)`; `document_number: varchar(100)?`; `document_date: date?`; `issuer_name: varchar(200)?`; `original_file_name: varchar(255)`; `file_url: varchar(1000)`; `mime_type: varchar(150)`; `file_size_bytes: bigint`; `sha256_hash: varchar(64)`; `capture_source: varchar(30) = 'FileUpload'::character varying`; `is_primary: bool = false`; `is_required: bool = false`; `verification_status: varchar(30) = 'PendingVerification'::character varying`; `uploaded_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `rejection_reason: varchar(500)?`; `notes: varchar(1000)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (advance_closure_document_id)`; `UNIQUE (company_id, advance_closure_id, sha256_hash)`; `UNIQUE (company_id, advance_closure_document_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, advance_closure_id) REFERENCES advance_closures(company_id, advance_closure_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, uploaded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_advance_closure_documents_capture_source`, `ck_advance_closure_documents_file_name`, `ck_advance_closure_documents_file_size`, `ck_advance_closure_documents_file_url`, `ck_advance_closure_documents_issuer`, `ck_advance_closure_documents_mime_type`, `ck_advance_closure_documents_notes`, `ck_advance_closure_documents_number`, `ck_advance_closure_documents_other_notes`, `ck_advance_closure_documents_pending_fields`, `ck_advance_closure_documents_primary_status`, `ck_advance_closure_documents_rejected_fields`, `ck_advance_closure_documents_required_status`, `ck_advance_closure_documents_sha256`, `ck_advance_closure_documents_status`, `ck_advance_closure_documents_title`, `ck_advance_closure_documents_type`, `ck_advance_closure_documents_verified_fields`, `ck_advance_closure_documents_verified_time`, `ck_advance_closure_documents_version`.

**Indexes:** 10 total. Partial: `CREATE UNIQUE INDEX ux_advance_closure_documents_one_primary ON ahdah.advance_closure_documents USING btree (company_id, advance_closure_id) WHERE ((is_primary = true) AND ((verification_status)::text <> 'Rejected'::text))`; `CREATE INDEX ix_advance_closure_documents_required ON ahdah.advance_closure_documents USING btree (company_id, advance_closure_id, verification_status) WHERE (is_required = true)`.

**Triggers:** `CREATE TRIGGER trg_advance_closure_documents_set_updated_at BEFORE UPDATE ON ahdah.advance_closure_documents FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>funding_sources</summary>

**Columns:** `funding_source_id: uuid = gen_random_uuid()`; `company_id: uuid`; `source_type: varchar(40)`; `source_date: date`; `currency_code: varchar(3) = 'LYD'::character varying`; `gross_amount: numeric(18,2)`; `fee_amount: numeric(18,2) = 0`; `net_amount: numeric(18,2)`; `available_amount: numeric(18,2)`; `reserved_amount: numeric(18,2) = 0`; `used_amount: numeric(18,2) = 0`; `reversed_amount: numeric(18,2) = 0`; `status: varchar(30) = 'PendingVerification'::character varying`; `description: varchar(1000)?`; `notes: varchar(1000)?`; `created_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (funding_source_id)`; `UNIQUE (company_id, funding_source_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id) REFERENCES companies(company_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_funding_sources_balance_equation`, `ck_funding_sources_cancellation_fields`, `ck_funding_sources_cancelled_after_creation`, `ck_funding_sources_currency_code`, `ck_funding_sources_description_not_blank`, `ck_funding_sources_fee_amount`, `ck_funding_sources_gross_amount`, `ck_funding_sources_net_amount`, `ck_funding_sources_non_negative_balances`, `ck_funding_sources_notes_not_blank`, `ck_funding_sources_reversal_fields`, `ck_funding_sources_reversed_after_verification`, `ck_funding_sources_status`, `ck_funding_sources_status_balances`, `ck_funding_sources_type`, `ck_funding_sources_verification_fields`, `ck_funding_sources_verified_after_creation`, `ck_funding_sources_version_number`.

**Indexes:** 7 total. Partial: `CREATE INDEX ix_funding_sources_available ON ahdah.funding_sources USING btree (company_id, available_amount) WHERE ((status)::text = ANY ((ARRAY['Available'::character varying, 'PartiallyUsed'::character varying])::text[]))`.

**Triggers:** `CREATE TRIGGER trg_funding_sources_set_updated_at BEFORE UPDATE ON ahdah.funding_sources FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>funding_source_ledger_entries</summary>

**Columns:** `funding_source_ledger_entry_id: uuid = gen_random_uuid()`; `company_id: uuid`; `funding_source_id: uuid`; `source_version_number: int`; `entry_type: varchar(50)`; `amount: numeric(18,2)`; `available_delta: numeric(18,2) = 0`; `reserved_delta: numeric(18,2) = 0`; `used_delta: numeric(18,2) = 0`; `reversed_delta: numeric(18,2) = 0`; `available_after: numeric(18,2)`; `reserved_after: numeric(18,2)`; `used_after: numeric(18,2)`; `reversed_after: numeric(18,2)`; `reference_type: varchar(100)?`; `reference_id: uuid?`; `performed_by_user_id: uuid?`; `correlation_id: uuid = gen_random_uuid()`; `description: varchar(1000)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (funding_source_ledger_entry_id)`; `UNIQUE (company_id, funding_source_ledger_entry_id)`; `UNIQUE (company_id, funding_source_id, source_version_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, performed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_funding_source_ledger_adjustment_deltas`, `ck_funding_source_ledger_adjustment_description`, `ck_funding_source_ledger_after_balances`, `ck_funding_source_ledger_amount`, `ck_funding_source_ledger_business_reference`, `ck_funding_source_ledger_confirmed_use`, `ck_funding_source_ledger_description`, `ck_funding_source_ledger_entry_type`, `ck_funding_source_ledger_internal_balance`, `ck_funding_source_ledger_reference_pair`, `ck_funding_source_ledger_reservation_released`, `ck_funding_source_ledger_reserved`, `ck_funding_source_ledger_source_created`, `ck_funding_source_ledger_source_created_reference`, `ck_funding_source_ledger_source_reversal`, `ck_funding_source_ledger_use_reversal`, `ck_funding_source_ledger_version`.

**Indexes:** 8 total. Partial: `CREATE INDEX ix_funding_source_ledger_reference ON ahdah.funding_source_ledger_entries USING btree (company_id, reference_type, reference_id) WHERE (reference_id IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_funding_source_ledger_immutable BEFORE DELETE OR UPDATE ON ahdah.funding_source_ledger_entries FOR EACH ROW EXECUTE FUNCTION prevent_immutable_record_change()`.

</details>

<details>
<summary>manager_contributions</summary>

**Columns:** `manager_contribution_id: uuid = gen_random_uuid()`; `company_id: uuid`; `funding_source_id: uuid`; `manager_user_id: uuid`; `contribution_date: date`; `contribution_amount: numeric(18,2)`; `purpose: varchar(1000)`; `status: varchar(30) = 'PendingVerification'::character varying`; `recorded_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `notes: varchar(1000)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (manager_contribution_id)`; `UNIQUE (company_id, manager_contribution_id)`; `UNIQUE (company_id, funding_source_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, manager_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, recorded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_manager_contributions_amount`, `ck_manager_contributions_cancelled_after_creation`, `ck_manager_contributions_cancelled_fields`, `ck_manager_contributions_confirmed_fields`, `ck_manager_contributions_notes`, `ck_manager_contributions_pending_fields`, `ck_manager_contributions_purpose`, `ck_manager_contributions_reversed_after_confirmation`, `ck_manager_contributions_reversed_fields`, `ck_manager_contributions_status`, `ck_manager_contributions_verified_after_creation`, `ck_manager_contributions_version`.

**Indexes:** 8 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_manager_contributions_set_updated_at BEFORE UPDATE ON ahdah.manager_contributions FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>project_owner_payments</summary>

**Columns:** `project_owner_payment_id: uuid = gen_random_uuid()`; `company_id: uuid`; `funding_source_id: uuid`; `project_owner_id: uuid`; `payment_number: varchar(50)`; `payment_date: date`; `payment_amount: numeric(18,2)`; `status: varchar(30) = 'PendingVerification'::character varying`; `recorded_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `notes: varchar(1000)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (project_owner_payment_id)`; `UNIQUE (company_id, project_owner_payment_id)`; `UNIQUE (company_id, funding_source_id)`; `UNIQUE (company_id, payment_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_owner_id) REFERENCES project_owners(company_id, project_owner_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, recorded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_project_owner_payments_amount`, `ck_project_owner_payments_cancelled_after_creation`, `ck_project_owner_payments_cancelled_fields`, `ck_project_owner_payments_confirmed_fields`, `ck_project_owner_payments_notes`, `ck_project_owner_payments_number_not_blank`, `ck_project_owner_payments_pending_fields`, `ck_project_owner_payments_reversed_after_verification`, `ck_project_owner_payments_reversed_fields`, `ck_project_owner_payments_status`, `ck_project_owner_payments_verified_after_creation`, `ck_project_owner_payments_version`.

**Indexes:** 9 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_project_owner_payments_set_updated_at BEFORE UPDATE ON ahdah.project_owner_payments FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>owner_payment_project_allocations</summary>

**Columns:** `owner_payment_project_allocation_id: uuid = gen_random_uuid()`; `company_id: uuid`; `project_owner_payment_id: uuid`; `project_id: uuid`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (owner_payment_project_allocation_id)`; `UNIQUE (company_id, owner_payment_project_allocation_id)`; `UNIQUE (company_id, project_owner_payment_id, project_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_owner_payment_id) REFERENCES project_owner_payments(company_id, project_owner_payment_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_id) REFERENCES projects(company_id, project_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_owner_payment_project_allocations_amount`, `ck_owner_payment_project_allocations_notes`.

**Indexes:** 6 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_owner_payment_project_allocations_set_updated_at BEFORE UPDATE ON ahdah.owner_payment_project_allocations FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>company_cashbox_entries</summary>

**Columns:** `company_cashbox_entry_id: uuid = gen_random_uuid()`; `company_id: uuid`; `funding_source_id: uuid`; `cashbox_entry_number: varchar(50)`; `entry_type: varchar(40)`; `entry_date: date`; `amount: numeric(18,2)`; `bank_name: varchar(150)?`; `reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `description: varchar(1000)`; `notes: varchar(1000)?`; `status: varchar(30) = 'PendingVerification'::character varying`; `recorded_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (company_cashbox_entry_id)`; `UNIQUE (company_id, company_cashbox_entry_id)`; `UNIQUE (company_id, funding_source_id)`; `UNIQUE (company_id, cashbox_entry_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, recorded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_company_cashbox_entries_amount`, `ck_company_cashbox_entries_bank_name`, `ck_company_cashbox_entries_bank_withdrawal_fields`, `ck_company_cashbox_entries_cancelled_after_creation`, `ck_company_cashbox_entries_cancelled_fields`, `ck_company_cashbox_entries_confirmed_fields`, `ck_company_cashbox_entries_description`, `ck_company_cashbox_entries_notes`, `ck_company_cashbox_entries_number`, `ck_company_cashbox_entries_other_notes`, `ck_company_cashbox_entries_pending_fields`, `ck_company_cashbox_entries_proof_file`, `ck_company_cashbox_entries_reference_number`, `ck_company_cashbox_entries_reversed_after_confirmation`, `ck_company_cashbox_entries_reversed_fields`, `ck_company_cashbox_entries_status`, `ck_company_cashbox_entries_type`, `ck_company_cashbox_entries_verified_after_creation`, `ck_company_cashbox_entries_version`.

**Indexes:** 9 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_company_cashbox_entries_set_updated_at BEFORE UPDATE ON ahdah.company_cashbox_entries FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expenses</summary>

**Columns:** `expense_id: uuid = gen_random_uuid()`; `company_id: uuid`; `expense_number: varchar(50)`; `project_id: uuid?`; `expense_category_id: uuid`; `supplier_id: uuid?`; `incurred_by_user_id: uuid`; `expense_date: date`; `payment_mode: varchar(30)`; `currency_code: varchar(3) = 'LYD'::character varying`; `subtotal_amount: numeric(18,2)`; `discount_amount: numeric(18,2) = 0`; `tax_amount: numeric(18,2) = 0`; `total_amount: numeric(18,2)`; `credit_due_date: date?`; `invoice_number: varchar(100)?`; `receipt_number: varchar(100)?`; `merchant_name: varchar(200)?`; `description: varchar(1000)`; `expense_location: varchar(500)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `submitted_by_user_id: uuid`; `submitted_at: timestamptz?`; `reviewed_by_user_id: uuid?`; `reviewed_at: timestamptz?`; `correction_reason: varchar(1000)?`; `rejection_reason: varchar(1000)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_id)`; `UNIQUE (company_id, expense_id)`; `UNIQUE (company_id, expense_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_category_id) REFERENCES expense_categories(company_id, expense_category_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, incurred_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_id) REFERENCES projects(company_id, project_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reviewed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_id) REFERENCES suppliers(company_id, supplier_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expenses_amounts`, `ck_expenses_approved_fields`, `ck_expenses_cancelled_after_creation`, `ck_expenses_cancelled_fields`, `ck_expenses_correction_fields`, `ck_expenses_currency`, `ck_expenses_description`, `ck_expenses_draft_fields`, `ck_expenses_invoice_number`, `ck_expenses_location`, `ck_expenses_merchant_name`, `ck_expenses_notes`, `ck_expenses_number`, `ck_expenses_payment_mode`, `ck_expenses_pending_fields`, `ck_expenses_receipt_number`, `ck_expenses_rejected_fields`, `ck_expenses_reversed_after_approval`, `ck_expenses_reversed_fields`, `ck_expenses_review_pair`, `ck_expenses_reviewed_after_submission`, `ck_expenses_status`, `ck_expenses_submitted_after_creation`, `ck_expenses_supplier_credit`, `ck_expenses_version`.

**Indexes:** 12 total. Partial: `CREATE UNIQUE INDEX ux_expenses_supplier_invoice ON ahdah.expenses USING btree (company_id, supplier_id, lower((invoice_number)::text)) WHERE ((supplier_id IS NOT NULL) AND (invoice_number IS NOT NULL))`; `CREATE INDEX ix_expenses_project ON ahdah.expenses USING btree (company_id, project_id) WHERE (project_id IS NOT NULL)`; `CREATE INDEX ix_expenses_supplier ON ahdah.expenses USING btree (company_id, supplier_id) WHERE (supplier_id IS NOT NULL)`; `CREATE INDEX ix_expenses_pending_review ON ahdah.expenses USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingReview'::text)`.

**Triggers:** `CREATE TRIGGER trg_expenses_set_updated_at BEFORE UPDATE ON ahdah.expenses FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expense_items</summary>

**Columns:** `expense_item_id: uuid = gen_random_uuid()`; `company_id: uuid`; `expense_id: uuid`; `line_number: int`; `item_name: varchar(200)`; `item_code: varchar(100)?`; `item_description: varchar(500)?`; `quantity: numeric(18,3)`; `unit_code: varchar(30)`; `custom_unit_name: varchar(50)?`; `unit_price: numeric(18,2)`; `subtotal_amount: numeric(18,2)? GENERATED round((quantity * unit_price), 2)`; `discount_amount: numeric(18,2) = 0`; `tax_amount: numeric(18,2) = 0`; `total_amount: numeric(18,2)? GENERATED ((round((quantity * unit_price), 2) - discount_amount) + tax_amount)`; `notes: varchar(500)?`; `created_by_user_id: uuid`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_item_id)`; `UNIQUE (company_id, expense_item_id)`; `UNIQUE (company_id, expense_id, line_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_id) REFERENCES expenses(company_id, expense_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expense_items_code`, `ck_expense_items_custom_unit`, `ck_expense_items_description`, `ck_expense_items_discount`, `ck_expense_items_line_number`, `ck_expense_items_name`, `ck_expense_items_notes`, `ck_expense_items_quantity`, `ck_expense_items_tax`, `ck_expense_items_total`, `ck_expense_items_unit`, `ck_expense_items_unit_price`.

**Indexes:** 7 total. Partial: `CREATE INDEX ix_expense_items_code ON ahdah.expense_items USING btree (company_id, item_code) WHERE (item_code IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_expense_items_set_updated_at BEFORE UPDATE ON ahdah.expense_items FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expense_advance_allocations</summary>

**Columns:** `expense_advance_allocation_id: uuid = gen_random_uuid()`; `company_id: uuid`; `expense_id: uuid`; `user_advance_balance_id: uuid`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_advance_allocation_id)`; `UNIQUE (company_id, expense_advance_allocation_id)`; `UNIQUE (company_id, expense_id, user_advance_balance_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, user_advance_balance_id) REFERENCES user_advance_balances(company_id, user_advance_balance_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_id) REFERENCES expenses(company_id, expense_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expense_advance_allocations_amount`, `ck_expense_advance_allocations_notes`.

**Indexes:** 6 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_expense_advance_allocations_set_updated_at BEFORE UPDATE ON ahdah.expense_advance_allocations FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expense_documents</summary>

**Columns:** `expense_document_id: uuid = gen_random_uuid()`; `company_id: uuid`; `expense_id: uuid`; `document_type: varchar(30)`; `document_number: varchar(100)?`; `document_date: date?`; `issuer_name: varchar(200)?`; `original_file_name: varchar(255)`; `file_url: varchar(1000)`; `mime_type: varchar(150)`; `file_size_bytes: bigint`; `sha256_hash: character(64)`; `capture_source: varchar(30) = 'FileUpload'::character varying`; `is_primary: bool = false`; `verification_status: varchar(30) = 'PendingVerification'::character varying`; `uploaded_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `rejection_reason: varchar(500)?`; `notes: varchar(1000)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_document_id)`; `UNIQUE (company_id, expense_document_id)`; `UNIQUE (company_id, expense_id, sha256_hash)`.

**Foreign keys:** `FOREIGN KEY (company_id, expense_id) REFERENCES expenses(company_id, expense_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, uploaded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expense_documents_capture_source`, `ck_expense_documents_file_name`, `ck_expense_documents_file_size`, `ck_expense_documents_file_url`, `ck_expense_documents_issuer`, `ck_expense_documents_mime_type`, `ck_expense_documents_notes`, `ck_expense_documents_number`, `ck_expense_documents_pending_fields`, `ck_expense_documents_rejected_fields`, `ck_expense_documents_sha256`, `ck_expense_documents_type`, `ck_expense_documents_verification_status`, `ck_expense_documents_verified_after_upload`, `ck_expense_documents_verified_fields`, `ck_expense_documents_version`.

**Indexes:** 9 total. Partial: `CREATE UNIQUE INDEX ux_expense_documents_one_primary ON ahdah.expense_documents USING btree (company_id, expense_id) WHERE ((is_primary = true) AND ((verification_status)::text <> 'Rejected'::text))`.

**Triggers:** `CREATE TRIGGER trg_expense_documents_set_updated_at BEFORE UPDATE ON ahdah.expense_documents FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expense_categories</summary>

**Columns:** `expense_category_id: uuid = gen_random_uuid()`; `company_id: uuid`; `parent_expense_category_id: uuid?`; `category_code: varchar(30)?`; `category_name: varchar(150)`; `category_group: varchar(30)`; `expense_scope: varchar(20) = 'ProjectOnly'::character varying`; `description: varchar(500)?`; `requires_supplier: bool = false`; `requires_receipt: bool = true`; `supports_quantity_details: bool = false`; `is_active: bool = true`; `display_order: int = 0`; `created_by_user_id: uuid`; `deactivated_by_user_id: uuid?`; `deactivated_at: timestamptz?`; `deactivation_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_category_id)`; `UNIQUE (company_id, expense_category_id)`.

**Foreign keys:** `FOREIGN KEY (company_id) REFERENCES companies(company_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, deactivated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, parent_expense_category_id) REFERENCES expense_categories(company_id, expense_category_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expense_categories_active_fields`, `ck_expense_categories_code`, `ck_expense_categories_deactivated_after_creation`, `ck_expense_categories_description`, `ck_expense_categories_display_order`, `ck_expense_categories_group`, `ck_expense_categories_name`, `ck_expense_categories_not_own_parent`, `ck_expense_categories_scope`, `ck_expense_categories_version`.

**Indexes:** 8 total. Partial: `CREATE UNIQUE INDEX ux_expense_categories_code_ci ON ahdah.expense_categories USING btree (company_id, lower((category_code)::text)) WHERE (category_code IS NOT NULL)`; `CREATE INDEX ix_expense_categories_parent ON ahdah.expense_categories USING btree (company_id, parent_expense_category_id) WHERE (parent_expense_category_id IS NOT NULL)`; `CREATE INDEX ix_expense_categories_active ON ahdah.expense_categories USING btree (company_id, display_order, category_name) WHERE (is_active = true)`.

**Triggers:** `CREATE TRIGGER trg_expense_categories_set_updated_at BEFORE UPDATE ON ahdah.expense_categories FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>personal_claims</summary>

**Columns:** `personal_claim_id: uuid = gen_random_uuid()`; `company_id: uuid`; `claim_number: varchar(50)`; `claimant_user_id: uuid`; `project_id: uuid?`; `source_type: varchar(40)`; `expense_id: uuid?`; `manager_contribution_id: uuid?`; `claim_date: date`; `due_date: date?`; `currency_code: varchar(3) = 'LYD'::character varying`; `claim_amount: numeric(18,2)`; `adjustment_amount: numeric(18,2) = 0`; `paid_amount: numeric(18,2) = 0`; `reduction_amount: numeric(18,2) = 0`; `written_off_amount: numeric(18,2) = 0`; `outstanding_amount: numeric(18,2)? GENERATED ((((claim_amount + adjustment_amount) - paid_amount) - reduction_amount) - written_off_amount)`; `description: varchar(1000)`; `notes: varchar(1000)?`; `status: varchar(30) = 'Open'::character varying`; `recorded_by_user_id: uuid`; `settled_at: timestamptz?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (personal_claim_id)`; `UNIQUE (company_id, personal_claim_id)`; `UNIQUE (company_id, claim_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, claimant_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_id) REFERENCES expenses(company_id, expense_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, manager_contribution_id) REFERENCES manager_contributions(company_id, manager_contribution_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_id) REFERENCES projects(company_id, project_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, recorded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_personal_claims_amounts`, `ck_personal_claims_cancelled_after_creation`, `ck_personal_claims_cancelled_fields`, `ck_personal_claims_currency`, `ck_personal_claims_description`, `ck_personal_claims_due_date`, `ck_personal_claims_notes`, `ck_personal_claims_number`, `ck_personal_claims_open_fields`, `ck_personal_claims_outstanding`, `ck_personal_claims_partial_fields`, `ck_personal_claims_reversed_after_creation`, `ck_personal_claims_reversed_fields`, `ck_personal_claims_settled_after_creation`, `ck_personal_claims_settled_fields`, `ck_personal_claims_source_reference`, `ck_personal_claims_source_type`, `ck_personal_claims_status`, `ck_personal_claims_version`.

**Indexes:** 11 total. Partial: `CREATE UNIQUE INDEX ux_personal_claims_expense ON ahdah.personal_claims USING btree (company_id, expense_id) WHERE (expense_id IS NOT NULL)`; `CREATE UNIQUE INDEX ux_personal_claims_manager_contribution ON ahdah.personal_claims USING btree (company_id, manager_contribution_id) WHERE (manager_contribution_id IS NOT NULL)`; `CREATE INDEX ix_personal_claims_project ON ahdah.personal_claims USING btree (company_id, project_id) WHERE (project_id IS NOT NULL)`; `CREATE INDEX ix_personal_claims_due_date ON ahdah.personal_claims USING btree (company_id, due_date) WHERE ((due_date IS NOT NULL) AND ((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[])))`; `CREATE INDEX ix_personal_claims_outstanding ON ahdah.personal_claims USING btree (company_id, claimant_user_id, outstanding_amount) WHERE ((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[]))`.

**Triggers:** `CREATE TRIGGER trg_personal_claims_set_updated_at BEFORE UPDATE ON ahdah.personal_claims FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>personal_claim_adjustments</summary>

**Columns:** `personal_claim_adjustment_id: uuid = gen_random_uuid()`; `company_id: uuid`; `adjustment_number: varchar(50)`; `personal_claim_id: uuid`; `claim_version_number: int`; `adjustment_date: date`; `adjustment_type: varchar(30)`; `adjustment_amount: numeric(18,2)`; `reason_type: varchar(40)`; `description: varchar(1000)`; `proof_file_url: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (personal_claim_adjustment_id)`; `UNIQUE (company_id, personal_claim_adjustment_id)`; `UNIQUE (company_id, adjustment_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, personal_claim_id) REFERENCES personal_claims(company_id, personal_claim_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_personal_claim_adjustments_amount`, `ck_personal_claim_adjustments_amount_direction`, `ck_personal_claim_adjustments_approval_pair`, `ck_personal_claim_adjustments_approved_after_submission`, `ck_personal_claim_adjustments_approved_fields`, `ck_personal_claim_adjustments_cancellation_pair`, `ck_personal_claim_adjustments_cancelled_after_creation`, `ck_personal_claim_adjustments_cancelled_fields`, `ck_personal_claim_adjustments_claim_version`, `ck_personal_claim_adjustments_description`, `ck_personal_claim_adjustments_draft_fields`, `ck_personal_claim_adjustments_notes`, `ck_personal_claim_adjustments_number`, `ck_personal_claim_adjustments_other_notes`, `ck_personal_claim_adjustments_pending_fields`, `ck_personal_claim_adjustments_proof`, `ck_personal_claim_adjustments_reason_type`, `ck_personal_claim_adjustments_rejected_after_submission`, `ck_personal_claim_adjustments_rejected_fields`, `ck_personal_claim_adjustments_rejection_pair`, `ck_personal_claim_adjustments_reversal_pair`, `ck_personal_claim_adjustments_reversed_after_approval`, `ck_personal_claim_adjustments_reversed_fields`, `ck_personal_claim_adjustments_status`, `ck_personal_claim_adjustments_submission_pair`, `ck_personal_claim_adjustments_submitted_after_creation`, `ck_personal_claim_adjustments_type`, `ck_personal_claim_adjustments_version`.

**Indexes:** 9 total. Partial: `CREATE UNIQUE INDEX ux_personal_claim_adjustments_one_pending ON ahdah.personal_claim_adjustments USING btree (company_id, personal_claim_id) WHERE ((status)::text = 'PendingApproval'::text)`; `CREATE INDEX ix_personal_claim_adjustments_pending ON ahdah.personal_claim_adjustments USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_personal_claim_adjustments_set_updated_at BEFORE UPDATE ON ahdah.personal_claim_adjustments FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>personal_claim_write_offs</summary>

**Columns:** `personal_claim_write_off_id: uuid = gen_random_uuid()`; `company_id: uuid`; `write_off_number: varchar(50)`; `personal_claim_id: uuid`; `claim_version_number: int`; `write_off_date: date`; `write_off_amount: numeric(18,2)`; `reason_type: varchar(40)`; `description: varchar(1000)`; `proof_file_url: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (personal_claim_write_off_id)`; `UNIQUE (company_id, personal_claim_write_off_id)`; `UNIQUE (company_id, write_off_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, personal_claim_id) REFERENCES personal_claims(company_id, personal_claim_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_personal_claim_write_offs_amount`, `ck_personal_claim_write_offs_approval_pair`, `ck_personal_claim_write_offs_approved`, `ck_personal_claim_write_offs_approved_proof`, `ck_personal_claim_write_offs_approved_time`, `ck_personal_claim_write_offs_cancel_pair`, `ck_personal_claim_write_offs_cancelled`, `ck_personal_claim_write_offs_cancelled_time`, `ck_personal_claim_write_offs_claim_version`, `ck_personal_claim_write_offs_description`, `ck_personal_claim_write_offs_draft`, `ck_personal_claim_write_offs_notes`, `ck_personal_claim_write_offs_number`, `ck_personal_claim_write_offs_other_notes`, `ck_personal_claim_write_offs_pending`, `ck_personal_claim_write_offs_proof`, `ck_personal_claim_write_offs_reason`, `ck_personal_claim_write_offs_rejected`, `ck_personal_claim_write_offs_rejected_time`, `ck_personal_claim_write_offs_rejection_pair`, `ck_personal_claim_write_offs_reversal_pair`, `ck_personal_claim_write_offs_reversed`, `ck_personal_claim_write_offs_reversed_time`, `ck_personal_claim_write_offs_status`, `ck_personal_claim_write_offs_submission_pair`, `ck_personal_claim_write_offs_submitted_time`, `ck_personal_claim_write_offs_version`.

**Indexes:** 9 total. Partial: `CREATE UNIQUE INDEX ux_personal_claim_write_offs_one_pending ON ahdah.personal_claim_write_offs USING btree (company_id, personal_claim_id) WHERE ((status)::text = 'PendingApproval'::text)`; `CREATE INDEX ix_personal_claim_write_offs_pending ON ahdah.personal_claim_write_offs USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_personal_claim_write_offs_set_updated_at BEFORE UPDATE ON ahdah.personal_claim_write_offs FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>personal_claim_ledger_entries</summary>

**Columns:** `personal_claim_ledger_entry_id: uuid = gen_random_uuid()`; `company_id: uuid`; `personal_claim_id: uuid`; `claim_version_number: int`; `entry_type: varchar(40)`; `delta_claim_amount: numeric(18,2) = 0`; `delta_adjustment_amount: numeric(18,2) = 0`; `delta_paid_amount: numeric(18,2) = 0`; `delta_reduction_amount: numeric(18,2) = 0`; `delta_written_off_amount: numeric(18,2) = 0`; `claim_after_amount: numeric(18,2)`; `adjustment_after_amount: numeric(18,2)`; `paid_after_amount: numeric(18,2)`; `reduction_after_amount: numeric(18,2)`; `written_off_after_amount: numeric(18,2)`; `outstanding_after_amount: numeric(18,2)`; `claim_status_after: varchar(30)`; `reference_type: varchar(40)?`; `reference_id: uuid?`; `correlation_id: uuid = gen_random_uuid()`; `description: varchar(1000)?`; `performed_by_user_id: uuid`; `occurred_at: timestamptz = CURRENT_TIMESTAMP`; `created_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (personal_claim_ledger_entry_id)`; `UNIQUE (company_id, personal_claim_id, claim_version_number)`; `UNIQUE (company_id, personal_claim_ledger_entry_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, personal_claim_id) REFERENCES personal_claims(company_id, personal_claim_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, performed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_personal_claim_ledger_after_amounts`, `ck_personal_claim_ledger_after_equation`, `ck_personal_claim_ledger_cancelled_status`, `ck_personal_claim_ledger_description`, `ck_personal_claim_ledger_entry_deltas`, `ck_personal_claim_ledger_entry_type`, `ck_personal_claim_ledger_open_status`, `ck_personal_claim_ledger_partial_status`, `ck_personal_claim_ledger_reference_match`, `ck_personal_claim_ledger_reference_pair`, `ck_personal_claim_ledger_reference_type`, `ck_personal_claim_ledger_reversed_status`, `ck_personal_claim_ledger_settled_status`, `ck_personal_claim_ledger_status`, `ck_personal_claim_ledger_version`.

**Indexes:** 8 total. Partial: `CREATE INDEX ix_personal_claim_ledger_reference ON ahdah.personal_claim_ledger_entries USING btree (company_id, reference_type, reference_id) WHERE (reference_id IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_personal_claim_ledger_immutable BEFORE DELETE OR UPDATE ON ahdah.personal_claim_ledger_entries FOR EACH ROW EXECUTE FUNCTION prevent_immutable_record_change()`.

</details>

<details>
<summary>personal_claim_payments</summary>

**Columns:** `personal_claim_payment_id: uuid = gen_random_uuid()`; `company_id: uuid`; `payment_number: varchar(50)`; `claimant_user_id: uuid`; `payment_date: date`; `currency_code: varchar(3) = 'LYD'::character varying`; `payment_amount: numeric(18,2)`; `fee_amount: numeric(18,2) = 0`; `total_disbursed_amount: numeric(18,2)? GENERATED (payment_amount + fee_amount)`; `payment_method: varchar(30)`; `recipient_name_snapshot: varchar(200)`; `recipient_bank_name: varchar(150)?`; `recipient_account_number: varchar(100)?`; `recipient_iban: varchar(34)?`; `wallet_provider: varchar(100)?`; `wallet_number: varchar(20)?`; `reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `description: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `confirmed_by_user_id: uuid?`; `confirmed_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (personal_claim_payment_id)`; `UNIQUE (company_id, personal_claim_payment_id)`; `UNIQUE (company_id, payment_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, claimant_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, confirmed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_personal_claim_payments_account_number`, `ck_personal_claim_payments_amounts`, `ck_personal_claim_payments_bank_name`, `ck_personal_claim_payments_bank_transfer_fields`, `ck_personal_claim_payments_cancelled_after_creation`, `ck_personal_claim_payments_cancelled_fields`, `ck_personal_claim_payments_cash_fields`, `ck_personal_claim_payments_cheque_fields`, `ck_personal_claim_payments_confirmed_after_submission`, `ck_personal_claim_payments_confirmed_fields`, `ck_personal_claim_payments_confirmed_proof`, `ck_personal_claim_payments_confirmed_reference`, `ck_personal_claim_payments_currency`, `ck_personal_claim_payments_description`, `ck_personal_claim_payments_draft_fields`, `ck_personal_claim_payments_iban`, `ck_personal_claim_payments_method`, `ck_personal_claim_payments_notes`, `ck_personal_claim_payments_number`, `ck_personal_claim_payments_other_description`, `ck_personal_claim_payments_pending_fields`, `ck_personal_claim_payments_proof`, `ck_personal_claim_payments_recipient_name`, `ck_personal_claim_payments_reference`, `ck_personal_claim_payments_rejected_after_submission`, `ck_personal_claim_payments_rejected_fields`, `ck_personal_claim_payments_reversed_after_confirmation`, `ck_personal_claim_payments_reversed_fields`, `ck_personal_claim_payments_status`, `ck_personal_claim_payments_submission_pair`, `ck_personal_claim_payments_submitted_after_creation`, `ck_personal_claim_payments_version`, `ck_personal_claim_payments_wallet_fields`, `ck_personal_claim_payments_wallet_number`, `ck_personal_claim_payments_wallet_provider`.

**Indexes:** 9 total. Partial: `CREATE INDEX ix_personal_claim_payments_pending ON ahdah.personal_claim_payments USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`; `CREATE INDEX ix_personal_claim_payments_reference ON ahdah.personal_claim_payments USING btree (company_id, reference_number) WHERE (reference_number IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_personal_claim_payments_set_updated_at BEFORE UPDATE ON ahdah.personal_claim_payments FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>personal_claim_payment_allocations</summary>

**Columns:** `personal_claim_payment_allocation_id: uuid = gen_random_uuid()`; `company_id: uuid`; `personal_claim_payment_id: uuid`; `personal_claim_id: uuid`; `claim_version_number: int`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (personal_claim_payment_allocation_id)`; `UNIQUE (company_id, personal_claim_payment_allocation_id)`; `UNIQUE (company_id, personal_claim_payment_id, personal_claim_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, personal_claim_id) REFERENCES personal_claims(company_id, personal_claim_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, personal_claim_payment_id) REFERENCES personal_claim_payments(company_id, personal_claim_payment_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_personal_claim_payment_allocations_amount`, `ck_personal_claim_payment_allocations_claim_version`, `ck_personal_claim_payment_allocations_notes`.

**Indexes:** 7 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_personal_claim_payment_allocations_set_updated_at BEFORE UPDATE ON ahdah.personal_claim_payment_allocations FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>personal_claim_payment_funding_sources</summary>

**Columns:** `personal_claim_payment_funding_source_id: uuid = gen_random_uuid()`; `company_id: uuid`; `personal_claim_payment_id: uuid`; `funding_source_id: uuid`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (personal_claim_payment_funding_source_id)`; `UNIQUE (company_id, personal_claim_payment_funding_source_id)`; `UNIQUE (company_id, personal_claim_payment_id, funding_source_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, personal_claim_payment_id) REFERENCES personal_claim_payments(company_id, personal_claim_payment_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_personal_claim_payment_funding_sources_amount`, `ck_personal_claim_payment_funding_sources_notes`.

**Indexes:** 7 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_personal_claim_payment_funding_sources_set_updated_at BEFORE UPDATE ON ahdah.personal_claim_payment_funding_sources FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_debts</summary>

**Columns:** `supplier_debt_id: uuid = gen_random_uuid()`; `company_id: uuid`; `debt_number: varchar(50)`; `supplier_id: uuid`; `expense_id: uuid`; `project_id: uuid?`; `expense_version_number: int`; `debt_date: date`; `due_date: date`; `currency_code: varchar(3) = 'LYD'::character varying`; `debt_amount: numeric(18,2)`; `adjustment_amount: numeric(18,2) = 0`; `paid_amount: numeric(18,2) = 0`; `credit_note_amount: numeric(18,2) = 0`; `written_off_amount: numeric(18,2) = 0`; `outstanding_amount: numeric(18,2)? GENERATED ((((debt_amount + adjustment_amount) - paid_amount) - credit_note_amount) - written_off_amount)`; `status: varchar(30) = 'Open'::character varying`; `description: varchar(1000)?`; `notes: varchar(1000)?`; `recorded_by_user_id: uuid`; `settled_at: timestamptz?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_debt_id)`; `UNIQUE (company_id, supplier_debt_id)`; `UNIQUE (company_id, expense_id)`; `UNIQUE (company_id, debt_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_id) REFERENCES expenses(company_id, expense_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_id) REFERENCES projects(company_id, project_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, recorded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_id) REFERENCES suppliers(company_id, supplier_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_debts_amounts`, `ck_supplier_debts_cancelled_after_creation`, `ck_supplier_debts_cancelled_fields`, `ck_supplier_debts_currency`, `ck_supplier_debts_description`, `ck_supplier_debts_due_date`, `ck_supplier_debts_expense_version`, `ck_supplier_debts_notes`, `ck_supplier_debts_number`, `ck_supplier_debts_open_fields`, `ck_supplier_debts_outstanding`, `ck_supplier_debts_partial_fields`, `ck_supplier_debts_reversed_after_creation`, `ck_supplier_debts_reversed_fields`, `ck_supplier_debts_settled_after_creation`, `ck_supplier_debts_settled_fields`, `ck_supplier_debts_status`, `ck_supplier_debts_version`.

**Indexes:** 10 total. Partial: `CREATE INDEX ix_supplier_debts_project ON ahdah.supplier_debts USING btree (company_id, project_id) WHERE (project_id IS NOT NULL)`; `CREATE INDEX ix_supplier_debts_due_date ON ahdah.supplier_debts USING btree (company_id, due_date) WHERE ((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[]))`; `CREATE INDEX ix_supplier_debts_outstanding ON ahdah.supplier_debts USING btree (company_id, supplier_id, outstanding_amount) WHERE ((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[]))`.

**Triggers:** `CREATE TRIGGER trg_supplier_debts_set_updated_at BEFORE UPDATE ON ahdah.supplier_debts FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_debt_adjustments</summary>

**Columns:** `supplier_debt_adjustment_id: uuid = gen_random_uuid()`; `company_id: uuid`; `adjustment_number: varchar(50)`; `supplier_debt_id: uuid`; `debt_version_number: int`; `adjustment_date: date`; `adjustment_type: varchar(30)`; `adjustment_amount: numeric(18,2)`; `reason_type: varchar(40)`; `description: varchar(1000)`; `proof_file_url: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_debt_adjustment_id)`; `UNIQUE (company_id, supplier_debt_adjustment_id)`; `UNIQUE (company_id, adjustment_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_debt_id) REFERENCES supplier_debts(company_id, supplier_debt_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_debt_adjustments_amount`, `ck_supplier_debt_adjustments_amount_direction`, `ck_supplier_debt_adjustments_approval_pair`, `ck_supplier_debt_adjustments_approved_after_submission`, `ck_supplier_debt_adjustments_approved_fields`, `ck_supplier_debt_adjustments_cancellation_pair`, `ck_supplier_debt_adjustments_cancelled_after_creation`, `ck_supplier_debt_adjustments_cancelled_fields`, `ck_supplier_debt_adjustments_debt_version`, `ck_supplier_debt_adjustments_description`, `ck_supplier_debt_adjustments_draft_fields`, `ck_supplier_debt_adjustments_notes`, `ck_supplier_debt_adjustments_number`, `ck_supplier_debt_adjustments_pending_fields`, `ck_supplier_debt_adjustments_proof`, `ck_supplier_debt_adjustments_reason_type`, `ck_supplier_debt_adjustments_rejected_after_submission`, `ck_supplier_debt_adjustments_rejected_fields`, `ck_supplier_debt_adjustments_rejection_pair`, `ck_supplier_debt_adjustments_reversal_pair`, `ck_supplier_debt_adjustments_reversed_after_approval`, `ck_supplier_debt_adjustments_reversed_fields`, `ck_supplier_debt_adjustments_status`, `ck_supplier_debt_adjustments_submission_pair`, `ck_supplier_debt_adjustments_submitted_after_creation`, `ck_supplier_debt_adjustments_type`, `ck_supplier_debt_adjustments_version`.

**Indexes:** 9 total. Partial: `CREATE UNIQUE INDEX ux_supplier_debt_adjustments_one_pending ON ahdah.supplier_debt_adjustments USING btree (company_id, supplier_debt_id) WHERE ((status)::text = 'PendingApproval'::text)`; `CREATE INDEX ix_supplier_debt_adjustments_pending ON ahdah.supplier_debt_adjustments USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_supplier_debt_adjustments_set_updated_at BEFORE UPDATE ON ahdah.supplier_debt_adjustments FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_debt_write_offs</summary>

**Columns:** `supplier_debt_write_off_id: uuid = gen_random_uuid()`; `company_id: uuid`; `write_off_number: varchar(50)`; `supplier_debt_id: uuid`; `debt_version_number: int`; `write_off_date: date`; `write_off_amount: numeric(18,2)`; `reason_type: varchar(40)`; `description: varchar(1000)`; `proof_file_url: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_debt_write_off_id)`; `UNIQUE (company_id, supplier_debt_write_off_id)`; `UNIQUE (company_id, write_off_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_debt_id) REFERENCES supplier_debts(company_id, supplier_debt_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_debt_write_offs_amount`, `ck_supplier_debt_write_offs_approval_pair`, `ck_supplier_debt_write_offs_approved_after_submission`, `ck_supplier_debt_write_offs_approved_fields`, `ck_supplier_debt_write_offs_approved_proof`, `ck_supplier_debt_write_offs_cancellation_pair`, `ck_supplier_debt_write_offs_cancelled_after_creation`, `ck_supplier_debt_write_offs_cancelled_fields`, `ck_supplier_debt_write_offs_debt_version`, `ck_supplier_debt_write_offs_description`, `ck_supplier_debt_write_offs_draft_fields`, `ck_supplier_debt_write_offs_notes`, `ck_supplier_debt_write_offs_number`, `ck_supplier_debt_write_offs_pending_fields`, `ck_supplier_debt_write_offs_proof`, `ck_supplier_debt_write_offs_reason_type`, `ck_supplier_debt_write_offs_rejected_after_submission`, `ck_supplier_debt_write_offs_rejected_fields`, `ck_supplier_debt_write_offs_rejection_pair`, `ck_supplier_debt_write_offs_reversal_pair`, `ck_supplier_debt_write_offs_reversed_after_approval`, `ck_supplier_debt_write_offs_reversed_fields`, `ck_supplier_debt_write_offs_status`, `ck_supplier_debt_write_offs_submission_pair`, `ck_supplier_debt_write_offs_submitted_after_creation`, `ck_supplier_debt_write_offs_version`.

**Indexes:** 9 total. Partial: `CREATE UNIQUE INDEX ux_supplier_debt_write_offs_one_pending ON ahdah.supplier_debt_write_offs USING btree (company_id, supplier_debt_id) WHERE ((status)::text = 'PendingApproval'::text)`; `CREATE INDEX ix_supplier_debt_write_offs_pending ON ahdah.supplier_debt_write_offs USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_supplier_debt_write_offs_set_updated_at BEFORE UPDATE ON ahdah.supplier_debt_write_offs FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_debt_ledger_entries</summary>

**Columns:** `supplier_debt_ledger_entry_id: uuid = gen_random_uuid()`; `company_id: uuid`; `supplier_debt_id: uuid`; `debt_version_number: int`; `entry_type: varchar(40)`; `delta_debt_amount: numeric(18,2) = 0`; `delta_adjustment_amount: numeric(18,2) = 0`; `delta_paid_amount: numeric(18,2) = 0`; `delta_credit_note_amount: numeric(18,2) = 0`; `delta_written_off_amount: numeric(18,2) = 0`; `debt_after_amount: numeric(18,2)`; `adjustment_after_amount: numeric(18,2)`; `paid_after_amount: numeric(18,2)`; `credit_note_after_amount: numeric(18,2)`; `written_off_after_amount: numeric(18,2)`; `outstanding_after_amount: numeric(18,2)`; `debt_status_after: varchar(30)`; `reference_type: varchar(40)?`; `reference_id: uuid?`; `correlation_id: uuid = gen_random_uuid()`; `description: varchar(1000)?`; `performed_by_user_id: uuid`; `occurred_at: timestamptz = CURRENT_TIMESTAMP`; `created_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_debt_ledger_entry_id)`; `UNIQUE (company_id, supplier_debt_ledger_entry_id)`; `UNIQUE (company_id, supplier_debt_id, debt_version_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, supplier_debt_id) REFERENCES supplier_debts(company_id, supplier_debt_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, performed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_debt_ledger_after_amounts`, `ck_supplier_debt_ledger_after_equation`, `ck_supplier_debt_ledger_cancelled_status`, `ck_supplier_debt_ledger_description`, `ck_supplier_debt_ledger_entry_deltas`, `ck_supplier_debt_ledger_entry_type`, `ck_supplier_debt_ledger_open_status`, `ck_supplier_debt_ledger_partial_status`, `ck_supplier_debt_ledger_reference_match`, `ck_supplier_debt_ledger_reference_pair`, `ck_supplier_debt_ledger_reference_type`, `ck_supplier_debt_ledger_settled_status`, `ck_supplier_debt_ledger_status`, `ck_supplier_debt_ledger_version`.

**Indexes:** 8 total. Partial: `CREATE INDEX ix_supplier_debt_ledger_reference ON ahdah.supplier_debt_ledger_entries USING btree (company_id, reference_type, reference_id) WHERE (reference_id IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_supplier_debt_ledger_immutable BEFORE DELETE OR UPDATE ON ahdah.supplier_debt_ledger_entries FOR EACH ROW EXECUTE FUNCTION prevent_immutable_record_change()`.

</details>

<details>
<summary>supplier_payments</summary>

**Columns:** `supplier_payment_id: uuid = gen_random_uuid()`; `company_id: uuid`; `payment_number: varchar(50)`; `supplier_id: uuid`; `supplier_payment_account_id: uuid?`; `payment_date: date`; `currency_code: varchar(3) = 'LYD'::character varying`; `payment_amount: numeric(18,2)`; `fee_amount: numeric(18,2) = 0`; `total_disbursed_amount: numeric(18,2)? GENERATED (payment_amount + fee_amount)`; `payment_method: varchar(30)`; `payer_bank_name: varchar(150)?`; `reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `description: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `confirmed_by_user_id: uuid?`; `confirmed_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_payment_id)`; `UNIQUE (company_id, supplier_payment_id)`; `UNIQUE (company_id, payment_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, supplier_payment_account_id) REFERENCES supplier_payment_accounts(company_id, supplier_payment_account_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, confirmed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_id) REFERENCES suppliers(company_id, supplier_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_payments_account_required`, `ck_supplier_payments_amounts`, `ck_supplier_payments_bank_required`, `ck_supplier_payments_cancelled_after_creation`, `ck_supplier_payments_cancelled_fields`, `ck_supplier_payments_confirmed_after_submission`, `ck_supplier_payments_confirmed_fields`, `ck_supplier_payments_confirmed_proof`, `ck_supplier_payments_confirmed_reference`, `ck_supplier_payments_currency`, `ck_supplier_payments_description`, `ck_supplier_payments_draft_fields`, `ck_supplier_payments_method`, `ck_supplier_payments_notes`, `ck_supplier_payments_number`, `ck_supplier_payments_other_description`, `ck_supplier_payments_payer_bank`, `ck_supplier_payments_pending_fields`, `ck_supplier_payments_proof`, `ck_supplier_payments_reference`, `ck_supplier_payments_rejected_after_submission`, `ck_supplier_payments_rejected_fields`, `ck_supplier_payments_reversed_after_confirmation`, `ck_supplier_payments_reversed_fields`, `ck_supplier_payments_status`, `ck_supplier_payments_submission_pair`, `ck_supplier_payments_submitted_after_creation`, `ck_supplier_payments_version`.

**Indexes:** 10 total. Partial: `CREATE INDEX ix_supplier_payments_account ON ahdah.supplier_payments USING btree (company_id, supplier_payment_account_id) WHERE (supplier_payment_account_id IS NOT NULL)`; `CREATE INDEX ix_supplier_payments_pending ON ahdah.supplier_payments USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`; `CREATE INDEX ix_supplier_payments_reference ON ahdah.supplier_payments USING btree (company_id, reference_number) WHERE (reference_number IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_supplier_payments_set_updated_at BEFORE UPDATE ON ahdah.supplier_payments FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_payment_debt_allocations</summary>

**Columns:** `supplier_payment_debt_allocation_id: uuid = gen_random_uuid()`; `company_id: uuid`; `supplier_payment_id: uuid`; `supplier_debt_id: uuid`; `debt_version_number: int`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_payment_debt_allocation_id)`; `UNIQUE (company_id, supplier_payment_debt_allocation_id)`; `UNIQUE (company_id, supplier_payment_id, supplier_debt_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_debt_id) REFERENCES supplier_debts(company_id, supplier_debt_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_payment_id) REFERENCES supplier_payments(company_id, supplier_payment_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_payment_debt_allocations_amount`, `ck_supplier_payment_debt_allocations_debt_version`, `ck_supplier_payment_debt_allocations_notes`.

**Indexes:** 7 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_supplier_payment_debt_allocations_set_updated_at BEFORE UPDATE ON ahdah.supplier_payment_debt_allocations FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_payment_funding_sources</summary>

**Columns:** `supplier_payment_funding_source_id: uuid = gen_random_uuid()`; `company_id: uuid`; `supplier_payment_id: uuid`; `funding_source_id: uuid`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_payment_funding_source_id)`; `UNIQUE (company_id, supplier_payment_funding_source_id)`; `UNIQUE (company_id, supplier_payment_id, funding_source_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_payment_id) REFERENCES supplier_payments(company_id, supplier_payment_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_payment_funding_sources_amount`, `ck_supplier_payment_funding_sources_notes`.

**Indexes:** 7 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_supplier_payment_funding_sources_set_updated_at BEFORE UPDATE ON ahdah.supplier_payment_funding_sources FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_credit_notes</summary>

**Columns:** `supplier_credit_note_id: uuid = gen_random_uuid()`; `company_id: uuid`; `credit_note_number: varchar(50)`; `supplier_id: uuid`; `supplier_reference_number: varchar(100)?`; `credit_note_date: date`; `currency_code: varchar(3) = 'LYD'::character varying`; `credit_note_amount: numeric(18,2)`; `reason_type: varchar(40)`; `description: varchar(1000)`; `proof_file_url: varchar(1000)?`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_credit_note_id)`; `UNIQUE (company_id, supplier_credit_note_id)`; `UNIQUE (company_id, credit_note_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_id) REFERENCES suppliers(company_id, supplier_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_credit_notes_amount`, `ck_supplier_credit_notes_approval_pair`, `ck_supplier_credit_notes_approved_after_submission`, `ck_supplier_credit_notes_approved_fields`, `ck_supplier_credit_notes_cancellation_pair`, `ck_supplier_credit_notes_cancelled_after_creation`, `ck_supplier_credit_notes_cancelled_fields`, `ck_supplier_credit_notes_currency`, `ck_supplier_credit_notes_description`, `ck_supplier_credit_notes_draft_fields`, `ck_supplier_credit_notes_notes`, `ck_supplier_credit_notes_number`, `ck_supplier_credit_notes_other_reason`, `ck_supplier_credit_notes_pending_fields`, `ck_supplier_credit_notes_proof`, `ck_supplier_credit_notes_reason_type`, `ck_supplier_credit_notes_rejected_after_submission`, `ck_supplier_credit_notes_rejected_fields`, `ck_supplier_credit_notes_rejection_pair`, `ck_supplier_credit_notes_reversal_pair`, `ck_supplier_credit_notes_reversed_after_approval`, `ck_supplier_credit_notes_reversed_fields`, `ck_supplier_credit_notes_status`, `ck_supplier_credit_notes_submission_pair`, `ck_supplier_credit_notes_submitted_after_creation`, `ck_supplier_credit_notes_supplier_reference`, `ck_supplier_credit_notes_version`.

**Indexes:** 9 total. Partial: `CREATE UNIQUE INDEX ux_supplier_credit_notes_supplier_reference ON ahdah.supplier_credit_notes USING btree (company_id, supplier_id, lower((supplier_reference_number)::text)) WHERE (supplier_reference_number IS NOT NULL)`; `CREATE INDEX ix_supplier_credit_notes_pending ON ahdah.supplier_credit_notes USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_supplier_credit_notes_set_updated_at BEFORE UPDATE ON ahdah.supplier_credit_notes FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_credit_note_allocations</summary>

**Columns:** `supplier_credit_note_allocation_id: uuid = gen_random_uuid()`; `company_id: uuid`; `supplier_credit_note_id: uuid`; `supplier_debt_id: uuid`; `debt_version_number: int`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_credit_note_allocation_id)`; `UNIQUE (company_id, supplier_credit_note_allocation_id)`; `UNIQUE (company_id, supplier_credit_note_id, supplier_debt_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_credit_note_id) REFERENCES supplier_credit_notes(company_id, supplier_credit_note_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_debt_id) REFERENCES supplier_debts(company_id, supplier_debt_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_credit_note_allocations_amount`, `ck_supplier_credit_note_allocations_debt_version`, `ck_supplier_credit_note_allocations_notes`.

**Indexes:** 7 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_supplier_credit_note_allocations_set_updated_at BEFORE UPDATE ON ahdah.supplier_credit_note_allocations FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expense_returns</summary>

**Columns:** `expense_return_id: uuid = gen_random_uuid()`; `company_id: uuid`; `return_number: varchar(50)`; `expense_id: uuid`; `expense_version_number: int`; `return_date: date`; `return_reason_type: varchar(40)`; `resolution_type: varchar(40)`; `currency_code: varchar(3) = 'LYD'::character varying`; `return_amount: numeric(18,2)`; `refund_method: varchar(30)?`; `bank_name: varchar(150)?`; `reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `description: varchar(1000)`; `notes: varchar(1000)?`; `status: varchar(30) = 'Draft'::character varying`; `created_by_user_id: uuid`; `submitted_by_user_id: uuid?`; `submitted_at: timestamptz?`; `approved_by_user_id: uuid?`; `approved_at: timestamptz?`; `rejected_by_user_id: uuid?`; `rejected_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_return_id)`; `UNIQUE (company_id, expense_return_id)`; `UNIQUE (company_id, return_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, approved_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_id) REFERENCES expenses(company_id, expense_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, rejected_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expense_returns_amount`, `ck_expense_returns_approval_pair`, `ck_expense_returns_approved_after_submission`, `ck_expense_returns_approved_fields`, `ck_expense_returns_approved_refund_proof`, `ck_expense_returns_bank_name`, `ck_expense_returns_cancellation_pair`, `ck_expense_returns_cancelled_after_creation`, `ck_expense_returns_cancelled_fields`, `ck_expense_returns_currency`, `ck_expense_returns_description`, `ck_expense_returns_draft_fields`, `ck_expense_returns_expense_version`, `ck_expense_returns_notes`, `ck_expense_returns_number`, `ck_expense_returns_other_details`, `ck_expense_returns_pending_fields`, `ck_expense_returns_proof`, `ck_expense_returns_reason_type`, `ck_expense_returns_reference`, `ck_expense_returns_refund_bank_fields`, `ck_expense_returns_refund_method`, `ck_expense_returns_refund_reference`, `ck_expense_returns_refund_resolution`, `ck_expense_returns_rejected_after_submission`, `ck_expense_returns_rejected_fields`, `ck_expense_returns_rejection_pair`, `ck_expense_returns_resolution_type`, `ck_expense_returns_reversal_pair`, `ck_expense_returns_reversed_after_approval`, `ck_expense_returns_reversed_fields`, `ck_expense_returns_status`, `ck_expense_returns_submission_pair`, `ck_expense_returns_submitted_after_creation`, `ck_expense_returns_version`.

**Indexes:** 9 total. Partial: `CREATE INDEX ix_expense_returns_pending ON ahdah.expense_returns USING btree (company_id, submitted_at) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_expense_returns_set_updated_at BEFORE UPDATE ON ahdah.expense_returns FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expense_return_items</summary>

**Columns:** `expense_return_item_id: uuid = gen_random_uuid()`; `company_id: uuid`; `expense_return_id: uuid`; `expense_item_id: uuid`; `line_number: int`; `item_name_snapshot: varchar(200)`; `returned_quantity: numeric(18,3)`; `unit_code_snapshot: varchar(30)`; `custom_unit_name_snapshot: varchar(50)?`; `unit_return_price: numeric(18,2)`; `subtotal_amount: numeric(18,2)? GENERATED round((returned_quantity * unit_return_price), 2)`; `discount_amount: numeric(18,2) = 0`; `tax_amount: numeric(18,2) = 0`; `total_return_amount: numeric(18,2)? GENERATED ((round((returned_quantity * unit_return_price), 2) - discount_amount) + tax_amount)`; `item_condition: varchar(30)?`; `is_restockable: bool = false`; `notes: varchar(500)?`; `created_by_user_id: uuid`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_return_item_id)`; `UNIQUE (company_id, expense_return_item_id)`; `UNIQUE (company_id, expense_return_id, expense_item_id)`; `UNIQUE (company_id, expense_return_id, line_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_item_id) REFERENCES expense_items(company_id, expense_item_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_return_id) REFERENCES expense_returns(company_id, expense_return_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expense_return_items_condition`, `ck_expense_return_items_custom_unit`, `ck_expense_return_items_discount`, `ck_expense_return_items_line_number`, `ck_expense_return_items_name`, `ck_expense_return_items_notes`, `ck_expense_return_items_other_notes`, `ck_expense_return_items_quantity`, `ck_expense_return_items_restockable`, `ck_expense_return_items_tax`, `ck_expense_return_items_total`, `ck_expense_return_items_unit`, `ck_expense_return_items_unit_price`.

**Indexes:** 8 total. Partial: `CREATE INDEX ix_expense_return_items_condition ON ahdah.expense_return_items USING btree (company_id, item_condition) WHERE (item_condition IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_expense_return_items_set_updated_at BEFORE UPDATE ON ahdah.expense_return_items FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>expense_return_advance_allocations</summary>

**Columns:** `expense_return_advance_allocation_id: uuid = gen_random_uuid()`; `company_id: uuid`; `expense_return_id: uuid`; `expense_advance_allocation_id: uuid`; `balance_version_number: int`; `restored_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (expense_return_advance_allocation_id)`; `UNIQUE (company_id, expense_return_advance_allocation_id)`; `UNIQUE (company_id, expense_return_id, expense_advance_allocation_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_advance_allocation_id) REFERENCES expense_advance_allocations(company_id, expense_advance_allocation_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_return_id) REFERENCES expense_returns(company_id, expense_return_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_expense_return_advance_allocations_amount`, `ck_expense_return_advance_allocations_balance_version`, `ck_expense_return_advance_allocations_notes`.

**Indexes:** 6 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_expense_return_advance_allocations_set_updated_at BEFORE UPDATE ON ahdah.expense_return_advance_allocations FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>supplier_refunds</summary>

**Columns:** `supplier_refund_id: uuid = gen_random_uuid()`; `company_id: uuid`; `refund_number: varchar(50)`; `expense_return_id: uuid`; `supplier_id: uuid`; `funding_source_id: uuid`; `refund_date: date`; `currency_code: varchar(3) = 'LYD'::character varying`; `refund_amount: numeric(18,2)`; `fee_amount: numeric(18,2) = 0`; `net_received_amount: numeric(18,2)? GENERATED (refund_amount - fee_amount)`; `refund_method: varchar(30)`; `sender_bank_name: varchar(150)?`; `supplier_reference_number: varchar(150)?`; `transaction_reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `description: varchar(1000)`; `notes: varchar(1000)?`; `status: varchar(30) = 'PendingVerification'::character varying`; `recorded_by_user_id: uuid`; `verified_by_user_id: uuid?`; `verified_at: timestamptz?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (supplier_refund_id)`; `UNIQUE (company_id, supplier_refund_id)`; `UNIQUE (company_id, funding_source_id)`; `UNIQUE (company_id, refund_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, expense_return_id) REFERENCES expense_returns(company_id, expense_return_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, recorded_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, supplier_id) REFERENCES suppliers(company_id, supplier_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, verified_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_supplier_refunds_amounts`, `ck_supplier_refunds_bank_fields`, `ck_supplier_refunds_cancelled_after_creation`, `ck_supplier_refunds_cancelled_fields`, `ck_supplier_refunds_confirmed_fields`, `ck_supplier_refunds_confirmed_proof`, `ck_supplier_refunds_currency`, `ck_supplier_refunds_description`, `ck_supplier_refunds_method`, `ck_supplier_refunds_notes`, `ck_supplier_refunds_number`, `ck_supplier_refunds_other_notes`, `ck_supplier_refunds_pending_fields`, `ck_supplier_refunds_proof`, `ck_supplier_refunds_reference_required`, `ck_supplier_refunds_reversed_after_verification`, `ck_supplier_refunds_reversed_fields`, `ck_supplier_refunds_sender_bank`, `ck_supplier_refunds_status`, `ck_supplier_refunds_supplier_reference`, `ck_supplier_refunds_transaction_reference`, `ck_supplier_refunds_verified_after_creation`, `ck_supplier_refunds_version`.

**Indexes:** 11 total. Partial: `CREATE INDEX ix_supplier_refunds_transaction_reference ON ahdah.supplier_refunds USING btree (company_id, transaction_reference_number) WHERE (transaction_reference_number IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_supplier_refunds_set_updated_at BEFORE UPDATE ON ahdah.supplier_refunds FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>owner_payment_refunds</summary>

**Columns:** `owner_payment_refund_id: uuid = gen_random_uuid()`; `company_id: uuid`; `project_owner_id: uuid`; `project_id: uuid`; `project_owner_payment_id: uuid?`; `refund_number: varchar(50)`; `refund_date: date`; `refund_amount: numeric(18,2)`; `refund_method: varchar(30)`; `bank_name: varchar(150)?`; `reference_number: varchar(150)?`; `proof_file_url: varchar(1000)?`; `reason: varchar(1000)`; `notes: varchar(1000)?`; `status: varchar(30) = 'PendingApproval'::character varying`; `requested_by_user_id: uuid`; `reviewed_by_user_id: uuid?`; `reviewed_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `reversed_by_user_id: uuid?`; `reversed_at: timestamptz?`; `reversal_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (owner_payment_refund_id)`; `UNIQUE (company_id, owner_payment_refund_id)`; `UNIQUE (company_id, refund_number)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_owner_payment_id) REFERENCES project_owner_payments(company_id, project_owner_payment_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_owner_id) REFERENCES project_owners(company_id, project_owner_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_id) REFERENCES projects(company_id, project_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, requested_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reversed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reviewed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_owner_payment_refunds_amount`, `ck_owner_payment_refunds_bank_name`, `ck_owner_payment_refunds_cancelled_after_creation`, `ck_owner_payment_refunds_cancelled_fields`, `ck_owner_payment_refunds_confirmed_fields`, `ck_owner_payment_refunds_method`, `ck_owner_payment_refunds_method_fields`, `ck_owner_payment_refunds_notes`, `ck_owner_payment_refunds_number`, `ck_owner_payment_refunds_other_notes`, `ck_owner_payment_refunds_pending_fields`, `ck_owner_payment_refunds_proof_file`, `ck_owner_payment_refunds_reason`, `ck_owner_payment_refunds_reference_number`, `ck_owner_payment_refunds_reference_required`, `ck_owner_payment_refunds_rejected_fields`, `ck_owner_payment_refunds_reversed_after_confirmation`, `ck_owner_payment_refunds_reversed_fields`, `ck_owner_payment_refunds_reviewed_after_creation`, `ck_owner_payment_refunds_status`, `ck_owner_payment_refunds_version`.

**Indexes:** 9 total. Partial: `CREATE INDEX ix_owner_payment_refunds_original_payment ON ahdah.owner_payment_refunds USING btree (company_id, project_owner_payment_id) WHERE (project_owner_payment_id IS NOT NULL)`.

**Triggers:** `CREATE TRIGGER trg_owner_payment_refunds_set_updated_at BEFORE UPDATE ON ahdah.owner_payment_refunds FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>owner_refund_funding_sources</summary>

**Columns:** `owner_refund_funding_source_id: uuid = gen_random_uuid()`; `company_id: uuid`; `owner_payment_refund_id: uuid`; `funding_source_id: uuid`; `allocated_amount: numeric(18,2)`; `allocated_by_user_id: uuid`; `notes: varchar(500)?`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (owner_refund_funding_source_id)`; `UNIQUE (company_id, owner_refund_funding_source_id)`; `UNIQUE (company_id, owner_payment_refund_id, funding_source_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, allocated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, owner_payment_refund_id) REFERENCES owner_payment_refunds(company_id, owner_payment_refund_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, funding_source_id) REFERENCES funding_sources(company_id, funding_source_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_owner_refund_funding_sources_amount`, `ck_owner_refund_funding_sources_notes`.

**Indexes:** 6 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_owner_refund_funding_sources_set_updated_at BEFORE UPDATE ON ahdah.owner_refund_funding_sources FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>project_contract_changes</summary>

**Columns:** `project_contract_change_id: uuid = gen_random_uuid()`; `company_id: uuid`; `project_id: uuid`; `change_type: varchar(30)`; `previous_contract_value: numeric(18,2)`; `change_amount: numeric(18,2)`; `new_contract_value: numeric(18,2)`; `effective_date: date`; `reason: varchar(1000)`; `supporting_document_url: varchar(1000)?`; `status: varchar(30) = 'PendingApproval'::character varying`; `requested_by_user_id: uuid`; `reviewed_by_user_id: uuid?`; `reviewed_at: timestamptz?`; `rejection_reason: varchar(500)?`; `cancelled_by_user_id: uuid?`; `cancelled_at: timestamptz?`; `cancellation_reason: varchar(500)?`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (project_contract_change_id)`; `UNIQUE (company_id, project_contract_change_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, cancelled_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, project_id) REFERENCES projects(company_id, project_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, requested_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, reviewed_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_project_contract_changes_amount`, `ck_project_contract_changes_approved_fields`, `ck_project_contract_changes_cancelled_after_creation`, `ck_project_contract_changes_cancelled_fields`, `ck_project_contract_changes_document`, `ck_project_contract_changes_new_value`, `ck_project_contract_changes_pending_fields`, `ck_project_contract_changes_previous_value`, `ck_project_contract_changes_reason`, `ck_project_contract_changes_rejected_fields`, `ck_project_contract_changes_reviewed_after_creation`, `ck_project_contract_changes_status`, `ck_project_contract_changes_type`, `ck_project_contract_changes_type_amount`, `ck_project_contract_changes_version`.

**Indexes:** 7 total. Partial: `CREATE UNIQUE INDEX ux_project_contract_changes_one_pending ON ahdah.project_contract_changes USING btree (company_id, project_id) WHERE ((status)::text = 'PendingApproval'::text)`.

**Triggers:** `CREATE TRIGGER trg_project_contract_changes_set_updated_at BEFORE UPDATE ON ahdah.project_contract_changes FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>audit_logs</summary>

**Columns:** `audit_log_id: uuid = gen_random_uuid()`; `audit_sequence: bigint IDENTITY ALWAYS`; `company_id: uuid`; `event_category: varchar(40)`; `event_name: varchar(100)`; `event_action: varchar(50)`; `severity: varchar(20) = 'Information'::character varying`; `outcome: varchar(20) = 'Success'::character varying`; `actor_type: varchar(30)`; `actor_user_id: uuid?`; `actor_name_snapshot: varchar(200)`; `actor_role_snapshot: varchar(30)?`; `entity_type: varchar(100)?`; `entity_id: uuid?`; `entity_version_number: int?`; `description: varchar(1500)`; `before_values: jsonb?`; `after_values: jsonb?`; `changed_fields: jsonb = '[]'::jsonb`; `metadata: jsonb = '{}'::jsonb`; `correlation_id: uuid = gen_random_uuid()`; `session_id: uuid?`; `request_id: varchar(150)?`; `idempotency_key: varchar(200)?`; `source_type: varchar(30) = 'Application'::character varying`; `request_method: varchar(10)?`; `request_path: varchar(1000)?`; `http_status_code: int?`; `ip_address: inet?`; `user_agent: varchar(1000)?`; `device_identifier_hash: character(64)?`; `client_app_version: varchar(50)?`; `failure_code: varchar(100)?`; `failure_message: varchar(1500)?`; `occurred_at: timestamptz = CURRENT_TIMESTAMP`; `created_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (audit_log_id)`; `UNIQUE (company_id, audit_log_id)`; `UNIQUE (audit_sequence)`.

**Foreign keys:** `FOREIGN KEY (company_id, actor_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id) REFERENCES companies(company_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_audit_logs_actor_name`, `ck_audit_logs_actor_reference`, `ck_audit_logs_actor_role`, `ck_audit_logs_actor_type`, `ck_audit_logs_after_values`, `ck_audit_logs_before_values`, `ck_audit_logs_changed_fields`, `ck_audit_logs_client_version`, `ck_audit_logs_created_time`, `ck_audit_logs_description`, `ck_audit_logs_device_hash`, `ck_audit_logs_entity_pair`, `ck_audit_logs_entity_type`, `ck_audit_logs_entity_version`, `ck_audit_logs_event_action`, `ck_audit_logs_event_category`, `ck_audit_logs_event_name`, `ck_audit_logs_failed_fields`, `ck_audit_logs_failure_code`, `ck_audit_logs_failure_message`, `ck_audit_logs_http_status`, `ck_audit_logs_idempotency_key`, `ck_audit_logs_metadata`, `ck_audit_logs_outcome`, `ck_audit_logs_request_id`, `ck_audit_logs_request_method`, `ck_audit_logs_request_path`, `ck_audit_logs_severity`, `ck_audit_logs_source`, `ck_audit_logs_success_fields`.

**Indexes:** 14 total. Partial: `CREATE UNIQUE INDEX ux_audit_logs_idempotency ON ahdah.audit_logs USING btree (company_id, idempotency_key) WHERE (idempotency_key IS NOT NULL)`; `CREATE INDEX ix_audit_logs_actor ON ahdah.audit_logs USING btree (company_id, actor_user_id, occurred_at DESC) WHERE (actor_user_id IS NOT NULL)`; `CREATE INDEX ix_audit_logs_entity ON ahdah.audit_logs USING btree (company_id, entity_type, entity_id, occurred_at DESC) WHERE (entity_id IS NOT NULL)`; `CREATE INDEX ix_audit_logs_session ON ahdah.audit_logs USING btree (company_id, session_id, occurred_at DESC) WHERE (session_id IS NOT NULL)`; `CREATE INDEX ix_audit_logs_failures ON ahdah.audit_logs USING btree (company_id, occurred_at DESC) WHERE ((outcome)::text = ANY ((ARRAY['Failure'::character varying, 'Denied'::character varying])::text[]))`; `CREATE INDEX ix_audit_logs_critical ON ahdah.audit_logs USING btree (company_id, occurred_at DESC) WHERE ((severity)::text = 'Critical'::text)`; `CREATE INDEX ix_audit_logs_security_ip ON ahdah.audit_logs USING btree (company_id, ip_address, occurred_at DESC) WHERE (((event_category)::text = ANY ((ARRAY['Authentication'::character varying, 'Authorization'::character varying, 'Security'::character varying])::text[])) AND (ip_address IS NOT NULL))`.

**Triggers:** `CREATE TRIGGER trg_audit_logs_immutable BEFORE DELETE OR UPDATE ON ahdah.audit_logs FOR EACH ROW EXECUTE FUNCTION prevent_immutable_record_change()`.

</details>

<details>
<summary>idempotency_records</summary>

**Columns:** `idempotency_record_id: uuid = gen_random_uuid()`; `company_id: uuid`; `idempotency_key: varchar(200)`; `operation_name: varchar(150)`; `request_method: varchar(10)`; `request_path: varchar(1000)`; `request_source: varchar(30) = 'Application'::character varying`; `request_fingerprint_hash: character(64)`; `request_payload_hash: character(64)?`; `actor_type: varchar(30) = 'User'::character varying`; `actor_user_id: uuid?`; `client_identifier: varchar(200)?`; `status: varchar(20) = 'InProgress'::character varying`; `attempt_count: int = 1`; `lock_token: uuid?`; `locked_by: varchar(200)?`; `lock_acquired_at: timestamptz?`; `lease_expires_at: timestamptz?`; `response_http_status: int?`; `response_content_type: varchar(150)?`; `response_payload: jsonb?`; `resource_type: varchar(100)?`; `resource_id: uuid?`; `resource_version_number: int?`; `failure_code: varchar(100)?`; `failure_message: varchar(2000)?`; `is_retryable: bool?`; `replay_count: int = 0`; `last_replayed_at: timestamptz?`; `correlation_id: uuid = gen_random_uuid()`; `started_at: timestamptz = CURRENT_TIMESTAMP`; `completed_at: timestamptz?`; `expires_at: timestamptz = (CURRENT_TIMESTAMP + '24:00:00'::interval)`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (idempotency_record_id)`; `UNIQUE (company_id, idempotency_key)`; `UNIQUE (company_id, idempotency_record_id)`.

**Foreign keys:** `FOREIGN KEY (company_id, actor_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id) REFERENCES companies(company_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_idem_actor_reference`, `ck_idem_actor_type`, `ck_idem_attempts`, `ck_idem_cancelled`, `ck_idem_client_identifier`, `ck_idem_completed`, `ck_idem_completed_time`, `ck_idem_content_type`, `ck_idem_created_time`, `ck_idem_expired`, `ck_idem_expiry_time`, `ck_idem_failed`, `ck_idem_failure_code`, `ck_idem_failure_message`, `ck_idem_fingerprint`, `ck_idem_http_status`, `ck_idem_in_progress`, `ck_idem_key`, `ck_idem_lease_time`, `ck_idem_lock_fields`, `ck_idem_method`, `ck_idem_operation`, `ck_idem_path`, `ck_idem_payload_hash`, `ck_idem_replay_count`, `ck_idem_replay_fields`, `ck_idem_resource_fields`, `ck_idem_resource_type`, `ck_idem_source`, `ck_idem_status`.

**Indexes:** 12 total. Partial: `CREATE INDEX ix_idem_active_lease ON ahdah.idempotency_records USING btree (lease_expires_at) WHERE ((status)::text = 'InProgress'::text)`; `CREATE INDEX ix_idem_actor ON ahdah.idempotency_records USING btree (company_id, actor_user_id, created_at DESC) WHERE (actor_user_id IS NOT NULL)`; `CREATE INDEX ix_idem_resource ON ahdah.idempotency_records USING btree (company_id, resource_type, resource_id) WHERE (resource_id IS NOT NULL)`; `CREATE INDEX ix_idem_retryable_failed ON ahdah.idempotency_records USING btree (company_id, completed_at) WHERE (((status)::text = 'Failed'::text) AND (is_retryable = true))`; `CREATE INDEX ix_idem_expiry ON ahdah.idempotency_records USING btree (expires_at) WHERE ((status)::text = ANY ((ARRAY['Completed'::character varying, 'Failed'::character varying, 'Cancelled'::character varying, 'Expired'::character varying])::text[]))`.

**Triggers:** `CREATE TRIGGER trg_idempotency_records_set_updated_at BEFORE UPDATE ON ahdah.idempotency_records FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

<details>
<summary>company_settings</summary>

**Columns:** `company_setting_id: uuid = gen_random_uuid()`; `company_id: uuid`; `default_currency_code: varchar(3) = 'LYD'::character varying`; `default_locale_code: varchar(10) = 'ar-LY'::character varying`; `time_zone: varchar(100) = 'Africa/Tripoli'::character varying`; `fiscal_year_start_month: smallint = 1`; `first_day_of_week: smallint = 6`; `money_decimal_places: smallint = 2`; `quantity_decimal_places: smallint = 3`; `rounding_mode: varchar(30) = 'HalfUp'::character varying`; `balance_tolerance_amount: numeric(18,2) = 0`; `de_minimis_write_off_limit: numeric(18,2) = 0`; `allow_multi_currency: bool = false`; `allow_negative_advance_balance: bool = false`; `allow_financial_overallocation: bool = false`; `advance_approval_mode: varchar(20) = 'Always'::character varying`; `advance_approval_threshold_amount: numeric(18,2)?`; `expense_approval_mode: varchar(20) = 'Always'::character varying`; `expense_approval_threshold_amount: numeric(18,2)?`; `transfer_approval_mode: varchar(20) = 'Always'::character varying`; `transfer_approval_threshold_amount: numeric(18,2)?`; `supplier_payment_approval_mode: varchar(20) = 'Always'::character varying`; `supplier_payment_approval_threshold_amount: numeric(18,2)?`; `personal_claim_payment_approval_mode: varchar(20) = 'Always'::character varying`; `personal_claim_payment_approval_threshold_amount: numeric(18,2)?`; `settlement_requires_approval: bool = true`; `closure_requires_approval: bool = true`; `allow_self_approval: bool = false`; `require_distinct_creator_approver: bool = true`; `require_distinct_submitter_approver: bool = true`; `expense_document_mode: varchar(20) = 'Threshold'::character varying`; `expense_document_threshold_amount: numeric(18,2)? = 0`; `payment_proof_mode: varchar(20) = 'Threshold'::character varying`; `payment_proof_threshold_amount: numeric(18,2)? = 0`; `max_document_size_bytes: bigint = 10485760`; `allowed_mime_types: jsonb = '["image/jpeg", "image/png", "application/pdf"]'::jsonb`; `allowed_file_extensions: jsonb = '[".jpg", ".jpeg", ".png", ".pdf"]'::jsonb`; `max_single_advance_amount: numeric(18,2)?`; `max_single_expense_amount: numeric(18,2)?`; `max_single_transfer_amount: numeric(18,2)?`; `max_single_supplier_payment_amount: numeric(18,2)?`; `max_single_personal_claim_payment_amount: numeric(18,2)?`; `default_advance_due_days: int = 30`; `default_supplier_debt_due_days: int = 30`; `default_personal_claim_due_days: int = 30`; `invitation_expiry_hours: int = 72`; `join_request_expiry_days: int = 30`; `session_timeout_minutes: int = 120`; `maximum_failed_login_attempts: int = 5`; `account_lockout_minutes: int = 30`; `require_mfa_for_manager: bool = false`; `require_mfa_for_deputy: bool = false`; `require_mfa_for_accountant: bool = false`; `audit_log_retention_days: int = 2555`; `notification_retention_days: int = 365`; `outbox_retention_days: int = 90`; `background_job_retention_days: int = 90`; `idempotency_retention_hours: int = 24`; `default_notification_channels: jsonb = '["InApp", "Push"]'::jsonb`; `additional_settings: jsonb = '{}'::jsonb`; `is_active: bool = true`; `created_by_user_id: uuid`; `updated_by_user_id: uuid`; `version_number: int = 1`; `created_at: timestamptz = CURRENT_TIMESTAMP`; `updated_at: timestamptz = CURRENT_TIMESTAMP`.

**Keys:** `PRIMARY KEY (company_setting_id)`; `UNIQUE (company_id)`; `UNIQUE (company_id, company_setting_id)`.

**Foreign keys:** `FOREIGN KEY (company_id) REFERENCES companies(company_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, created_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`; `FOREIGN KEY (company_id, updated_by_user_id) REFERENCES app_users(company_id, user_id) ON UPDATE RESTRICT ON DELETE RESTRICT`.

**CHECKs inspected:** `ck_company_settings_additional`, `ck_company_settings_advance_approval`, `ck_company_settings_approval_modes`, `ck_company_settings_approval_separation`, `ck_company_settings_channels`, `ck_company_settings_claim_payment`, `ck_company_settings_currency`, `ck_company_settings_decimal_places`, `ck_company_settings_default_days`, `ck_company_settings_document_modes`, `ck_company_settings_document_size`, `ck_company_settings_expense_approval`, `ck_company_settings_expense_document`, `ck_company_settings_extensions`, `ck_company_settings_financial_values`, `ck_company_settings_first_day`, `ck_company_settings_fiscal_month`, `ck_company_settings_locale`, `ck_company_settings_max_amounts`, `ck_company_settings_membership_expiry`, `ck_company_settings_mime_types`, `ck_company_settings_payment_proof`, `ck_company_settings_retention`, `ck_company_settings_rounding`, `ck_company_settings_security`, `ck_company_settings_supplier_payment`, `ck_company_settings_time_zone`, `ck_company_settings_transfer_approval`, `ck_company_settings_version`.

**Indexes:** 6 total. No partial indexes.

**Triggers:** `CREATE TRIGGER trg_company_settings_set_updated_at BEFORE UPDATE ON ahdah.company_settings FOR EACH ROW EXECUTE FUNCTION set_updated_at()`.

</details>

## Appendix B. Repository source register

- [Generated context](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Context/AhdahDbContext.cs) and its sibling Entities directory; [non-generated concurrency configuration](../backend/src/Ahdah.Infrastructure/Persistence/AhdahDbContext.IdentityConfiguration.cs).
- [AdvanceService](../backend/src/Ahdah.Infrastructure/Advances/AdvanceService.cs), [ExpenseService](../backend/src/Ahdah.Infrastructure/Expenses/ExpenseService.cs), [SupplierService](../backend/src/Ahdah.Infrastructure/Suppliers/SupplierService.cs), [ProjectService](../backend/src/Ahdah.Infrastructure/Projects/ProjectService.cs).
- [Settlement service](../backend/src/Ahdah.Infrastructure/Settlements/ProjectSettlementService.cs), [settlement queries](../backend/src/Ahdah.Infrastructure/Settlements/ProjectSettlementQueries.cs), [settlement rules](../backend/src/Ahdah.Application/Settlements/SettlementRules.cs), [architecture](ARCHITECTURE.md).
- API controller directory and Infrastructure Identity/Access services were searched for equivalent mutation paths; existing audit/idempotency writes belong to their command transactions.
