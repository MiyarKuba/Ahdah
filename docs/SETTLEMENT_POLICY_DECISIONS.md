# Project settlement and financial closure: policy decisions

**Status: RECOMMENDED DESIGN — REQUIRES PRODUCT OWNER APPROVAL.** No recommendation in this document is an implemented or approved business rule.

Review date: 2026-09-16. Baseline: `eb5fbadb6387557dc87cbba6b114c3d3374f4d82`. Scope: project advance attribution, settlement, financial closure, and correction. This is an operational financial-control design, not a claim of compliance with a particular accounting standard or jurisdiction.

**Reading guide:** start with the executive summary, then use sections 15–16 to record decisions. Sections 2–14 contain the evidence, alternatives and consequences; section 17 turns approved choices into implementation gates.

## 1. Executive summary

Ahdah can identify some project obligations, but it cannot yet certify that a project is financially settled. A user holding money, the source that funded that money, and the project receiving an expense are different facts. Today there is no authoritative assignment of unused custody to projects. Nor is there a project certification record, approved closure authority, or comprehensive lock against financial activity after closure.

**Recommended policy package:**

1. Adopt explicit project allocations within user/advance balances, with immutable movement history. Keep an explicit unassigned company-custody remainder. Never infer remaining project custody from past expenses or owner funding.
2. Keep physical completion separate from financial certification. Use a separate settlement cycle, not a new `FinanciallySettling` project status.
3. Define settlement as an approved, reproducible reconciliation at a specific financial revision: all required evidence evaluated, no pending project operations, no outstanding project liabilities, no unresolved differences, and no remaining project-designated custody. Approved release of designation may leave custody at company level; it is not repayment or disappearance of money.
4. Define closure as Manager certification of that still-current settlement and activation of enforceable financial write restrictions. Persist the evidence and decisions, not just a project status.
5. Accountant/Deputy prepare and submit; an independent Manager approves and closes. Supervisor acknowledges physical completion only. No default self-approval escape for a one-person company.
6. Deliver audited reopening/correction before enabling closure in production. Preserve every old certificate; reopen normal projects to Completed, not Active. Cancelled projects need financial run-off and their own closure basis, without pretending that work completed.

All seven decision areas—attribution, eligibility, evidence, authority, locking, cancellation, and correction—must be approved together. Sections 15–16 are the owner response sheet. Choosing an easier attribution model does not remove the other gaps.

## 2. Current system facts

### Evidence and review boundary

Findings below were checked against application code, generated Database-First entities, and EF mappings at the baseline. No live PostgreSQL connection, financial-row query, or database mutation was performed for this review. Generated mappings do not include every database CHECK/trigger. Earlier catalog findings are explicitly identified below; revalidate them against the deployed catalog before preparing schema changes. The repository reports 91 tables in `ahdah_db.ahdah`; this review did not independently recount live tables.

| Ref | Inspected source and what it establishes |
|---|---|
| E1 | [ProjectConstants and lifecycle rules](../backend/src/Ahdah.Application/Projects/ProjectConstants.cs), [requests](../backend/src/Ahdah.Application/Projects/Contracts/ProjectRequests.cs), [ProjectService](../backend/src/Ahdah.Infrastructure/Projects/ProjectService.cs): five known statuses; create Active; metadata transitions only Active/Paused; terminal project metadata and assignment changes rejected. |
| E2 | [Project entity](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/Project.cs), [generated mappings](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Context/AhdahDbContext.cs): completion dates and version exist; no project currency, settlement FK, closed-by, closed-at, or certification evidence fields. |
| E3 | [Advance](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/Advance.cs), [UserAdvanceBalance](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/UserAdvanceBalance.cs), [AdvanceService](../backend/src/Ahdah.Infrastructure/Advances/AdvanceService.cs), [advance contract review](ADVANCES.md): advance issued to Deputy; per-user/per-advance custody; no project relationship. |
| E4 | [MoneyTransfer](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/MoneyTransfer.cs), [TransferAdvanceAllocation](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/TransferAdvanceAllocation.cs), [BalanceLedgerEntry](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/BalanceLedgerEntry.cs): delivery/distribution/return lineage and balance movements, without project slices. |
| E5 | [AdvanceSettlement](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/AdvanceSettlement.cs), [resolution](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/AdvanceSettlementResolution.cs), [AdvanceClosure](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/AdvanceClosure.cs), E2 mappings: balance-level declared-versus-book difference and advance-level closure snapshots; not project certification. |
| E6 | Generated [FundingSource](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/FundingSource.cs), [ManagerContribution](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/ManagerContribution.cs), [owner payment](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/ProjectOwnerPayment.cs), [owner allocation](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/OwnerPaymentProjectAllocation.cs), [cashbox entry](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/CompanyCashboxEntry.cs), advance/source allocation and source ledger mappings: source availability and provenance, not project custody ownership. |
| E7 | [ExpenseService](../backend/src/Ahdah.Infrastructure/Expenses/ExpenseService.cs), [ExpenseDocumentRules](../backend/src/Ahdah.Application/Expenses/ExpenseDocumentRules.cs), generated Expense, ExpenseAdvanceAllocation, ExpenseDocument, PersonalClaim and claim-payment allocations; [expense review](EXPENSES.md): direct optional project, custody reservations and unpaid claims; attachment metadata is not verified physical/digital evidence. |
| E8 | [SupplierService](../backend/src/Ahdah.Infrastructure/Suppliers/SupplierService.cs), generated SupplierDebt, payment/credit allocation, SupplierRefund, ExpenseReturn and ExpenseReturnAdvanceAllocation mappings; [supplier review](SUPPLIERS.md): computed debt balances and allocation lineage; incomplete return-effect workflows. |
| E9 | Generated [ProjectContractChange](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/ProjectContractChange.cs), [OwnerPaymentRefund](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/OwnerPaymentRefund.cs), OwnerRefundFundingSource: direct project operations but no currency on contract/refund headers. |
| E10 | [SettlementRules](../backend/src/Ahdah.Application/Settlements/SettlementRules.cs), [service](../backend/src/Ahdah.Infrastructure/Settlements/ProjectSettlementService.cs), [queries](../backend/src/Ahdah.Infrastructure/Settlements/ProjectSettlementQueries.cs), [contract](SETTLEMENT.md): nullable readiness, visible categories, permanent and conditional gaps. |
| E11 | [API authorization](../backend/src/Ahdah.Api/Authentication/AuthenticationDependencyInjection.cs), [ProjectsController](../backend/src/Ahdah.Api/Controllers/ProjectsController.cs), E1/E10: role allow-list plus active membership, tenant and assignment checks; read permission does not grant financial transition permission. |
| E12 | [CompanySetting](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/CompanySetting.cs), [AuditLog](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/AuditLog.cs), [IdempotencyRecord](../backend/src/Ahdah.Infrastructure/Persistence/Generated/Entities/IdempotencyRecord.cs), [concurrency configuration](../backend/src/Ahdah.Infrastructure/Persistence/AhdahDbContext.IdentityConfiguration.cs): settings, safe replay and per-record concurrency exist; no aggregate financial revision or project certification protocol. |

README, PROJECT_STATUS, ARCHITECTURE and the advance/expense/supplier/settlement documents were cross-checked. Searches covered project statuses, completion/closure, settlement, reopening, returns/refunds, project IDs, funding and user balances. No application project-finalization/reopen command was found. Existing advance settlement/closure entities do not mean those workflows are implemented.

### Accounting and relationship facts

