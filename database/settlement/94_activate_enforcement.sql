\set ON_ERROR_STOP on
\echo 'ENFORCEMENT ACTIVATION — SEPARATE ALL-COMPANY CUTOVER APPROVAL REQUIRED'
\if :{?activation_approved}
\else
\quit 3
\endif
\if :activation_approved
\else
\echo 'STOP: activation_approved must be true'
\quit 3
\endif
\if :{?all_companies_reviewed}
\else
\quit 3
\endif
\if :all_companies_reviewed
\else
\echo 'STOP: all_companies_reviewed must be true'
\quit 3
\endif
-- This checkpoint is global because the reviewed design has no per-company
-- activation flag. Companies may be prepared in phases, but activation waits
-- until ALL legacy rows are covered and ALL 48 writer/action entries reviewed.
-- Must run during an independently controlled financial-write maintenance window.
BEGIN;
SET LOCAL lock_timeout='5s';
LOCK TABLE ahdah.projects,ahdah.advances,ahdah.user_advance_balances,
 ahdah.project_financial_controls,ahdah.project_custody_positions,ahdah.custody_reservations
 IN SHARE ROW EXCLUSIVE MODE;
DO $activation$
BEGIN
 IF current_database()<>'ahdah_db' THEN RAISE EXCEPTION 'Wrong database'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.projects p LEFT JOIN ahdah.project_financial_controls c ON c.company_id=p.company_id AND c.project_id=p.project_id WHERE c.project_id IS NULL) THEN RAISE EXCEPTION 'Missing legacy control'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.projects p JOIN ahdah.project_financial_controls c ON c.company_id=p.company_id AND c.project_id=p.project_id WHERE p.status='FinanciallyClosed' AND c.active_closure_id IS NULL) THEN RAISE EXCEPTION 'Uncertified legacy Closed status requires reviewed remediation'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.user_advance_balances b WHERE
  (SELECT count(*) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id AND p.project_id IS NULL)<>1 OR
  b.available_amount<>(SELECT coalesce(sum(p.available_amount),0) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id) OR
  b.reserved_amount<>(SELECT coalesce(sum(p.reserved_amount),0) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id)) THEN RAISE EXCEPTION 'Opening balance conservation fails'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.project_custody_positions p WHERE p.reserved_amount<>(SELECT coalesce(sum(r.amount),0) FROM ahdah.custody_reservations r WHERE r.company_id=p.company_id AND r.project_custody_position_id=p.project_custody_position_id AND r.status='Reserved')) THEN RAISE EXCEPTION 'Opening reservation conservation fails'; END IF;
END $activation$;
ALTER TABLE ahdah.projects ENABLE TRIGGER trg_settlement_project_control;
ALTER TABLE ahdah.user_advance_balances ENABLE TRIGGER trg_settlement_balance_conservation;
ALTER TABLE ahdah.advances ENABLE TRIGGER trg_settlement_advance_currency;
COMMIT;
-- This activates structural enforcement ONLY. It grants no runtime write rights
-- and does not enable closure. D16, typed evaluator, reopening, authorization,
-- retries, fixtures/races and separately reviewed command privileges still gate it.
