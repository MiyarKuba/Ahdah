# Suppliers and supplier debt foundation — backend Phase 1

## Scope and schema findings

This phase uses the existing PostgreSQL 18 `ahdah` schema without migrations or generated-model edits. There is no `supplier_invoices` table. A supplier invoice is an `expenses` row with payment mode `SupplierCredit`, paired one-to-one with `supplier_debts` through unique `(company_id, expense_id)`. Optional invoice lines use `expense_items`.

Implemented records:

- `suppliers`: tenant supplier directory, cash/credit mode, metadata, active lifecycle, and version.
- `supplier_payment_accounts`: bank, wallet, cash-collection, or other metadata with verification lifecycle. Responses mask identifiers.
- `expenses`, `expense_items`, `supplier_debts`, and immutable `supplier_debt_ledger_entries`: invoice, lines, computed outstanding balance, and history.
- `supplier_payments`, `supplier_payment_debt_allocations`, and `supplier_payment_funding_sources`: payment lifecycle and allocation history.
- `funding_sources`, immutable `funding_source_ledger_entries`, `manager_contributions`, and `company_cashbox_entries`: authoritative availability and payer/source lineage.
- `supplier_credit_notes` and `supplier_credit_note_allocations`: approval and debt-credit application history.
- `supplier_refunds`: read-only history because each write requires an approved `expense_returns` lineage and a one-to-one refund funding source.
- `audit_logs`, `idempotency_records`, and `company_settings`: audit, replay protection, thresholds, limits, and separation rules.

No standalone invoice, payable, accounts-payable, supplier-balance, or supplier-statement table/view exists. No supplier-payment relationship targets `user_advance_balances` or `advances`.

## Exact values

Supplier lifecycle is `is_active`, not a status string. Supplier types are `GeneralSupplier`, `MaterialsSupplier`, `EquipmentSupplier`, `EquipmentRental`, `FuelSupplier`, `Subcontractor`, `TransportProvider`, `MaintenanceProvider`, `ServiceProvider`, and `Other`. Transaction modes are `CashOnly`, `CreditOnly`, and `CashAndCredit`.

Payment-account types are `BankAccount`, `MobileWallet`, `CashCollection`, and `Other`. Verification statuses are `PendingVerification`, `Verified`, and `Rejected`.

Supplier-credit expense statuses are `Draft`, `PendingReview`, `CorrectionRequired`, `Approved`, `Rejected`, `Cancelled`, and `Reversed`. Debt statuses are `Open`, `PartiallySettled`, `Settled`, `Cancelled`, and `Reversed`.

Payment statuses are `Draft`, `PendingApproval`, `Confirmed`, `Rejected`, `Cancelled`, and `Reversed`. Payment/refund methods are `Cash`, `BankTransfer`, `Cheque`, `Card`, `MobileWallet`, and `Other`. Refund statuses are `PendingVerification`, `Confirmed`, `Cancelled`, and `Reversed`.

Credit-note statuses are `Draft`, `PendingApproval`, `Approved`, `Rejected`, `Cancelled`, and `Reversed`. Reasons are `ReturnedGoods`, `DamagedGoods`, `PricingCorrection`, `Overbilling`, `AdditionalDiscount`, `ServiceCompensation`, and `Other`.

Funding-source types are `ProjectOwnerPayment`, `ManagerContribution`, `CompanyCashbox`, `ReturnedAdvance`, `SupplierRefund`, and `Other`. Funding statuses are `PendingVerification`, `Available`, `PartiallyUsed`, `FullyUsed`, `Cancelled`, and `Reversed`.

## API

Directory and accounts:

- `GET /api/v1/suppliers`
- `GET /api/v1/suppliers/{supplierId}`
- `POST /api/v1/suppliers`
- `PATCH /api/v1/suppliers/{supplierId}`
- `GET /api/v1/suppliers/{supplierId}/payment-accounts`
- `POST /api/v1/suppliers/{supplierId}/payment-accounts`
- `GET /api/v1/suppliers/{supplierId}/statement`