- `advances` has amount/currency, required Deputy recipient, and lifecycle; `user_advance_balances` is unique by company/advance/user. Source-to-advance allocations fund it; delivery creates custody. There is no project ID on advance, balance, transfer, advance settlement, or advance closure. [E3–E5]
- Balance accounting tracks received + restored against expensed + transferred-out + returned + adjustment-out + available + reserved. These are stored authoritative balance fields updated with ledger entries by application transactions; do not assume a trigger computes them. A return credits the upstream recipient's restored custody. Returning money to a Manager/Deputy is not necessarily a deposit into company cashbox or extinguishment of custody. [E3–E4]
- An expense has one optional project. An expense funding allocation identifies the balance consumed, not ownership of everything left in it. Personal claims also have optional direct project links. Supplier debt has both expense and project links; inconsistency between them must be treated as a data-quality issue, not silently resolved during certification. [E7–E8]
- Supplier debt outstanding is generated as debt + adjustment − paid − credits − write-offs. Claim outstanding is claim + adjustment − paid − reductions − write-offs. Pending payments do not reduce these balances. A credit operation and the debt it will affect are overlapping views, not two new liabilities to add together. [E2 mappings, E7–E8]
- Supplier payments can cover several projects through debt allocations; claim payments through claim allocations. Credit headers have no project and allocations target debts. The implemented credit workflow allocates only approved notes: a new pending unallocated note can therefore be invisible to project-specific pending-credit detection. Supplier identity does not establish its project intent. [E8, E10]
- Manager contribution is a source of funds, not automatically a repayable loan. A linked personal claim establishes a liability under the existing model; project attribution of that claim must be explicit. Owner-payment allocations establish funding provenance, not necessarily restrictions on the future use of custody. [E6–E7]
- The advance closure computed `ReconciliationStatus` tests settled balance count, pending operations, unresolved issues, available/reserved money and difference. It does **not itself test** the separate outstanding-debt/claim snapshot fields. Copying that expression into project closure would be unsafe. [E5, generated mapping]
- Current read-side document rules accept non-rejected metadata; PendingVerification can satisfy them. They do not establish bytes exist or paper was received. Company tolerance and de-minimis settings do not authorize automatic forgiveness. [E7, E12]
- The GET uses RepeatableRead READ ONLY. Expense and supplier invoice creation validate project existence, but do not enforce a project financial-closure guard. Other financial operations lock individual records; they do not share a project-wide gate or update its version. [E7–E8, E10]

### Exact current lifecycle versus proposed meaning

| Status | Current implementation | Recommended operational meaning — approval required |
|---|---|---|
| Active | Created here; editable by Manager | Work may proceed. |
| Paused | Manager metadata transition from/to Active/Paused | Work paused; valid accrued obligations remain payable. |
| Completed | Recognized/readable; no completion command | Physical/contracted work finished, recorded date and accountable completion decision; reconciliation and legitimate late costs may continue. |
| FinanciallyClosed | Recognized/readable; no closure command | Ordinary financial changes locked by an active closure certificate, separate from physical completion. |
| Cancelled | Recognized/readable; no cancellation command | Work terminated, with reason/actor/date; liabilities and custody survive. |

`CompletedAt` and `ActualEndDate` exist. Earlier PostgreSQL inspection in [SETTLEMENT.md](SETTLEMENT.md) reports both required for Completed/FinanciallyClosed; that CHECK was not independently reread from PostgreSQL in this review. It is a material constraint when designing cancelled-project closure.

**Proposed lifecycle:** Active ↔ Paused; either → Completed with evidence, or → Cancelled with termination evidence. Completed → FinanciallyClosed only through certified closure. Reopening FinanciallyClosed → Completed, never implicitly Active. Resuming physical work is a separate approved operation. A separate settlement cycle moves Draft → Submitted → Approved, or Rejected/Withdrawn/Superseded, with immutable transition events. Material financial changes make an approved cycle stale. Do not add `FinanciallySettling`: it conflates reconciliation progress with physical work and does not represent repeated cycles. [Decision D02]

## 3. Problems preventing final settlement

This table combines actual emitted gaps with newly identified design gaps. `Code` means code-only work can resolve the gap using current data; it does not authorize implementation. `Schema` is conditional on the recommended policy. Every new business policy requires approval.

| Gap | Why it matters / current data | Missing rule or data | Code-only? | Schema? | Business decision? | Recommended resolution |
|---|---|---|---|---|---|---|
| ProjectAdvanceAttributionUnavailable | Custody/source/expense lineage exists, unused project ownership does not | Explicit intent, custody slices and disposition | No | Necessary | Yes | D01 explicit allocations and movement history |
| ProjectSettlementPolicyUndefined | Visible blocker report exists | Exact pass criteria and certification scope | Partly | Persistence needed | Yes | D03 predicates in section 6 |
| ProjectClosurePolicyUndefined | Project status/version exist | Closure gate, lock, evidence and authority | No | Necessary | Yes | D04 certificate and section 12 protocol |
| FinancialCategoryNotVisible | Supervisor has limited categories; Deputy/Accountant cannot see owner operations | Full-scope certifier versus partial preparer | Yes for masking; workflow needs persistence | Necessary for packet/approval | Yes | No full-ready conclusion from a partial view; Manager completes owner section |
| ProjectCompletionEvidenceUnavailable / no completion command | Dates exist, no completion actor/acknowledgment workflow | Operational completion authority/evidence | Partly | Recommended evidence event | Yes | D02 controlled completion separate from finance |
| ActiveDocumentSettingsUnavailable / DocumentPolicyUnavailable | Settings can be missing or malformed | Applicable valid version, category rules | Yes to detect/block | Policy binding necessary | Yes | Resolve configuration; never interpret unknown policy as Never |
| Digital/paper evidence | Metadata, hash and verification fields exist; no storage verification/paper chain | Verified bytes, required originals or approved exception | No for paper | Necessary if originals required | Yes | D06 scoped document checklist and custody events |
| UnallocatedCreditProjectIntentUnavailable | Credit header and debt allocations exist | Whether pending credit belongs to a project | No for reliable intent | Necessary | Yes | Explicit credit intent allocations or reviewed non-project designation |
| ApprovedReturnEffectsRequireReconciliation | Return status, some restoration allocations and refund link exist | Complete effects for every resolution, including credit/Other | Not completely | Necessary links | Yes | D07 resolution-to-effect reconciliation with duplicate prevention |
| FinancialRecordRequiresReview | Unknown statuses or null outstanding values surface in queries | Approved status treatment / trustworthy amounts | Often data/code | Only if model incomplete | Yes for new treatment | Fail closed; fix underlying record, never substitute zero |
| Owner currency/final owner position | Contract value, payments, allocations, refunds exist; headers lack complete currency semantics | Contract denomination, refund currency, final certified claim/retention/dispute treatment | No | Necessary | Yes | D08 explicit currency and signed final owner reconciliation |
| Manager funding classification | Contribution and optional claim exist | Capital/company financing vs repayable project obligation | No reliable inference | Decision evidence/link may be needed | Yes | D09 confirm classification; no blanket repayment rule |
| Unlinked/legacy records | Optional project on expenses/claims/debts; no comprehensive cutover attribution | Completeness attestation and approved opening slices | No automatic solution | Necessary cutover evidence | Yes | D10 reviewed migration; no inferred historical rewrite |
| Settlement persistence | Advance snapshots exist, project snapshot does not | Reproducible evidence and policy revision | No | Necessary | Yes | D05 detailed certification record |
| Closure authority/separation | Role gates and generic settings exist | Named action permissions and independent reviewer | Partly | Approval evidence necessary | Yes | D11 matrix; defaults cannot silently bypass it |
| Stale results / post-close mutation | Entity versions, transactions, idempotency exist | Shared writer gate, financial revision and closure enforcement | Mostly, with new state | Necessary | Yes | D12 update all affected write paths before enabling close |
| Reopen and cancellation | Statuses exist, financial correction path absent | Cycles, preserved certificates, run-off policy | No | Necessary events/state | Yes | D13–D14 before closure release |
| Deferred resolution commands | Claim payments, returns/refunds, advance difference workflows have schema but missing public workflows | Safe authorized methods to clear legitimate blockers | Partly; some need new links | Conditional | Yes | Implement prerequisite workflows; no admin SQL workaround |
| Tolerance/retention/settings changes | Numeric settings and per-record versions exist | Zero rule, rounding scope, durable policy history/retention | Partly | Policy snapshot necessary | Yes | D15 exact zero after approved posted resolution; preserve certification policy |

