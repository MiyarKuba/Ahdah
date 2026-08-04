# Project status

## Project

Ahdah — عُهدة

## Current phase

Flutter Expenses and Reimbursements

## Date, workspace, and branch

- Date: `2026-08-04`.
- Workspace confirmed before edits: `C:\dev\Ahdah`.
- Branch confirmed before edits: `feat/flutter-expenses-reimbursements`.
- The pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` was left untouched and unstaged.

## Backend contracts consumed

- Categories: `GET/POST /api/v1/expense-categories`.
- Expenses: paged list, detail, creation, allocations, metadata attachments, history, approval, and rejection under `/api/v1/expenses`.
- Reimbursements: paged list and detail under `/api/v1/reimbursements`.
- Exact request/response fields, filters, validation lengths, lifecycle values, roles, Problem Details codes, pagination, and idempotency requirements were taken from controllers, contracts, response models, constants, service interface, rules tests, API tests, and OpenAPI verification.
- A genuine contract blocker was corrected: `AdvanceBalanceSummary` now exposes the existing safe `userAdvanceBalanceId` required by `CreateExpenseRequest.advanceAllocations`. Its infrastructure projection and OpenAPI assertion were updated. No persistence or database change was made.

## Flutter architecture and routes

- `features/expenses` contains handwritten immutable models, exact request objects, repository interface/API adapter, focused paged read controllers, focused create/review command controllers, and responsive presentation pages.
- Stable guarded routes are `/expenses`, `/expenses/new`, `/expenses/:expenseId`, `/expenses/:expenseId/history`, `/expenses/:expenseId/documents`, `/expenses/:expenseId/review`, `/expense-categories`, `/expense-categories/new`, `/reimbursements`, and `/reimbursements/:reimbursementId`.
- Expenses is present in the authenticated responsive `NavigationBar`/`NavigationRail` for all five known roles. Unknown roles retain Home and Account only.

## Role capabilities

| Role | Expense visibility | Create | Review | Category create | Project link | Reimbursements |
|---|---|---:|---:|---:|---:|---:|
| Manager | All company | Yes | Yes | Yes | Yes | All company |
| Deputy | All company | Yes | Yes | No | Yes | All company |
| Accountant | All company | No | Yes | No | No | All company |
| Supervisor | Own and assigned-project | Yes | No | No | Assigned only | Own |
| Worker | Own | Yes | No | No | No selector | Own |
| Unknown | None | No | No | No | No | None |

Broad Flutter capabilities guard navigation and routes, while loaded status, actor/project relationship, separation rules, and the backend remain authoritative.

## Implemented behavior

- Category list/filter and Manager-only creation with exact group/scope/options, normalized optional code, validation, duplicate-submit prevention, and no delete/update action.
- Expense list with server-side exact status/payment/reference filters, 2–50 character bounded reference search, page size 20, refresh, stable deduplication, and recoverable load-more failure.
- Expense detail with reference, amount/currency, category, status, payment mode, payer/submitter, one optional direct project, dates, receipt/invoice numbers, allocations, returned items, metadata documents, optional reimbursement, review information, and safe notes.
- Expense creation with exact positive `NUMERIC(18,2)`-compatible decimal input, one optional direct project, exact category scope behavior, and only `AdvanceBalance`/`PersonalFunds`.
- Multiple unique authoritative personal balance allocations with one currency and exact integer-minor-unit equality. No client balance reconstruction or optimistic subtraction.
- `PersonalFunds` clearly creates an unpaid reimbursement liability and sends no allocation or claim status.
- Receipt/invoice state is inferred only from actual numbers and document metadata. No separate receipt-state enum was invented.
- Document metadata list is read-only. The POST requires a storage-produced application-relative path and SHA-256 checksum unavailable without binary storage; Flutter does not fabricate these values or add a picker.
- Manager/Deputy/Accountant review of `PendingReview` expenses with explicit approval/rejection confirmation, authoritative version, required rejection reason, and stale conflict handling through existing Problem Details behavior.
- Read-only paged reimbursement list/detail and read-only paged audit history. No claim payment, raw audit JSON, ledger IDs, storage paths, or checksums are displayed.

## Decimal and idempotency safety

- Financial requests contain no Dart `double`; validated canonical decimal text is emitted as an exact JSON number token.
- Exact allocation comparison reuses `DecimalMoney` and `BigInt` minor units. Mixed currencies are rejected locally and remain server-authoritative.
- Expense creation, approval, and rejection reuse `FinancialOperationKeyFactory` and `FinancialOperationKeySession`: 32 secure random bytes, unpadded URL-safe Base64, controller-memory only.
- No automatic write retry exists. Network/timeout outcomes are explicitly uncertain; only Retry Same Operation reuses the identical payload/key. Changed payload, success, definitive failure, cancellation, and disposal clear the pending operation.
- Category creation and all read requests send no idempotency header. Document metadata creation is not exposed.

## Localization and platforms

- Arabic remains default RTL and English remains LTR.
- Expense statuses, payment modes, category groups/scopes, document types/states, reimbursement states, forms, errors, confirmations, warnings, audit events, and constrained-visibility empty states are localized.
- The shared SafeArea-based shell and responsive pages support mobile cards, wider constrained content, browser URLs/back navigation, text scaling, keyboard-safe forms, semantic money labels, and text-based status communication.

## Verification

- `dotnet restore backend\Ahdah.sln`: passed; all projects up to date.
- `dotnet build backend\Ahdah.sln --no-restore`: passed with 0 warnings and 0 errors.
- `dotnet test backend\Ahdah.sln --no-restore --no-build`: passed 220 total, 0 failed, 0 skipped (128 unit, 92 integration).
- OpenAPI integration assertion: passed and confirms `userAdvanceBalanceId` plus the expense/category/reimbursement contracts.
- Flutter dependency resolution: passed; packages added or changed: none.
- Strict Dart formatting: passed after final formatting.
- `flutter analyze`: passed with no issues.
- `flutter test`: passed 163 total, 0 failed (137 existing plus 26 new).
- Web release build: passed with `API_BASE_URL=http://localhost:5231`; WebAssembly dry run passed.
- Android debug APK build: passed with `API_BASE_URL=http://10.0.2.2:5231`; the app was not launched.
- iOS source compatibility was inspected; build, signing, simulator, and device verification remain pending macOS/Xcode.
- Automated tests used fakes/controlled Dio only. No real expense API write or financial command was invoked.

## Safety status

- PostgreSQL mutation: none.
- PostgreSQL schema change: none.
- Generated Database-First file change: none.
- Migration created or run: none.
- `EnsureCreated`, `EnsureDeleted`, or `Database.Migrate`: not introduced.
- Real financial data: not used.
- Secrets, tokens, idempotency values, hashes, paths, and credentials: not printed, persisted, or committed.
- Staging, commit, and push: not performed.

## Limitations

- No binary upload/download or proof that a metadata row's file exists.
- No document verification/rejection command or original-paper custody state.
- No supplier credit/debt, mixed payment, project split, claim payment, final settlement, cancellation, reversal, or correction editing.
- Reimbursement is read-only and claims are never marked paid by Flutter.
- The Android artifact was built but not runtime-tested. iOS remains unbuilt on Windows.

## Exact recommended next task

Design and implement the backend Suppliers, Supplier Credit, Supplier Invoices, Payments, Refunds, Credit Notes, and Supplier Debt foundation using the existing PostgreSQL schema, without implementing final settlement yet.