Invoices and debts:

- `GET /api/v1/supplier-invoices`
- `GET /api/v1/supplier-invoices/{debtId}`
- `POST /api/v1/supplier-invoices`
- `GET /api/v1/supplier-debts`
- `GET /api/v1/supplier-debts/{debtId}`

Payments:

- `GET /api/v1/supplier-payments`
- `GET /api/v1/supplier-payments/{paymentId}`
- `POST /api/v1/supplier-payments`
- `POST /api/v1/supplier-payments/{paymentId}/confirm`
- `POST /api/v1/supplier-payments/{paymentId}/reject`

Credit and refunds:

- `GET /api/v1/supplier-credit-notes`
- `GET /api/v1/supplier-credit-notes/{creditNoteId}`
- `POST /api/v1/supplier-credit-notes`
- `POST /api/v1/supplier-credit-notes/{creditNoteId}/approve`
- `POST /api/v1/supplier-credit-notes/{creditNoteId}/allocations`
- `GET /api/v1/supplier-refunds`

Collections use page 1/default 20/maximum 100, stable ordering, SQL filtering before count, and explicit DTOs.

## Role and visibility

| Role | Directory/invoices | Company financials | Supplier metadata | Accounts | Invoice create | Payments | Credits |
|---|---|---|---|---|---|---|---|
| Manager | All tenant | All tenant | Create/update/deactivate | List/create | Yes | Record/review | Create/approve/apply |
| Deputy | All tenant | Read-only | No | Read-only | Yes | No | Read-only |
| Accountant | All tenant | All tenant | No | List/create | Yes | Record/review | Create/approve/apply |
| Supervisor | Assigned-project suppliers/debts only | No | No | No | No | No | No |
| Worker/unknown | None | None | No | No | No | No | No |

Policies are outer gates. Every query and lock includes `company_id`; Supervisor visibility additionally requires an active matching `project_supervisors` row.

## Directory and payment accounts

Creation derives tenant and creator from authentication. Supplier code is optional and schema-formatted. The database has no unique supplier-code/name constraint, so the API does not invent one. Deactivation requires `expectedVersion` and a reason and preserves history. Reactivation and hard delete are not exposed.

Account creation records `PendingVerification` metadata and never makes it default. Account number, IBAN, and wallet number are masked in responses. Verification, update, default selection, and deactivation are deferred; credentials, tokens, and card data are never accepted.

## Invoices, debt, and lines

Invoice creation atomically creates a `PendingReview` supplier-credit expense, optional items, one `Open` debt, one immutable `DebtCreated` ledger entry, audit, and idempotency result. Supplier/category/project/currency relationships are tenant-validated. Lines require a quantity-supporting category and total the invoice exactly. Quantity is `NUMERIC(18,3)`; money is `NUMERIC(18,2)`.

Expense rejection atomically cancels only an untouched open debt and appends `DebtCancelled`. The database has no supplier invoice-number uniqueness constraint, so no similarity or application uniqueness rule is invented; idempotency prevents duplicate effects for exact retries.

## Payments and funding attribution

Debt and funding allocation sets are unique, positive, same-currency, and each equals the payment amount. Phase 1 fixes `fee_amount` to zero because no fee-allocation rule is approved. Debts and sources lock in stable UUID order. Pending allocations reduce allocatable debt and reserve funding, preventing duplicate/excess payment before approval.

Confirmation consumes reserved funding, updates debt `paid_amount`, sets `PartiallySettled`/`Settled`, and appends immutable funding/debt ledgers. Rejection releases funding and preserves payment/allocation history. Company `Always`/`Threshold`/`Never`, payment-proof, maximum-payment, and separation settings are enforced.

`ManagerContribution` funding can be used only by its authenticated Manager owner. Other sources preserve their configured tenant subtype, including `CompanyCashbox`. The client cannot spoof payer identity. Supplier payments cannot allocate from `user_advance_balances`, so advance funding and final settlement are not claimed.