Known blockers also prevent true: unfinished expenses, missing required metadata, unsettled claims/debts, pending allocated payment/credit/return/refund/owner operations. Lifecycle impediments currently block Cancelled, FinanciallyClosed, unknown project statuses, and closure of Active/Paused projects. These are not missing-data gaps and must not be waived by merely removing the permanent gap codes. The current summarizer deliberately never returns true, even if the gap collection is empty. [E10]

## 4. Advance attribution options

### Five distinct concepts

**Source** answers where money came from. **Holder** answers who must account for custody. **Intended project** answers what use is authorized. **Spent-on project** answers which project received a recorded cost. **Unused custody** answers what remains held or reserved. Intent can change through an approved reallocation; past expenditure remains history. None of these changes automatically changes source ownership, creates repayment, or moves cash.

### Common example (all amounts LYD)

Manager receives 20,000 into company funding. Management intends 10,000 for A. The actual current advance workflow issues custody to a Deputy; “Manager receives” must not be mistaken for creating a Manager-held advance balance. After confirmed delivery/distribution, the holder spends 6,000 on A and later 5,000 on B. Cash/book custody remaining is 9,000, assuming no fees, transfers or other effects. Only an explicit allocation policy can establish that 4,000 of the 9,000 still belongs to A's designated envelope. The original other 10,000 would then have 5,000 remaining.

| Option | Accounting meaning and example | Strengths | Weakness/failure scenario | Schema and workflow impact | Settlement, migration and audit impact |
|---|---|---|---|---|---|
| A. No attribution | All 9,000 is company/user custody. A has 6,000 spending, no asserted residual 4,000. | Matches current custody model; shared site purchasing stays flexible. | Project “settled” could coexist with funds still informally promised to A; cannot certify project-specific unused cash. | No attribution schema; keep custody reconciliation separate. | Viable only if owner explicitly narrows certificate to project costs/payables and accepts company custody outside its scope. No balance migration; document exclusions and separate custody accountability. |
| B. Infer from expenses | Observes 6,000 A and 5,000 B; cannot assign remaining 9,000. A 6:5 split would invent intent. | Useful historical cost analytics; no new intent capture. | No spending gives no attribution; later expense changes apparent ownership; shared source funds unrelated jobs. Zero cost can hide all unused intended cash. | Query-only heuristic possible, but cannot produce authoritative ownership. | Reject for certification. Migration creates fabricated history; audit cannot explain original authorization. Use only clearly labeled spending analysis. |
| C. One project per advance | Assigning all 20,000 to A would make B spending invalid. Need separate advances (for example 10,000 A, 5,000 B, 5,000 company custody) or explicit transfer of designation before B expense. | Clear for dedicated site advances; straightforward boundary. | Shared foreman wallets and consolidated procurement need many advances; splitting historical mixed advances breaks lineage if rewritten. Transfers between holders must preserve project. | Add nullable project FK plus restrictions to creation, transfers, expenses, refunds; cross-project use needs controlled release/reissue. | A can reconcile its 4,000, but mixed legacy advances need manual partition/exception. Audit every designation change; never change FK retroactively to reclassify old spending. |
| D. Project allocations within custody | Allocate 10,000 A; leave 10,000 company-unassigned. Spend 6,000 A → A available 4,000. Assign 5,000 pool to B then spend it → pool 5,000, B available 0. | Fits shared advances and explicit operational intent; reconciles project and holder dimensions. | More complex reservation/transfer/return logic; careless implementation duplicates money or lets release hide shortages. | New balance/project slices and movement/effect links; all custody writes update them transactionally. | Project can finish independently of other slices. Legacy opening positions require signed classification, not expense-ratio backfill. Immutable movements explain every change. |
| E. Full event-sourced, double-entry project/custodian subledger | Represent source, holder, purpose and every posting as balanced journal dimensions; example uses explicit A/B designation journals. | Strong replay and general accounting integration if adopted company-wide. | Existing ledgers are not a complete general ledger or event store. Retrofitting creates dual authorities and requires accounting policy far beyond project settlement. | Broad chart-of-accounts/event/reversal/rebuild work; extensive service replacement and migration. | Best considered a separate accounting-platform decision. Higher audit potential only after validated openings and complete event capture; no immediate shortcut to certification. |

**Recommendation: D with append-only movement history, not a full E rewrite.** It fits construction teams sharing custody across sites while preserving explicit responsibility. C remains a possible business constraint for companies forbidding shared advances, but should not be imposed merely to simplify development. A is an honest narrower product if selected explicitly. B cannot justify unused-cash ownership. [D01]

### Transfer and return stress test for every option

| Option | Transfer between holders | Return to upstream holder / company | Failure that the design must expose |
|---|---|---|---|
| A | Existing advance lineage and user custody move unchanged; no project designation moves | Existing return reduces one holder's custody and restores another's; project settlement ignores residual custody by explicit scope | A holder may still retain money informally promised to a supposedly settled project; certificate must disclose this exclusion |
| B | Cannot determine which project's unused money moved from the sender's expense history, particularly if recipient has never spent it | A return has no project expense from which to derive an ownership split | Recalculating historical ratios reassigns remaining cash without an authorization event; therefore unsuitable for certification |
| C | Every transferred part retains the advance's one project; recipient cannot spend on another project | Return retains project designation until approved discharge or release/reissue; changing holder is not project clearance | Reassigning the whole advance FK to permit B would misleadingly reclassify A's past spending; split/reissue must preserve history |
| D | Transfer explicitly identifies and reserves slices; confirmation carries each project to recipient; rejection restores each slice | Return carries the slice upstream; separate authorized designation release or verified custody discharge is recorded | Dropping designation at transfer/return would make A appear reconciled while its cash remains held; conservation and paired movements prevent this |
| E | Balanced journal postings move custodian dimension and retain project/purpose dimension | Separate custody-return, designation-release and actual treasury-receipt postings | An incomplete journal migration can balance only part of the system while losing the original project/holder lineage; complete openings and event coverage are prerequisites |

In all viable options a physical money movement, a change in permitted use, and settlement of a liability are distinct events. No option permits a return to erase an employee claim or supplier debt automatically.

## 5. Recommended advance model

**RECOMMENDED; REQUIRES PRODUCT OWNER APPROVAL.** A project allocation is an authorized designation inside existing custody, not new cash, a project bank account, a liability, or a replacement for `user_advance_balances`.

