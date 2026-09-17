\set ON_ERROR_STOP on
\echo 'AUTHORIZED CUTOVER USE ONLY — BUSINESS DATA — NOT EXECUTED DURING PREPARATION'
\if :{?authorized_cutover}
\else
\quit 3
\endif
\if :authorized_cutover
\else
\echo 'STOP: authorized_cutover must be true'
\quit 3
\endif
\if :{?company_id}
\else
\quit 3
\endif
BEGIN ISOLATION LEVEL REPEATABLE READ READ ONLY;
SELECT status,count(*) FROM ahdah.projects WHERE company_id=:'company_id'::uuid GROUP BY status;
SELECT a.currency_code,count(*) balances,sum(b.available_amount) available,sum(b.reserved_amount) reserved
 FROM ahdah.user_advance_balances b JOIN ahdah.advances a ON a.company_id=b.company_id AND a.advance_id=b.advance_id WHERE b.company_id=:'company_id'::uuid GROUP BY a.currency_code;
SELECT transfer_type,status,currency_code,count(*),sum(transfer_amount) FROM ahdah.money_transfers WHERE company_id=:'company_id'::uuid AND status IN ('Draft','PendingConfirmation','CorrectionRequired') GROUP BY transfer_type,status,currency_code;
SELECT (project_id IS NULL) unassigned,status,currency_code,count(*),sum(total_amount) FROM ahdah.expenses WHERE company_id=:'company_id'::uuid GROUP BY (project_id IS NULL),status,currency_code;
SELECT source_type,(project_id IS NULL) unassigned,status,currency_code,count(*),sum(outstanding_amount) FROM ahdah.personal_claims WHERE company_id=:'company_id'::uuid GROUP BY source_type,(project_id IS NULL),status,currency_code;
SELECT (project_id IS NULL) unassigned,status,currency_code,count(*),sum(outstanding_amount) FROM ahdah.supplier_debts WHERE company_id=:'company_id'::uuid GROUP BY (project_id IS NULL),status,currency_code;
SELECT status,currency_code,count(*),sum(credit_note_amount) FROM ahdah.supplier_credit_notes WHERE company_id=:'company_id'::uuid GROUP BY status,currency_code;
SELECT status,resolution_type,currency_code,count(*) FROM ahdah.expense_returns WHERE company_id=:'company_id'::uuid GROUP BY status,resolution_type,currency_code;
SELECT status,currency_code,count(*),sum(refund_amount),sum(net_received_amount) FROM ahdah.supplier_refunds WHERE company_id=:'company_id'::uuid GROUP BY status,currency_code;
SELECT status,currency_code,count(*),sum(refund_amount) FROM ahdah.owner_payment_refunds WHERE company_id=:'company_id'::uuid GROUP BY status,currency_code;
SELECT count(*) missing_contract_currency FROM ahdah.projects WHERE company_id=:'company_id'::uuid AND contract_currency_code IS NULL;
SELECT count(*) uncertified_legacy_closed FROM ahdah.projects p WHERE p.company_id=:'company_id'::uuid AND p.status='FinanciallyClosed' AND NOT EXISTS(SELECT 1 FROM ahdah.project_financial_controls c WHERE c.company_id=p.company_id AND c.project_id=p.project_id AND c.active_closure_id IS NOT NULL);
SELECT verification_status,count(*) FROM ahdah.expense_documents WHERE company_id=:'company_id'::uuid GROUP BY verification_status;
SELECT count(*) metadata_without_verified_binary FROM ahdah.expense_documents d WHERE d.company_id=:'company_id'::uuid AND NOT EXISTS(SELECT 1 FROM ahdah.verified_evidence_objects e WHERE e.company_id=d.company_id AND e.expense_document_id=d.expense_document_id);
SELECT count(*) inconsistent_debt_project FROM ahdah.supplier_debts d JOIN ahdah.expenses e ON e.company_id=d.company_id AND e.expense_id=d.expense_id WHERE d.company_id=:'company_id'::uuid AND d.project_id IS DISTINCT FROM e.project_id;
ROLLBACK;
-- Capture holder declarations, pending allocation details and source-version
-- digests in a separately authorized restricted workpaper. These aggregate
-- inventories do not determine project intent or prove binary/paper custody.
-- Never combine rows across currency/category or backfill from expense ratios.