## Credit, refunds, statement, and balances

Credit notes start `PendingApproval`. Only `Approved` notes allocate. Allocation is unique per note/debt, same supplier/currency, cannot exceed available credit, and cannot exceed debt after pending-payment reservations. It updates authoritative debt totals and appends `CreditNoteApplied`.

Refund history is read-only. A safe write needs the unimplemented expense-return approval and maximum-return rules plus atomic creation of the one-to-one `SupplierRefund` funding source.

`supplier_debts.outstanding_amount` is database-generated as debt plus adjustment minus paid, credit-note, and written-off amounts. The API never accepts it. Balances group by currency and never mix currencies. Statements project immutable debt ledger history without raw audit JSON or internal ledger IDs.

## Idempotency, transactions, and concurrency

Account, invoice, payment/review, credit creation/approval/allocation commands require a 16–200 character `Idempotency-Key`. Existing tenant/actor/operation/payload fingerprinting replays exact completed calls and conflicts on mismatched reuse. Keys are not logged or returned.

Multi-table commands use explicit PostgreSQL transactions and tenant-filtered `FOR UPDATE`. Supplier, account, debt, payment, refund, credit, expense, and funding versions are EF concurrency tokens outside generated files. Concurrency, SQLSTATE `23505`, overpayment, duplicate allocation, insufficient funding, and credit over-application map to safe HTTP 409. Writes are never automatically retried.

## Deferred behavior

- Supplier refund creation/verification and expense-return approval/maximum rules.
- Account verification/rejection/default/update/deactivation.
- Supplier reactivation/hard delete; debt adjustment/write-off/cancellation/reversal.
- Payment cancellation/reversal/correction and credit rejection/cancellation/reversal.
- Advance-balance payment funding and final advance settlement.
- Binary upload, OCR, notifications, exchange rates, journal export, bank/gateway integration, and invoice similarity matching.

No PostgreSQL object, generated Database-First file, migration, or final settlement workflow changed during the backend foundation.

## Flutter Suppliers and Payables phase

The shared Flutter client now consumes these contracts on Android, iOS, and Web. Handwritten immutable DTO mappings preserve backend enum strings, render unknown future values safely, and keep money as validated two-decimal text. Invoice item quantities use three-decimal `BigInt` arithmetic and banker’s rounding to match the backend; request JSON emits validated numeric tokens without converting through `double`.

Authenticated routes cover supplier list/detail/create/edit/deactivate, masked payment accounts, supplier-credit invoice and debt list/detail/create, supplier payment list/detail/create/review, credit-note list/detail/create/approve/allocate, read-only refunds, and supplier statements. Centralized role capabilities mirror the API matrix. Supervisor screens rely on the API’s assigned-project predicates and never broaden visibility locally.

The existing Manager-only advance funding selector could not safely support Accountant payment creation. The smallest backend correction adds `GET /api/v1/supplier-payments/funding-sources`, authorized for supplier-payment recorders. Its query is tenant-scoped, returns only available or partially used sources with positive availability, accepts optional currency/payment-method filters, and restricts Manager contributions to their authenticated Manager owner. It does not expose advance balances or create a new financial rule.

Supplier payment creation requires one or more unique debt allocations and funding allocations whose exact integer-minor-unit totals equal the payment amount. Bank transfer and mobile wallet flows select a verified masked supplier account. The client treats displayed availability as an early usability bound only; the API revalidates and locks authoritative rows.

Financial commands generate a 32-byte URL-safe key with `Random.secure`, retain it only in the auto-disposed operation controller, and never display, log, or persist it. There is no automatic write retry. Only an ambiguous timeout/network outcome offers an explicit retry of the identical payload with the identical key; editing the payload creates a new operation and key.

Refund history remains read-only. Account verification/default management, supplier reactivation, payment or credit reversal/correction, supplier refund creation, binary proof upload, advance-balance funding, settlement, and closure remain intentionally unavailable.