- Identify each slice by tenant, user-advance balance and project, with one explicit unassigned component per balance. Currency is inherited from the advance and enforced on every effect. Do not store an independent currency that can disagree with it.
- Track available and reserved positions, plus immutable allocated-in, released-out, consumed, restored, transferred and returned movements. “Allocation amount” is cumulative authorization history; it is not a second spendable balance.
- At each committed revision: sum of slice available equals balance available; sum of slice reserved equals balance reserved, including the unassigned component. A slice's opening + inflows + restorations must equal consumption + outward transfers/returns + designation releases + approved adjustment out + current available/reserved. Designation transfers have paired entries, so assigning/releasing does not alter company custody totals. Never sum historic gross transfers to obtain current holdings.
- Expense creation reserves the selected project slice; approval consumes it; rejection releases the reservation. Multiple balances may fund one project expense, in the same currency. A company expense must use the unassigned component. Project spending from an unassigned component first needs explicit designation in the same transaction under an approved allocation authority.
- Holder transfer reserves identified slices. Confirmation debits sender and credits recipient with the same project designations; rejection restores sender. Each transfer allocation needs slice-level effect references. Changing recipient does not change project intent. A multi-project transfer locks every affected project and balance.
- A balance return preserves designation while money moves upstream. It does not reconcile A merely because a different employee holds A's 4,000. Separately approved release can move that 4,000 into company-unassigned custody after holder reconciliation, or a real verified funding/cashbox receipt can discharge custody when that workflow exists. Do not fake a cashbox receipt to unblock closure.
- Cross-project reallocation is an explicit paired A-release/B-assignment with actor, approver, reason and versions. Only available funds move; reserved or disputed money cannot be reallocated. Neither affected project may be financially closed. Reallocation does not rewrite original expenses or source restrictions.
- Refund/return effects restore the original funding slice where appropriate; a supplier refund received as a new funding source is a different effect, not an automatic duplicate restoration to custody. If an original project is closed, reopen before applying a financial correction.
- Before releasing the last project designation, reconcile declared custody and all project effects. Shortages/surpluses require approved attributed resolution; shared unresolved discrepancies block affected projects until evidence assigns responsibility. Do not charge the entire discrepancy to every project or divide it by expense ratio.
- Allocation/reallocation authority: recommend Accountant/Deputy preparation with independent Manager approval, while existing holder confirmation authority remains unchanged. Immediate spend authorization can be preapproved within an allocation; it does not need a new approval for each expense beyond existing expense policy. [D01, D11]

### Legacy cutover

Pause affected custody writes during opening reconciliation or use a controlled revision boundary. Inventory balances, pending transfers, expenses/claims/debts with absent or inconsistent project links, manager claims, and unallocated credits. Obtain holder declarations and authorized classification; reconcile totals to current authoritative balances. Record evidence, effective date, opening revision and uncertainty explicitly. Preserve original ledger rows. Historical expenses may supply evidence but never automatically assign remaining money. Unknown legacy attribution keeps affected project readiness indeterminate. Reviewed company-only custody can be excluded only by explicit accountable classification. Do not block every unrelated project because some company custody exists; identify and certify the scope of unresolved legacy exposure. [D10]

## 6. Settlement definition

**Proposed exact definition:** A project is financially settled at financial revision R and policy version P when an independent authorized approver has accepted a persisted reconciliation cycle for R/P, every mandatory predicate below passes on complete authoritative data, and that cycle has not been superseded by a material financial or evidence change. Settlement does not itself freeze future legitimate adjustments; such changes invalidate its currency and require a new cycle.

`canSettle=true` would mean the complete data and predicates permit submission/approval, not that approval has already occurred. Preserve separate certification status. A known failed predicate yields false; an unresolved mandatory evaluation yields null when there is no known failure. Missing visibility never becomes a pass. For restricted readers retain scope-limited uncertainty; do not reveal hidden global conclusions through counts or booleans. The command performs full evaluation only for authorized certification actions. [D03]

Classification is multi-dimensional: **Required** is the recommended predicate; **Optional policy** is an explicit owner choice; **Currently unverifiable** means current data/workflows cannot prove it; **Requires schema change** and **Requires business approval** identify prerequisites. All new certification conditions require business approval, even when the current reader already detects the underlying record.

| Condition | Proposed machine-testable rule | Classification / current limitation |
|---|---|---|
| Operational completion | Status Completed with completion evidence, or separately approved Cancelled run-off basis (section 11) | Required; business approval; completion workflow currently absent |
| Draft expenses | Count of project Draft expenses = 0 | Required; available data/code |
| PendingReview expenses | Count = 0 | Required; available data/code |
| CorrectionRequired expenses | Count = 0 | Required; available data; cannot skip because payment occurred |
| Required expense evidence | Every required expense evidence item satisfies the pinned policy and verification, or an approved scoped exception exists | Required; currently unverifiable beyond metadata; evidence workflow/schema for exceptions |
| Personal claims | Every attributable claim is resolved with authoritative outstanding = 0 and consistent final status/effect history | Required; payment/adjustment workflows partly deferred |
| Manager contribution claims | Same rule for explicitly attributable repayable claims | Required for claims; **not required** to repay all manager contributions; D09 classification |
| Supplier debt | Every project debt has authoritative outstanding = 0 and valid terminal/resolved state | Required; no cross-credit netting across debts without posted allocation |
| Supplier payments | No non-final payment allocation belonging to project; unknown status fails evaluation | Required; use allocation amount, not shared header total |
| Reimbursement payments | No non-final claim-payment allocation belonging to project | Required; schema exists, command workflow deferred |
| Credit operations | No unresolved project-intended credit operation; posted allocations reconcile to debt effects | Required; explicit intent needs schema. Approved unallocated company credit is **not required** to be exhausted |
| Expense returns | No pending project return; every approved return has reconciled financial effects | Required; complete effects currently unverifiable; schema links needed |
| Supplier refunds | Every project-required refund is confirmed with correct funding/fees/effect links, or valid cancelled/reversed resolution | Required; receipt workflow deferred; “refund row exists” is insufficient |
| Owner refunds | No pending or unreconciled attributable owner refund | Required; currency/effect semantics need schema and D08 |
| Contract changes | No pending project contract change; approved changes reconcile to final owner position | Required; existing fields, workflow and currency rule missing |
| Project custody | Every project slice has available = 0 and reserved = 0 after approved consumption, verified return/discharge or approved designation release; movement conservation passes | Required; currently unverifiable; attribution schema needed |
| Transfers/returns | No Draft/PendingConfirmation/CorrectionRequired project-attributed transfer/return; confirmed/rejected effects reconcile | Required; project split links missing |
| Advance settlement differences | No unresolved difference attributable to project; ambiguous shared differences remain unresolved until investigated | Required; project-specific resolution links needed. Whole advance need not close if other projects still hold money |
| Original-paper custody | Every paper item required by policy is received/verified or has an approved documented exception | Optional policy: recommend require where paper is required by company contract/process; currently unverifiable; schema needed if adopted |
| Unknown states/data integrity | No unknown financial lifecycle, missing computed amount, inconsistent project link, unexplained ledger/balance difference or orphan project-intended record | Required; data remediation and full evaluator beyond current filter queries |
| Approved-return gap | For each resolution type, expected effect amount equals linked posted effect amount with correct currency/direction and no duplicate application | Required; currently unverifiable for all types; no blanket “Approved is enough” |
| Final owner reconciliation | Approved schedule reconciles contract changes, attributable receipts/refunds and final owner obligations, with no unresolved project receivable/payable/retention/dispute under selected policy | Recommend Required for final settlement; currently unverifiable; D08/schema |
| Evidence completeness/cutoff | Signed scope checklist covers all project costs/obligations, late invoices, subcontractor retention and legacy/unlinked records as of cutoff; no unresolved items | Required; new certification evidence. Software cannot prove an unrecorded real-world invoice does not exist |
| All company advances closed | No requirement beyond project slices and implicated shared differences | Not required; would unnecessarily block A on unrelated B custody |
| Company-wide cash/profit equals zero | No such rule | Not required; costs, financing, custody and profit are different concepts |

Zero means exact zero in the supported money precision after valid posted resolution. Tolerance may flag investigation thresholds; it must not turn a nonzero liability into zero. Any write-off requires its own approved authority, posted adjustment and retained reason. A terminal label with contradictory outstanding amount is an integrity failure; future certification must inspect more than the current reader's non-terminal filters. [D15]

## 7. Closure definition

**Proposed exact definition:** Financial closure is an authorized Manager's durable certification of a current approved settlement cycle, after authoritative in-transaction re-evaluation, coupled atomically with a lock on ordinary project financial changes. It is an operational certificate at a point in time, not a guarantee against later discovery of error or a declaration that the company holds no cash.

Additional closure predicates:

1. Completed basis or approved cancelled run-off basis; valid operational evidence.
2. Latest approved settlement exists, with complete scope, zero blockers/gaps under the selected policy, and no material changes since approval.
3. Expected project version, financial revision, settlement version, and policy/evidence revision match current values. The timestamp of an old GET is not authority.
4. Full evaluation passes again under the shared writer gate. All required owner reconciliation and final documentation attestations are current.
5. Active tenant membership and closure permission are revalidated; preparer/submitter differs from approver. The approving Manager may also close the unchanged packet; this does not require three people. A Manager who prepared it cannot approve/close it under the default policy.
6. Persist closure actor, UTC time, basis, reason/comment, settlement ID/revision, exact snapshot/hash/schema/policy versions and project pre/post versions. Preserve identity/role at the time plus stable actor reference.
7. Persist the active financial lock and immutable audit/certification event in the same transaction as status and idempotency result.

**Current schema is insufficient.** Generic `audit_logs` can record an event but do not provide a typed certificate, complete preserved evidence or enforce one active closure. `projects.CompletedAt` is a physical-completion timestamp, not a replacement for financial closure evidence. Advance closure has the wrong ownership and currency scope. [E2, E5, E12; D04]

### Snapshot options

| Option | Audit/reproduction and disputes | Concurrent change and reopening | Reporting/cost |
|---|---|---|---|
| A. Live calculation only | Cannot reconstruct what a user certified after records/settings change | A fresh query helps current readiness, not historical evidence; reopening has no prior certificate | Lowest storage, weak certification/reporting |
| B. Settlement header plus totals | Records who/when, but identical totals can hide different records or policies | Can be revision-bound; weak explanation of changes between cycles | Moderate cost; insufficient dispute detail |
| C. Record plus detailed category/evidence snapshot | Preserves evaluated records, versions, meanings, exceptions, evidence and policy for review | Bind to financial revision; later changes supersede, never overwrite certified snapshot | Recommended balance of cost, reportability and traceability |
| D. Certification entirely from event/ledger replay | Strong only if every relevant financial, policy and evidence event is complete and replayable | Can support as-of reconstruction; current ledgers are incomplete for this purpose | Highest conversion cost; not justified for this milestone |

**Recommend C.** Retain live calculation for preview; create immutable snapshot on submission, then approve only that unchanged snapshot after re-evaluation. Rejected/stale submissions remain historical. A hash detects content mismatch but is not a legal signature, independent proof of truth or replacement for the actual snapshot. [D05]

Conceptual fields: `project_settlement_id`, `company_id`, `project_id`, cycle number, previous-cycle ID, normal/cancelled basis, workflow status, project version, financial revision, policy/evaluator/schema version, evaluated/submitted/approved times and actors, blocker/gap counts, required-evidence checklist, notes, snapshot/hash, record version. Detail contains category/evaluation status, currency, amount meaning, exact amount, referenced record ID/version/status, relevant evaluated values, resolution/evidence links and exceptions. Store only necessary safe evidence; no account credentials, raw request secrets or ephemeral file URLs. Approved snapshots are immutable; status progression is documented by immutable events and versioned current-state projection.

## 8. Authorization proposal

Every future write cell below requires product-owner approval; **none follows automatically from existing read permission**. “Tenant” means active authenticated membership plus company-scoped queries, never a client company ID. [D11]

| Role | View settlement now | Prepare (proposed) | Submit (proposed) | Approve (proposed) | Financially close (proposed) | Reopen (proposed) | Historical view (proposed) |
|---|---|---|---|---|---|---|---|
| Manager | Tenant, including owner operations | Yes, but cannot approve own preparation | Yes, independent approver then required | Yes, independent | Yes, independent of preparer/submitter | Yes, reason and control review | Tenant full authorized certificate |
| Deputy | Tenant, owner operations hidden | Yes, visible scope only | Yes, partial packet for Manager completion | No | No | No; request correction | Tenant redacted; no hidden owner details |
| Accountant | Tenant, owner operations hidden | Yes, visible scope only | Yes, partial packet for Manager completion | No | No | No; request correction | Tenant redacted; no hidden owner details |
| Supervisor | Assigned project, expense/document/debt scope | No financial packet; physical-completion acknowledgment only | No | No | No | No; report issue | Assigned project redacted summary only |
| Worker / unknown | No | No | No | No | No | No | No project certificate access |

Manager supplies a separately identified owner-reconciliation attestation; it cannot silently appear as Accountant-verified evidence. Require independent review of the financial packet. For a conflict involving a Manager's personal claim, require another authorized, non-beneficiary Manager/reviewer under an expressly approved conflict policy; otherwise block certification. Do not solve this by exposing all owner data to every Accountant.

**Alternative:** formally delegated Deputy closure with explicit project/amount/time scope, revocation and independent Manager approval of the mandate. This needs permission/evidence persistence and disclosure rules; do not inherit it from Deputy project reads. Another alternative permits two authorized Managers to divide preparation and approval. A small company lacking a second qualified person must onboard an eligible reviewer or remain read-only for certification. A self-approval exception is a separate high-risk decision, disabled by recommendation—not an automatic consequence of `allow_self_approval` or `closure_requires_approval=false` settings.

Historical access uses **current** authorization, with the certificate's role-limited projection; past assignment is not permanent access. Full snapshots stay server-side. A generic “approved” badge must not disclose hidden obligations or owner balances to restricted readers. Revalidate company/user status and authorization under the command's synchronization protocol, including relevant permission revocation paths.

## 9. Post-closure behavior

An active closure certificate is the financial gate, including when operational status remains Cancelled. Do not rely exclusively on `projects.status`. Every existing and future API, job, import and administrative path must participate before closure is enabled. [D12]

| Action | Recommended classification after closure | Reason/control |
|---|---|---|
| New expense | Allowed only through correction/reopen workflow | Ordinary create is blocked; new cost invalidates certification |
| Expense amount/project/payment-mode edit, approval, rejection or reversal | Allowed only through correction/reopen workflow | Ordinary edit/review is blocked; affects costs, custody or liabilities |
| New reimbursement/claim, payment, write-off or adjustment | Allowed only through correction/reopen workflow | Includes indirectly project-linked records |
| Supplier invoice/debt changes | Allowed only through correction/reopen workflow | Creating via supplier API must not bypass project gate |
| Supplier payment allocation/confirmation/rejection involving project | Allowed only through correction/reopen workflow | Check all project allocations in shared payment; unrelated projects may proceed in a separate transaction |
| Credit-note allocation/approval with project intent | Allowed only through correction/reopen workflow | Company-only unused credit remains outside project; do not reassign intent to evade lock |
| Expense/advance returns affecting project | Allowed only through correction/reopen workflow | Retain request/evidence pending reopen; movement can change a certified position |
| Supplier/owner refunds, including discovered late recoveries | Allowed only through correction/reopen workflow | No financial posting merely because recovery is beneficial |
| Contract value/change or owner payment allocation | Allowed only through correction/reopen workflow | Final owner position and denomination are certified |
| Advance assignment/reallocation/transfer affecting project slice | Allowed only through correction/reopen workflow | Whole shared advance can still operate on unrelated/unassigned slices |
| New supplemental document metadata for historical reference | Allowed, with authorization and append-only audit | Label added after certificate; no change to the certified evidence checklist or existing metadata |
| Binary document attachment for historical reference | Allowed through verified storage and append-only reference | Actor/time/purpose recorded; no overwrite of certified bytes; binary storage remains an unimplemented prerequisite today |
| Document rejection, replacement, exception, or evidence change affecting prior conclusion | Allowed only through correction/reopen workflow | A purported “attachment correction” cannot silently change a certified predicate |
| Administrative name/contact/site typo | Allowed through narrow authorized correction with before/after audit | Does not alter original certificate snapshot. Owner identity, project reassignment, dates of certification and financial terms are material, not clerical |
| Read/export/history | Allowed under current permissions | Redact hidden categories; preserve old cycles |
| Delete/rewrite certified snapshot, ledger history or closure actor/time/reason; direct status unlock | Blocked, including through reopen | Reopening appends a new decision; it does not erase or backdate the old one |

Current project service rejects terminal metadata edits. The narrow correction path above is a **proposed separate command**, not a description of current behavior. FinanciallyClosed must not silently become editable through a general PATCH.

## 10. Reopening/correction policy

**Recommend a minimal audited reopen in the same production release as closure.** Deferring it would trap legitimate late invoices/refunds or encourage destructive database edits. A richer reversal interface can follow, but a project that cannot safely be corrected must not be closed irreversibly by the first release. [D13]

Manager authorizes reopening with required reason, evidence/reference, expected closure/cycle/project/financial versions and idempotency key. Preserve the original certificate and every ledger entry. Append a reopening event referencing it; invalidate the active financial lock and open a new numbered settlement cycle atomically. Normal project status returns to Completed; cancelled project stays Cancelled. Increment project/control versions and financial revision. Notify/report that prior certification is superseded, without deleting it. No automatic reopening to Active and no reversal of old transactions merely because the project reopened.

Correct financial records through their own approved append-only adjustment/reversal workflows, linked to the correction cycle. Re-evaluate and independently approve before reclosure. A second concurrent reopen must replay the same operation or conflict, never create two open cycles. The default permits Manager authorization of reopen without a second approval so evidence can be investigated; no financial write becomes valid merely because it was requested. Alternative: two-person reopen approval for high-risk tenants, accepting delay to corrections. Emergency unlock without audit is rejected.

## 11. Cancellation behavior

Cancellation stops planned work; it does not delete supplier debt, employee/manager claims, returned-goods obligations, owner refunds or unused custody. Recommend a **cancelled run-off settlement basis** with the same financial predicates, replacing physical-completion evidence with approved termination evidence and final owner/contract reconciliation. Necessary payments, claims and recovery operations continue under existing roles plus approved run-off controls; new work commitments require explicit authority, not routine expense entry. [D14]

Do not mark a cancelled project Completed just to satisfy a closure CHECK. Keep `projects.status=Cancelled` and record closed/open financial state in the project financial-control/certificate relationship. Normal completed projects may still project FinanciallyClosed in the existing status field. The resulting two-dimensional representation must be explicit in APIs/UI and constraints. Alternative: replace the mixed project status with separate operational/financial statuses across the product; cleaner long-term, larger migration, not the default minimum change.

Existing `SettlementRules` deliberately reports ProjectCancelled as a blocker. Supporting run-off settlement would be a future approved business-rule change. Until implemented, cancelled projects remain diagnostically blocked, not deemed clear.

## 12. Concurrency model

**Recommendation: project-wide financial write gate + authoritative re-evaluation + monotonic financial revision + atomic certificate/idempotency persistence.** This is a design requirement for all writers, not merely the close endpoint. [D12]

Why existing mechanisms are insufficient: project version does not change when an expense is inserted; child row locks do not lock absent future rows; a GET snapshot is not a lock; idempotency prevents duplicate commands, not stale eligibility. PostgreSQL Repeatable Read gives a stable snapshot but still permits serialization anomalies. A transaction started before a competing commit can evaluate an older view. Serializable can detect anomalies but requires transaction-failure handling and does not replace a rule forbidding financial writes after closure. See [PostgreSQL isolation](https://www.postgresql.org/docs/18/transaction-iso.html).

**Locking the project row only in the close command is insufficient.** A financial writer that neither takes the common gate nor rechecks closed state can still commit a child change outside this protocol. Incidental foreign-key locking is not a closure policy and does not cover updates to existing child rows. Either every writer participates in the common project protocol, or equivalent database-enforced restrictions are needed; until coverage is proven, do not enable closure.

### Proposed command protocol

1. Authenticate and derive tenant/actor server-side. Start one write transaction. Validate/claim the tenant idempotency key with actor/operation/payload fingerprint. Replays require current permitted access and return the stored original result, not another transition.
2. Acquire the stable tenant policy/security gate, then lock all affected project financial-control rows in deterministic project-ID order, before financial child locks. The existing company row can anchor the tenant gate: ordinary commands take a shared lock; settings/membership/role changes must first take the corresponding exclusive lock. No extra tenant-gate table is necessary. This proposed protocol prevents review rules or authority from changing midway; current authorization checks alone do not implement it. Establish a common order across services, not a special order only for closure.
3. Recommend Read Committed for this explicit-gate protocol: after waiting for locks, re-read authoritative state. Locking participants prevent relevant writes during evaluation. If using Repeatable Read/Serializable instead, restart the entire transaction on serialization failure and re-evaluate; never continue with an old snapshot or just retry the final update. Bounded server retries, if introduced, require explicit design and idempotent whole-transaction behavior; current client writes remain non-automatic.
4. Revalidate active membership, role, policy version, project/cycle versions and expected financial revision. Validate current operational/financial state. Re-evaluate every mandatory category from authoritative current data and reject certification if any blocker or required evaluation gap remains. Full authoritative evaluation must include hidden categories; partial preview data cannot be submitted as authority.
5. Validate approved immutable snapshot identity and compare every material dependency/revision. If changed, return a safe conflict/new evaluation and require a new approval cycle even if totals happen to match. Do not silently approve a different record set.
6. Persist certification snapshot/event, actor/time/reason, active closure state, project transition, version changes, audit and completed safe idempotency response in that one transaction. Commit once. No external file/network call while holding locks; use previously verified immutable evidence references.

### Writer coverage and the stale-page race

Every material create/update/review/allocation/confirmation/reversal, evidence decision, owner operation and attribution movement must take the same gate, reject closed state, update the financial revision, and invalidate current approval as appropriate. This includes changes whose project is reached through expense, debt, claim, return, funding-intent or shared payment relations. Derive the complete set server-side, lock projects in sorted order, then lock/recheck child relationships; if the set changes during discovery, roll back and retry discovery rather than acquiring out-of-order locks. Reparenting locks both old and new projects. Shared unassigned funding/balances need their own locks; their project effects still require the affected project gates. No direct database writer may bypass this contract.

If expense creation obtains A's gate first, it commits and advances the revision; the closer sees a conflict or blocker. If closure obtains it first, creation waits and then rejects the closed state. An insert into an empty category cannot slip between evaluation and certification because its writer also needs the gate. A status-only check without a common gate has a time-of-check race.

Row locks on a dedicated tenant/project financial-control row are preferred to advisory locks: they provide an explicit persistent anchor. Transaction-scoped advisory locks are a possible alternative only with a collision-safe shared key convention and universal participation. Advisory locks have application-defined meaning; they are not automatic enforcement. PostgreSQL documents row/advisory lock behavior and deadlock risks in [Explicit Locking](https://www.postgresql.org/docs/18/explicit-locking.html). Restrict database write roles; if operational imports/direct writers cannot comply, add reviewed database enforcement or keep closure disabled.

Required race tests use real PostgreSQL and two transactions: close versus new expense/debt, pending payment confirmation, credit allocation, slice transfer, refund, owner change, document rejection, policy change and role revocation; inverse ordering; two closers; stale approval with unchanged totals; shared multi-project deadlock ordering; retry after timeout; idempotency mismatch; rollback leaves no partial certificate. In-memory tests alone cannot validate this protocol.

## 13. Multi-currency rules

Certify a collection of independent currency positions, never one overall total. Each summary row carries currency and amount meaning, such as outstanding debt, unpaid claims, custody available/reserved, pending operation allocations, costs, receipts or owner balance. Do not add overlapping categories even within the same currency. No exchange-rate, conversion, profit or netting assumption is introduced. [D08, D15]

Use C# decimal/PostgreSQL NUMERIC and exact Flutter decimal text; preserve the current supported two-decimal financial precision. A positive LYD position cannot offset negative USD, and a claim cannot be netted against custody without a separately authorized posted transaction. Enforce same-currency effect links and reconciliation per currency; null currency means unresolved, never the current company default by assumption.

Projects/contract changes/owner refunds lack explicit currency fields. Owner payment's source supplies a currency, but a project may receive several currencies and an owner refund can have optional payment linkage. Recommend an explicit immutable contract denomination (single contract currency initially), explicit refund currency with same-currency funding allocations, and an approved owner reconciliation per currency. Cross-currency receipts must remain separately unresolved or have a separately approved future contract rule; no exchange-rate feature in this rollout. Existing ambiguous records require reviewed backfill and evidence, not today's default currency. Interest, taxes, retention and final owner balance formula must be specified by the owner; contract value minus funding receipts alone is not an approved final receivable rule.

## 14. Schema impact

Conceptual only: **no SQL, migration, generated-model edit or database change is authorized by this document**. “Necessary” means necessary for the recommended policy, not for every alternative. Approve concrete constraints and a Database-First change plan separately. All tenant-owned parent/child references require same-company keys; monetary effects cannot cross tenant/currency boundaries.

| Classification / proposed persistence | Purpose and authoritative owner | Tenant/project/currency | Concurrency and audit |
|---|---|---|---|
| Necessary: project custody slices and movement/effect references | Advance/custody domain owns designation within balances; links expense/transfer/return/resolution effects | Company + balance + project; explicit unassigned component; currency inherited from advance | Unique current slice, nonnegative positions, conservation, optimistic version and balance locks; immutable paired movements with actor/reason/correlation; reversals reference originals |
| Necessary: project financial-control row | Project financial lifecycle owns open/closed state, active cycle/closure and monotonic financial revision | Unique company/project; no monetary total | Stable locking anchor, version, constrained active certificate; all relevant writers participate; normal/cancelled basis retained |
| Necessary: project settlement cycles + detailed snapshot | Settlement domain owns certification packet, category evidence and policy binding | Company/project/cycle; per-category/per-currency rows or versioned canonical snapshot | Immutable submitted content; versioned projection/events; one active workflow cycle; previous-cycle reference; exact content retained with hash |
| Necessary: closure/reopening events or typed certification table | Project financial lifecycle owns who approved/closed/reopened, when, why and which snapshot | Company/project/settlement; actor references plus historical identity; no cross-currency total | Append-only events, one effective closure; closure FK and sequence uniqueness; audit retained independently of generic log cleanup |
| Necessary: credit project-intent allocation/classification | Payables domain owns pending intent; company-only classification also recorded | Company/credit/project, same credit currency; amount cannot overassign header | Versioned pending intent, immutable history once certified; allocation must consume or reconcile intent, not count as extra liability |
| Necessary: return and difference resolution effect links | Expenses/custody/payables own exact resolution to refund, credit, claim or restoration | Company, project inherited through origin; exact currency/amount | Prevent duplicate effect and excess settlement; version/references to original resolution; explicit treatment of Other required |
| Necessary: contract/refund denomination + owner reconciliation evidence | Project/owner-finance domain owns currency, final position, approved retention/dispute treatment | Company/project/owner; contract denomination and per-currency reconciliation | Versioned contract/receipt/refund lineage; no guessed historical currency; approved schedule captured in snapshot |
| Necessary: policy and cutover evidence within certification | Settlement domain binds approved policy, evaluator version, settings/category versions and migration scope | Company/project/cycle; affected currencies explicit | Preserve full applied rules and opening attestations, not only mutable FK IDs. Separate table unnecessary if typed snapshot is sufficient |
| Recommended: operational completion/termination acknowledgment | Project operations owns declaration, Supervisor acknowledgment, Manager decision | Company/project/actor; nonmonetary | Versioned event and date/evidence; reuse existing dates, do not invent completion from zero expenses |
| Necessary if paper policy adopted: original custody/exception events | Document domain owns receipt, custodian, verification and approved exception | Company + document + affected project; nonmonetary | Append-only chain and independent exception approval; supplemental attachment cannot overwrite certificate evidence |
| Recommended: durable policy version records and evidence-retention controls | Company policy/document domains own policy changes and retention | Tenant-scoped; certificate references exact version | Locks invalidate affected approvals; retention cannot delete evidence still needed by a certificate |
| Optional: electronic signature provider references / trusted timestamp | Certification owner, if contractual assurance demands it | Tenant/project/certificate; nonmonetary | Bind signature to canonical snapshot; hashes alone are not signatures; separate product/legal decision |
| Optional: delegated approval mandate | Access domain, only if delegated Deputy closure is selected | Company/user/project or explicit approved scope | Expiry/revocation/version; immutable mandate history and conflict-of-interest restrictions |
| Avoid: project FK on funding source as custody shortcut; copying advance closure to project; full event-store rewrite; combined currency total; redundant derived balances without invariant ownership | Misstates ownership or creates conflicting authorities | — | Increases audit ambiguity and migration risk |

Existing expense/debt/claim project links, financial ledgers, per-record versions, idempotency storage and audit infrastructure can be reused. No duplicate payable ledger or standalone invoice table is needed. Existing document metadata fields support references but do not implement verified storage or paper custody. Existing partial unique indexes and generated singular navigations require careful querying and cannot be repurposed blindly for multiple historical project cycles.

## 15. Decision matrix

Every row is **RECOMMENDED, NOT APPROVED**. Owner response should be Approve / Choose alternative / Amend / Defer, with reason. “Defer” keeps the dependent certification capability disabled.

| ID / decision | Recommended choice | Alternative | Impact | Schema change | Must owner approve? |
|---|---|---|---|---|---|
| D01 Attribution and unused funds | D: audited balance/project slices; approved release preserves company custody | A narrower costs/payables certificate; C dedicated advances; E separate accounting program; reject B | Shared-site flexibility with explicit accountability | Yes for D | Yes |
| D02 Operational lifecycle | Completion/termination evidence + separate settlement cycles; no FinanciallySettling status | Full operational/financial status split | Completed can still receive valid late financial activity | Evidence/cycle persistence | Yes |
| D03 Settlement eligibility | All section 6 required predicates, complete scope, independent approval | Narrower scope explicitly named; never silent exclusions | More prerequisites, defensible certification | Yes | Yes |
| D04 Closure meaning | Manager certification + current snapshot + enforced financial lock | No closure release yet | Prevents status-only irreversible action | Yes | Yes |
| D05 Snapshot/retention | Detailed immutable C; preserved policies/evidence and safe historical projections | Header only (weaker); full event replay (larger) | Reproducibility and dispute support | Yes | Yes |
| D06 Documents/paper | Verified required digital evidence; paper custody where required; explicit approved exceptions | Digital-only with explicit scope; stricter all-originals | Operational collection effort; missing policy cannot mean pass | Paper/exception schema conditional | Yes |
| D07 Return/difference reconciliation | Exact linked effects for every resolution; no approval-label shortcut | Keep affected projects indeterminate | Blocks double restoration or unresolved recoveries | Yes | Yes |
| D08 Owner final position/currency | Explicit denomination and final per-currency owner schedule; no unresolved retention/dispute | Separate receivables phase with narrower certificate label | May delay final closure until owner position settles | Yes | Yes |
| D09 Manager contributions | Explicit source-vs-liability classification; only established project claims require resolution | Approved loan/capital policy with new evidence | Avoids invented repayment obligation | Conditional evidence | Yes |
| D10 Legacy migration/completeness | Reviewed opening slices and scope attestation; no proportional inference | Legacy projects stay uncertifiable | Manual reconciliation cost and honest uncertainty | Cutover evidence | Yes |
| D11 Authority/separation | Accountant/Deputy prepare; independent Manager approves/closes; Supervisor physical acknowledgment; conflict-of-interest gate; no default self-approval | Audited scoped Deputy mandate or independently reviewed exception | Two-person availability and limited-data preparation | Approval/mandate evidence | Yes |
| D12 Concurrency/post-close | Universal write gate, financial revision, command re-evaluation; narrow append-only document/admin exceptions | Serializable plus equivalent writer coverage; no closure until coverage complete | Cross-service work, locking and performance cost | Control row/revision | Yes |
| D13 Reopening | Manager reasoned reopen, new cycle, preserved certificate; ship before closure activation | Two-person reopen approval | Safe correction without destructive rollback | Events/cycles | Yes |
| D14 Cancellation | Run-off settlement, preserve Cancelled, independent financial closed state | Broader two-axis status migration | Obligations survive; schema CHECK compatibility | Control/basis persistence | Yes |
| D15 Precision/zero/settings | Exact zero after posted resolution; no silent tolerance write-off; pinned policy | Explicit approved tolerance-resolution workflow | More review of small differences; no hidden forgiveness | Reuse amounts; retain policy | Yes |
| D16 Rollout boundary | Prerequisite resolution workflows, writer guards and reopen before enabling closure | Keep read-only diagnostic product | Longer safe rollout, no stranded closed projects | As above | Yes |

## 16. Required product-owner approvals

Approve each D01–D16 row explicitly; approval of this document's existence is not approval of its policies. A response can be `D01 Approve; D02 Amend: ...`. Record approver identity, date, selected alternative, conditions and effective policy version in a later approved decision record.

Concrete details still needing answers:

- D01: who authorizes allocation/reallocation/release; whether company pool release is permitted; how shared custody differences are assigned; whether owner-origin funds are contractually restricted (the current provenance link does not answer this).
- D02/D14: completion and cancellation signatories/evidence; permission for cancelled-project run-off costs; acceptance of Cancelled plus financial-closed state.
- D03/D08: treatment of retention, disputed/late invoices, owner receivables/refunds, contract currency, multi-currency receipts and final owner reconciliation formula. Recommendation is no unresolved position for a certificate labeled final; deferred retention needs an expressly narrower product scope.
- D05/D06: evidence checklist, original-paper applicability, exception authority, digital verification, certificate/evidence retention duration and whether electronic signatures are required. Do not copy generic audit-retention days without review.
- D07/D09/D15: permitted return/difference outcomes, claim/write-off authority, manager-loan versus capital classification, exact-zero policy and currency-specific threshold interpretation. No automatic forgiveness or foreign-exchange netting.
- D10: responsible cutover reviewers and treatment of legacy records whose project/currency cannot be proven.
- D11: approve every future action/role cell, independent-person rule, beneficiary conflict policy, whether delegation is allowed and what happens when no independent reviewer is available.
- D12/D13: approve the financial write restrictions, narrow administrative/supplemental-evidence exceptions, reopening authority and any second approval requirement.
- D16: approve prerequisite milestones and accept that true readiness/closure remain unavailable until their exit criteria pass.

**Implementation is blocked on these policy choices, not on permission to finish this documentation task.** No application rule or database proposal here is an instruction to execute it automatically.

## 17. Recommended implementation sequence

Each phase needs a separately scoped implementation request after policy approval. No migrations are written here.

| Phase | Goal and scope | Principal risks | Required tests/review | Exit criteria |
|---|---|---|---|---|
| A. Policy approval and deployed-schema verification | Resolve D01–D16; inspect live read-only catalog for constraints/triggers; confirm accounting scope and write-path inventory | Ambiguous retention/owner funds; schema drift; implicit approval | Product-owner/accounting review; trace every predicate and permission to a decision | Signed decision version; no unresolved mandatory rule; source/catalog differences documented |
| B. Detailed Database-First design and cutover rehearsal | Produce separate reviewed schema artifact, composite tenant keys, conservation/uniqueness controls and migration/backfill plan; apply only after explicit database authorization, then scaffold | Lossy legacy attribution; incompatible constraints; singular generated navigations | Backup/restore rehearsal; legacy mixed-currency and shared-advance fixtures; same-tenant FK/invariant checks; old/new read compatibility | Approved artifact and cutover evidence; generated models refreshed without manual edits; rollback strategy accepted |
| C. Attribution and missing resolution/evidence workflows | Implement slices, reservations/transfers/returns, credit intent, difference/return effects, claim/refund prerequisites, owner currency/final schedule and document/completion evidence | Double counting, stranded blockers, permissions broadened by convenience | Conservation/property tests, precision, all resolution types, negative/unknown/legacy cases, tenant/role boundaries | Every mandatory predicate has authoritative evidence and a safe resolution path; no inferred attribution |
| D. Universal financial gate and readiness revision | Inventory and update all material writers, policy/evidence change handling, aggregate revision and full evaluator; evolve read contract truthfully | Missed write path, leakage, deadlock/performance, false true | Real PostgreSQL races, shared-project allocations, missing-data fail-closed, masked read scopes, Flutter contract tests | Coverage demonstrated for every writer; canSettle true only for complete eligible authoritative evaluation, partial views remain safe |
| E. Settlement submission/approval and history | Immutable detailed snapshots, role-limited preparation, independent approval, stale-cycle invalidation and history | Self-approval/conflict, policy drift, incomplete evidence, data retention | Snapshot reproduction, hash/contents, wrong actor/tenant, stale revision with identical totals, safe historical redaction, idempotency | Approved cycles reproducible and invalidated by every material change; no close control yet |
| F. Closure plus minimal reopening/correction | Implement atomic certification, lock enforcement, normal/cancelled status projections and audited reopen/new-cycle behavior together; correct through approved financial workflows | Irreversible lock without recovery, status-only enforcement, duplicate certificates | Full race matrix, two closers/reopeners, retry/rollback, supplemental docs/admin exceptions, late invoice/refund and cancellation end-to-end | Closure and correction both operationally rehearsed; owner acceptance; no direct SQL escape hatch; enable by controlled rollout |
| G. Platform and operational release | Expose permitted Flutter actions only after backend gates; EN/AR history/evidence/status, staging acceptance and limited tenant rollout | UI suggests clearance outside scope; platform/security regressions | Backend/Flutter suites, Web/Android builds, iOS on macOS, real API role tests, live staging reconciliation and support rehearsal | Signed acceptance, monitored rollout and retained evidence; broader delegation/signature/reporting features remain separately scoped |

### Documentation-only verification for this review

Only this document, PROJECT_STATUS, SETTLEMENT and relevant README phase wording are intended to change. Run working/staged `git diff --check`, inspect status/stat and complete diff, and confirm the pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` is untouched. No C#, Dart, ARB, generated model, SQL or database change; no backend/Flutter builds or tests are required or claimed for this document.

**Exact next Codex task after decisions are approved:** “Record the owner's D01–D16 selections and produce the detailed Database-First schema and cutover design for the approved project custody, certification, evidence and concurrency model. Verify current PostgreSQL catalog metadata read-only, specify constraints and every writer's lock/revision obligations, and prepare migration/reconciliation acceptance fixtures. Do not execute DDL, re-scaffold, or implement commands until the concrete schema artifact and cutover plan receive separate approval.”
